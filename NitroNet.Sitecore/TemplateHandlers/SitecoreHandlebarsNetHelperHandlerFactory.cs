using System.Collections.Generic;
using HandlebarsDotNet;
using NitroNet.HandlebarsNet.Handlers;
using NitroNet.HandlebarsNet.Handlers.Grid;
using NitroNet.HandlebarsNet.ViewEngine;
using NitroNet.ViewEngine.Context;
using NitroNet.ViewEngine.TemplateHandler;

namespace NitroNet.Sitecore.TemplateHandlers
{
    public class SitecoreHandlebarsNetHelperHandlerFactory : IHandlebarsNetHelperHandlerFactory
    {
        private readonly INitroTemplateHandlerFactory _nitroTemplateHandlerFactory;
        private readonly IRenderingContextFactory _renderingContextFactory;

        public SitecoreHandlebarsNetHelperHandlerFactory(INitroTemplateHandlerFactory nitroTemplateHandlerFactory, IRenderingContextFactory renderingContextFactory)
        {
            _nitroTemplateHandlerFactory = nitroTemplateHandlerFactory;
            _renderingContextFactory = renderingContextFactory;
        }

        public IDictionary<string, HandlebarsHelper> Create()
        {
            var helperDictionary = new Dictionary<string, HandlebarsHelper>
            {
                {"pattern", new SitecoreHandlebarsNetComponentHandler(_nitroTemplateHandlerFactory.Create(), _renderingContextFactory).Evaluate},
                {"component", new SitecoreHandlebarsNetComponentHandler(_nitroTemplateHandlerFactory.Create(), _renderingContextFactory).Evaluate},
                {"partial", new HandlebarsNetPartialHandler(_nitroTemplateHandlerFactory.Create(), _renderingContextFactory).Evaluate}, //TODO: Verhalten wie ">"
                {"placeholder", new HandlebarsNetPlaceholderHandler(_nitroTemplateHandlerFactory.Create(), _renderingContextFactory).Evaluate},
                {"t", new HandlebarsNetLabelHandler(_nitroTemplateHandlerFactory.Create(), _renderingContextFactory).Evaluate},
                {"template-id", new HandlebarsNetTemplateIdHandler().Evaluate},
                {"grid-width", new HandlebarsNetGridWidthHandler(_renderingContextFactory).Evaluate},
                {"grid-component-width", new HandlebarsNetGridComponentWidthHandler(_renderingContextFactory).Evaluate}
            };

            return helperDictionary;
        }

        public IDictionary<string, HandlebarsBlockHelper> CreateForBlocks()
        {
            var helperDictionary = new Dictionary<string, HandlebarsBlockHelper>
            {
                {"grid-cell", new HandlebarsNetGridCellHandler(_renderingContextFactory).Evaluate}
            };


            return helperDictionary;
        }
    }
}
