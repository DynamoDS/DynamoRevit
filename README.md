DynamoRevit
===========

Dynamo libraries and ExternalApplication implementation for Revit.

Branches:
- Revit2015 : Built on Revit2015 API, works only for Revit2015
- Revit2016 : Built on Revit2016 API, works only for Revit2016
- Revit2017 : Built on Revit2017 API, works only for Revit2017


### Building DynamoRevit

#### Background Info
**DynamoRevit** is a library of Dynamo Nodes as well as a set of utilities for interacting with Revit via the Dynamo visual programming language.

**DynamoRevit** has different branches for different versions of Revit. For example to run **DynamoRevit** on Revit 2016 you want the **DynamoRevit** 2016 branch.
**DynamoRevit** is also built on top of Dynamo, and in some cases requires matching the version of Dynamo as well. For example, to run **DynamoRevit** on Revit 2016 using Dynamo .91 you want to build The Dynamo .91 Branch, then build the **DynamoRevit** .91 2016 branch.


####Steps
Download Dynamo from http://www.github.com/DyanmoDS/Dynamo
Download **DynamoRevit** from here.
Build Dynamo.all.sln
Build DynamoRevit.2013.sln

if you have a version of revit installed, you can build an installer from Dynamo/Tools/Install/...

if you do not have an installed version of revit, you can create an .addin file that points to the Dynamo application manually:
in the correct Revit adding location for your application add a text file as follows:

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

you want to point the addin button thats loaded into Revit to the VersionSelector.dll that is built during a build of  **DynamoRevit** and copied into the Dynamo/bin/Revitxxx/ folder.


#### DynamoRevit requires a few dependencies
* Dynamo from http://www.github.com/DyanmoDS/Dynamo (path set with *DYNAMOAPI*)
* RevitAPI.dll + RevitAPIUI.dll (path set with *REVITAPI*)
* IronPython 2.7 (this is already installed if you install Dynamo)
* 

####Build Issues
* make sure Dynamo and DynamoRevit are in the same root directory and are named exactly "Dynamo" and "DynamoRevit" (it's easiest to just pull both repos from github directly)

*  if you see errors like: ```1>  "C:\Program Files (x86)\Common Files\microsoft shared\TextTemplating\11.0\TextTransform.exe" -out AssemblySharedInfo.cs AssemblySharedInfo.tt
1>c:\Users\bykovsm\AppData\Local\Temp\AssemblySharedInfo.tt(1,1): error CS1519: Compiling transformation: Invalid token 'this' in class, struct, or interface member declaration
1>c:\Users\bykovsm\AppData\Local\Temp\AssemblySharedInfo.tt(1,6): error CS1520: Compiling transformation: Method must have a return type```  	then you need to get rid of any white space in the last line of *DynamoRevit/src/AssemblyInfoGenerator/transform_all.tt*
it's also possible that *transform_all.bat* is looking for a text templating engine for a version of visual studio you do not have installed.

* if you see missing classes or namespaces from the Revit or Dynamo APIS see here: [CS.props](https://github.com/DynamoDS/DynamoRevit/blob/Revit2015/src/Config/CS.props) contains environment variable **_DYNAMOAPI_** and **_REVITAPI_** to define path for Dynamo and Revit libraries respectively.  These environment variables can be overwritten by providing correct path for Dynamo and Revit libraries in [user_locals.props](https://github.com/DynamoDS/DynamoRevit/blob/Revit2015/src/Config/user_local.props)

####Note####
the installer structure for DynamoCore and DynamoRevit has recently changed - if you have trouble with these instructions please reach out to the Dynamo team for help.