using Castle.MicroKernel.Registration;
using Castle.Windsor;
using NitroNet.CastleWindsorModules;
using NitroNet.Sitecore.Caching;
using NitroNet.Sitecore.Rendering;
using NitroNet.ViewEngine.TemplateHandler;
using NitroNet.ViewEngine.TemplateHandler.Grid;
using Sitecore;
using Sitecore.Data;
using Sitecore.Mvc.Common;
using Veil;

namespace NitroNet.Sitecore.CastleWindsorModules
{
    public class SitecoreCastleWindsorModule : ICastleWindsorModule
    {
        public void Configure(IWindsorContainer container)
        {
            container.Register(Component.For<GridContext>().UsingFactoryMethod(() => GridContext.GetFromRenderingContext(ContextService.Get().GetCurrent<RenderingContext>())).LifestyleTransient());
            container.Register(Component.For<ISitecoreRenderingRepository>().ImplementedBy<SitecoreRenderingRepository>());
            container.Register(Component.For<ISitecoreCacheManager>().ImplementedBy<SitecoreCacheManager>());
            container.Register(Component.For<INitroTemplateHandlerFactory>().ImplementedBy<SitecoreMvcNitroTemplateHandlerFactory>());
            container.Register(Component.For<Database>().UsingFactoryMethod(() => Context.Database));
        }
    }
}