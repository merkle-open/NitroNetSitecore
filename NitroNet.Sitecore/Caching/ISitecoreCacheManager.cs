namespace NitroNet.Sitecore.Caching
{
    public interface ISitecoreCacheManager
    {
        ISitecoreCache Get(string name);

        void Clear(string name);
    }
}
