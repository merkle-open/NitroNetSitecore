Please also see the [release notes of *NitroNet*](https://github.com/namics/NitroNet/releases/tag/1.1.0.0) to have a complete picture.

### New Features / Enhancements
- Greatly improved documentation:
	- Updated, corrected and extended
	- Better syntax highligthing of code examples
- Prereleases are now supported by the build routine and delivered via nuget.org if necessary
- The NuGet distribution was updated to version `3.5`
- The assembly infos have been unified and updated
- The .nuspec infos have been unified and updated
- The SharpZipLib NuGet has been removed from the project because it is no longer used
- The NuGet `NitroNet.Sitecore.Microsoft.DependencyInjection.Sitecorexx` was added to the project
- ...and many code refactorings and small improvements

### Fixed Issues
- `NitroNet.UnityModules` and `NitroNet.CastleWindsorModules` now reference the correct `NitroNet.Sitecore` dependency
- Build numbers have been fixed
- Unused DLLs were removed
- The Caching of components has been fixed and works now corrects in Sitecore (Sitecore support DLLs are also included)
- ...and many more small bug-fixes

### Removed Features / Breaking Changes
- The controller parameters `skin` and `dataVariation` have been renamed to `template` and `data` to meet the naming convention of *Nitro*

### Update/Installation Instructions

TODO