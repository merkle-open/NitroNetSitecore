using Sitecore.Globalization;
using Sitecore.Mvc.Extensions;
using Sitecore.Mvc.Pipelines.Response.RenderRendering;
using Sitecore.Mvc.Presentation;
using System;

namespace NitroNet.Sitecore.Caching.Support
{
    public class GenerateCacheKey : global::Sitecore.Mvc.Pipelines.Response.RenderRendering.GenerateCacheKey
    {
        protected override string GenerateKey(global::Sitecore.Mvc.Presentation.Rendering rendering, RenderRenderingArgs args)
        {
            string text = rendering.Caching.CacheKey.OrIfEmpty(args.Rendering.Renderer.ValueOrDefault((Renderer renderer) => renderer.CacheKey));
            string result;
            if (StringExtensions.IsEmptyOrNull(text))
            {
                result = null;
            }
            else
            {
                string text2 = text + "_#lang:" + Language.Current.Name.ToUpper();
                RenderingCachingDefinition caching = rendering.Caching;
                if (rendering["ClearOnIndexUpdate"] == "1")
                {
                    text2 += "_#index";
                }
                if (caching.VaryByData)
                {
                    text2 += this.GetDataPart(rendering);
                }
                if (caching.VaryByDevice)
                {
                    text2 += this.GetDevicePart(rendering);
                }
                if (caching.VaryByLogin)
                {
                    text2 += this.GetLoginPart(rendering);
                }
                if (caching.VaryByUser)
                {
                    text2 += this.GetUserPart(rendering);
                }
                if (caching.VaryByParameters)
                {
                    text2 += this.GetParametersPart(rendering);
                }
                if (caching.VaryByQueryString)
                {
                    text2 += this.GetQueryStringPart(rendering);
                }
                result = text2;
            }
            return result;
        }
    }
}
