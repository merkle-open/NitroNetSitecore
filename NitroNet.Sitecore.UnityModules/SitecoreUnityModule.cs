using Microsoft.Practices.Unity;
using NitroNet.Sitecore.Caching;
using NitroNet.Sitecore.Rendering;
using NitroNet.UnityModules;
using NitroNet.ViewEngine.TemplateHandler;
using NitroNet.ViewEngine.TemplateHandler.Grid;
using Sitecore.Mvc.Common;
using Veil;

namespace NitroNet.Sitecore.UnityModules
{
    public class SitecoreUnityModule : IUnityModule
    {
        public void Configure(IUnityContainer container)
        {
            container.RegisterType<GridContext>(
                new InjectionFactory(u => GridContext.GetFromRenderingContext(ContextService.Get().GetCurrent<RenderingContext>())));
            container.RegisterType<ISitecoreRenderingRepository, SitecoreRenderingRepository>();
            container.RegisterType<ISitecoreCacheManager, SitecoreCacheManager>(new ContainerControlledLifetimeManager());
            container.RegisterType<INitroTemplateHandlerFactory, SitecoreMvcNitroTemplateHandlerFactory>(
                new ContainerControlledLifetimeManager());
        }
    }
}
