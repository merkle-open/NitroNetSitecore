using System.Collections.Generic;
using NitroNet.ViewEngine.TemplateHandler;

namespace NitroNet.Sitecore.TemplateHandlers
{
    public class SitecoreComponentHelperHandler : ComponentHelperHandler
    {
        public SitecoreComponentHelperHandler(INitroTemplateHandler handler) : base(handler)
        {
        }

        protected override void AddAdditionalRenderingParameters(IDictionary<string, string> parameters,
            Dictionary<string, RenderingParameter> renderingParametersDictionary)
        {
            base.AddAdditionalRenderingParameters(parameters, renderingParametersDictionary);
            //key is already present
            if (renderingParametersDictionary.ContainsKey(SitecoreComponentHelperConstants.ForceController))
            {
                return;
            }

            //if no key is in parameters, use standard behaviour for renderingparameter creation
            if (!parameters.ContainsKey(SitecoreComponentHelperConstants.ForceController))
            {
                renderingParametersDictionary.Add(SitecoreComponentHelperConstants.ForceController,
                    CreateRenderingParameter(SitecoreComponentHelperConstants.ForceController, parameters));
                return;
            }

            var value = parameters[SitecoreComponentHelperConstants.ForceController];
            //transform bool value into string, to support boolean values as well as strings
            if (bool.TryParse(value, out var isForceController))
            {
                renderingParametersDictionary.Add(SitecoreComponentHelperConstants.ForceController,
                    new RenderingParameter(SitecoreComponentHelperConstants.ForceController)
                    {
                        Value = isForceController.ToString()
                    });
                return;
            }

            //use standard behaviour
            renderingParametersDictionary.Add(SitecoreComponentHelperConstants.ForceController,
                CreateRenderingParameter(SitecoreComponentHelperConstants.ForceController, parameters));
        }
    }

    public static class SitecoreComponentHelperConstants
    {
        public const string ForceController = "forceController";
    }
}
