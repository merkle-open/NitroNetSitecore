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

This guide explains all exceptional cases of NitroNet and talks about features of Nitro which aren't implemented in NitroNet yet.

**The Nitro documentation**
First of all: You can find the full documentation of Nitro under this [link](https://github.com/namics/generator-nitro/blob/master/generators/app/templates/project/docs/nitro.md).

### Validation with the `schema.json`
Nitro validates the patterns against their `schema.json`.
It is currently not possible to validate the `C#` models against the `schema.json` of the corresponding Nitro patterns.

### Flexible attributes on component helper
You will find the Nitro documentation for the flexible attribute [here](https://github.com/namics/generator-nitro/blob/master/generators/app/templates/project/docs/nitro.md#render-patterns).

```
{{pattern 'example' modifier='blue'}}
```

In the current version NitroNet does not support this feature. You need to pass another data variation:
```
{{pattern 'example' data='example-blue'}}
```

### Render patterns with children
You will find the Nitro documentation for patterns with children [here](https://github.com/namics/generator-nitro/blob/master/generators/app/templates/project/docs/nitro.md#render-patterns-with-children).

```
{{#pattern 'box'}}
    {{pattern 'example'}}
{{/pattern}}
```

In the current version NitroNet does not support this feature. Please use the placeholder feature for this use case.

### Pattern elements
You will find the Nitro documentation for pattern elements [here](https://github.com/namics/generator-nitro/blob/master/generators/app/templates/project/docs/nitro.md#creating-pattern-elements) and [here](https://github.com/namics/generator-nitro/blob/master/generators/app/templates/project/docs/nitro.md#render-pattern-elements).

The current version NitroNet does not support this feature. Please use ordinary patterns for this use case.

### Partials with handlebars expressions
Currently it is not possible to have partials with handlebars expressions. Only static markup is supported.

Use the `pattern` handlebars helper to achieve this functionality:
```
{{pattern name='head'}}
```

### Translation handlebars helper
Nitro ships with a Handlebars helper called `t`. This is not supported in NitroNet as there is no need for such a helper in ASP.NET. You can simply use a string expressions in its place and fill the according model property dynamically with data from any source.