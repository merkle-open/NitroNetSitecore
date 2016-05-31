using System.Collections.Generic;

namespace NitroNet.Sitecore.Skin
{
	public interface ISkinDefinition
	{
		ITemplateInfo DefaultTemplate { get; }
		IDictionary<string, ITemplateInfo> Skins { get; } 
	}
}