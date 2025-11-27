using baraka.promo.Models.LoyaltyApiModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace baraka.promo.Core.Authorize
{
    [Route("api/authorize")]
    [ApiController]
    public class AuthorizeController: ControllerBase
    {
        readonly ILogger<AuthorizeController> _logger;
        readonly IMemoryCache _memory_cache;
        readonly UserManager<IdentityUser> _user_manager;

        public AuthorizeController(ILogger<AuthorizeController> logger, IMemoryCache memorycache,
            UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _memory_cache = memorycache;
            _user_manager = userManager;
        }

        [ProducesResponseType(typeof(TokenModel), 200)]
        [AllowAnonymous]
        [Route("token")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] AuthorizeModel model)
        {
            try
            {
                string cache_key = $"AuthorizeController.Token.{model.Username}_{model.Password}";

                if (!_memory_cache.TryGetValue(cache_key, out TokenModel result))
                {
                    _logger.LogWarning($"AuthorizeController-> {model.Username}");

                    var userToVerify = await _user_manager.FindByNameAsync(model.Username);
                    if (userToVerify != null && await _user_manager.CheckPasswordAsync(userToVerify, model.Password))
                    {
                        var claims = new List<Claim>
                        {
                            new Claim("scope", "read"),
                            new Claim(ClaimsIdentity.DefaultNameClaimType, userToVerify.Id),
                        };

                        var now = DateTime.UtcNow;

                        var jwt = new JwtSecurityToken(
                            issuer: AuthOptions.ISSUER,
                        audience: AuthOptions.AUDIENCE,
                        notBefore: now,
                            claims: claims,
                            expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                            signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
                        var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

                        result = new TokenModel
                        {
                            Token = encodedJwt,
                            Expires = now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                        };

                        _memory_cache.Set(cache_key, result, DateTime.Now.AddMinutes(5));
                    }
                    else return BadRequest();
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest();
            }
        }
    }
}
