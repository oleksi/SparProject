Branch: migrate/aspnetcore
Last updated: 2025-11-30

Summary
-------
- Goal: migrate `SparWeb` (ASP.NET MVC 5 / System.Web / OWIN / Identity 2 / EF6) into `SparWebCore` (ASP.NET Core, .NET 9).
- Current milestone: static assets migrated and bundling removed; Core app builds and serves assets, but views/layout are not yet ported into `SparWebCore`.

What we completed
------------------
- Inventory & dependency mapping (controllers, views, models, packages, startup, Web.config) — DONE
- Moved static assets from `SparWeb/Content` and `SparWeb/Scripts` into `SparWebCore/wwwroot` and reorganized into:
	- `wwwroot/lib/` (vendor libs such as bootstrap, jquery)
	- `wwwroot/css/` (site styles)
	- `wwwroot/js/` (site scripts and plugins)
	- `wwwroot/images/` and `wwwroot/fonts/`
- Removed System.Web bundling usage:
	- Replaced `@Styles.Render` / `@Scripts.Render` occurrences found with explicit `<link>`/`<script>` tags in edited views.
	- Neutralized `BundleConfig.cs` with a migration note and commented out bundle registration in `Global.asax`.
	- Removed the `System.Web.Optimization` compile/reference from `SparWeb.csproj` (left a comment explaining why).
- Verified: `dotnet build` of `SparWebCore` succeeds and running the Core app serves static assets (CSS/JS/fonts load in browser).

- Ported registration UI and basic handling into `SparWebCore`:
	- Added `SparWebCore/Models/RegisterViewModels.cs` (RegisterMainViewModel, RegisterViewModel, RegisterFighterViewModel, RegisterTrainerViewModel).
	- Added partials and views: `Views/Shared/RegisterMain.cshtml`, `Views/Shared/RegisterFighterViewModel.cshtml`, `Views/Shared/RegisterTrainerViewModel.cshtml`, `Views/Account/Register.cshtml`, `RegisterFighter.cshtml`, `RegisterTrainer.cshtml`, and `DisplayEmail.cshtml`.
	- Implemented `AccountController.GetRegisterPopupModal()` to return a Razor partial and added `AccountController` GET and POST actions for registration (POST handlers are placeholders that validate model and return `DisplayEmail`).
	- Added `UtilCompat` helper and re-enabled the registration popup logic in the migrated layout.

Current status (what’s still pending)
-------------------------------------
- Port layout & shared partials: NOT STARTED — We need to copy/adapt `SparWeb/Views/Shared/_Layout.cshtml` and dependent partials into `SparWebCore/Views/Shared`, add `_ViewImports.cshtml`, and replace System.Web-specific helpers.
- Port controllers & views: IN PROGRESS (view bundle helper replacements done, but actual porting into Core and controller wiring remain).
- Port configuration: NOT STARTED — Move settings and connection strings from `Web.config` -> `appsettings.json` and wire IConfiguration.
- Startup/OWIN middleware migration: NOT STARTED — Replace OWIN startup with ASP.NET Core middleware (authentication, session, error handling, etc.).
- Data layer migration: NOT STARTED — Decide EF6 vs EF Core and implement.
- Auth & Identity migration: NOT STARTED — Move from ASP.NET Identity 2.x/OWIN to ASP.NET Core Identity or equivalent.
- Logging/telemetry: NOT STARTED — Replace ELMAH with Serilog/AI or ElmahCore for Core.
- CI/Docker/tests & Final polish: NOT STARTED

Files changed (recent)
----------------------
- `SparWebCore/wwwroot/` — many files added: vendor libs, css, js, fonts, images
- `SparWeb/Views/Shared/_Layout.cshtml` and other views — replaced bundle helper calls with explicit tags (to simplify migration)
- `SparWeb/App_Start/BundleConfig.cs` — neutralized with migration note
- `SparWeb/Global.asax.cs` — commented out `BundleConfig.RegisterBundles` call
- `SparWeb/SparWeb.csproj` — removed `System.Web.Optimization` compile/reference (left migration comment)
- `SparWeb/Views/Web.config` — removed System.Web.Optimization namespace entry

Verification & notes
--------------------
- Build: `dotnet build ./SparWebCore` succeeded after asset moves.
- Run: `dotnet run --project ./SparWebCore` started and served static files (user confirmed via DevTools that assets load). The app currently displays the Core default layout because `SparWeb`'s layout/partials haven't been ported.
- ApplicationInsights.config contains an entry referencing `System.Web.Optimization.BundleHandler` — remove this when telemetry is migrated.

Next recommended step (short-term)
---------------------------------
1. Port the old layout and shared partials into `SparWebCore/Views/Shared`. While porting, replace System.Web-only helpers (child actions, Html.Action, Request.Browser, Url.Content) with Core equivalents or small adapter helpers. Add `_ViewImports.cshtml` and ensure tag helpers/namespaces are available.

Short-term status & next actions
--------------------------------
- The registration form markup has been ported and is visible as partials, but the backend persistence (Identity + repositories) is not yet implemented. The POST handlers currently simulate success by returning `DisplayEmail`.
- To get full registration working we should:
	1. Wire up ASP.NET Core Identity (UserManager/SignInManager) in `Program.cs` and port ApplicationUser/UserManager setup.
	2. Port or adapt the repository/data layer (FighterRepository/TrainerRepository) or choose EF Core and migrate models.
	3. Migrate email sending and configuration to `appsettings.json` and `IConfiguration`.

How to resume
-------------
- Checkout branch `migrate/aspnetcore` (already on branch). 
- Continue with the todo list (port layout -> wire configuration -> migrate middleware/auth -> port controllers/views -> migrate data -> logging -> CI/tests).

Contact
-------
If you'd like, I can start the layout port now (copy/adapt `_Layout.cshtml` and partials into `SparWebCore`) — say "Port layout now" and I'll begin.
