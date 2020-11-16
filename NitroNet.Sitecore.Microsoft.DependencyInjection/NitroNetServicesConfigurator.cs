using Microsoft.Extensions.DependencyInjection;
using NitroNet.Sitecore.Caching;
using NitroNet.Sitecore.Rendering;
using NitroNet.Sitecore.TemplateHandlers;
using NitroNet.ViewEngine;
using NitroNet.ViewEngine.Cache;
using NitroNet.ViewEngine.Config;
using NitroNet.ViewEngine.IO;
using NitroNet.ViewEngine.TemplateHandler;
using NitroNet.ViewEngine.TemplateHandler.Grid;
using NitroNet.ViewEngine.TemplateHandler.Utils;
using Sitecore.DependencyInjection;
using Sitecore.Mvc.Common;
using System.Configuration;
using System.Web.Hosting;
using NitroNet.HandlebarsNet.ViewEngine;
using NitroNet.Mvc.Context;
using NitroNet.Veil.ViewEngine;
using NitroNet.ViewEngine.Context;
using Veil.Helper;


namespace NitroNet.Sitecore.Microsoft.DependencyInjection
{
    public class NitroNetServicesConfigurator : IServicesConfigurator
    {
        public void Configure(IServiceCollection serviceCollection)
        {
            RegisterNitroNet(serviceCollection);
            RegisterNitroNetSitecore(serviceCollection);
        }

        protected virtual void RegisterNitroNetSitecore(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<SitecoreNitroNetViewEngine>();
            serviceCollection.AddTransient<ISitecoreRenderingRepository, SitecoreRenderingRepository>();
            serviceCollection.AddTransient(x => GridContext.GetFromRenderingContext(ContextService.Get().GetCurrent<RenderingContext>()));
            serviceCollection.AddSingleton<ISitecoreCacheManager, SitecoreCacheManager>();
            serviceCollection.AddSingleton<INitroTemplateHandlerFactory, SitecoreMvcNitroTemplateHandlerFactory>();
        }

        protected virtual void RegisterNitroNet(IServiceCollection serviceCollection)
        {
            var basePath = GetNitroNetBasePath();

            var config = ConfigurationLoader.LoadNitroConfiguration(basePath);
            serviceCollection.AddSingleton(config);
            serviceCollection.AddSingleton<IFileSystem>(new FileSystem(basePath, config));
            serviceCollection.AddTransient<IModelTypeProvider, DefaultModelTypeProvider>();
            
            serviceCollection.AddTransient<ICacheProvider, MemoryCacheProvider>();
            serviceCollection.AddSingleton<IComponentRepository, DefaultComponentRepository>();
            serviceCollection.AddSingleton<ITemplateRepository, NitroTemplateRepository>();
            serviceCollection.AddSingleton<INitroTemplateHandlerUtils, NitroTemplateHandlerUtils>();
            serviceCollection.AddSingleton<IRenderingContextFactory, MvcRenderingContextFactory>();

            RegisterHandlebarsNet(serviceCollection);
        }

        protected virtual string GetNitroNetBasePath()
        {
            var rootPath = HostingEnvironment.MapPath("~/");
            var basePath = PathInfo.Combine(PathInfo.Create(rootPath), PathInfo.Create(ConfigurationManager.AppSettings["NitroNet.BasePath"]));

            return basePath.ToString();
        }

        protected virtual void RegisterHandlebarsNet(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IHandlebarsNetHelperHandlerFactory, SitecoreHandlebarsNetHelperHandlerFactory>();
            serviceCollection.AddSingleton<IHandlebarsNetEngine, HandlebarsNetEngine>();
            serviceCollection.AddTransient<IViewEngine, HandlebarsNetViewEngine>();
        }

        protected virtual void RegisterVeil(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<INamingRule, NamingRule>();
            serviceCollection.AddSingleton<IHelperHandlerFactory, SitecoreRenderingHelperHandlerFactory>();
            serviceCollection.AddTransient<IViewEngine, VeilViewEngine>();
        }
    }
}
