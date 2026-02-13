# Copilot instructions for OrchardDashboard

## Big picture
- This solution is an Orchard Core modular host app with two in-repo modules.
- `DashboardApplication` is the host web app; `Module1` and `Module2` are loaded via project references in `DashboardApplication.csproj`.
- Orchard package versions are centralized in `Directory.Build.props` (`OrchardCoreVersion`). Keep module/app package versions aligned through that property.
- Tenant state and tenant-scoped settings live under `DashboardApplication/App_Data/` (`tenants.json`, `Sites/Default/appsettings.json`).

## Runtime architecture and flow
- App startup is in `DashboardApplication/Program.cs` and uses `builder.Services.AddOrchardCore()` with feature chaining.
- The host sets a fallback route that redirects `/` to the Orchard admin URL (uses `ShellSettings` + `AdminOptions`).
- `app.UseOrchardCore()` is the main middleware entrypoint; avoid adding conflicting endpoint middleware after it.
- Module endpoints are defined in each module `Startup` class via `Configure(IEndpointRouteBuilder)`.
  - Example: `Module2/Startup.cs` maps `/Module2/hello` and `/info`.
- MVC/UI examples are in `Module1` (`Controllers/`, `Views/Home/Index.cshtml`, `Pages/About.cshtml`).

## Orchard-specific patterns to preserve
- Use `OrchardCore.Module.Targets` in module projects (`Module1.csproj`, `Module2.csproj`) and keep `OutputType=Library`.
- Register new module behavior in module `Startup` classes; do not add module-specific logic into the host unless truly cross-cutting.
- When tenant-aware behavior is needed, resolve `OrchardCore.Environment.Shell.ShellSettings` from DI/request services (see `Module2/Startup.cs`).
- Setup feature composition is recipe-driven: `DashboardApplication/Recipes/dashboard.recipe.json` enables Orchard features/themes.

## Build, run, and validation workflow
- Restore/build from repo root:
  - `dotnet restore`
  - `dotnet build OrchardCore.Samples.sln`
- Run host app from `DashboardApplication` (or set it as startup in Visual Studio):
  - `dotnet run`
- CI parity checks from `.github/workflows/ci.yml`:
  - Build with `-c Release`
  - `/p:TreatWarningsAsErrors=true`
  - `/p:RunAnalyzers=true`
  - `/p:NuGetAudit=false`
- There is no dedicated test project in this repo; rely on build + manual endpoint/UI verification.

## Editing guidance for agents
- Prefer minimal, additive changes and keep existing style per file (some files use file-scoped namespaces, others block namespaces).
- If adding Orchard packages, use `$(OrchardCoreVersion)` instead of hardcoding versions.
- Keep sample intent intact: this repo demonstrates modular Orchard patterns, so examples should stay simple and explicit.
- Update docs/recipe/config when behavior changes (especially under `DashboardApplication/Recipes` and `App_Data`).
