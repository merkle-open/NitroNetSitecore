using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using NitroNet.Mvc;
using NitroNet.Sitecore.Rendering;
using NitroNet.Sitecore.TemplateHandlers;
using NitroNet.ViewEngine.TemplateHandler;
using NitroNet.ViewEngine.TemplateHandler.Utils;
using Sitecore.Diagnostics;
using Sitecore.Mvc;
using Sitecore.Mvc.Presentation;
using RenderingContext = Veil.RenderingContext;
using SC = Sitecore;

#if SC8
using NitroNet.Sitecore.DynamicPlaceholder;
#else
using Sitecore.Data;
using NitroNet.Sitecore.DynamicPlaceholder.Helpers;
#endif

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
            {
                throw new InvalidOperationException("SitecoreMvcNitroTemplateHandler can only be used inside a Mvc application.");
            }

			return mvcContext;
		}

		private static HtmlHelper CreateHtmlHelper(MvcRenderingContext mvcContext)
		{
			return new HtmlHelper(mvcContext.ViewContext, mvcContext.ViewDataContainer);
		}

        public void RenderPlaceholder(object model, string key, string index, RenderingContext context)
		{
		    var htmlHelper = CreateHtmlHelper(context);
#if SC8
		    var dynamicKey = key;
		    if (!string.IsNullOrEmpty(index))
		    {
		        dynamicKey = key + "_" + index;
		    }
            
		    context.Writer.Write(htmlHelper.Sitecore().DynamicPlaceholder(dynamicKey));
#else
		    if (string.IsNullOrEmpty(index))
		    {
		        context.Writer.Write(htmlHelper.Sitecore().Placeholder(key));
		        return;
		    }

            if (int.TryParse(index, out var parsedIntIndex))
            {
                context.Writer.Write(htmlHelper.Sitecore().DynamicPlaceholder(key, 1, 0, parsedIntIndex));
                return;
            }

		    if (ID.TryParse(index, out var parsedIdIndex))
		    {
		        context.Writer.Write(htmlHelper.Sitecore().DynamicPlaceholder(key, parsedIdIndex));
		        return;
		    }

		    throw new ArgumentException($"'Index' attribute of {{placeholder}} helper needs to be an integer or Sitecore.Data.ID string. The chosen index is '{index}'");
#endif
        }

        public void RenderComponent(RenderingParameter component, RenderingParameter skin, RenderingParameter dataVariation,
            object model, RenderingContext context)
        {
            RenderComponent(new Dictionary<string, RenderingParameter>
            {
                { ComponentConstants.Name, component },
                { ComponentConstants.DataParameter, dataVariation},
                { ComponentConstants.SkinParameter, skin}
            }, model, context, new Dictionary<string, string>());
        }

        public void RenderComponent(IDictionary<string, RenderingParameter> renderingParameters, object model, RenderingContext context, IDictionary<string, string> parameters)
        {
            var requestContext = PageContext.Current.RequestContext;

            var savedRouteValues = new Dictionary<string, object>
            {
                {ComponentConstants.SkinParameter, requestContext.RouteData.Values[ComponentConstants.SkinParameter]},
                {ModelParameter, requestContext.RouteData.Values[ModelParameter]},
                {ComponentConstants.DataParameter, requestContext.RouteData.Values[ComponentConstants.DataParameter]}
            };

            try
            {
                var component = renderingParameters[ComponentConstants.Name];
                var skin = renderingParameters[ComponentConstants.SkinParameter];
                var dataVariation = renderingParameters[ComponentConstants.DataParameter];
                var forceController = renderingParameters[SitecoreComponentHelperConstants.ForceController];

                // Try to get values from model
                AggregateRenderingParameter(component, model);
                AggregateRenderingParameter(skin, model);
                AggregateRenderingParameter(forceController, model);
                
                var subModel = _templateHandlerUtils.FindSubModel(renderingParameters, model, context);
                var additionalParameters = _templateHandlerUtils.ResolveAdditionalArguments(model, parameters,
                    new HashSet<string>(renderingParameters.Keys));

                if (_templateHandlerUtils.TryCreateModel(subModel, additionalParameters, out var finalModel) &&
                    !(bool.TryParse(forceController.Value, out var isForceController) && isForceController))
                {
                    _templateHandlerUtils.RenderPartial(finalModel, component.Value, skin.Value, context,
                        RenderPartial);
                    return;
                }

                _templateHandlerUtils.ThrowErrorIfSubModelFoundAndNull(subModel.SubModelFound, subModel.Value,
                    subModel.PropertyName, model);

                requestContext.RouteData.Values[ComponentConstants.SkinParameter] = skin.Value ?? string.Empty;
                requestContext.RouteData.Values[ComponentConstants.DataParameter] = dataVariation.Value ?? string.Empty;

                foreach (var keyValuePair in additionalParameters)
                {
                    savedRouteValues.Add(keyValuePair.Key, requestContext.RouteData.Values[keyValuePair.Key]);
                    requestContext.RouteData.Values[keyValuePair.Key] = keyValuePair.Value.Value;
                }
                
                var parts = component.Value.Split('/');
                var componentName = parts[parts.Length - 1];
                var cleanComponentName = _templateHandlerUtils.CleanName(componentName);
                var renderingId = _renderingRepository.GetRenderingId(cleanComponentName);

                var htmlHelper = CreateHtmlHelper(context);

                if (renderingId != null)
                {
                    context.Writer.Write(htmlHelper.Sitecore()
                        .Rendering(renderingId, new {data = dataVariation.Value ?? string.Empty}));
                }
                else
                {
                    var controller = CleanControllerName(componentName);
                    context.Writer.Write(htmlHelper.Sitecore().Controller(controller));

                    Log.Warn(
                        $"Controller {controller} gets directly called by NitroNet. Consider to create a rendering with name \"{cleanComponentName}\" in order to let the controller be called by the Sitecore rendering pipeline. Component: {component.Value}, Template: {skin.Value}, Data: {dataVariation.Value}",
                        this);
                }
            }
            finally
            {
                foreach (var savedRouteValue in savedRouteValues)
                {
                    if (savedRouteValue.Value == null)
                    {
                        requestContext.RouteData.Values.Remove(savedRouteValue.Key);
                    }
                    else
                    {
                        requestContext.RouteData.Values[savedRouteValue.Key] = savedRouteValue.Value;
                    }
                }
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
            if (_templateHandlerUtils.GetPropertyValueFromObjectHierarchically(model, propertyName, out var dynamicName) && 
                dynamicName is string)
            {
                renderingParameter.Value = dynamicName.ToString();
                return true;
            }

	        return false;
	    }
	}
}
