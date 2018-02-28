using System.Web;
using Sitecore.Mvc.Helpers;

namespace NitroNet.Sitecore.DynamicPlaceholder
{
    /// <summary>
    /// Dynamic Placeholders for Sitecore 8.x
    /// </summary>
    public static class DynamicPlaceholderExtension
    {
        public static HtmlString DynamicPlaceholder(this SitecoreHelper helper, string key)
        {
            var id = helper.CurrentRendering.UniqueId;
            return helper.Placeholder(DynamicPlaceholderKeyProvider.GetKey(key, id));
        }
    }
}