using Sitecore.Mvc.Pipelines.Response.GetDynamicPlaceholderKeys;

namespace NitroNet.Sitecore.DynamicPlaceholder.Pipelines.GetDynamicPlaceholderKeys
{
    /// <summary>
    /// Dynamic Placeholders extension for Sitecore 9.x
    /// </summary>
    public class GetUniqueIdKeyWithinRendering : GetDynamicPlaceholderKeysProcessor
    {
        public static readonly string RenderingParameterKey = "GetUniqueIdKeyWithinRendering";

        public override void Process(GetDynamicPlaceholderKeysArgs args)
        {
            var rendering = args.Rendering;

            var uniqueKeysWithinRendering = rendering.Parameters[RenderingParameterKey];

            if (!string.IsNullOrEmpty(uniqueKeysWithinRendering))
            {
                args.UniqueSuffixesWithinRendering[0] = uniqueKeysWithinRendering;

                rendering.Parameters[RenderingParameterKey] = string.Empty;
            }
        }
    }
}
