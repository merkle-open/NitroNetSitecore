using Sitecore.Pipelines;
using System.Web.Mvc;

namespace NitroNet.Sitecore.Registration
{
    public class RegisterNitroNetViewEngine
    {
        public virtual void Process(PipelineArgs args)
        {
            ViewEngines.Engines.Add(DependencyResolver.Current.GetService<SitecoreNitroNetViewEngine>());
        }
    }
}
