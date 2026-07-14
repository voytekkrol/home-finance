# CLAUDE.md — Home Finance

Agent guide for this repo. Read this before doing anything. The long-form vision, roadmap and rationale live in `PLAN.md`; this file is the operational contract.

## 1. What this project is

A household-finance app for two users (owner + partner), built as a **learning exercise for multi-agent development with Claude Code**. Every feature is done by delegating to subagents (`architect` → `implementer` → `test-writer` → `reviewer`), not by the user typing code. Keep tasks **small and independently mergeable** (a few hundred diff lines max).

Current phase: **Phase 1 — MVP expense tracking** (see `PLAN.md` §3).

## 2. Stack (authoritative — do not swap without asking)

- **.NET 10**, C#, nullable + implicit usings enabled everywhere.
- **Blazor Web App** (unified model, per-component render modes). Server interactivity is the default; opt into WASM per component only when needed.
- **MudBlazor** for all UI components (forms, tables, charts, dialogs, snackbars, theming). Do not introduce Bootstrap, Tailwind, or another component library.
- **EF Core** with migrations. SQLite for local dev, Azure SQL (free tier, serverless) in production.
- **ASP.NET Core Identity**, registration disabled — two seeded accounts only.
- **xUnit** for unit tests, **bUnit** for Blazor component tests (add the package when the first component test lands).
- **GitHub Actions** for CI, deploying to **Azure Container Apps** via OIDC (no stored secrets).

## 3. Solution layout

```
HomeFinance.slnx
├── src/HomeFinance.Web/    # Blazor Web App host (shell, auth, routing, pages)
├── src/HomeFinance.Core/   # Entities, domain logic, module contracts
├── src/HomeFinance.Data/   # EF Core DbContext, migrations, repositories
├── modules/                # Feature modules (Razor Class Libraries) — added from Phase 4
└── tests/HomeFinance.Tests/
```

Dependency rules:
- `Web` → `Core`, `Data`.
- `Data` → `Core`.
- `Core` → nothing project-local.
- Modules (later) → `Core` only. **Modules must never reference each other.**
- Tests reference the projects they cover; production code never references tests.

## 4. Conventions

**Naming**
- Types, methods, properties: `PascalCase`. Locals, parameters: `camelCase`. Private fields: `_camelCase`. Constants: `PascalCase`. Async methods end in `Async`.
- Files match the primary type they contain. One public type per file unless tiny records live together.
- Interfaces: `IThing`. Do not prefix classes with `C`.

**C# style**
- File-scoped namespaces. `using` directives sorted, `System.*` first.
- Prefer `record` for immutable DTOs, `sealed class` by default for services, `readonly struct` for small value types.
- Nullable reference types are ON — no `!` bang operator unless a comment explains why the null is impossible.
- `var` when the type is obvious from the RHS, explicit type otherwise.
- No `#region`. No commented-out code. No `TODO` without an owner and a reason.

**Blazor / MudBlazor**
- Pages under `src/HomeFinance.Web/Components/Pages/`, layouts under `Layout/`, shared components under `Shared/`.
- Use MudBlazor components (`MudTable`, `MudForm`, `MudDialog`, `MudSnackbar`, `MudChart`) — no raw `<table>` / Bootstrap.
- Prefer `InteractiveServer` render mode for now. Only mark a component `InteractiveAuto`/`InteractiveWebAssembly` if it has a real reason to run client-side.
- Extract logic-heavy components into a `.razor.cs` code-behind partial class.

**EF Core**
- One `DbContext` per bounded area (`HomeFinanceDbContext` in `Data`; modules get their own later).
- Entity configs go in `IEntityTypeConfiguration<T>` classes under `Data/Configurations/`, applied via `ApplyConfigurationsFromAssembly`.
- Every schema change ships as a migration. Never edit an applied migration — add a new one.
- Money as `decimal(18,2)`. Dates as `DateOnly` where time-of-day is meaningless.

**Dependency injection**
- Register services in feature-scoped extension methods (`AddCoreServices`, `AddDataServices`) — do not fatten `Program.cs`.
- Services are `sealed` and depend on interfaces, not concretes.

## 5. Build & test commands

Run from repo root:

```bash
dotnet restore
dotnet build           # must be warning-free; treat warnings as errors in CI
dotnet test            # all tests must pass before any commit
dotnet format          # run before finishing a task
```

Blazor dev loop:

```bash
dotnet watch --project src/HomeFinance.Web
```

EF migrations:

```bash
dotnet ef migrations add <Name> --project src/HomeFinance.Data --startup-project src/HomeFinance.Web
dotnet ef database update --project src/HomeFinance.Data --startup-project src/HomeFinance.Web
```

## 6. Testing rules

- **Always add tests.** A feature is not done until it has meaningful test coverage.
- xUnit for pure logic; bUnit for Blazor components.
- Test naming: `MethodOrScenario_Condition_ExpectedResult`.
- Arrange–Act–Assert, one behavior per test. No shared mutable state between tests.
- Prefer in-memory SQLite (not the EF InMemory provider — it lies about relational constraints) for Data-layer tests.
- Test-writer agent **must not** modify production code. Implementer agent **must** run `dotnet test` before declaring done.

## 7. Git & commits

- Small commits, imperative subject lines: `Add transaction entity`, `Fix category filter on monthly view`.
- One logical change per commit. No "wip" or "fix stuff" messages.
- Never `--no-verify`, never force-push to `main`.
- Keep a `DECISIONS.md` log for non-obvious architectural choices (see `PLAN.md` §4.6).

## 8. Multi-agent workflow (see `PLAN.md` §4.2)

Per feature: **architect** plans → **implementer** builds → **test-writer** covers → **reviewer** critiques → user arbitrates.

- `architect` writes specs and interfaces, **never code**.
- `implementer` executes the architect's plan literally. If the plan is wrong, escalate — do not silently redesign.
- `test-writer` writes tests only; may read all code but edits only under `tests/`.
- `reviewer` is read-only; produces a findings list against this file.

When an agent produces something wrong, **fix the instruction** (this file, the agent definition, or the prompt) rather than patching the output by hand.

## 9. Do NOT

- Add new NuGet packages, change target framework, or swap the UI/data stack without an explicit go-ahead.
- Introduce microservices, separate APIs, or Kubernetes. This is a **modular monolith** (`PLAN.md` §2.1).
- Build open-banking / PSD2 integrations — CSV import only (`PLAN.md` §5).
- Add features not requested by the current task. No "while I'm here" refactors.
- Add error handling, retries, or validation for scenarios that cannot happen. Validate only at system boundaries.
- Write comments that restate what the code does. Only comment the non-obvious *why*.
- Create `.md` documentation files unless explicitly asked.
- Commit secrets. Connection strings come from environment or Key Vault, never from source.

## 10. What to read next

- `PLAN.md` — full roadmap, rationale, module architecture, cost model.
- `.claude/agents/*.md` — role definitions for the four subagents.
- `DECISIONS.md` — created on first non-obvious decision.
