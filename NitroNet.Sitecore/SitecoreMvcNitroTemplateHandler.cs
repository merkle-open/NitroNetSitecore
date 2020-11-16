using System;
using System.Collections.Generic;
using System.Globalization;
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
using SC = Sitecore;
using System.IO;
using Sitecore.Mvc.Presentation;
using RenderingContext = NitroNet.ViewEngine.Context.RenderingContext;
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

	    public void RenderPlaceholder(object model, string key, string index, TextWriter writer, ViewContext viewContext)
	    {
	        var htmlHelper = new HtmlHelper(viewContext, new ViewDataContainer(viewContext.ViewData));
#if SC8
		    var dynamicKey = key;
		    if (!string.IsNullOrEmpty(index))
		    {
		        dynamicKey = key + "_" + index;
		    }

            viewContext.Writer.Write(htmlHelper.Sitecore().DynamicPlaceholder(dynamicKey));
#else
	        if (string.IsNullOrEmpty(index))
	        {
	            writer.Write(htmlHelper.Sitecore().Placeholder(key));
	            return;
	        }

	        if (int.TryParse(index, out var parsedIntIndex))
	        {
	            writer.Write(htmlHelper.Sitecore().DynamicPlaceholder(key, 1, 0, parsedIntIndex));
	            return;
	        }

	        if (ID.TryParse(index, out var parsedIdIndex))
	        {
	            writer.Write(htmlHelper.Sitecore().DynamicPlaceholder(key, parsedIdIndex));
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
                    !(bool.TryParse(forceController.GetValueAsString(), out var isForceController) && isForceController))
                {
                    _templateHandlerUtils.RenderPartial(finalModel, component.GetValueAsString(), skin.GetValueAsString(), context,
                        RenderPartial);
                    return;
                }

                _templateHandlerUtils.ThrowErrorIfSubModelFoundAndNull(subModel.SubModelFound, subModel.Value,
                    subModel.PropertyName, model);

                requestContext.RouteData.Values[ComponentConstants.SkinParameter] = skin.ValueObject ?? string.Empty;
                requestContext.RouteData.Values[ComponentConstants.DataParameter] = dataVariation.ValueObject ?? string.Empty;

                foreach (var keyValuePair in additionalParameters)
                {
                    savedRouteValues.Add(keyValuePair.Key, requestContext.RouteData.Values[keyValuePair.Key]);
                    requestContext.RouteData.Values[keyValuePair.Key] = keyValuePair.Value.Value;
                }
                
                var parts = component.GetValueAsString().Split('/');
                var componentName = parts[parts.Length - 1];
                var cleanComponentName = _templateHandlerUtils.CleanName(componentName);
                var renderingId = _renderingRepository.GetRenderingId(cleanComponentName);

                var htmlHelper = CreateHtmlHelper(context);

                if (renderingId != null)
                {
                    context.Writer.Write(htmlHelper.Sitecore()
                        .Rendering(renderingId, new {data = dataVariation.ValueObject ?? string.Empty}));
                }
                else
                {
                    var controller = CleanControllerName(componentName);
                    context.Writer.Write(htmlHelper.Sitecore().Controller(controller));

                    Log.Warn(
                        $"Controller {controller} gets directly called by NitroNet. Consider to create a rendering with name \"{cleanComponentName}\" in order to let the controller be called by the Sitecore rendering pipeline. Component: {component.GetValueAsString()}, Template: {skin.GetValueAsString()}, Data: {dataVariation.GetValueAsString()}",
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

        public void RenderComponent(IDictionary<string, RenderingParameter> renderingParameters, object model, RenderingContext context, IDictionary<string, object> parameters)
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
                var additionalParameters = _templateHandlerUtils.ConvertAdditionalArguments(parameters, new HashSet<string>(renderingParameters.Keys));

                if (_templateHandlerUtils.TryCreateModel(subModel, additionalParameters, out var finalModel) &&
                    !(bool.TryParse(forceController.GetValueAsString(), out var isForceController) && isForceController))
                {
                    _templateHandlerUtils.RenderPartial(finalModel, component.GetValueAsString(), skin.GetValueAsString(), context,
                        RenderPartial);
                    return;
                }

                _templateHandlerUtils.ThrowErrorIfSubModelFoundAndNull(subModel.SubModelFound, subModel.Value,
                    subModel.PropertyName, model);

                requestContext.RouteData.Values[ComponentConstants.SkinParameter] = skin.ValueObject ?? string.Empty;
                requestContext.RouteData.Values[ComponentConstants.DataParameter] = dataVariation.ValueObject ?? string.Empty;

                foreach (var keyValuePair in additionalParameters)
                {
                    savedRouteValues.Add(keyValuePair.Key, requestContext.RouteData.Values[keyValuePair.Key]);
                    requestContext.RouteData.Values[keyValuePair.Key] = keyValuePair.Value.Value;
                }

                var parts = component.GetValueAsString().Split('/');
                var componentName = parts[parts.Length - 1];
                var cleanComponentName = _templateHandlerUtils.CleanName(componentName);
                var renderingId = _renderingRepository.GetRenderingId(cleanComponentName);

                var htmlHelper = CreateHtmlHelper(context);

                if (renderingId != null)
                {
                    context.Writer.Write(htmlHelper.Sitecore()
                        .Rendering(renderingId, new { data = dataVariation.ValueObject ?? string.Empty }));
                }
                else
                {
                    var controller = CleanControllerName(componentName);
                    context.Writer.Write(htmlHelper.Sitecore().Controller(controller));

                    Log.Warn(
                        $"Controller {controller} gets directly called by NitroNet. Consider to create a rendering with name \"{cleanComponentName}\" in order to let the controller be called by the Sitecore rendering pipeline. Component: {component.GetValueAsString()}, Template: {skin.GetValueAsString()}, Data: {dataVariation.GetValueAsString()}",
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

	    public void RenderLabel(string key, ViewContext context)
	    {
	        var label = SC.Globalization.Translate.Text(key);
	        context.Writer.Write(label);
        }

		public void RenderPartial(string template, object model, RenderingContext context)
		{
			CreateHtmlHelper(context).RenderPartial(template, model);
		}

	    public void RenderPartial(string template, object model, ViewContext context)
	    {
	        new HtmlHelper(context, new ViewDataContainer(context.ViewData)).RenderPartial(template, model);
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

	        if (renderingParameter.Type != RenderingParameterType.Unresolved)
	        {
	            return false;
	        }

            var propertyName = _templateHandlerUtils.CleanName(renderingParameter.GetValueAsString());
            if (_templateHandlerUtils.GetPropertyValueFromObjectHierarchically(model, propertyName, out var dynamicName) && 
                dynamicName is string)
            {
                renderingParameter.ValueObject = dynamicName.ToString();
                return true;
            }

	        return false;
	    }

        /*public void RenderComponent(RenderingParameter component, RenderingParameter skin, RenderingParameter dataVariation, object model, ViewContext viewContext)
        {
            var requestContext = PageContext.Current.RequestContext;
            var savedSkin = requestContext.RouteData.Values[SkinParameter];
            var savedModel = requestContext.RouteData.Values[ModelParameter];
            var savedDataVariation = requestContext.RouteData.Values[DataParameter];
            try
            {
                // Try to get values from model
                AggregateRenderingParameter(component, model);
                AggregateRenderingParameter(skin, model);

                if (string.IsNullOrEmpty(dataVariation.Value))
                {
                    dataVariation.Value = component.Value;
                }

                var propertyName = CleanName(dataVariation.Value);

                object subModel = null;

                if (dataVariation.Value.Equals(ThisIdentifier))
                {
                    subModel = model;
                }

                var modelFound = false;

                if (subModel == null)
                {
                    modelFound = GetValueFromObjectHierarchically(model, propertyName, out subModel);
                }

                if (subModel != null && !(subModel is string))
                {
                    var componentIdBySkin = GetComponentId(component.Value, skin.Value);
                    RenderPartial(componentIdBySkin, subModel, viewContext);
                    return;
                }

                if (modelFound && subModel == null)
                {
                    Log.Error(
                        string.Format("Property {0} of model {1} is null.", propertyName, model.GetType()), this);
                    return;
                }

                var htmlHelper = new HtmlHelper(viewContext, new ViewDataContainer(viewContext.ViewData));
                var parts = component.Value.Split('/');
                var componentName = parts[parts.Length - 1];
                var cleanComponentName = CleanName(componentName);
                var renderingId = _renderingRepository.GetRenderingId(cleanComponentName);
                requestContext.RouteData.Values[SkinParameter] = skin.Value ?? string.Empty;
                requestContext.RouteData.Values[DataParameter] = dataVariation.Value ?? string.Empty;

                if (renderingId != null)
                {
                    // TODO: Cache!
                    viewContext.Writer.Write(htmlHelper.Sitecore()
                        .Rendering(renderingId, new { data = dataVariation.Value ?? string.Empty }));
                }
                else
                {
                    var controller = CleanControllerName(componentName);

                    viewContext.Writer.Write(htmlHelper.Sitecore().Controller(controller));

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
                requestContext.RouteData.Values[SkinParameter] = savedSkin;
                requestContext.RouteData.Values[DataParameter] = savedDataVariation;
                requestContext.RouteData.Values[ModelParameter] = savedModel;
            }
        }*/
    }
}
