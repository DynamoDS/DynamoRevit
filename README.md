# Dynamo for Revit

**Dynamo for Revit** is a plugin for Revit and a library of [Dynamo](https://github.com/DynamoDS/Dynamo) nodes. It is also commonly referred to as **DynamoRevit** or **D4R**.

**Dynamo for Revit** maintains separate branches for different Revit versions. For example, to run **Dynamo for Revit** on Revit 2016, use the **Dynamo for Revit** 2016 branch.

## How to build and use DynamoRevit

To use a locally built DynamoRevit plugin within Revit, complete the following steps:

1. [Prerequisites](#prerequisites)
2. [Build DynamoRevit](#1-build-dynamorevit)
3. [Get or Build Dynamo Core](#2-get-or-build-dynamo-core)
4. [Associate DynamoRevit with DynamoCore](#3-associate-dynamorevit-with-dynamo-core)
5. [Create a Revit Addin](#4-create-a-revit-addin)

### Prerequisites

Ensure the following software is installed on your system:

- **Revit**: Revit 2026 (.NET 8), Revit 2027 or later (.NET 10)
- **Visual Studio**: Visual Studio 2022 (17.8.0 or newer)
- **.NET SDK**: [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) for Windows x64
- **.NET SDK**: [.NET 10 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/10.0) for Windows x64

### 1. Build DynamoRevit

- Clone the DynamoRevit repository:

  > `git clone https://github.com/DynamoDS/DynamoRevit.git`

- Check out the branch corresponding to your target Revit version:

  - For the latest Revit release: `git checkout master`
  - For older Revit releases: `git checkout Revit2026`

- Open `DynamoRevit.All.sln` in Visual Studio and select a build configuration:

  - Configuration: `Debug` or `Release`
  - Platform: `NET80` (for Revit 2025 and later)

- In `src/AssemblySharedInfoGenerator/AssemblySharedInfo.tt`, change `MajorVersion` from `27` to `4` to match DynamoCore.dll version `4.0.0.xxxx`. This workaround is only needed for Revit 2027; post-Dynamo 4.0 versions will use the host-specified version.

  ```cs
  int MajorVersion = 4; // Change from `27`
  int MinorVersion = 0;
  int BuildNumber = 0;
  ```

- Configure `src/Config/user_local.props` by setting the `RevitVersionNumber` to match your installed Revit version (e.g., `2025` or `2026`).

  - If Revit is not installed or you need to override the default location, also specify a custom `REVITAPI` path in `user_local.props`.

- Build `DynamoRevit` solution to produce the right version of `DynamoRevitDS.dll` (i.e. `4.0.0.xxxx` in this case).

#### Revit API DLL Lookup Order

The build process automatically locates Revit API DLLs in the following order:

1. Custom `REVITAPI` path defined in `src/Config/user_local.props` (highest priority if set)
2. `$(SolutionDir)..\lib\Revit $(RevitVersionNumber)\net8.0`
3. `%ProgramFiles%\Autodesk\Revit Architecture $(RevitVersionNumber)`
4. `%ProgramFiles%\Autodesk\Revit $(RevitVersionNumber)`

### 2. Get or Build Dynamo Core

Building both DynamoRevit and DynamoCore from source is often beneficial for development. To build from source:

- Clone Dynamo Core from https://github.com/dynamods/dynamo
- Check out the branch for your target Dynamo version. For the latest Revit release or preview releases, use the `master` branch. For release candidate testing, select the appropriate branch (e.g., `RC4.0.0_master`).
- Build Dynamo following the instructions in the [README](https://github.com/DynamoDS/Dynamo/blob/master/README.md).

Alternatively, you can use the prebuilt Dynamo that ships with Revit or retrieve a specific Dynamo Core version from NuGet without building from source. Documentation for these approaches is pending.

### 3. Associate DynamoRevit with Dynamo Core

After building DynamoRevit, a `Dynamo.config` file will be generated at `DynamoRevit\bin\NET100\Debug\Dynamo.config` (or `Release`). This configuration file specifies which DynamoCore installation to use with your DynamoRevit build.

For example, the following configuration:

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <appSettings>
     <add key="DynamoRuntime" value="C:\my\repos\Dynamo\bin\AnyCPU\Debug"/>
  </appSettings>
</configuration>
```

will configure DynamoRevit to use the DynamoCore located at `C:\my\repos\Dynamo\bin\AnyCPU\Debug`. This configuration is particularly useful for testing DynamoRevit with different DynamoCore versions or when using the Revit Test Framework (RTF) with a custom build. Note that rebuilding will overwrite this file, requiring you to update the path after each build.

### 4. Create a Revit Addin

Beginning with Revit 2020, DynamoCore and the DynamoRevit addin are included in the Revit installation folder. For development purposes, first remove or delete the DynamoForRevit addin folder from: `%ProgramFiles%\Autodesk\Revit 2020\AddIns` (or `%ProgramFiles%\Autodesk\Revit Preview Release\AddIns` for pre-RTM installation of Revit).

For development, you must manually create an addin file that instructs Revit to load your custom plugin on startup. A `DynamoForRevit.addin` file should be structured as follows:

```xml
<?xml version="1.0" encoding="utf-8" standalone="no"?>
<RevitAddIns>
  <AddIn Type="Application">
    <Name>Dynamo For Revit</Name>
    <Assembly>"C:\my\repos\DynamoRevit\bin\NET100\Debug\Revit\DynamoRevitDS.dll"</Assembly>
    <AddInId>8D83C886-B739-4ACD-A9DB-1BC78F315B2B</AddInId>
    <FullClassName>Dynamo.Applications.DynamoRevitApp</FullClassName>
    <VendorId>ADSK</VendorId>
    <VendorDescription>Dynamo</VendorDescription>
  </AddIn>
</RevitAddIns>
```

Place this `DynamoForRevit.addin` file in the following location:

- `%AppData%\Autodesk\Revit\Addins\<version>`

where `<version>` corresponds to your target Revit version. Note that the `Assembly` tag must point to the output folder of the **Dynamo for Revit** build created in step 1.

After completing these steps, launch Revit to verify that the Dynamo and Dynamo Player icons appear on the Manage tab. If you encounter issues, refer to the troubleshooting section below.

### Build DynamoRevit Against a Local Dynamo Version

1. Build Dynamo locally
2. Execute the Dynamo NuGet script (detailed instructions [here](https://github.com/DynamoDS/Dynamo/blob/master/tools/NuGet/README.md)):
   `.\BuildPackages.bat "template-nuget" "...\GitHub\Dynamo\bin\AnyCPU\Release"`
   This command generates all Dynamo NuGet packages locally.
3. In Visual Studio, configure the NuGet package sources to reference your local NuGet folder containing the generated packages.
   ![alt text](images/local_dynamo_nugets.png)

## Troubleshooting Build Issues

- If you encounter errors similar to:

  ```1> "C:\Program Files (x86)\Common Files\microsoft shared\TextTemplating\11.0\TextTransform.exe" -out AssemblySharedInfo.cs AssemblySharedInfo.tt
  1>c:\Users\bykovsm\AppData\Local\Temp\AssemblySharedInfo.tt(1,1): error CS1519: Compiling transformation: Invalid token 'this' in class, struct, or interface member declaration
  1>c:\Users\bykovsm\AppData\Local\Temp\AssemblySharedInfo.tt(1,6): error CS1520: Compiling transformation: Method must have a return type
  ```

  Remove any trailing whitespace from the last line of _DynamoRevit/src/AssemblyInfoGenerator/transform_all.tt_. This error may also occur if _transform_all.bat_ references a text templating engine for a Visual Studio version that is not installed on your system.

- If you encounter missing classes or namespaces from the Revit or Dynamo APIs, review the environment variable values in [CS_SDK.props](https://github.com/DynamoDS/DynamoRevit/blob/master/src/Config/CS_SDK.props). These environment variables can be overridden by specifying the correct paths for Dynamo and Revit libraries in [user_local.props](https://github.com/DynamoDS/DynamoRevit/blob/master/src/Config/user_local.props).

- If your addin does not appear in Revit, remove any existing copies of the Dynamo.addin file from these locations:

  - `Users/<user>/AppData/Roaming/Autodesk/Revit/Addins/<version>`
  - `ProgramFiles/Autodesk/Revit <version>/AddIns`

- Revit 2020 and later versions do not use the DynamoVersionSelector by default, though it remains available in the DynamoRevit build. To use the DynamoVersionSelector, create a Dynamo.addin file with the following structure:

```xml
<?xml version="1.0" encoding="utf-8" standalone="no"?>
<RevitAddIns>
  <AddIn Type="Application">
    <Name>Dynamo For Revit</Name>
    <Assembly>"C:\my\repos\Dynamo\bin\AnyCPU\Debug\Revit_xxxx\DynamoRevitVersionSelector.dll"</Assembly>
    <AddInId>8D83C886-B739-4ACD-A9DB-1BC78F315B2B</AddInId>
    <FullClassName>Dynamo.Applications.VersionLoader</FullClassName>
    <VendorId>ADSK</VendorId>
    <VendorDescription>Dynamo</VendorDescription>
  </AddIn>
</RevitAddIns>
```

## Running DynamoRevit Tests with RevitTestFramework

_Note: This documentation is currently under development and being compiled from internal sources._
For comprehensive information, refer to https://github.com/DynamoDS/RevitTestFramework/blob/master/README.md

### Option 1: RevitTestFrameworkConsole.exe

This console application enables running RTF without a user interface. To view available command line options for RTF, execute "RevitTestFrameworkConsole -h".

Example command:

`RevitTestFrameworkConsole.exe --dir [DynamoRevit dev root]\test\System -a [DynamoRevit dev root]\bin\AnyCPU\Debug\Revit\RevitSystemTests.dll -r MyTestResults.xml -revit:"C:\Program Files\Autodesk\Revit 2019\Revit.exe" --copyAddins --continuous`

This command executes all tests in MyTest.dll located in C:\MyTestDir and outputs results to MyTestResults.xml (in the same directory). It uses Revit 2019 as specified and runs all tests continuously without shutting down Revit.

For development packages, use this alternative example:

`RevitTestFrameworkConsole.exe --dir [DynamoRevit dev root]\test\System -a [DynamoRevit dev root]\bin\AnyCPU\Debug\Revit\RevitSystemTests.dll -r MyTestResults.xml -revit:"D:\Revit\Revit.exe" --continuous`

When specifying a non-standard Revit.exe installation path, the '--copyAddins' parameter is not required. However, you must manually copy a Dynamo.addin file to your working directory 'C:\MyTestDir'.

### Option 2: RevitTestFrameworkGUI.exe (Supports Revit Installation Builds Only)

This tool provides a visual interface for selecting tests from a tree view and monitoring test execution results in real-time. All settings available through command line arguments are accessible in the UI, and the interface supports saving testing sessions.

The input fields for test assembly, working directory, and results file, along with the test tree view, support drag-and-drop functionality for files and folders.

- **Test Assembly**: The DLL containing your tests
- **Working Directory**: The folder containing test Revit files and Dynamo files (e.g., Empty.rvt and D4RCreateWallSystemTests.dyn)
- **Additional Resolution Directories**: The DynamoCore and DynamoRevit locations for test execution (these should correspond to your Dynamo.addin file configuration)
- **Revit Version**: Select your target Revit version from the dropdown (displays only installed Revit versions)
