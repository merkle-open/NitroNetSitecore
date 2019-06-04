using NitroNet.Sitecore.Rendering;
using NitroNet.ViewEngine.Config;
using NitroNet.ViewEngine.TemplateHandler.RenderHandler;

namespace NitroNet.Sitecore
{
    public class SitecoreMvcNitroTemplateHandlerFactory : INitroTemplateHandlerFactory
    {
        private readonly ISitecoreRenderingRepository _sitecoreRenderingRepository;
        private readonly INitroTemplateHandlerUtils _templateHandlerUtils;
        private readonly INitroNetConfig _nitroNetConfig;

        public SitecoreMvcNitroTemplateHandlerFactory(ISitecoreRenderingRepository sitecoreRenderingRepository,
            INitroTemplateHandlerUtils templateHandlerUtils, INitroNetConfig nitroNetConfig)
        {
            _sitecoreRenderingRepository = sitecoreRenderingRepository;
            _templateHandlerUtils = templateHandlerUtils;
            _nitroNetConfig = nitroNetConfig;
        }

        public INitroTemplateHandler Create()
        {
            return new SitecoreMvcNitroTemplateHandler(_sitecoreRenderingRepository, _templateHandlerUtils, _nitroNetConfig);
        }
    }
}
