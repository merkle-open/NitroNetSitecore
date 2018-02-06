namespace NitroNet.Sitecore.MvcRendering.Pipelines
{
    public class GenerateCacheKey : global::NitroNet.Sitecore.Caching.Support.GenerateCacheKey
    {
        protected override string GetDataPart(global::Sitecore.Mvc.Presentation.Rendering rendering)
        {
            var baseResult = base.GetDataPart(rendering) ?? string.Empty;

            var variation = rendering["data"];
            if (variation == null)
            {
                return baseResult;
            }

            return baseResult + "_#data:" + variation;
        }
    }
}
