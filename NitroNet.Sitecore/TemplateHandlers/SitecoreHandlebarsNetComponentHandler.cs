using System.Collections.Generic;
using NitroNet.HandlebarsNet.Handlers;
using NitroNet.ViewEngine.Context;
using NitroNet.ViewEngine.TemplateHandler;

namespace NitroNet.Sitecore.TemplateHandlers
{
    public class SitecoreHandlebarsNetComponentHandler : HandlebarsNetComponentHandler
    {
        public SitecoreHandlebarsNetComponentHandler(INitroTemplateHandler handler, IRenderingContextFactory renderingContextFactory) : base(handler, renderingContextFactory)
        {
        }

        protected override void AddAdditionalRenderingParameters(IDictionary<string, object> parameters,
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
            if (bool.TryParse(value.ToString(), out var isForceController))
            {
                renderingParametersDictionary.Add(SitecoreComponentHelperConstants.ForceController,
                    new RenderingParameter(SitecoreComponentHelperConstants.ForceController)
                    {
                        ValueObject = isForceController.ToString()
                    });
                return;
            }

            //use standard behaviour
            renderingParametersDictionary.Add(SitecoreComponentHelperConstants.ForceController,
                CreateRenderingParameter(SitecoreComponentHelperConstants.ForceController, parameters));
        }
    }
}
