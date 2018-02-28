using System.Text.RegularExpressions;
using Sitecore.Mvc.Pipelines.Response.GetDynamicPlaceholderInitialKey;

namespace NitroNet.Sitecore.DynamicPlaceholder.Pipelines.GetDynamicPlaceholderInitialKey
{
    /// <summary>
    /// Dynamic Placeholders extension for Sitecore 9.x
    /// </summary>
    public class RemovePlaceholderUniqueKeySuffixWithCountOrId : RemovePlaceholderUniqueKeySuffix
    {
        private static readonly Regex DynamicPartMatcherWithId =
            new Regex("-{[0-9A-F]{8}-[0-9A-F]{4}-[0-9A-F]{4}-[0-9A-F]{4}-[0-9A-F]{12}}-([0-9]+|{[0-9A-F]{8}-[0-9A-F]{4}-[0-9A-F]{4}-[0-9A-F]{4}-[0-9A-F]{12}})$", RegexOptions.Compiled);

        public RemovePlaceholderUniqueKeySuffixWithCountOrId()
        {
            DynamicPartMatcher = DynamicPartMatcherWithId;
        }
    }
}
