# Build System Cleanup — Open Questions & TODOs

These are known issues and open design questions in the build system that need
to be resolved, particularly for the `D4R_DA_2027` branch targeting Revit 2027 /
net10.0-windows.

---

## 1. Platform naming is confusing and redundant

`CS_SDK.props` defines three platforms:

| Platform   | TargetFramework      | Notes |
|------------|----------------------|-------|
| `NET80`    | `net8.0-windows`     | Interactive Revit 2026 build — kept on this branch, not used for DA |
| `NET10_DA` | `net10.0-windows`    | DA build for Revit 2027 — TFM now matches the name |
| `NET100`   | `net10.0-windows`    | Interactive Revit 2027 build |

On the `D4R_DA_2027` branch `NET10_DA` and `NET100` share the same TFM
(`net10.0-windows`). The only distinction is the `_DA` suffix, which is a
meaningful build profile — not just a TFM alias. Any platform whose name
contains `_DA` triggers these behaviours across multiple projects:

| Project | What `_DA` activates |
|---|---|
| `CS_SDK.props` | Defines `DESIGN_AUTOMATION` compile constant |
| `DynamoRevit.csproj` | Removes `DynamoRevit.cs`, `DynamoRevitApp.cs`, entire `ViewModel/` (interactive startup + ribbon) |
| `RevitNodesUI.csproj` | Removes all WPF controls, selection dialogs, XAML pages |
| `RevitServices.csproj` | Removes `Threading/*.cs` (UI thread marshalling, not needed in headless DA) |

`NET80` is also still present in `CS_SDK.props` — it is carried over from the 2026
branch and is not needed for any DA work on 2027. It could be dropped from
`DynamoRevit.DA.sln` and DA-related props without impact.

**Questions:**
- Should `NET10_DA` be renamed `NET100_DA` to make the TFM explicit and consistent
  with the `NET100` sibling?
- Should `NET80` be removed from `DynamoRevit.DA.sln` and DA props entirely?
- Should DA just set a property/constant (e.g. `IsDesignAutomation=true`) under
  `NET100` instead of maintaining a separate platform name?

---

## 2. Solution file responsibilities are unclear

| Solution              | Projects included              | Platforms exposed     | Typical build command |
|-----------------------|--------------------------------|-----------------------|-----------------------|
| `DynamoRevit.DA.sln`  | DADynamoApp + deps only        | `NET10_DA`            | `/p:Platform=NET10_DA` |
| `DynamoRevit.All.sln` | Everything incl. tests         | `NET80`, `NET100`     | `/p:Platform=NET100` |

`DADynamoApp` is **not** present in `All.sln` at all (not even as a transitive
entry). Building `All.sln` will not produce the DA app bundle. The two solutions
are effectively independent build paths.

**Questions:**
- Should `DADynamoApp` be added to `All.sln` with a `NET100` → `NET10_DA` platform
  mapping, so a single solution build covers everything?
- Should `DA.sln` remain the canonical path for DA-only work, or be retired in
  favour of a configuration within `All.sln`?

---

## 3. `dotnet build` vs VS MSBuild inconsistency

`DynamoRevit.All.sln` builds cleanly with VS MSBuild:
```
& "C:\Program Files\Microsoft Visual Studio\18\Professional\MSBuild\Current\Bin\MSBuild.exe" DynamoRevit.All.sln -p:Configuration=Debug -p:Platform=NET100 -m
```

But fails with `dotnet build` using .NET SDK 10:
```
dotnet build DynamoRevit.All.sln -c Debug /p:Platform=NET100
```

Two known `dotnet build`-only failures:

**a) `DADynamoApp` — `MSB3992: 'RootElementName' is not set`**
- Root cause: `EnableDynamicLoading=true` causes SDK 10 to set
  `UseAttributeForTargetFrameworkInfoPropertyNames=true`, which requires
  `RootElementName` to be set explicitly. VS MSBuild does not have this requirement.
- Possible fixes: set `<UseAttributeForTargetFrameworkInfoPropertyNames>false</UseAttributeForTargetFrameworkInfoPropertyNames>`,
  or set `<RootElementName>DADynamoApp</RootElementName>` explicitly.

**b) `DynamoRevitIcons` — `ResGen.exe not supported on .NET Core MSBuild`**
- Pre-existing issue. The `.resx` code generation step uses ResGen.exe which is
  not available in the .NET Core MSBuild toolchain.
- Possible fix: migrate resource generation to use `<EmbeddedResource>` without
  ResGen, or exclude from `dotnet build` paths.

