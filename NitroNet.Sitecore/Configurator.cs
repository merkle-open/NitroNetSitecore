using Microsoft.Extensions.DependencyInjection;
using NitroNet.Sitecore.Caching;
using NitroNet.Sitecore.Rendering;
using NitroNet.ViewEngine.TemplateHandler;
using NitroNet.ViewEngine.TemplateHandler.Grid;
using Sitecore;
using Sitecore.DependencyInjection;
using Sitecore.Mvc.Common;
using Veil;

namespace NitroNet.Sitecore
{
	public class Configurator : IServicesConfigurator
	{
		public void Configure(IServiceCollection serviceCollection)
		{
			serviceCollection.AddTransient(provider => GridContext.GetFromRenderingContext(ContextService.Get().GetCurrent<RenderingContext>()));
			serviceCollection.AddTransient<ISitecoreRenderingRepository, SitecoreRenderingRepository>();
			serviceCollection.AddTransient<ISitecoreCacheManager, SitecoreCacheManager>();
			serviceCollection.AddTransient<INitroTemplateHandlerFactory, SitecoreMvcNitroTemplateHandlerFactory>();
			serviceCollection.AddTransient(provider => Context.Database);
		}
	}
}