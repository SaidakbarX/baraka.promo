using Microsoft.Extensions.Caching.Memory;

namespace baraka.promo.Pages.TgPushSender
{
    public class MyCacheService
    {
        private readonly IMemoryCache _cache;

        public MyCacheService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public async Task<T> GetFromCache<T>(string key)
        {
            var cachedValue = _cache.Get<T>(key);
            if (cachedValue != null)
            {
                return cachedValue;
            }

            // Fetch data if not cached (replace with your actual data fetching logic)
            var fetchedData = await Task.FromResult(default(T)); // Placeholder for actual data fetching
            _cache.Set(key, fetchedData, GetCacheOptions());
            return fetchedData;
        }

        // New method to create cache options with absolute expiration
        private MemoryCacheEntryOptions GetCacheOptions()
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromSeconds(10)); // Set expiration to 5 minutes
            return cacheEntryOptions;
        }

        public void SetCache<T>(string key, T value)
        {
            _cache.Set(key, value, GetCacheOptions());
        }

        public void RemoveFromCache(string key)
        {
            _cache.Remove(key);
        }
    }
}
