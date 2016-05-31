using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using NitroNet.Sitecore.Caching;
using NitroNet.Sitecore.Rendering;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Events;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Events;
using Sitecore.Pipelines;
using Sitecore.Publishing;

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
            var sitecoreArgs = e as SitecoreEventArgs;
            var sitecoreArgsRemote = e as PublishEndRemoteEventArgs;

            var publishedItems = new List<Item>();

            if (sitecoreArgs != null)
            {
                if (sitecoreArgs.Parameters[0] is Publisher)
                {
                    var publishingOptions = sitecoreArgs.Parameters[0] as Publisher;
                    publishedItems.AddRange(ClearByOption(publishingOptions.Options));
                }
                if (sitecoreArgs.Parameters[0] is IEnumerable<DistributedPublishOptions>)
                {
                    var publishingOptions = sitecoreArgs.Parameters[0] as IEnumerable<DistributedPublishOptions>;

                    foreach (var option in publishingOptions)
                    {
                        publishedItems.AddRange(ClearByOption(option));
                    }
                }
            }
            if (sitecoreArgsRemote != null)
            {
                publishedItems.AddRange(ClearByOption(sitecoreArgsRemote));
            }

            if (publishedItems.Count > 0)
            {
                ExecutePublishEvents(publishedItems);
            }
        }

        private static IEnumerable<Item> ClearByOption(PublishOptions option)
        {
            return ExtractPublishItems(option.RootItem, option.Deep);
        }

        private static IEnumerable<Item> ClearByOption(DistributedPublishOptions option)
        {
            var database = Factory.GetDatabase(option.SourceDatabaseName);

            var item = database.GetItem(new ID(option.RootItemId));
            return ExtractPublishItems(item, option.Deep);
        }

        private static IEnumerable<Item> ClearByOption(PublishEndRemoteEventArgs option)
        {
            var database = Factory.GetDatabase(option.SourceDatabaseName);

            var item = database.GetItem(new ID(option.RootItemId));
            return ExtractPublishItems(item, option.Deep);
        }

        private static IEnumerable<Item> ExtractPublishItems(Item item, bool deep)
        {
            if (item != null)
            {
                yield return item;

                if (!deep)
                    yield break;

                foreach (var subItem in item.Axes.GetDescendants())
                {
                    yield return subItem;
                }
            }
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

        private static void ExecutePublishEvents(IEnumerable<Item> items)
        {
            ClearSitecoreRenderingCache(items);
        }

        private static void ClearSitecoreRenderingCache(IEnumerable<Item> items)
        {
            var cacheManager = DependencyResolver.Current.GetService<ISitecoreCacheManager>();

            if (items.Any(i => i.TemplateID.ToString().Equals(SitecoreRenderingRepository.ControllerRenderingId, StringComparison.InvariantCultureIgnoreCase)))
            {
                cacheManager.Clear(SitecoreRenderingRepository.SitecoreRenderingCache);
                Log.Info(string.Format("{0} Cache has been cleared.", SitecoreRenderingRepository.SitecoreRenderingCache), typeof(EventHandlingInitializer));
            }
        }
    }
}