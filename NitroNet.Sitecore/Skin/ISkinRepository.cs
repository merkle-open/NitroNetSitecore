namespace NitroNet.Sitecore.Skin
{
	public interface ISkinRepository
	{
		bool TryGetSkinDefinition(string id, out ISkinDefinition skinDefinition);

        bool TryGetSkinTemplateInfo(string id, string skin, out ITemplateInfo templateInfo);
	}
}