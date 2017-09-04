## Table of contents
- [Installation](installation.md)
- [Configuration](configuration.md)
- [Getting started](getting-started.md)
- [Samples](samples.md)
- [Release Notes](https://github.com/namics/NitroNetSitecore/releases)
- [Known Issues](known-issues.md)

## Getting started with NitroNet for Sitecore

Please see the [getting started guide of NitroNet](https://github.com/namics/NitroNet/blob/master/docs/getting-started.md) to learn about the basic MVC features of NitroNet.

### Create a Layout in Sitecore

Create a layout item below `/sitecore/layout/Layouts` and set the `Path` field relative to the path you configured in the setting `NitroNet.BasePath`. Please make sure that you set the file name without the file extension.

*Important:* The path must not have a leading slash.

#### Example
Assumption:
- The layout file path is `/Nitro/Sample/frontend/views/layout.html`
- `NitroNet.BasePath` setting is `Nitro/Sample`

Conclusion: The resulting `Path` field value of the layout item is `frontend/views/layout`

### Create a Controller rendering in Sitecore

You only need to create Controller renderings for static components or renderings you want to place on a placeholder. But when creating a Sitecore Controller rendering for a Nitro component you also have the big advantage that you can configure the Sitecore caching for this component.
For more information on this matter please follow the [Samples](samples.md) page.

Create a Controller rendering item below `/sitecore/layout/Renderings` and set the item name accoording to the components directory name (hyphens and case sensitivity can be ignored). Also set the `Controller` field to the controller you have created as shown in the example above.