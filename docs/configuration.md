## Table of contents
- [Installation](installation.md)
- [Configuration](configuration.md)
- [Getting started](getting-started.md)
- [Samples](samples.md)
- [Known Issues](known-issues.md)

## Configuration

### Change the location of your Nitro application
The location of your Nitro application can be configured very flexibly.

As default it is set to be located at the root folder of your web application. If you like to change it, you can add an AppSetting with the Key-Value *NitroNet.BasePath* in your *Web.config*:

```xml
<configuration>
  <appSettings>
    <add key="NitroNet.BasePath" value="[path of your choice]" />
  </appSettings>
</configuration>
```

### Change the Nitro file paths
In addition, you got a new `nitronet-config.json.example`-File in the root directory of the website project after installation of NitroNet for Sitecore. Rename it to `nitronet-config.json` to activate the config.

```json
{
  "viewPaths": [
    "frontend/views/"
  ],
  "partialPaths": [
    "frontend/views/_partials",
  ],
  "componentPaths": [
    "frontend/patterns/atoms",
    "frontend/patterns/molecules",
    "frontend/patterns/organisms",
  ],
  "extensions": [
    "hbs",
    "html"
  ],
    "filters": [
    ".*?\\/template\\/([\\w][^\\/]+)$",
    ".*?\\/spec\\/([\\w][^\\/]+)$"
 ]
}
```

Explanation to the individual settings/properties:
* **viewPaths**: The file path to your views, starting at your `NitroNet.BasePath`
* **partialPaths**: The file path to your partials, starting at your `NitroNet.BasePath`
* **componentPaths**: The file path to your components, starting at your `NitroNet.BasePath`
* **extensions**: The extensions of your handlebar files.
* **filters**: File paths which match with the `filters` regex are being ignored

That's all about view logic resolving of Nitro.