## Table of contents
- [Installation](installation.md)
- [Configuration](configuration.md)
- [Getting started](getting-started.md)
- [Samples](samples.md)
- [Release Notes](https://github.com/namics/NitroNetSitecore/releases)
- [Known Issues](known-issues.md)

## Configuration

Please see the [configuration guide of NitroNet](https://github.com/namics/NitroNet/blob/master/docs/configuration.md) to configure your NitroNet for Sitecore installation.

### Generate C# models based on your Json's (Experimental)

Since Release 1.1.2, NitroNet supports the generation of C# classes based on the pattern Json-Files of Nitro.

To activate this feature in your Sitecore solution, please follow the instructions under [NitroNet configuration guide](https://github.com/namics/NitroNet/blob/master/docs/configuration.md) and register the additional NitroNet controller especially for Sitecore as follows:

#### Create Custom Route-Definition
Please add a controller registration class to register your NitroNet route into Sitecore such as this example:

```csharp
namespace sitecore82rev170728.Website
{
    using System.Web.Mvc;
    using System.Web.Routing;

    public class RegisterRoutes
    {
        public void Process(Sitecore.Pipelines.PipelineArgs args)
        {
            RouteTable.Routes.MapRoute("controller", "nitronet/{controller}/{action}", new
            {
                action = "Index"
            });
        }
    }
}
```
#### Patch your Route-Definition
... and register your route definition before Sitecore initializes his `ControllerFactory`:

```xml
<configuration xmlns:patch="http://www.sitecore.net/xmlconfig/">
  <sitecore>
    <pipelines>
      <initialize>
        <processor type="sitecore82rev170728.Website.RegisterRoutes, sitecore82rev170728.Website" 
            patch:before="processor[@type='Sitecore.Mvc.Pipelines.Loader.InitializeControllerFactory, Sitecore.Mvc']" 
        />
      </initialize>
    </pipelines>
  </sitecore>
</configuration>
```

Based on the Route-Definition defined in `RegisterRoutes` you can generate C# POCO's after entering `http(s)://yourSitecoreSolutionUrl/nitronet/modelbuilder/` in your browser, such as presented in [NitroNet configuration guide](https://github.com/namics/NitroNet/blob/master/docs/configuration.md).