using System;
using System.Text.RegularExpressions;

namespace NitroNet.Sitecore.DynamicPlaceholder
{
    /// <summary>
    /// Dynamic Placeholders for Sitecore 8.x
    /// </summary>
    static internal class DynamicPlaceholderKeyProvider
    {
        //text that ends in a GUID
		private const string DynamicKeyRegex = @"^(.+?)(?:_([^_]*))?_[\d\w]{8}\-([\d\w]{4}\-){3}[\d\w]{12}$";

        public static bool TryGetValue(string placeholderKey, out string resultPlaceholderKey, out string resultPlaceholderIndex)
        {
            resultPlaceholderKey = null;
            resultPlaceholderIndex = null;

            var regex = new Regex(DynamicKeyRegex);
            var match = regex.Match(placeholderKey);
            if (match.Success && match.Groups.Count > 0)
            {
                resultPlaceholderKey = match.Groups[1].Value;
                resultPlaceholderIndex = match.Groups[2].Value;
                return true;
            }

            return false;
        }

        public static string GetKey(string key, Guid currentRenderingId)
        {
            if (currentRenderingId == Guid.Empty)
                return key;

            return string.Format("{0}_{1}", key, currentRenderingId);
        }
    }
}