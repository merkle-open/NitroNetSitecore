### New Features / Enhancements
- NitroNet updated to version **1.1.1**
	- You can find the release notes [here](https://github.com/namics/NitroNet/releases/tag/1.1.1.0)
- Prereleases are now supported by the build routine and delivered via nuget.org if necessary
- The NuGet distribution was updated to version `3.5`
- The assembly infos have been unified and updated
- There will be Sitecore specific NuGets. Currently there are Sitecore 8.1 and 8.2 NuGets.
- The .nuspec infos have been unified and updated
- The SharpZipLib NuGet has been removed from the project because it is no longer used
- The NuGet `NitroNet.Sitecore.Microsoft.DependencyInjection.Sitecorexx` was added to the project
- Greatly improved documentation:
	- Updated, corrected and extended
	- Better syntax highligthing of code examples
- ...and many code refactorings and small improvements

### Fixed Issues
- `NitroNet.UnityModules` and `NitroNet.CastleWindsorModules` now reference the correct `NitroNet.Sitecore` dependency
- Build numbers have been fixed
- Unused DLLs were removed
- The Caching of components has been fixed and works now corrects in Sitecore (Sitecore support DLLs are also included)
- NitroNet.Sitecore.dll was not updated respectively installed properly. This has been fixed.
- It is now possible to have several files in a component folder which contain hyphens in the file name. Before it was only possible to have one file with hyphens.
- ...and many small bug-fixes

### Removed Features / Breaking Changes
- The controller parameters `skin` and `dataVariation` have been renamed to `template` and `data` to meet the naming convention of *Nitro*
- AsyncLocal has been removed. It was legacy code from the previous project and is no longer needed.

### Update/Installation Instructions

#### NitroNet upgrade instructions
Please consult the [release notes of NitroNet 1.1.1](https://github.com/namics/NitroNet/releases/tag/1.1.1.0).

#### Upgrading the NitroNet.Sitecore NuGet
It is not possible to just update the NitroNet.Sitecore NuGet in Visual Studio. You have to uninstall it and then install the new Sitecore specific NuGet of NitroNet.Sitecore. This is because we change the names of the NuGets to have specific Sitecore version compatibility.

#### Guide to renaming the controller parameters
You have to do a Find/Replace operation in your solution in your controller classes.  
Then you need to replace `skin` with `template` and `dataVariation` with `data`.

Example:

```csharp
//Before
public ActionResult Index(string skin, string dataVariation)
{
	//Your code
}
```

```csharp
//After
public ActionResult Index(string template, string data)
{
	//Your code
}
```