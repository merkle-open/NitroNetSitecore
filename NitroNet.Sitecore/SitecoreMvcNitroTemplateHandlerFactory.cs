using NitroNet.Sitecore.Rendering;
using NitroNet.ViewEngine;
using NitroNet.ViewEngine.TemplateHandler;

namespace NitroNet.Sitecore
{
    public class SitecoreMvcNitroTemplateHandlerFactory : INitroTemplateHandlerFactory
    {
        private readonly IComponentRepository _componentRepository;
        private readonly ISitecoreRenderingRepository _sitecoreRenderingRepository;

        public SitecoreMvcNitroTemplateHandlerFactory(IComponentRepository componentRepository,
            ISitecoreRenderingRepository sitecoreRenderingRepository)
        {
            _componentRepository = componentRepository;
            _sitecoreRenderingRepository = sitecoreRenderingRepository;
        }

        public INitroTemplateHandler Create()
        {
            return new SitecoreMvcNitroTemplateHandler(_componentRepository, _sitecoreRenderingRepository);
        }
    }
}
