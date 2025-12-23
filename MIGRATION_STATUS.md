Branch: migrate/aspnetcore
Last updated: 2025-12-14

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

Latest updates
- Identity wiring: `ApplicationUser` and `ApplicationDbContext` added; Identity registered in `Program.cs` and configured to select provider via configuration.
- Local dev DB: added SQLite support and `DatabaseProvider: Sqlite` in `appsettings.Development.json`; added `Microsoft.EntityFrameworkCore.Sqlite` and `Microsoft.EntityFrameworkCore.Design` packages.
- Migrations: created `InitialIdentity` migration and applied it to a local SQLite DB file (`SparWebCore/spardev.db`).
- Remote: pushed branch `migrate/aspnetcore` to origin (SSH); a PR can be opened at https://github.com/oleksi/SparProject/pull/new/migrate/aspnetcore

Current status (what’s still pending)
-------------------------------------
- Port layout & shared partials: NOT STARTED — We need to copy/adapt `SparWeb/Views/Shared/_Layout.cshtml` and dependent partials into `SparWebCore/Views/Shared`, add `_ViewImports.cshtml`, and replace System.Web-specific helpers.
- Port controllers & views: IN PROGRESS (view bundle helper replacements done, but actual porting into Core and controller wiring remain).
- Port configuration: IN PROGRESS — `appsettings.Development.json` created for local dev (SQLite). Remaining: finalize `appsettings.json` mapping and move sensitive keys to secrets or env vars.
- Startup/OWIN middleware migration: NOT STARTED — Replace OWIN startup with ASP.NET Core middleware (authentication, session, error handling, etc.).
- Data layer migration: NOT STARTED — Decide EF6 vs EF Core and implement.
- Auth & Identity migration: IN PROGRESS — Identity wired and migrations applied; remaining: confirm registration/login flows persist to DB and port any custom cookie/external provider options.
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
- **Registration Form Updates (2025-12-14)**:
	- `SparWebCore/Models/Util.cs` — NEW: Static dictionaries for Height, Weight Class, and State dropdowns
	- `SparWebCore/Controllers/AccountController.cs` — Added `PopulateViewBagForRegistration()` method and updated RegisterFighter GET action
	- `SparWebCore/Views/Shared/RegisterFighterViewModel.cshtml` — Converted to dropdowns (Height, Weight, State) and radio buttons (Gender)
	- `SparWebCore/wwwroot/css/site.css` — Added minimal CSS fix for form-group text-align; removed all custom .register-form overrides

Verification & notes
--------------------
- Build: `dotnet build ./SparWebCore` succeeded after asset moves.
- Run: `dotnet run --project ./SparWebCore` started and served static files (user confirmed via DevTools that assets load). The app currently displays the Core default layout because `SparWeb`'s layout/partials haven't been ported.
- ApplicationInsights.config contains an entry referencing `System.Web.Optimization.BundleHandler` — remove this when telemetry is migrated.

Next recommended step (short-term)
---------------------------------
1. Port the old layout and shared partials into `SparWebCore/Views/Shared`. While porting, replace System.Web-only helpers (child actions, Html.Action, Request.Browser, Url.Content) with Core equivalents or small adapter helpers. Add `_ViewImports.cshtml` and ensure tag helpers/namespaces are available.
2. Verify Identity end-to-end: run the app in Development (uses `appsettings.Development.json`), register a user via the registration view, and confirm the `AspNetUsers` row is created in `SparWebCore/spardev.db`.
3. Migrate middleware (cookie options, external providers) and port remaining controllers & views so the UI can be manually exercised.

Short-term status & next actions
--------------------------------
- The registration form markup has been ported and is visible as partials. Identity wiring has been implemented (ApplicationUser, ApplicationDbContext, Identity registered in `Program.cs`) and the `AccountController` POST handlers now create users via `UserManager`.
- **Registration Form UI - IMPROVED (2025-12-14)**:
	- Fixed async partial rendering issue; registration form inputs now visible and functional
	- Resolved Bootstrap styling conflicts by removing custom CSS overrides and using Bootstrap's native form styling
	- Added single minimal CSS rule to fix form-group text-align issue caused by parent .text-center
	- **Created `Models/Util.cs`** with static dictionaries for dropdown data (Height, Weight Class, States/Provinces)
	- **Updated `AccountController.RegisterFighter` GET action** to populate ViewBag with dropdown data via `PopulateViewBagForRegistration()` method
	- **Converted form controls to match original**:
		- Height: Now dropdown with imperial height options (4'8" to 6'3")
		- Weight Class: Now dropdown with boxing/MMA weight classes
		- State/Province: Now dropdown with US states and Canadian provinces
		- Gender: Now radio buttons (Male/Female) instead of checkbox
	- Form submission working and persists users to SQLite database (spardev.db)
	- **Status**: Form functional with proper control types; additional UI polish needed to match original pixel-perfect
- Remaining work to reach feature parity:
	1. Create and apply EF Core migrations (Identity schema + domain entities) and point `SparConnection` at a dev SQL Server for testing.
	2. Implement persistence of Fighter/Trainer domain entities (repository/service) and associate them with the created Identity user.
	3. Wire email sending (Elastic Email) and move sensitive keys to secrets or a key vault (we added `appsettings.Development.json.example` and ignored the local dev file).
	4. Continue UI polish on registration form to match original layout/spacing/typography exactly

Notes:
- EF Core and Identity NuGet packages (compatible with .NET 9) were added; build succeeds locally but runtime DB is required for user creation.
- There are non-blocking nullable warnings in the view models and POCOs that can be addressed in a tidy-up pass.
- Static assets (Bootstrap 3.x, CSS, fonts, images) are being served correctly at http://localhost:5028
- Using minimal CSS approach: letting Bootstrap handle form styling with single targeted override for layout fix

How to resume
-------------
- Checkout branch `migrate/aspnetcore` (already on branch). 
- Continue with the todo list (port layout -> wire configuration -> migrate middleware/auth -> port controllers/views -> migrate data -> logging -> CI/tests).

Contact
-------
If you'd like, I can start the layout port now (copy/adapt `_Layout.cshtml` and partials into `SparWebCore`) — say "Port layout now" and I'll begin.
