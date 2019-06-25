using Microsoft.Practices.Unity;
using NitroNet.Sitecore.Caching;
using NitroNet.Sitecore.Rendering;
using NitroNet.Sitecore.TemplateHandlers;
using NitroNet.UnityModules;
using NitroNet.ViewEngine.TemplateHandler;
using NitroNet.ViewEngine.TemplateHandler.Grid;
using Sitecore;
using Sitecore.Data;
using Sitecore.Mvc.Common;
using Veil;
using Veil.Helper;

namespace NitroNet.Sitecore.UnityModules
{
    public class SitecoreUnityModule : IUnityModule
    {
        public void Configure(IUnityContainer container)
        {
            container.RegisterType<GridContext>(
                new InjectionFactory(u =>
                    GridContext.GetFromRenderingContext(ContextService.Get().GetCurrent<RenderingContext>())));
            container.RegisterType<ISitecoreRenderingRepository, SitecoreRenderingRepository>();
            container.RegisterType<ISitecoreCacheManager, SitecoreCacheManager>(
                new ContainerControlledLifetimeManager());
            container.RegisterType<INitroTemplateHandlerFactory, SitecoreMvcNitroTemplateHandlerFactory>(
                new ContainerControlledLifetimeManager());
            container.RegisterType<Database>(new InjectionFactory(u => Context.Database));

            //overwrite registration with the specific sitecore implementation
            container.RegisterType<IHelperHandlerFactory, SitecoreRenderingHelperHandlerFactory>(
                new ContainerControlledLifetimeManager());

        }
    }
}