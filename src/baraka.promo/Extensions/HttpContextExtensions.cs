using baraka.promo.Core.Authorize;

namespace baraka.promo.Extensions
{
    public static class HttpContextExtensions
    {
        public static int GetApiKey(this HttpContext context)
        {
            if (context.Items.TryGetValue(ApiKeyAuthAttribute.ApiKeyItemName, out var api_key_info))
            {
                return (int)api_key_info;
            }
            return 0;
        }

        public static string GetApiKeyName(this HttpContext context)
        {
            if (context.Items.TryGetValue(ApiKeyAuthAttribute.ApiKeyName, out var api_key_info))
            {
                return (string)api_key_info;
            }
            return null;
        }
    }
}
