# Build System Cleanup — Open Questions & TODOs

These are known issues and open design questions in the build system that need
to be resolved, particularly for the `D4R_DA_2027` branch targeting Revit 2027 /
net10.

---

## 1. Platform naming is confusing and redundant

`CS_SDK.props` defines three platforms:

| Platform   | TargetFramework     | Notes |
|------------|---------------------|-------|
| `NET80`    | `net8.0-windows`    | Interactive Revit build (Revit 2026) |
| `NET10_DA` | `net8.0-windows`    | **Same TFM as NET80 on the 2026 branch — name implies net10 but it's net8** |
| `NET100`   | `net10.0-windows`   | Revit 2027 targeting net10 |

For the `D4R_DA_2027` branch (Revit 2027), `NET100` is the primary TFM.
`NET10_DA` is now genuinely net10 on this branch (unlike on 2026), but the naming
is still confusing without context.

**Important:** `_DA` in the platform name is a meaningful build profile — it is not just a
TFM alias. Any platform whose name contains `_DA` triggers these behaviours across multiple projects:

| Project | What `_DA` removes |
|---|---|
| `CS_SDK.props` | Defines `DESIGN_AUTOMATION` compile constant |
| `DynamoRevit.csproj` | Removes `DynamoRevit.cs`, `DynamoRevitApp.cs`, entire `ViewModel/` (interactive startup + ribbon) |
| `RevitNodesUI.csproj` | Removes all WPF controls, selection dialogs, XAML pages |
| `RevitServices.csproj` | Removes `Threading/*.cs` (UI thread marshalling, not needed in headless DA) |

So the `_DA` suffix carries real semantic weight. The problem is solely the `NET10`
prefix on the 2026 branch, which implied net10 when the TFM was actually net8.

**Questions:**
- Should DA just use `NET100` with a separate property/flag to denote DA vs interactive,
  rather than a separate platform name?
- If `NET10_DA` stays, should it be renamed (e.g. `NET100_DA`) to align with the actual TFM name?

---

## 2. Solution file responsibilities are unclear

| Solution              | Projects included              | Typical build command |
|-----------------------|--------------------------------|-----------------------|
| `DynamoRevit.DA.sln`  | DADynamoApp + deps only        | `/p:Platform=NET10_DA` |
| `DynamoRevit.All.sln` | Everything incl. tests         | `/p:Platform=NET80` or `NET100` |

`DADynamoApp` is **not** listed explicitly in `All.sln` — it is pulled in as a
transitive project reference from `RevitSystemTests`. This causes the solution to
build it without a proper platform mapping, which leads to build errors under
`dotnet build` with .NET SDK 10 (see issue 3 below).

**Questions:**
- Should `DADynamoApp` be a first-class explicit project in `All.sln`?
- Should `DA.sln` be a proper subset configuration of `All.sln`, or kept fully separate?
- Is there a reason DA uses `NET10_DA` platform instead of `NET100`? If not, unify them
  and remove the extra platform.

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

## 4. Research needed / action items

- [ ] Check `DynamoRevitUtils` Jenkins scripts: which solution, which platform, `dotnet` or VS MSBuild?
- [ ] Confirm whether `NET10_DA` is used in any CI job for this branch
- [ ] Decide on NET10_DA rename/merge with NET100
- [ ] Decide whether All.sln should be the canonical "build everything" solution
- [ ] Fix `dotnet build` compatibility if CI requires it (MSB3992 + ResGen)
- [ ] Add `DADynamoApp` explicitly to `All.sln` with correct platform mapping
