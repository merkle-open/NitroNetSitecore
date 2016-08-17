using System.Collections.Generic;
using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Pipelines.GetPlaceholderRenderings;

namespace NitroNet.Sitecore.DynamicPlaceholder.Pipelines
{
    public class GetDynamicPlaceholderRenderings : GetAllowedRenderings
    {
        private readonly GetAllowedRenderings _innerAllowedRenderings;

        public GetDynamicPlaceholderRenderings()
        {
            _innerAllowedRenderings = new GetAllowedRenderings();
        }

        public void Process(GetPlaceholderRenderingsArgs args)
        {
            Assert.IsNotNull(args, "args");

            string placeholderKey;
            string placeholderIndex;
            if (!DynamicPlaceholderKeyProvider.TryGetValue(args.PlaceholderKey, out placeholderKey, out placeholderIndex))
                return;

            Item placeholderItem = null;
            if (ID.IsNullOrEmpty(args.DeviceId))
            {
                placeholderItem = Client.Page.GetPlaceholderItem(placeholderKey, args.ContentDatabase,
                                                                 args.LayoutDefinition);
            }
            else
            {
                using (new DeviceSwitcher(args.DeviceId, args.ContentDatabase))
                {
                    placeholderItem = Client.Page.GetPlaceholderItem(placeholderKey, args.ContentDatabase,
                                                                     args.LayoutDefinition);
                }
            }

            List<Item> collection = null;
            if (placeholderItem != null)
            {
                bool flag;
                args.HasPlaceholderSettings = true;
                collection = GetRenderings(placeholderItem, out flag);
                if (flag)
                {
                    args.Options.ShowTree = false;
                }
            }
            if (collection != null)
            {
                if (args.PlaceholderRenderings == null)
                {
                    args.PlaceholderRenderings = new List<Item>();
                }
                args.PlaceholderRenderings.AddRange(collection);
            }
        }
    }
}
