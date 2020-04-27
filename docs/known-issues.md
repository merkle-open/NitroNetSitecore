## Table of contents
- [Installation](installation.md)
- [Configuration](configuration.md)
- [Getting started](getting-started.md)
- [Samples](samples.md)
- [Release Notes](https://github.com/namics/NitroNetSitecore/releases)
- [Known Issues](known-issues.md)

## Limitations

### Caching of components called by `Html.Sitecore().Rendering(pathOrId)`
Sitecore 8.2 (and 8.1) has a bug that the method mentioned above doesn't read the cache settings correctly. As NitroNet for Sitecore uses this internally to render components where a Sitecore controller rendering exists for, those components won't be cached correctly.

Therefore we added the pipeline processor `NitroNet.Sitecore.Pipelines.MvcRenderRendering` which replaces the default one from Sitecore.

In addition, as explained in the [samples](samples.md), there is the possibility to statically add subcomponents to another component and those subcomponents are rendered via the Sitecore rendering pipeline. NitroNet for Sitecore needs to add this data parameter to the cache key in order for the components to be cached correctly. Therefore we made some further adjustments to the `GenerateCacheKey` processor of the MVC rendering pipeline.

These two pipeline processors get activated with the shipped `MvcRenderingPipeline.config`.

**Important:** Please pay attention if you have already included the official Sitecore fixes (*Sitecore.Support.387950* and *Sitecore.Support.414987*) or customized the `SetCacheability` or the `GenerateCacheKey` processor in your solution!

## Currently not implemented
Please consult the [known issues documentation of NitroNet](https://github.com/namics/NitroNet/blob/master/docs/known-issues.md) to see which features are currently not supported.