using System;
using System.Collections.Generic;
using System.Globalization;
using NitroNet.Sitecore.Caching;
using Sitecore.Diagnostics;
using SC = Sitecore;

namespace NitroNet.Sitecore.Rendering
{
    public class SitecoreRenderingRepository : ISitecoreRenderingRepository
    {
        public const string ControllerRenderingId = "{2A3E91A0-7987-44B5-AB34-35C2D9DE83B9}";
        public const string SitecoreRenderingCache = "NitroNet.SitecoreRenderings";
        private readonly ISitecoreCache _cache;

        public SitecoreRenderingRepository(ISitecoreCacheManager cacheManager)
        {
            _cache = cacheManager.Get(SitecoreRenderingCache);
        }

        private static string GetCacheKey()
        {
            return "allRenderings";
        }

        private IDictionary<string, string> GetAllRenderings()
        {
            var allRenderings = new Dictionary<string, string>();
            var layoutItem = SC.Context.Database.GetItem(SC.ItemIDs.LayoutRoot);

            if (layoutItem != null)
            {
                var renderings = layoutItem.Axes.GetDescendants();
                foreach (var rendering in renderings)
                {
                    if (rendering.TemplateID.ToString().Equals(ControllerRenderingId, StringComparison.InvariantCultureIgnoreCase))
                    {
                        var cleanedName = CleanName(rendering.Name);

                        try
                        {
                            allRenderings.Add(cleanedName, rendering.ID.Guid.ToString());
                        }
                        catch (ArgumentException ex)
                        {
                            if (ex.Message.Contains("item with the same key has already been added"))
                            {
                                var duplicateRendering = SC.Context.Database.GetItem(allRenderings[cleanedName]);
                                throw new ArgumentException($"There exist two renderings with the same name '{cleanedName}' located under '{rendering.Paths.FullPath}' and '{duplicateRendering.Paths.FullPath}'", ex);
                            }

                            throw;
                        }
                    }
                }
            }

            return allRenderings;
        }

        public string GetRenderingId(string renderingName)
        {
            var cleanRenderingName = CleanName(renderingName);
            var renderings = _cache.GetAs<IDictionary<string, string>>(GetCacheKey());

            if (renderings == null)
            {
                renderings = GetAllRenderings();
                _cache.Set(GetCacheKey(), renderings);
            }

            return !renderings.ContainsKey(cleanRenderingName) ? null : renderings[cleanRenderingName];
        }

        private static string CleanName(string text)
        {
            return string.IsNullOrEmpty(text) ? string.Empty : text.Replace(" ", string.Empty).Replace("-", string.Empty).ToLower(CultureInfo.InvariantCulture);
        }
    }
}
