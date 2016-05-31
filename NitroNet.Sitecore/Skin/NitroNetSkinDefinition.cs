using System.Collections.Generic;
using NitroNet.ViewEngine;

namespace NitroNet.Sitecore.Skin
{
	public class NitroNetSkinDefinition : ISkinDefinition
	{
		private readonly ITemplateInfo _defaultTemplate;
		private readonly IDictionary<string, ITemplateInfo> _skins;

		public NitroNetSkinDefinition(ComponentDefinition componentDefinition)
		{
			_defaultTemplate = new NitroNetTemplateInfo(componentDefinition.DefaultTemplate);
			_skins = new Dictionary<string, ITemplateInfo>();
			foreach (var skin in componentDefinition.Skins)
			{
				if (!_skins.ContainsKey(skin.Key))
					_skins.Add(skin.Key, new NitroNetTemplateInfo(skin.Value));
			}
		}

		public ITemplateInfo DefaultTemplate
		{
			get { return _defaultTemplate; }
		}

		public IDictionary<string, ITemplateInfo> Skins
		{
			get { return _skins; }
		}
	}
}