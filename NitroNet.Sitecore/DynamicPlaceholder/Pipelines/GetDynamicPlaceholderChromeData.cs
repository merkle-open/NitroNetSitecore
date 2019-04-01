using System;
using Sitecore;
using Sitecore.Diagnostics;
using Sitecore.Pipelines.GetChromeData;
using Sitecore.Web.UI.PageModes;

namespace NitroNet.Sitecore.DynamicPlaceholder.Pipelines
{
    /// <summary>
    /// Dynamic Placeholders for Sitecore 8.x
    /// </summary>
	public class GetDynamicPlaceholderChromeData : GetChromeDataProcessor
	{
		public override void Process(GetChromeDataArgs args)
		{
			Assert.ArgumentNotNull(args, "args");
			Assert.IsNotNull(args.ChromeData, "Chrome Data");

			if (!"placeholder".Equals(args.ChromeType, StringComparison.OrdinalIgnoreCase)) 
				return;

			var argument = args.CustomData["placeHolderKey"] as string;

			string placeholderKey;
		    string placeholderIndex;
		    if (!DynamicPlaceholderKeyProvider.TryGetValue(argument, out placeholderKey, out placeholderIndex))
				return;

			if (args.Item == null) 
				return;

			var layout = ChromeContext.GetLayout(args.Item);
			var item = Client.Page.GetPlaceholderItem(placeholderKey, args.Item.Database, layout);
			if (item == null) 
				return;
				
			args.ChromeData.DisplayName = item.DisplayName;

			if (!string.IsNullOrEmpty(item.Appearance.ShortDescription))
				args.ChromeData.ExpandedDisplayName = item.Appearance.ShortDescription;
		}
	}
}