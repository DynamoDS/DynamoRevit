# DynamoRevit (D4R) Code Review Guidelines

## Repository Context
This is the **Dynamo for Revit** plugin and node library (D4R): a Revit add-in plus Dynamo zero-touch and custom nodes.
Primary language is **C#** (~99%). CI is a thin Jenkins pipeline that delegates builds to `DynamoRevitUtils`.
Tests use **RevitTestFramework (RTF)** under `test/` and require a Revit install for system tests.

Projects target **.NET Framework 4.8** (Revit 2024 and older) and **.NET 8** (Revit 2025+) on Windows (`src/Config/`, `global.json`).

## Review Scope
Focus review on hand-written source and configuration:
- `src/**/*.cs` (except auto-generated files listed below)
- `test/**/*.cs`, `test/**/*.dyn` when graph wiring or assertions change
- `Jenkinsfile`, `Jenkins.cbci`, `tools/**/*.ps1`, `tools/**/*.bat`
- `.beacode/**`, `.github/**`, `global.json`, build props under `src/Config/`

Deprioritize or skip review-only noise from:
- `**/AssemblySharedInfo.cs` and T4 output from `AssemblySharedInfoGenerator/` (regenerated)
- `**/*.resx`, `**/*.Designer.cs` (localization / designer output)
- `**/*.png`, `**/*.ico`, `images/**` (binary assets)
- `extern/**`, vendored third-party trees unless intentionally modified
- Large `.dyn` / `.rvt` fixtures unless test logic changed

## Language-Specific Rules

### C# (.NET / Revit API)
- Follow Microsoft naming: PascalCase for public members, camelCase for locals
- Wrap Revit mutations in `Transaction` / `SubTransaction`; never modify the model outside a transaction
- Prefer `ElementId`/`UniqueId` over string element identifiers; validate `Element` nullability after `GetElement`
- Use `FilteredElementCollector` with narrow filters; avoid full-document scans in hot paths
- Never call Revit API from non-UI threads unless documented thread-safe API
- Validate public API parameters at method entry; use `nameof()` in exceptions
- Avoid `async void` except event handlers; prefer pattern matching over casts
- Keep Dynamo node entry points thin — business logic belongs in shared/library classes

### Dynamo Nodes
- Zero-touch nodes: use `[NodeName]`, `[NodeCategory]`, `[NodeDescription]` attributes consistently
- Port names should be descriptive; avoid breaking changes to port order without migration notes
- Nodes should be side-effect free where possible; document required document state in tooltips
- Do not expose raw `Element` wrappers without null-safe handling and clear failure messages

### Groovy (Jenkins Pipeline)
- This repo's `Jenkinsfile` triggers `DynamoRevitUtils`; still follow pipeline hygiene when editing
- Use `def` for locals; quote paths in shell steps; use `error()` to fail builds
- Pass branch names as parameters — never interpolate untrusted input into shell commands

### PowerShell / Batch (tools/)
- Use `$ErrorActionPreference = 'Stop'` in scripts
- Prefer cmdlets over aliases; check `$LASTEXITCODE` after native commands
- Do not hardcode machine-specific Revit install paths — use env vars or documented props

## CI/CD & Build
- D4R builds are orchestrated via `DynamoRevitUtils` — keep local `Jenkinsfile` changes minimal and intentional
- Pin NuGet/tool versions when adding dependencies; respect existing `global.json` SDK constraints
- Test results must be TRX or JUnit XML; coverage in Cobertura when enabled upstream
- Set `RevitVersionNumber` via environment or `user_locals.props` — do not commit machine-specific paths

## Security
- Never hardcode credentials, tokens, or license keys
- Do not log PII or customer model data from Revit documents
- Validate file paths in tools/scripts to prevent path traversal
- Do not disable SSL or use `http` for production artifact fetches

## Testing (RTF)
- System tests require Revit; unit tests should avoid Revit runtime when possible
- Document prerequisites for any test that needs a specific Revit version or `.rvt`/`.dyn` fixture
- Prefer deterministic assertions over image/screenshot compares unless explicitly a visual regression test
- When adding RTF coverage, keep test graphs minimal and name tests after the behavior under test
