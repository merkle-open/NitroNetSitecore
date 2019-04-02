using System.Web.Mvc;
using Sitecore.Pipelines;

namespace NitroNet.Sitecore.Pipelines.Initialize
{
    public class RegisterNitroNetViewEngine
    {
        public virtual void Process(PipelineArgs args)
        {
            ViewEngines.Engines.Add(DependencyResolver.Current.GetService<SitecoreNitroNetViewEngine>());
        }
    }
}
