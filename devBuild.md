# Dev Build (DynamoRevit)

## Quick start

**Prerequisites**
- VS 2022 (.NET desktop)
- DynamoRevitUtils clone as sibling `../DynamoRevitUtils` (or set `WORKSPACE_DYNAMOREVITUTILS`)
- [Artifactory creds](https://autodesk.atlassian.net/wiki/spaces/aeceng/pages/350564841/Setting+up+Artifactory+Credentials) as `REVIT_ARTIFACTORY_USER` / `REVIT_ARTIFACTORY_TOKEN` (or `ARTIFACTORY_USERNAME` / `ARTIFACTORY_PASSWORD`)

## Upfront dependency setup (recommended)

From **DynamoRevitUtils** (DRU) with D4R checked out at `../DynamoRevit`:

```bat
cd ..\DynamoRevitUtils
RevitAPInuGet.bat
BuildSolutions.bat
```

`RevitAPInuGet.bat` installs RevitAPI NuGet packages into `DynamoRevit\lib\Revit …\`.
`BuildSolutions.bat` restores NuGet packages and builds the solution.

## MSBuild hook (255442)

`src\Directory.Build.targets` calls DRU `ensure-build-dependencies.ps1` before compile when:
- `SkipEnsureBuildDependencies` is not `true`
- Local Revit dev paths are not configured (`UsingLocalRevitDev`)
- DRU helpers are found via `WORKSPACE_DYNAMOREVITUTILS` or sibling `../DynamoRevitUtils`

Local CLI example:

```powershell
cd src
$env:WORKSPACE_DYNAMOREVITUTILS = 'D:\Repos\DynamoRevitUtils'
& "$env:ProgramFiles\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe" DynamoRevit.All.sln /t:Build /p:Configuration=Release /p:Platform=NET100
```

Jenkins sets `SkipEnsureBuildDependencies=true` on the Build stage because Download Dependencies already ran the ensure script.

## Local Revit (no NuGet RevitAPI path)

Set `REVITAPI` in `src\Config\user_local.props` to your Revit install. The MSBuild hook skips dependency download when `UsingLocalRevitDev=true`.

## CI (master-3)

DRU Jenkins checks out D4R at `BranchDynamoRevit` and runs Download Dependencies before Build. Durable build-deps cache lives on agent **C:** (`build-deps-cache/`); Revit RTF procure uses `revit-procure-cache/` (255441).

Cross-links: [DRU PR #170](https://git.autodesk.com/Dynamo/DynamoRevitUtils/pull/170) · REVIT-255442
