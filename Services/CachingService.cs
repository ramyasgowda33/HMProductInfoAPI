using Microsoft.Extensions.Caching.Memory;

namespace HMProductInfoAPI.Services
{
    public interface ICachingService
    {
        public void SetEntry(object key, object value);
        public object GetEntry(object key);

        public void RemoveEntry(object key);
    }
    public  class CachingService : ICachingService
    {
        private readonly IMemoryCache _memoryCache;
        public CachingService(IMemoryCache memoryCache)
        {
            memoryCache = _memoryCache;
        }

        public void SetEntry(object key, object keyValue)
        {
            if(_memoryCache.TryGetValue(key, out string _))
            {
                _memoryCache.Remove(key);
                _memoryCache.Set(key, keyValue, new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromDays(1))
                    .SetSlidingExpiration(TimeSpan.FromDays(7)));
            }
        }

        public object GetEntry(object key)
        {
           _memoryCache.TryGetValue(key , out var keyValue);
           return keyValue;

        }

        public void RemoveEntry(object key)
        {
            _memoryCache?.Remove(key);
        }
    }
}
