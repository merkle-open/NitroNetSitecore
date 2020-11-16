using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;
using NitroNet.Mvc;
using NitroNet.ViewEngine;
using Sitecore.Mvc.Common;
using Sitecore.Mvc.Presentation;
using IView = NitroNet.ViewEngine.IView;
using IViewEngine = System.Web.Mvc.IViewEngine;
using RenderingContext = NitroNet.ViewEngine.Context.RenderingContext;

namespace NitroNet.Sitecore
{
	public class SitecoreNitroNetViewEngine : NitroNetViewEngine, IViewEngine
	{
		public SitecoreNitroNetViewEngine(ViewEngine.IViewEngine viewEngine, ITemplateRepository templateRepository, IModelTypeProvider modelTypeProvider, IComponentRepository componentRepository)
			: base(viewEngine, templateRepository, modelTypeProvider, componentRepository)
		{
		}

		ViewEngineResult IViewEngine.FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache)
		{
			string templateId;
			if (TryGetView(viewName, out templateId))
				return FindView(controllerContext, templateId, masterName, useCache);

			return FindView(controllerContext, viewName, masterName, useCache);
		}

		ViewEngineResult IViewEngine.FindPartialView(ControllerContext controllerContext, string partialViewName, bool useCache)
		{
			string templateId;
			if (TryGetView(partialViewName, out templateId))
				return FindPartialView(controllerContext, templateId, useCache);

			return FindPartialView(controllerContext, partialViewName, useCache);
		}

		protected override IView CreateAdapter(IView view)
		{
			return new SitecoreViewAdapter(view);
		}

		protected override MvcRenderingContext ResolveContext(ViewContext viewContext, IViewDataContainer viewDataContainer, TextWriter writer)
		{
			var context = ContextService.Get().GetCurrentOrDefault<RenderingContext>();
			return new MvcRenderingContext(viewContext, viewDataContainer, writer, context);
		}

		private static bool TryGetView(string viewName, out string templateId)
		{
			templateId = null;
			if (Path.GetExtension(viewName) == ".cshtml")
			{
				templateId = GetTemplateId(viewName);
				return true;
			}
			return false;
		}

		private static string GetTemplateId(string viewName)
		{
			var path = Path.Combine(Path.GetDirectoryName(viewName), Path.GetFileNameWithoutExtension(viewName));

			path = path.Replace("\\", "/");
			path = path.TrimStart('/');

			return path;
		}

		private class SitecoreViewAdapter : ViewEngine.IView
		{
			private readonly IView _adaptee;

			public SitecoreViewAdapter(IView adaptee)
			{
				_adaptee = adaptee;
			}

			public Task RenderAsync(object model, RenderingContext context)
			{
				Render(model, context);
				return Task.FromResult(true);
			}

			public void Render(object model, RenderingContext context)
			{
				using (ContextService.Get().Push(context))
				{
					var renderingModel = model as RenderingModel;
					if (renderingModel != null)
					{
						var renderingParameters = renderingModel.Rendering.Parameters.ToDictionary(k => k.Key, k => k.Value);
						_adaptee.Render(JObject.FromObject(renderingParameters), context);
					}
					else
						_adaptee.Render(model, context);
				}
			}
		}
	}
}