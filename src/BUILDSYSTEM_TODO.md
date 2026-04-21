# Build System Cleanup — Open Questions & TODOs

These are known issues and open design questions in the build system that need
to be resolved, particularly for the `D4R_DA_2026` branch targeting Revit 2026 / net8.

---

## 1. Platform naming is confusing and redundant

`CS_SDK.props` defines three platforms:

| Platform   | TargetFramework     | Notes |
|------------|---------------------|-------|
| `NET80`    | `net8.0-windows`    | Interactive Revit build |
| `NET80_DA` | `net8.0-windows`    | **Same as NET80 — name implies net10 but it's net8** |
| `NET100`   | `net10.0-windows`   | Future Revit targeting net10 |

For the `D4R_DA_2026` branch (Revit 2026), only `net8.0-windows` is relevant.
`NET100` adds noise and `NET80_DA` is actively misleading.

**Important:** `_DA` in the platform name is a meaningful build profile — it is not just a
TFM alias. Any platform whose name contains `_DA` triggers these behaviours across multiple projects:

| Project | What `_DA` removes |
|---|---|
| `CS_SDK.props` | Defines `DESIGN_AUTOMATION` compile constant |
| `DynamoRevit.csproj` | Removes `DynamoRevit.cs`, `DynamoRevitApp.cs`, entire `ViewModel/` (interactive startup + ribbon) |
| `RevitNodesUI.csproj` | Removes all WPF controls, selection dialogs, XAML pages |
| `RevitServices.csproj` | Removes `Threading/*.cs` (UI thread marshalling, not needed in headless DA) |

So the `_DA` suffix carries real semantic weight. The problem is solely the `NET10` prefix,
which implies net10 when the TFM is actually net8. Renaming `NET80_DA` → `NET80_DA` everywhere
would fix the confusion without changing any build behaviour.

**Questions:**
- Should `D4R_DA_2026` drop `NET100` entirely and only expose `NET80`?
- Was `NET80_DA` ever actually net10 and got downgraded, or has it always been net8?
- Should DA just use `NET80` with a separate property/flag to denote DA vs interactive,
  rather than a separate platform name?
- If `NET80_DA` stays, should it be renamed (e.g. `NET80_DA`) to reflect actual TFM?

---

## 2. Solution file responsibilities are unclear

| Solution              | Projects included              | Typical build command |
|-----------------------|--------------------------------|-----------------------|
| `DynamoRevit.DA.sln`  | DADynamoApp + deps only        | `/p:Platform=NET80_DA` |
| `DynamoRevit.All.sln` | Everything incl. tests         | `/p:Platform=NET80` or `NET100` |

`DADynamoApp` is **not** listed explicitly in `All.sln` — it is pulled in as a
transitive project reference from `RevitSystemTests`. This causes the solution to
build it without a proper platform mapping, which leads to build errors under
`dotnet build` with .NET SDK 10 (see issue 3 below).

A workaround was added: explicit `DADynamoApp` entry in `All.sln` mapping
`NET80`/`NET100` → `NET80_DA`. But this is a band-aid.

**Questions:**
- Should `DADynamoApp` be a first-class explicit project in `All.sln`?
- Should `DA.sln` be a proper subset configuration of `All.sln`, or kept fully separate?
- Is there a reason DA uses `NET80_DA` platform instead of `NET80`? If not, unify them
  and remove the extra platform.

---

## 3. `dotnet build` vs VS MSBuild inconsistency

`DynamoRevit.All.sln` builds cleanly with VS MSBuild:
```
& "C:\Program Files\Microsoft Visual Studio\18\Professional\MSBuild\Current\Bin\MSBuild.exe" DynamoRevit.All.sln -p:Configuration=Debug -p:Platform=NET80 -m
```

But fails with `dotnet build` using .NET SDK 10:
```
dotnet build DynamoRevit.All.sln -c Debug /p:Platform=NET80
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

Running a normal build of `DADynamoApp.csproj` with `Platform=NET80_DA` now:

1. Compiles `DADynamoApp` and writes output to `$(OutputPath)` (`bin\<Configuration>\net8.0\`).
2. **`Copy dll`** — copies `GregRevitAuth.dll` into `$(OutputPath)`.
3. **`CopyDynamoPlayerFiles`** — copies `$(PkgDynamoPlayer)\bin\Release\net8.0\bin\**` into `$(OutputPath)\..` (the parent folder of the DADynamoApp OutputPath, aka where the DynamoCore dlls will be).
4. **`publish_bundle`** — assembles the bundle at `$(SolutionDir)\..\DynamoRevitDA.bundle\`:
   - Creates the `Contents\Revit\` directory tree.
   - Copies `PackageContents.xml` (template at `src/DADynamoApp/app_bundle_template/`) to the bundle root.
   - Copies `DynamoRevit.addin` (same template) into `Contents\Revit\`.
   - Copies all of `$(OutputPath)` into `Contents\Revit\`, preserving subdirectory structure.

### What still requires a manual step (TODO)

**DynamoCore** must be built separately from the [Dynamo](https://github.com/DynamoDS/Dynamo) repo (`DynamoCore.sln`) and its output copied into `Contents\` before the bundle is usable. There is currently no Windows CI pipeline for this — the existing pipeline produces a Linux/DAAS-targeted build only.

The `publish_bundle` target has a commented-out stub (`TODO: copy DynamoCore contents`) that should be wired up once a reliable source for the Windows DynamoCore binaries exists.

**Options to resolve:**
- Extend the existing Linux DAAS pipeline to produce an OS-agnostic DynamoCore NuGet/artifact that works for both DA and DAAS.
- Add a separate Windows CI pipeline for `DynamoCore.sln` and surface its output as a versioned artifact consumed here.
- For local development: build `DynamoCore.sln` locally and point `$(Pkgdynamovisualprogramming_servicecoreruntime)` (or an equivalent property) at the output before running `publish_bundle`.


---

## 4. Research needed / action items

- [ ] Check `DynamoRevitUtils` Jenkins scripts: which solution, which platform, `dotnet` or VS MSBuild?
- [ ] Confirm whether `NET100` is used in any CI job for this branch
- [ ] Decide on NET80_DA rename/merge with NET80
- [ ] Decide whether All.sln should be the canonical "build everything" solution
- [ ] Fix `dotnet build` compatibility if CI requires it (MSB3992 + ResGen)
