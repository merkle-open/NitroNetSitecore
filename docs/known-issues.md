## Table of contents
- [Installation](installation.md)
- [Configuration](configuration.md)
- [Getting started](getting-started.md)
- [Samples](samples.md)
- [Release Notes](https://github.com/namics/NitroNetSitecore/releases)
- [Known Issues](known-issues.md)

## Limitations

### Caching of components called by `Html.Sitecore().Rendering(pathOrId)`
The Sitecore versions 8.1 and 8.2 have a bug that the method mentioned above doesn't read the cache settings correctly. As NitroNet for Sitecore uses this internally to render components where a Sitecore controller rendering exists for, those components won't be cached correctly.

Sitecore provides two support DLLs for these issues (387950 and 414987) which are now included in the NuGet package.

As explained in the [samples](samples.md) there is the possibility to statically add subcomponents to another component and those subcomponents are rendered via the Sitecore rendering pipeline. NitroNet for Sitecore needs to add this data parameter to the cache key in order for the components to be cached correctly. Therefore we made some further adjustments to the `GenerateCacheKey` processor of the MVC rendering pipeline. This is also automatically included in the NuGet package.

**Important:** If you already have included these fixes or customized the `SetCacheability` or the `GenerateCacheKey` processor in your solution please pay attention!

## Currently not implemented
Please consult the [known issues documentation of NitroNet](https://github.com/namics/NitroNetSitecore/blob/master/docs/known-issues.md) to see which features are currently not supported.