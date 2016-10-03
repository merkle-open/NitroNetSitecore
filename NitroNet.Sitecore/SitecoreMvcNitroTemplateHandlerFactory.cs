using System;
using System.Web;
using NitroNet.Sitecore.Rendering;
using NitroNet.ViewEngine;
using NitroNet.ViewEngine.TemplateHandler;

namespace NitroNet.Sitecore
{
    public class SitecoreMvcNitroTemplateHandlerFactory : INitroTemplateHandlerFactory
    {
        private readonly AsyncLocal<HttpContext> _asyncLocal;
        private readonly IComponentRepository _componentRepository;
        private readonly ISitecoreRenderingRepository _sitecoreRenderingRepository;
        private static readonly string SlotName = Guid.NewGuid().ToString("N");

        public SitecoreMvcNitroTemplateHandlerFactory(AsyncLocal<HttpContext> asyncLocal, IComponentRepository componentRepository, ISitecoreRenderingRepository sitecoreRenderingRepository)
        {
            _asyncLocal = asyncLocal;
            _componentRepository = componentRepository;
            _sitecoreRenderingRepository = sitecoreRenderingRepository;
        }

        public INitroTemplateHandler Create()
        {
            INitroTemplateHandler templateHandler;

            if (HttpContext.Current == null)
            {
                HttpContext.Current = _asyncLocal.Value;
            }

            if (HttpContext.Current != null)
            {
                templateHandler = HttpContext.Current.Items[SlotName] as INitroTemplateHandler;
                if (templateHandler != null)
                    return templateHandler;
            }

            templateHandler =
                new SitecoreMvcNitroTemplateHandler(
                    _componentRepository,
                    _sitecoreRenderingRepository);

            if (HttpContext.Current != null)
            {
                HttpContext.Current.Items[SlotName] = templateHandler;
            }

            return templateHandler;
        }
    }
}
