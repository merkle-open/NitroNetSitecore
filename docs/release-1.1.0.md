### New Features / Enhancements
- NitroNet updated to version **1.1.0**
	- You can find the release notes [here](https://github.com/namics/NitroNet/releases/tag/1.1.0.0)
- Prereleases are now supported by the build routine and delivered via nuget.org if necessary
- The NuGet distribution was updated to version `3.5`
- The assembly infos have been unified and updated
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
- ...and many small bug-fixes

### Removed Features / Breaking Changes
- The controller parameters `skin` and `dataVariation` have been renamed to `template` and `data` to meet the naming convention of *Nitro*

### Update/Installation Instructions

**NitroNet configuration upgrade informations**:
Please consult the [release notes of NitroNet 1.1.0](https://github.com/namics/NitroNet/releases/tag/1.1.0.0).

**Renaming of the controller parameters informations**
You have to do a Find/Replace operation in your solution in your controller classes.  
You need to replace `skin` with `template` and `dataVariation` with `data`.

Example:

```
//Before
public ActionResult Index(string skin, string dataVariation){
	[...]
}
```

```
//After
public ActionResult Index(string template, string data){
	[...]
}
```