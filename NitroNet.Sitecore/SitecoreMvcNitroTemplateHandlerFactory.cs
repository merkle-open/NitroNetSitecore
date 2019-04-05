using NitroNet.Sitecore.Rendering;
using NitroNet.ViewEngine;
using NitroNet.ViewEngine.TemplateHandler;
using NitroNet.ViewEngine.TemplateHandler.RenderHandler;

namespace NitroNet.Sitecore
{
    public class SitecoreMvcNitroTemplateHandlerFactory : INitroTemplateHandlerFactory
    {
        private readonly ISitecoreRenderingRepository _sitecoreRenderingRepository;
        private readonly INitroTemplateHandlerUtils _templateHandlerUtils;

        public SitecoreMvcNitroTemplateHandlerFactory(ISitecoreRenderingRepository sitecoreRenderingRepository,
            INitroTemplateHandlerUtils templateHandlerUtils)
        {
            _sitecoreRenderingRepository = sitecoreRenderingRepository;
            _templateHandlerUtils = templateHandlerUtils;
        }

        public INitroTemplateHandler Create()
        {
            return new SitecoreMvcNitroTemplateHandler(_sitecoreRenderingRepository, _templateHandlerUtils);
        }
    }
}
