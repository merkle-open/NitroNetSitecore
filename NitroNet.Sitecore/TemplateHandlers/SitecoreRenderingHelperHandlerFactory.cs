using System.Collections.Generic;
using NitroNet.Veil.Handlers;
using NitroNet.Veil.Handlers.Grid;
using NitroNet.ViewEngine.TemplateHandler;
using Veil.Helper;

namespace NitroNet.Sitecore.TemplateHandlers
{
    public class SitecoreRenderingHelperHandlerFactory : IHelperHandlerFactory
    {
        private readonly INitroTemplateHandlerFactory _nitroTemplateHandlerFactory;

        public SitecoreRenderingHelperHandlerFactory(INitroTemplateHandlerFactory nitroTemplateHandlerFactory)
        {
            _nitroTemplateHandlerFactory = nitroTemplateHandlerFactory;
        }

        public IEnumerable<IHelperHandler> Create()
        {
            //Nitro helpers
            yield return new SitecoreComponentHelperHandler(_nitroTemplateHandlerFactory.Create());
            yield return new PartialHelperHandler(_nitroTemplateHandlerFactory.Create());
            yield return new PlaceholderHelperHandler(_nitroTemplateHandlerFactory.Create());
            yield return new LabelHelperHandler(_nitroTemplateHandlerFactory.Create());
            yield return new GridHelperHandler();
            yield return new GridWidthHelperHandler();
            yield return new GridComponentWidthHelperHandler();
            yield return new TemplateIdHelperHandler();
        }
    }
}
