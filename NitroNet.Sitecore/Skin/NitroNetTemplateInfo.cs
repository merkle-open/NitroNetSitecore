using NitroNet.ViewEngine;

namespace NitroNet.Sitecore.Skin
{
	public class NitroNetTemplateInfo : ITemplateInfo
	{
		private readonly TemplateInfo _templateInfo;

		public NitroNetTemplateInfo(TemplateInfo templateInfo)
		{
			_templateInfo = templateInfo;
		}

		public string Id
		{
			get { return _templateInfo.Id; }
		}
	}
}