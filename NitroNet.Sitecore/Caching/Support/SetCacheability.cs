using Sitecore.Mvc.Pipelines.Response.RenderRendering;
using Sitecore.Mvc.Presentation;
using System;
using System.Net;

namespace NitroNet.Sitecore.Caching.Support
{
    public class SetCacheability : global::Sitecore.Mvc.Pipelines.Response.RenderRendering.SetCacheability
    {
        protected virtual string ClearOnIndexUpdateCacheKey
        {
            get { return "ClearOnIndexUpdate"; }
        }

        protected override bool IsCacheable(global::Sitecore.Mvc.Presentation.Rendering rendering,
            RenderRenderingArgs args)
        {
            if (rendering.RenderingItem != null && rendering.RenderingItem.Caching != null)
            {
                rendering.Caching.Cacheable = rendering.RenderingItem.Caching.Cacheable;
            }
            bool flag = rendering.Caching.Cacheable && this.DoesContextAllowCaching(args);
            if (flag)
            {
                this.AddCachingSettings(rendering);
            }
            return flag;
        }

        protected virtual void AddCachingSettings(global::Sitecore.Mvc.Presentation.Rendering rendering)
        {
            rendering.Caching.VaryByData = rendering.RenderingItem.Caching.VaryByData;
            rendering.Caching.VaryByDevice = rendering.RenderingItem.Caching.VaryByDevice;
            rendering.Caching.VaryByLogin = rendering.RenderingItem.Caching.VaryByLogin;
            rendering.Caching.VaryByParameters = rendering.RenderingItem.Caching.VaryByParm;
            rendering.Caching.VaryByQueryString = rendering.RenderingItem.Caching.VaryByQueryString;
            rendering.Caching.VaryByUser = rendering.RenderingItem.Caching.VaryByUser;
            rendering[this.ClearOnIndexUpdateCacheKey] =
                (rendering.RenderingItem.Caching.ClearOnIndexUpdate ? "1" : string.Empty);
        }
    }
}    