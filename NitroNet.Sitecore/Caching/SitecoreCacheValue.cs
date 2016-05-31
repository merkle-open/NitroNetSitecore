using Sitecore.Caching;
using SC = Sitecore;

namespace NitroNet.Sitecore.Caching
{
    public class SitecoreCacheValue : ICacheable
    {
        public SitecoreCacheValue(object value)
        {
            Value = value;
            Cacheable = true;
        }

        public object Value { get; set; }

        public long GetDataLength()
        {
            return SC.Reflection.TypeUtil.SizeOfObject();
        }

        public bool Cacheable { get; set; }

        public bool Immutable
        {
            get { return true; }
        }

        public event DataLengthChangedDelegate DataLengthChanged;
    }
}
