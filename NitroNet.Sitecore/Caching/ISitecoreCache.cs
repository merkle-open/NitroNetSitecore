namespace NitroNet.Sitecore.Caching
{
    public interface ISitecoreCache
    {
        T GetAs<T>(string key) where T : class;

        void Set<T>(string key, T value) where T : class;
    }
}
