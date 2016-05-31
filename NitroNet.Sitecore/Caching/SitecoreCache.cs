using Sitecore.Caching;

namespace NitroNet.Sitecore.Caching
{
    public class SitecoreCache : CustomCache, ISitecoreCache
    {
        public SitecoreCache(string name, long maxSize) : base(name, maxSize)
        {
        }

        public T GetAs<T>(string key) where T : class
        {
            var cacheValue = GetObject(key) as SitecoreCacheValue;
            if (cacheValue == null || cacheValue.Value == null)
            {
                return null;
            }

            return cacheValue.Value as T;
        }

        public void Set<T>(string key, T value) where T : class
        {
            SetObject(key, new SitecoreCacheValue(value));
        }
    }
}
