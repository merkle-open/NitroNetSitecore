using System.Web;
using NitroNet.Sitecore.DynamicPlaceholder.Pipelines.GetDynamicPlaceholderKeys;
using Sitecore.Data;
using Sitecore.Diagnostics;
using Sitecore.Mvc.Helpers;
using Sitecore.Mvc.Presentation;

namespace NitroNet.Sitecore.DynamicPlaceholder.Helpers
{
    /// <summary>
    /// Dynamic Placeholders extension for Sitecore 9.x
    /// </summary>
    public static class SitecoreHelperExtension
    {
        public static HtmlString DynamicPlaceholder(this SitecoreHelper helper, string placeholderName, ID placeholderSuffix)
        {
            Assert.ArgumentNotNull(placeholderName, nameof(placeholderName));

            helper.CurrentRendering.Parameters[GetUniqueIdKeyWithinRendering.RenderingParameterKey] = placeholderSuffix.ToString();

            return helper.DynamicPlaceholder(new DynamicPlaceholderDefinition(placeholderName));
        }
    }
}
