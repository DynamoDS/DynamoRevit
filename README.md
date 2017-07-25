### Dynamo for Revit
**Dynamo for Revit** is a plugin for Revit and a library of [Dynamo](https://github.com/DynamoDS/Dynamo) Nodes.

**Dynamo for Revit** has different branches for different versions of Revit. For example to run **Dynamo for Revit** on Revit 2016 you want the **Dynamo for Revit** 2016 branch.

#### To Build
- Clone the DynamoRevit repository.
```
git clone https://github.com/DynamoDS/DynamoRevit.git
```
- Run `restorepackages.bat` with administrative privileges.
- Open DynamoRevit.2013.sln in Visual Studio, and select a build confiuration (Debug | Release)
- Build

After DynamoRevit is built, you will notice that there is a `Dynamo.config` file in DynamoRevit\bin\AnyCPU\Debug[Or Release]. With this file you can specify which DynamoCore you want to run with the DynamoRevit built.

For example, if you specify
```
<add key="DynamoRuntime" value="D:\Dynamo\bin\AnyCPU\Debug"/>
```
you will run DynamoRevit with the DynamoCore at `D:\Dynamo\bin\AnyCPU\Debug`. This is especially useful when you want to test the DynamoRevit built with different flavors of DynamoCore. Or you are using RTF to test a built version of DynamoRevit.

#### Creating an Addin
DynamoRevit requires an addin which instructs Revit to load the plugin on startup. The Dynamo for Revit installer takes care of building an addin for use on the end user's machine. For development, you'll have to create an addin manually. A DynamoRevit addin would look like this:

```
<?xml version="1.0" encoding="utf-8" standalone="no"?>
<RevitAddIns>
<AddIn Type="Application">
<Name>Dynamo For Revit</Name>
<Assembly>"E:\MyGitPath\Dynamo\bin\AnyCPU\Debug\Revit_xxxx\DynamoRevitVersionSelector.dll"</Assembly>
<AddInId>8D83C886-B739-4ACD-A9DB-1BC78F315B2B</AddInId>
<FullClassName>Dynamo.Applications.VersionLoader</FullClassName>
<VendorId>ADSK</VendorId>
<VendorDescription>Dynamo</VendorDescription>
</AddIn>
</RevitAddIns>
```

or

```
<?xml version="1.0" encoding="utf-8" standalone="no"?>
<RevitAddIns>
<AddIn Type="Application">
<Name>Dynamo For Revit</Name>
<Assembly>"D:\DynamoRevit\bin\AnyCPU\Debug\Revit_2017\DynamoRevitDS.dll"</Assembly>
<AddInId>8D83C886-B739-4ACD-A9DB-1BC78F315B2B</AddInId>
<FullClassName>Dynamo.Applications.DynamoRevitApp</FullClassName>
<VendorId>ADSK</VendorId>
<VendorDescription>Dynamo</VendorDescription>
</AddIn>
</RevitAd
```
These two addin files are mostly same but the second one specifies which version of DynamoRevit you want to load with Revit without going through version selector at all. This could be useful sometimes when you have multiple versions of DynamoRevit built or installed, thus you want to be more specific which one you are testing against.

This .addin file should be placed in one of the following locations:
-  `ProgramData/Autodesk/Revit/Addins/<version>`
-  `Users/<user>/AppData/Roaming/Autodesk/Revit/Addins/<version>`
-  `ProgramFiles/Autodesk/Revit <version>/AddIns`

where `<user>` is the user who will be using the addin and `<version>` is the version of Revit for which the addin is built. Notice that the `Assembly` tag points to the VersionSelector.dll that is built during a build of  **Dynamo for Revit** and copied into the Dynamo/bin/Revitxxx/ folder.



#### Dynamo for Revit requires a few dependencies
* Dynamo from http://www.github.com/DyanmoDS/Dynamo. The required binaries are obtained from DynamoVisualProgramming, NuGet Packages.
* RevitAPI.dll + RevitAPIUI.dll (path set with *REVITAPI*)
* IronPython 2.7 (this is already installed if you install Dynamo)
* Few other dependencies are also fetched from NuGet package.

#### Build Issues
* NuGet packages are downloaded using [restorepackages.bat](https://github.com/DynamoDS/DynamoRevit/blob/Revit2017/src/restorepackages.bat), it creates soft link for all the NuGet packages folder dropping the version information so that the projects files doesn't need to be changed when package versions are changed. The package versions are defined in [packages-template.aget](https://github.com/DynamoDS/DynamoRevit/blob/Revit2017/src/Config/packages-template.aget) file. LatestBeta is used for Dynamo specific packages to automatically download the latest beta packages. 

* restorepackages.bat file is run as a pre-build step for AssemblySharedInfoGenerator project. Sometimes this file may fail to create the symbolic links in absence of administrative privilege. To fix the issue you can run restorepackages.bat file as administrator.

*  if you see errors like: ```1>  "C:\Program Files (x86)\Common Files\microsoft shared\TextTemplating\11.0\TextTransform.exe" -out AssemblySharedInfo.cs AssemblySharedInfo.tt
1>c:\Users\bykovsm\AppData\Local\Temp\AssemblySharedInfo.tt(1,1): error CS1519: Compiling transformation: Invalid token 'this' in class, struct, or interface member declaration
1>c:\Users\bykovsm\AppData\Local\Temp\AssemblySharedInfo.tt(1,6): error CS1520: Compiling transformation: Method must have a return type```  	then you need to get rid of any white space in the last line of *DynamoRevit/src/AssemblyInfoGenerator/transform_all.tt*
it's also possible that *transform_all.bat* is looking for a text templating engine for a version of visual studio you do not have installed.

* if you see missing classes or namespaces from the Revit or Dynamo APIS see here: [CS.props](https://github.com/DynamoDS/DynamoRevit/blob/Revit2017/src/Config/CS.props) contains environment variable **_DYNAMOAPI_** and **_REVITAPI_** to define path for Dynamo and Revit libraries respectively.  These environment variables can be overwritten by providing correct path for Dynamo and Revit libraries in [user_locals.props](https://github.com/DynamoDS/DynamoRevit/blob/Revit2017/src/Config/user_local.props)

####Note####
the installer structure for DynamoCore and DynamoRevit has recently changed - if you have trouble with these instructions please reach out to the Dynamo team for help.
