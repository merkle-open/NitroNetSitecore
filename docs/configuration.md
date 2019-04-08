## Table of contents
- [Installation](installation.md)
- [Configuration](configuration.md)
- [Getting started](getting-started.md)
- [Samples](samples.md)
- [Release Notes](https://github.com/namics/NitroNetSitecore/releases)
- [Known Issues](known-issues.md)

## Configuration

At firs, please go through the [configuration guide of NitroNet](https://github.com/namics/NitroNet/blob/master/docs/configuration.md) to configure your *NitroNet for Sitecore* installation.

### *Optional:* Exlude certain rendering paths
You have the possibility to exclude certain rendering paths.  
This can be useful when using Sitecore modules like SXA or EXM where you could ran into the situation that you have duplicate rendering names. This is not allowed with NitroNet. So therefore you can exlude all renderings which are not used with NitroNet.

Create a patch config in your solution like this and define your desired excluded paths:

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration xmlns:patch="http://www.sitecore.net/xmlconfig/" >
	<sitecore>
		<settings>
			<!--  Pipe separated list containing paths inside renderings folder. All renderings below these paths are excluded from NitroNet. -->
			<setting name="NitroNet.Sitecore.RenderingExclusions">
				<patch:attribute name="value">/sitecore/layout/Renderings/Sample|/sitecore/layout/Renderings/System</patch:attribute>
			</setting>
		</settings>
	</sitecore>
</configuration>
```