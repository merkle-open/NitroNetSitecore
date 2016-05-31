using System.Collections.Concurrent;
using Sitecore;

namespace NitroNet.Sitecore.Caching
{
    public class SitecoreCacheManager : ISitecoreCacheManager
    {
        private readonly ConcurrentDictionary<string, SitecoreCache> _caches;

        public  SitecoreCacheManager()
        {
            _caches = new ConcurrentDictionary<string, SitecoreCache>();
        }

        public ISitecoreCache Get(string name)
        {
            if (_caches.ContainsKey(name))
            {
                return _caches[name];
            }

            var cache = new SitecoreCache(name, StringUtil.ParseSizeString("10MB"));
            _caches[name] = cache;

            return cache;
        }

        public void Clear(string name)
        {
            if (_caches.ContainsKey(name))
            {
                _caches[name].Clear();
            }
        }
    }
}