**Question:** Is `dotnet build` a supported/required path in CI, or does CI always
use VS MSBuild? The `Jenkinsfile` delegates to `DynamoRevitUtils` — need to check
those build scripts to confirm.

---

## 4. Build/Package the AppBundle

The target bundle layout is:

```
DynamoRevitDA.bundle/
  PackageContents.xml
  Contents/
    <DynamoCore files — dlls, nodes, extensions, libg, …>
    <DynamoPlayer binaries — copied by CopyDynamoPlayerFiles target>
    Revit/
      DynamoRevit.addin
      <DADynamoApp build output — DADynamoApp.dll, DynamoRevitDS.dll, RevitNodes.dll, …>
```

### What is automated (DADynamoApp.csproj `publish_bundle` target)

Running a build of `DADynamoApp.csproj` with `Platform=NET10_DA` now:

1. Compiles `DADynamoApp` and writes output to `$(OutputPath)` (`bin\NET10_DA\<Configuration>\Revit\`).
2. **`Copy dll`** — copies `GregRevitAuth.dll` into `$(OutputPath)`.
3. **`CopyDynamoPlayerFiles`** — copies `$(PkgDynamoPlayer)\bin\Release\net8.0\bin\**` into `$(OutputPath)\..` (sibling to the Revit output folder).
4. **`publish_bundle`** — assembles the bundle at `$(SolutionDir)\..\DynamoRevitDA.bundle\`:
   - Creates the `Contents\Revit\` directory tree.
   - Copies `PackageContents.xml` (template at `src/DADynamoApp/app_bundle_template/`) to the bundle root.
   - Copies `DynamoRevit.addin` (same template) into `Contents\Revit\`.
   - Copies everything from `$(OutputPath)\..` (build output + DynamoPlayer binaries) into `Contents\Revit\`, preserving subdirectory structure via `GetFullPath` to correctly resolve `%(RecursiveDir)`.

### What still requires a manual step (TODO)

**DynamoCore** must be built separately from the [Dynamo](https://github.com/DynamoDS/Dynamo) repo (`DynamoCore.sln`) and its output copied into `Contents\` before the bundle is usable. There is currently no Windows CI pipeline for this — the existing pipeline produces a Linux/DAAS-targeted build only.

The `publish_bundle` target has a commented-out stub (`TODO: copy DynamoCore contents`) that should be wired up once a reliable source for the Windows DynamoCore binaries exists.

**Options to resolve:**
- Extend the existing Linux DAAS pipeline to produce an OS-agnostic DynamoCore NuGet/artifact that works for both DA and DAAS.
- Add a separate Windows CI pipeline for `DynamoCore.sln` and surface its output as a versioned artifact consumed here.
- For local development: build `DynamoCore.sln` locally and point `$(Pkgdynamovisualprogramming_servicecoreruntime)` (or an equivalent property) at the output before running `publish_bundle`.


---

## How to debug Design Automation locally

There are two options depending on how closely you need to replicate the cloud environment:

**Option 1 — Full Revit (with UI)**

Use the [APS local debug tool](https://github.com/autodesk-platform-services/aps-automation-csharp-revit.local.debug.tool) against a standard Revit installation. This is the easiest way to validate general AppBundle functionality.

**Option 2 — Headless Revit engine (matches cloud)**

Download the exact Design Automation engine from Artifactory:
`https://art-bobcat.autodesk.com/ui/repos/tree/General/team-designautomation-generic/Revit/Engine`

This is the same headless executable used on the cloud backend — it starts Revit without loading any UI. Before running it, edit `revitcoreconsole.dll.config` to point to your local Revit installation:

```xml
<add key="RevitPath" value="C:\Program Files\Autodesk\Revit 2027" />
```

## 5. Research needed / action items

- [ ] Check `DynamoRevitUtils` Jenkins scripts: which solution (`DA.sln` or `All.sln`), which platform, `dotnet` or VS MSBuild? The `Jenkinsfile` delegates entirely to `DynamoRevitUtils/master` — the actual build commands are opaque from this repo.
- [ ] Confirm whether CI builds the DA bundle at all, or whether that is a manual step today.
- [ ] Decide on `NET10_DA` rename to `NET100_DA` (cosmetic but eliminates future confusion).
- [ ] Decide whether `NET80` should be removed from `DynamoRevit.DA.sln` — it is unused for DA on the 2027 branch.
- [ ] Decide whether `DADynamoApp` should be added to `All.sln` with a `NET100` → `NET10_DA` platform mapping.
- [ ] Fix `dotnet build` compatibility if CI requires it (MSB3992 on `DADynamoApp`, ResGen on `DynamoRevitIcons`).
