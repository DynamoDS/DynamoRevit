# Dynamo for Revit
**Dynamo for Revit** is a plugin for Revit and a library of [Dynamo](https://github.com/DynamoDS/Dynamo) Nodes. It's also often referred to as **DynamoRevit** or **D4R** for short.

**Dynamo for Revit** has different branches for different versions of Revit. For example, to run **Dynamo for Revit** on Revit 2016 you want the **Dynamo for Revit** 2016 branch.

## How to build and use DynamoRevit

To use a locally built DynamoRevit plugin inside Revit, you'll need to do three things:
1. [Build DynamoRevit](#1-build-dynamorevit)
2. [Get or Build Dynamo Core](#2-get-or-build-dynamo-core)
3. [Associate DynamoRevit with DynamoCore](#3-associate-dynamorevit-with-dynamo-core)
4. [Create a Revit Addin](#4-create-a-revit-addin)

### 1. Build DynamoRevit

- Clone the DynamoRevit repository.
```
git clone https://github.com/DynamoDS/DynamoRevit.git
```
- Get the branch for the version of Revit you want to use. For the latest release of Revit or a preview release, master may be fine. Otherwise, run `git checkout Revit2019` or similar.
- Make sure you have the following installed on your computer:
   - [.Net Framework 4.8 SDK](https://dotnet.microsoft.com/download) 
- Run `restorepackages.bat` from a command prompt with administrative privileges (Located in your _Github\DynamoRevit\src folder_)
  Note: `restorepackages.bat` employs the use of a legacy tool `aget.exe`, which requires VC++ 2010 redistributable installed, before you can run it. When missing you will get an `msvcr100.dll not found` error. 
- Copy `RevitAPI.dll`&`RevitAPIUI.dll` to the folder `DynamoRevit\lib\Revit Preview Release\net48`, these 2 dlls are in the folder same with `Revit.exe` installed on your computer
	(if you want to build other branch of DynamoRevit, but corresponding version of Revit is not installed locally, you can get these dlls from https://www.nuget.org/ )
- Set the `RevitVersionNumber` environment variable to the Revit version you're building against (e.g. `2020`) either in the system environment or in the [user_locals.props](https://github.com/DynamoDS/DynamoRevit/blob/Revit2017/src/Config/user_local.props) file in your build folder.
- Open `DynamoRevit.All.sln` in Visual Studio, and select a build configuration (Debug | Release)

### 2. Get or Build Dynamo Core

It's often helpful to build both DynamoRevit and Dynamo Core from source. To do that:
- Clone Dynamo Core from https://github.com/dynamods/dynamo
- Get the branch for the version of Dynamo that you want to use. For the latest release of Revit or a preview release, master is fine. Otherwise, run `git checkout RC2.2.0_master` or similar.
- Build Dynamo according to the instructions in the [README](https://github.com/DynamoDS/Dynamo/blob/master/README.md).

It's also possible to use the prebuilt Dynamo that ships with Revit, or to retrieve a particular Dynamo Core version from Nuget without building it yourself. These approaches need to be documented.

### 3. Associate DynamoRevit with Dynamo Core

After DynamoRevit is built, you will notice that there is a `Dynamo.config` file in DynamoRevit\bin\AnyCPU\Debug[Or Release]. With this file you must specify which DynamoCore you want to run with the DynamoRevit build.

For example, if you specify
```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <appSettings>
     <add key="DynamoRuntime" value="C:\Workspace\GitHub\Dynamo\bin\AnyCPU\Debug"/>
  </appSettings>
</configuration>
```
you will run DynamoRevit with the DynamoCore at `C:\Workspace\GitHub\Dynamo\bin\AnyCPU\Debug`. This is especially useful when you want to test the DynamoRevit built with different flavors of DynamoCore, or you are using RTF to test a built version of DynamoRevit. Rebuilding will overwrite this file, so you must replace the path each time you build.

### 4. Create a Revit Addin

Starting in Revit 2020, there is a version of DynamoCore and the DynamoRevit addin included in the Revit install folder. First, remove or delete the DynamoForRevit addin folder from the following location: `C:\Program Files\Autodesk\Revit 2020\AddIns`

For development, you'll have to manually create an addin file that instructs Revit to load the plugin on startup. A DynamoRevit addin file looks like this:

```xml
<?xml version="1.0" encoding="utf-8" standalone="no"?>
<RevitAddIns>
  <AddIn Type="Application">
    <Name>Dynamo For Revit</Name>
    <Assembly>"D:\DynamoRevit\bin\AnyCPU\Debug\Revit\DynamoRevitDS.dll"</Assembly>
    <AddInId>8D83C886-B739-4ACD-A9DB-1BC78F315B2B</AddInId>
    <FullClassName>Dynamo.Applications.DynamoRevitApp</FullClassName>
    <VendorId>ADSK</VendorId>
    <VendorDescription>Dynamo</VendorDescription>
  </AddIn>
</RevitAddIns>
```

This .addin file should be placed in the following location:
-  `ProgramData/Autodesk/Revit/Addins/<version>`

where `<version>` is the version of Revit for which the addin is built. Notice that the `Assembly` tag points to the output folder of the **Dynamo for Revit** build you created in step 1.

Now you should be able to launch Revit and see the Dynamo and Dynamo Player icons on the Manage tab. If you experience issues, check the troubleshooting tips in the next section.

## Troubleshooting Build Issues

* Make sure you ran [restorepackages.bat](https://github.com/DynamoDS/DynamoRevit/blob/Revit2017/src/restorepackages.bat) in a command prompt with administrator privileges. It creates soft links for all the NuGet packages folder dropping the version information so that the projects files don't need to be changed when package versions are changed. The package versions are defined in the [packages-template.aget](https://github.com/DynamoDS/DynamoRevit/blob/Revit2017/src/Config/packages-template.aget) file. LatestBeta is used for Dynamo specific packages to automatically download the latest beta packages. 

* If you see errors like: 
   
   ```1>  "C:\Program Files (x86)\Common Files\microsoft shared\TextTemplating\11.0\TextTransform.exe" -out AssemblySharedInfo.cs AssemblySharedInfo.tt
   1>c:\Users\bykovsm\AppData\Local\Temp\AssemblySharedInfo.tt(1,1): error CS1519: Compiling transformation: Invalid token 'this' in class, struct, or interface member declaration
   1>c:\Users\bykovsm\AppData\Local\Temp\AssemblySharedInfo.tt(1,6): error CS1520: Compiling transformation: Method must have a return type
   ```  	
   then you need to get rid of any white space in the last line of *DynamoRevit/src/AssemblyInfoGenerator/transform_all.tt*. It's also possible that *transform_all.bat* is looking for a text templating engine for a version of visual studio you do not have installed.

* If you see missing classes or namespaces from the Revit or Dynamo APIs, look at the environment variable values in [CS.props](https://github.com/DynamoDS/DynamoRevit/blob/Revit2017/src/Config/CS.props). These environment variables can be overwritten by providing correct path for Dynamo and Revit libraries in [user_locals.props](https://github.com/DynamoDS/DynamoRevit/blob/Revit2017/src/Config/user_local.props)

* If your addin is not appearing in Revit, try removing any old copies of the Dynamo.addin file from these locations:
   -  `Users/<user>/AppData/Roaming/Autodesk/Revit/Addins/<version>`
   -  `ProgramFiles/Autodesk/Revit <version>/AddIns`

* Revit 2020 and later do not use the DynamoVersionSelector by default, but it's still in the DynamoRevit build. If you'd like to try using it, you can create a Dynamo.addin file that looks like this:

```xml
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

## Running DynamoRevit Tests with RevitTestFramework

(This documentation is a work in progress, still being assembled and verified from internal documents)
For more information, see https://github.com/DynamoDS/RevitTestFramework/blob/master/README.md

### Option 1: RevitTestFrameworkConsole.exe

A console application which allows running RTF without a user interface. If you'd like to learn more about the command line options for RTF, you can simply type "RevitTestFrameworkConsole -h".

As an example, the following command:

```RevitTestFrameworkConsole.exe --dir [DynamoRevit dev root]\test\System -a [DynamoRevit dev root]\bin\AnyCPU\Debug\Revit\RevitSystemTests.dll -r MyTestResults.xml -revit:"C:\Program Files\Autodesk\Revit 2019\Revit.exe" --copyAddins --continuous```

will execute all tests in MyTest.dll located in C:\MyTestDir and place all results in MyTestResults.xml (in the same folder). It will use Revit 2019 as specified and will run all tests without shutting down Revit.

If you use dev package, another example:

```RevitTestFrameworkConsole.exe --dir [DynamoRevit dev root]\test\System -a [DynamoRevit dev root]\bin\AnyCPU\Debug\Revit\RevitSystemTests.dll -r MyTestResults.xml -revit:"D:\Revit\Revit.exe" --continuous```

You specified a non-normally installed Revit.exe, you do not need to add the '--copyAddins' parameter. You need to manually copy a Dynamo.addin file to your working directory 'C:\MyTestDir'.

### Option 2: RevitTestFrameworkGUI.exe (This only supports Revit install build)

Provides a visual interface for you to choose tests from a treeview and to visualize the results of the tests as they are run. The same settings provided in the command line argument help above are available in the UI. The UI also allows you to save your testing session.

The input fields to set the test assembly, the working directory, and the results file, as well as the tree view where available tests are displayed, support dragging and dropping of files and folders.

- *Test Assembly* is your dll to test. 
- *Working Directory* is the folder contains your test Revit file and dyn file like Empty.rvt and D4RCreateWallSystemTests.dyn.
- *Additional Resolution Directories* are the DynamoCore and DynamoRevit locations you want to use to run the test (Do they need to match what's in the Dynamo.addin file?)
- *Revit 2020* you can choose which Revit version you want to use in this DropDown List. (Only installed versions of Revit.)

