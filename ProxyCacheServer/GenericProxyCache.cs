using System;
using System.Runtime.Caching;

namespace ProxyCacheServer
{
    public class GenericProxyCache<T> where T : class, new()
    {
        private readonly MemoryCache _cache = new MemoryCache("GenericProxyCache");
        public DateTimeOffset dt_default { get; set; } = ObjectCache.InfiniteAbsoluteExpiration;

        public T Get(string CacheItemName)
        {
            return Get(CacheItemName, dt_default);
        }

        public T Get(string CacheItemName, double dt_seconds)
        {
            var expiration = DateTimeOffset.Now.AddSeconds(dt_seconds);
            return Get(CacheItemName, expiration);
        }

        public T Get(string CacheItemName, DateTimeOffset dt)
        {
            if (string.IsNullOrEmpty(CacheItemName))
                throw new ArgumentNullException(nameof(CacheItemName));

            if (!_cache.Contains(CacheItemName))
            {
                var newItem = new T();
                _cache.Set(CacheItemName, newItem, dt);
            }

            return _cache.Get(CacheItemName) as T;
        }
        public void Set(string CacheItemName, T value, double dt_seconds)
        {
            if (string.IsNullOrEmpty(CacheItemName))
                throw new ArgumentNullException(nameof(CacheItemName));

            if (value == null)
                throw new ArgumentNullException(nameof(value));

            var expiration = DateTimeOffset.Now.AddSeconds(dt_seconds);
            _cache.Set(CacheItemName, value, expiration);
        }

        internal void DisplayCacheContents()
        {
            throw new NotImplementedException();
        }
    }
}
