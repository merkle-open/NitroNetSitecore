using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using NitroNet.Sitecore.Caching;
using NitroNet.Sitecore.Rendering;
using Sitecore.Data.Events;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Events;
using Sitecore.Pipelines;

namespace NitroNet.Sitecore.Events
{
    public class EventHandlingInitializer
    {
        public void Process(PipelineArgs args)
        {
            Event.Subscribe("item:saved", OnSavedRaised);
            Event.Subscribe("item:saved:remote", OnSavedRaised);

            Event.Subscribe("publish:end", OnPublishRaised);
            Event.Subscribe("publish:end:remote", OnPublishRaised);
        }

        private static void OnPublishRaised(object sender, EventArgs e)
        {
            ExecutePublishEvents();
        }

        private static void OnSavedRaised(object sender, EventArgs e)
        {
            var eventArgs = e as SitecoreEventArgs;
            var remoteEventArgs = e as ItemSavedRemoteEventArgs;

            Item item = null;
            if (eventArgs != null)
            {
                item = eventArgs.Parameters[0] as Item;
            }
            if (remoteEventArgs != null)
            {
                item = remoteEventArgs.Item;
            }

            if (item == null || item.Database.Name.Equals("web", StringComparison.OrdinalIgnoreCase))
            {
                // Do not trigger save events for the web db
                return;
            }

            ExecuteSaveEvents(new[] { item });
        }

        private static void ExecuteSaveEvents(IEnumerable<Item> items)
        {
            ClearSitecoreRenderingCache(items);
        }

        private static void ExecutePublishEvents()
        {
            ClearSitecoreRenderingCache(null);
        }

        private static void ClearSitecoreRenderingCache(IEnumerable<Item> items)
        {
            var cacheManager = DependencyResolver.Current.GetService<ISitecoreCacheManager>();

            if (items == null || 
                items.Any(i => i.TemplateID.ToString().Equals(SitecoreRenderingRepository.ControllerRenderingId, StringComparison.InvariantCultureIgnoreCase)))
            {
                ClearSitecoreRenderingCacheInternal(cacheManager);
            }
        }

        private static void ClearSitecoreRenderingCacheInternal(ISitecoreCacheManager cacheManager)
        {
            cacheManager.Clear(SitecoreRenderingRepository.SitecoreRenderingCache);
            Log.Info(string.Format("{0} Cache has been cleared.", SitecoreRenderingRepository.SitecoreRenderingCache), typeof(EventHandlingInitializer));
        }
    }
}