using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace baraka.promo.Services
{
    public class CurrentUserService : ICurrentUser
    {
        readonly IHttpContextAccessor _httpContextAccessor;
        readonly UserManager<IdentityUser> _userManager;
        readonly ILogger<CurrentUserService> _logger;
        readonly AuthenticationStateProvider _provider;
        Guid? clientId = null;
        IdentityUser _user;
        string rawAccessToken;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor,
            UserManager<IdentityUser> userManager,
            ILogger<CurrentUserService> logger,
            AuthenticationStateProvider provider)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _logger = logger;
            _provider = provider;
        }

        public string GetCurrentUserName()
        {
            LoadUser();
            if (_user == null) return null;

            return _user.UserName;
        }

        public Guid GetCurrentClientId()
        {
            LoadUser();

            return clientId ?? throw new InvalidOperationException("invalid token");
        }

        public bool IsAuthenticated()
        {
            LoadUser();
            return _user != null;
        }

        public bool IsAdmin()
        {
            return true;
            LoadUser();

            bool isAdmin = _userManager.IsInRoleAsync(_user, "admin").Result;
            return isAdmin;
        }
        public bool IsPOS()
        {
            LoadUser();

            bool is_pos = _userManager.IsInRoleAsync(_user, "pos").Result;
            return is_pos;
        }

        public string GetCurrentToken()
        {
            return rawAccessToken ?? throw new InvalidOperationException("token is null");
        }
        private void LoadUser()
        {
            if (_user == null)
            {
                var claimsPrincipal = _httpContextAccessor?.HttpContext?.User;

                if (_httpContextAccessor == null) _logger.LogWarning("CurrentUserService.LoadUser _httpContextAccessor is null");

                else if (_httpContextAccessor.HttpContext == null) _logger.LogWarning("CurrentUserService.LoadUser HttpContext is null");

                else if (_httpContextAccessor.HttpContext.User == null) _logger.LogWarning("CurrentUserService.LoadUser HttpContext.User is null");

                if (claimsPrincipal == null)
                {
                    var provicerResult = _provider.GetAuthenticationStateAsync().Result;
                    claimsPrincipal = provicerResult?.User;
                }
                if (claimsPrincipal == null)
                    throw new Exception("user is null");

                var claim = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier);

                if (claim != null)
                {
                    var userId = claim.Value;
                    _user = _userManager.FindByIdAsync(userId).Result;
                }
                else
                {
                    rawAccessToken = _httpContextAccessor.HttpContext.Request.Headers.Values.FirstOrDefault(f => f.ToString().StartsWith("Bearer")).ToString();

                    if (string.IsNullOrWhiteSpace(rawAccessToken)) return;

                    rawAccessToken = rawAccessToken.Replace("Bearer ", "");
                    var handler = new JwtSecurityTokenHandler();
                    var accessToken = handler.ReadJwtToken(rawAccessToken);

                    claim = accessToken.Claims.FirstOrDefault(f => f.Type.EndsWith("_name"));
                    claim ??= accessToken.Claims.FirstOrDefault(f => f.Type == ClaimTypes.Name);

                    if (claim != null && accessToken.ValidTo > DateTime.UtcNow) _user = _userManager.FindByIdAsync(claim.Value).Result;
                }
            }
        }
    }
}
