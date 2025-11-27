using baraka.promo.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace baraka.promo.Core.Authorize
{
    public class ApiKeyAuthAttribute : Attribute, IAsyncActionFilter
    {
        const string ApiKeyAuthHeaderName = "Authorization";
        const string ApiKeyHeaderName = "X-API-KEY";
        public const string ApiKeyItemName = "ApiKeyValue";
        public const string ApiKeyName = "ApiKeyName";

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            try
            {
                if (!context.HttpContext.Request.Headers.TryGetValue(ApiKeyHeaderName, out var potentialApiKey)
                    && !context.HttpContext.Request.Headers.TryGetValue(ApiKeyAuthHeaderName, out potentialApiKey))
                {
                    context.Result = new UnauthorizedResult();
                    return;
                }

                var _db = context.HttpContext.RequestServices.GetRequiredService<ApplicationDbContext>();
                string api_key = potentialApiKey.ToString();

                var apiKeyExists = await _db.Integrations.FirstOrDefaultAsync(k => k.SecretKey == api_key && k.IsActive);

                if (apiKeyExists == null)
                {
                    context.Result = new UnauthorizedResult();
                    return;
                }

                context.HttpContext.Items[ApiKeyItemName] = apiKeyExists.Id;
                context.HttpContext.Items[ApiKeyName] = apiKeyExists.Name;
                await next();
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync($"ApiKeyAuth.OnActionExecutionAsync error -> {ex.Message}");
                context.Result = new BadRequestResult();
                return;
            }
        }
    }
}
