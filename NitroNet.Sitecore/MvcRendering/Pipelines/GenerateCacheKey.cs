namespace NitroNet.Sitecore.MvcRendering.Pipelines
{
    public class GenerateCacheKey : global::Sitecore.Support.Mvc.Pipelines.Response.RenderRendering.GenerateCacheKey
    {
        protected override string GetDataPart(global::Sitecore.Mvc.Presentation.Rendering rendering)
        {
            var baseResult = base.GetDataPart(rendering) ?? string.Empty;

            var variation = rendering["dataVariation"];
            if (variation == null)
            {
                return baseResult;
            }

            return baseResult + "_#dataVariation:" + variation;
        }
    }
}
