using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using NitroNet.Mvc;
using NitroNet.Sitecore.DynamicPlaceholder;
using NitroNet.Sitecore.Rendering;
using NitroNet.ViewEngine;
using NitroNet.ViewEngine.TemplateHandler;
using NitroNet.ViewEngine.TemplateHandler.RenderHandler;
using Sitecore.Diagnostics;
using Sitecore.Mvc;
using Sitecore.Mvc.Presentation;
using RenderingContext = Veil.RenderingContext;
using SC = Sitecore;

namespace NitroNet.Sitecore
{
	public class SitecoreMvcNitroTemplateHandler : INitroTemplateHandler
	{
	    private const string ModelParameter = "model";

	    private readonly ISitecoreRenderingRepository _renderingRepository;
	    private readonly INitroTemplateHandlerUtils _templateHandlerUtils;

	    public SitecoreMvcNitroTemplateHandler(ISitecoreRenderingRepository renderingRepository,
	        INitroTemplateHandlerUtils templateHandlerUtils)
	    {
	        _renderingRepository = renderingRepository;
	        _templateHandlerUtils = templateHandlerUtils;
	    }

	    private static HtmlHelper CreateHtmlHelper(RenderingContext context)
		{
			return CreateHtmlHelper(GetMvcContext(context));
		}

		private static MvcRenderingContext GetMvcContext(RenderingContext context)
		{
			var mvcContext = context as MvcRenderingContext;
			if (mvcContext == null)
				throw new InvalidOperationException("SitecoreMvcNitroTemplateHandler can only be used inside a Mvc application.");
			return mvcContext;
		}

		private static HtmlHelper CreateHtmlHelper(MvcRenderingContext mvcContext)
		{
			return new HtmlHelper(mvcContext.ViewContext, mvcContext.ViewDataContainer);
		}

		public void RenderPlaceholder(object model, string key, string index, RenderingContext context)
		{
			var htmlHelper = CreateHtmlHelper(context);
		    var dynamicKey = key;
		    if (!string.IsNullOrEmpty(index))
		    {
		        dynamicKey = key + "_" + index;
		    }
				

			context.Writer.Write(htmlHelper.Sitecore().DynamicPlaceholder(dynamicKey));
		}

	    public void RenderComponent(RenderingParameter component, RenderingParameter skin, RenderingParameter dataVariation,
	        object model, RenderingContext context)
	    {
            var requestContext = PageContext.Current.RequestContext;
	        var savedSkin = requestContext.RouteData.Values[ComponentConstants.SkinParameter];
	        var savedModel = requestContext.RouteData.Values[ModelParameter];
	        var savedDataVariation = requestContext.RouteData.Values[ComponentConstants.DataParameter];
	        try
	        {
                // Try to get values from model
                AggregateRenderingParameter(component, model);
                AggregateRenderingParameter(skin, model);

	            var propAssignments = _templateHandlerUtils.DoPropertyAssignments(component, skin, dataVariation, model, context);

                if (propAssignments.SubModel != null && !(propAssignments.SubModel is string))
	            {
	                var componentIdBySkin = _templateHandlerUtils.GetComponentId(component.Value, skin.Value);
	                RenderPartial(componentIdBySkin, propAssignments.SubModel, context);
	                return;
	            }

	            _templateHandlerUtils.ThrowExceptionIfPropertyNull(propAssignments.ModelFound,
	                propAssignments.SubModel, propAssignments.PropertyName, model);

                var htmlHelper = CreateHtmlHelper(context);
                var parts = component.Value.Split('/');
                var componentName = parts[parts.Length - 1];
                var cleanComponentName = _templateHandlerUtils.CleanName(componentName);
                var renderingId = _renderingRepository.GetRenderingId(cleanComponentName);
                requestContext.RouteData.Values[ComponentConstants.SkinParameter] = skin.Value ?? string.Empty;
	            requestContext.RouteData.Values[ComponentConstants.DataParameter] = dataVariation.Value ?? string.Empty;

	            if (renderingId != null)
	            {
	                // TODO: Cache!
	                context.Writer.Write(htmlHelper.Sitecore()
	                    .Rendering(renderingId, new  { data = dataVariation.Value ?? string.Empty}));
	            }
	            else
	            {
                    var controller = CleanControllerName(componentName);

                    context.Writer.Write(htmlHelper.Sitecore().Controller(controller));

                    Log.Warn(
                        string.Format(
                            "Controller {0} gets directly called by NitroNet. " +
                            "Consider to create a rendering with name \"{1}\" in order to let the controller be called by the Sitecore rendering pipeline. " +
                            "Component: {2}, Template: {3}, Data: {4}",
                            controller, cleanComponentName, component.Value, skin.Value, dataVariation.Value), this);
                }
	        }
	        finally
	        {
	            requestContext.RouteData.Values[ComponentConstants.SkinParameter] = savedSkin;
	            requestContext.RouteData.Values[ComponentConstants.DataParameter] = savedDataVariation;
	            requestContext.RouteData.Values[ModelParameter] = savedModel;
	        }
	    }

		public Task RenderLabelAsync(string key, RenderingContext context)
		{
            var label = SC.Globalization.Translate.Text(key);

            context.Writer.Write(label);

			return Task.FromResult(true);
		}

		public void RenderLabel(string key, RenderingContext context)
		{
		    var label = SC.Globalization.Translate.Text(key);

			context.Writer.Write(label);
		}

		public void RenderPartial(string template, object model, RenderingContext context)
		{
			CreateHtmlHelper(context).RenderPartial(template, model);
		}

        private static string CleanControllerName(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            var words = text.Split(' ', '-');
            var sb = new StringBuilder();

            foreach (var s in words)
            {
                var firstLetter = s.Substring(0, 1);
                var rest = s.Substring(1, s.Length - 1);
                sb.Append(firstLetter.ToUpper(CultureInfo.InvariantCulture) + rest);
            }

            return sb.ToString();
        }

	    private bool AggregateRenderingParameter(RenderingParameter renderingParameter, object model)
	    {
	        if (renderingParameter == null)
	        {
	            return false;
	        }

	        if (!renderingParameter.IsDynamic)
	        {
	            return false;
	        }

            var propertyName = _templateHandlerUtils.CleanName(renderingParameter.Value);
            object dynamicName;
            if (_templateHandlerUtils.GetValueFromObjectHierarchically(model, propertyName, out dynamicName) && dynamicName is string)
            {
                renderingParameter.Value = dynamicName.ToString();
                return true;
            }

	        return false;
	    }
	}
}
