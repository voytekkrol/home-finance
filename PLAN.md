# Home Finance App — Build Plan

A real household-finance app for two users (you + partner), built with .NET/C# and Blazor, hosted for ~$0/month — and deliberately structured as a **learning project for multi-agent development with Claude Code**.

Drop this file into your repo root. It doubles as the roadmap you feed to Claude Code agents.

---

## 1. Goals

1. Working app you two actually use daily (expense tracking, budgets, shared view).
2. Learn to build software by **orchestrating multiple Claude Code agents in parallel**, not by typing code yourself.
3. Total running cost as close to $0/month as possible.

## 2. Stack & Architecture

| Layer | Choice | Why |
|---|---|---|
| Framework | **.NET 9/10, Blazor Web App** (unified model, per-component render modes) | What you chose; modern default, lets you mix Server + WASM interactivity |
| UI components | **MudBlazor** (free, MIT) | Forms, tables, charts, dark mode out of the box — huge time saver |
| Data access | **EF Core** + migrations | Standard, agents know it well |
| Local dev DB | **SQLite** (or SQL Server LocalDB) | Zero-friction local development |
| Auth | **ASP.NET Core Identity**, registration disabled — 2 seeded accounts | You don't need OAuth complexity for 2 users |
| Hosting | **Azure Container Apps** (consumption, scale-to-zero) | You know Azure; monthly free grant covers a household app → ~$0/mo (details §2.1) |
| Database | **Azure SQL Database free tier** (serverless, auto-pause) | 100k vCore-s + 32 GB free per month, per database — replaces Neon if you go Azure |
| CI/CD | **GitHub Actions** (repo on GitHub) → deploy to Azure via OIDC | Build, test, deploy on push to `main`; no stored secrets |
| Upgrade path | App Service **B1** (~$13/mo) if cold starts annoy you | Always-on, zero cold start, dead-simple deploys |

### 2.1 Hosting on Azure — costs and deployment

Since you work with Azure daily, host there — familiarity beats saving pennies on an unfamiliar platform.

**Estimated monthly cost (2 users, household traffic):**

| Setup | Cost | Trade-off |
|---|---|---|
| **Container Apps consumption + Azure SQL free tier** | **~$0–3/mo** | Scales to zero → first page load after idle takes ~5–15 s. Free grants: 180k vCPU-s, 360k GiB-s, 2M requests/mo (Container Apps) + 100k vCore-s, 32 GB (SQL serverless, auto-pause). Your usage (say 2 h/day active at 0.25 vCPU) fits comfortably inside both. |
| **Container Apps with min-replica = 1** | ~$10–15/mo | No cold starts, still consumption-billed |
| **App Service B1 Linux + Azure SQL free** | **~$13/mo** | Always-on, WebSockets fine, simplest mental model — the "little cost, works better" option |
| App Service F1 (free) | $0 | 60 CPU-min/day, quirky limits (5 WebSocket connections) — OK for a demo, not recommended as the target |

**Is a little cost worth it?** Yes, eventually. Recommended path: start on **Container Apps + SQL free tier ($0)**; if the cold start irritates you two in daily use, flip min-replicas to 1 or move to B1 — a one-line change, ~$13/mo. Don't pay before the annoyance is real. Note: Blazor Web App with Server interactivity holds a SignalR/WebSocket connection — this works on Container Apps and App Service alike; it's the reason Static Web Apps (WASM-only) is *not* an option here.

**Recommended deployment (also a great agent exercise):**

1. Repo on GitHub; app has a `Dockerfile`.
2. Infra as code with **Bicep** (resource group, Container Apps env, Azure SQL, Key Vault) — or use **`azd` (Azure Developer CLI)**, which scaffolds Bicep + GitHub Actions pipeline in one `azd init` / `azd up`.
3. GitHub Actions workflow with **OIDC federated credentials** to Azure (no client secrets in the repo): build → test → push image to GHCR or ACR → `az containerapp update`.
4. Connection strings in **Key Vault**, referenced via managed identity — no secrets anywhere.

Let a Claude Code agent write the Bicep + workflow in Phase 0; reviewing IaC an agent wrote is exactly the multi-agent skill you're here to learn.

**Architecture: a modular monolith** — one deployable Blazor Web App, but every feature beyond the core ledger is a **plugin module** in its own project, implementing contracts from `Core`. No separate API, no microservices — extensibility comes from assemblies + DI, which costs nothing to run. Suggested solution layout:

```
HomeFinance.sln
├── src/HomeFinance.Web/            # Blazor Web App host (shell, auth, routing)
├── src/HomeFinance.Core/           # Entities, domain logic, module contracts (IFeatureModule etc.)
├── src/HomeFinance.Data/           # EF Core, DbContext, migrations
├── modules/                        # each module = Razor Class Library, self-contained
│   ├── HomeFinance.Modules.Budgets/
│   ├── HomeFinance.Modules.GroceryDeals/
│   └── HomeFinance.Modules.MealPlanner/
└── tests/HomeFinance.Tests/        # xUnit + bUnit (component tests)
```

Full module architecture in §7.

**Cost summary:** Container Apps free grant $0 + Azure SQL free tier $0 + GitHub $0 = **~$0/month**, with a clean ~$13/mo upgrade path (§2.1) when cold starts get annoying. (Non-Azure alternatives if ever needed: Render free tier + Neon free Postgres.)

## 3. Roadmap (phases = agent-training milestones)

Each phase ships something usable AND teaches you a new multi-agent technique (details in §4).

### Phase 0 — Foundation (weekend 1)
- Repo, solution skeleton, `CLAUDE.md`, EF Core + SQLite, CI pipeline, deploy "hello world" to Azure Container Apps via azd/Bicep + GitHub Actions OIDC.
- *Agent skill learned: writing a good `CLAUDE.md`, plan mode, single-agent flow.*

### Phase 1 — MVP: expense tracking (weeks 1–2)
- Entities: `User`, `Account`, `Category`, `Transaction`.
- Add/edit/delete transactions, category list, monthly view, login for 2 seeded users.
- Deploy to Azure Container Apps with Azure SQL free tier.
- *Agent skill learned: custom subagents (implementer + test-writer + reviewer).*

### Phase 2 — Shared household finances (weeks 3–4)
- One joint household ledger: both partners log into the same picture — all accounts, all transactions, no settlement/"who owes whom" logic (you manage money together, not against each other).
- Budgets per category per month, with progress bars.
- Dashboard: current month summary, spending by category (MudBlazor charts), month-over-month trend — the "where is our money going?" view that feeds all later savings features.
- *Agent skill learned: parallel agents in git worktrees on independent features (budgets vs dashboard).*

### Phase 3 — Automation (weeks 5–6)
- CSV import from your banks (each Polish bank has its own format — one importer each, great parallel-agent task).
- Recurring transactions (rent, subscriptions).
- Rules engine: auto-categorize by merchant name.
- *Agent skill learned: agent-driven TDD; hooks that auto-run tests/format on every edit.*

### Phase 4 — Polish & module infrastructure (weeks 7–8)
- PWA so it installs like a mobile app on your phones.
- Savings goals, trends over months, export to Excel.
- **Build the module contracts (§7.1) and migrate Budgets into the first module** — proves the plugin architecture on a feature you already have.
- *Agent skill learned: long-running background agents, security review workflows.*

### Phase 5 — Savings modules (ongoing, one module at a time)
- GroceryDeals, MealPlanner, SubscriptionAudit, FuelPrices... (catalog in §7.3).
- Each module is built end-to-end by an agent (or agent team) in its own worktree against the frozen contracts.
- *Agent skill learned: contracts-first parallel development — the endgame of this whole curriculum.*

## 4. Learning multi-agent development with Claude Code

This is the actual curriculum. Do these in order — each phase of the app is the exercise for one technique.

### 4.1 Phase 0 technique: the foundation of all agent work
- Write **`CLAUDE.md`** in repo root: stack, conventions (naming, folder layout, "always add tests", "use MudBlazor components"), build/test commands. Every agent reads it automatically — it's how you keep 5 agents consistent.
- Use **plan mode** (Shift+Tab) before any non-trivial task: make the agent propose a plan, critique it, then approve. You're learning to be a reviewer of plans, not a typist.
- One agent, one task, small commits. Get the review muscle first.

### 4.2 Phase 1 technique: role-based subagents
Create custom subagents in `.claude/agents/`:
- `architect.md` — plans features, designs entities/interfaces, writes no code.
- `implementer.md` — writes code following the architect's plan.
- `test-writer.md` — writes xUnit/bUnit tests only; not allowed to modify production code.
- `reviewer.md` — read-only tools; reviews diffs against `CLAUDE.md` conventions and leaves findings.

Workflow per feature: architect plans → implementer builds → test-writer covers → reviewer critiques → you arbitrate. This mirrors a real team and teaches you delegation + verification.

### 4.3 Phase 2 technique: true parallelism with git worktrees
- `git worktree add ../finance-budgets feature/budgets` and `../finance-settlement feature/settlement`.
- Run a separate Claude Code session in each worktree, each on an independent feature.
- You learn: slicing work so agents don't collide, reviewing/merging two PRs, resolving conflicts.
- Rule of thumb: parallelize by **vertical feature** (budgets vs settlement), never by layer (one agent on DB + one on UI of the same feature = merge hell).

### 4.4 Phase 3 technique: TDD loops and hooks
- Tell the test-writer agent to write failing tests from the spec first; implementer's only goal is making them green. Agents are dramatically more reliable with a test target.
- Add **hooks** (`.claude/settings.json`): post-edit hook runs `dotnet format` + `dotnet test`; agents self-correct without you asking.
- CSV importers are the perfect parallel exercise: same interface (`IBankImporter`), one agent per bank format, in separate worktrees.

### 4.5 Phase 4 technique: scale up
- Background/long-running agents for bigger refactors; check in periodically instead of watching.
- Run a dedicated **security-review** pass (Claude Code has a `/security-review` command) before exposing the app publicly.
- Custom slash commands in `.claude/commands/` for repeated rituals, e.g. `/new-feature <name>` that scaffolds the architect→implementer→tests cycle.

### 4.6 Habits that make or break multi-agent work
- **You are the integrator.** Read every diff. Agents drift; `CLAUDE.md` + reviewer agent + tests are your drift guards.
- Keep tasks **small and independently mergeable** (max a few hundred lines of diff).
- When an agent goes wrong, don't patch its output by hand — fix the instruction (`CLAUDE.md`, the subagent definition, or the prompt) so it can't happen again. That's the skill.
- Keep a `DECISIONS.md` log — future agents (and future you) read it.

## 5. Feature ideas (what I'd add)

**Core (Phases 1–2):** transactions, categories, monthly budgets, one joint household view, dashboard with category chart, month-over-month comparison.

**High value for a couple:**
- **Bank CSV/OFX import** — nobody sustains manual entry long-term; this decides whether the app survives.
- **Auto-categorization rules** ("Biedronka → Groceries"), learning from your corrections.
- **Recurring transactions** with "upcoming bills" view.
- **Monthly savings review** — automated summary: spend vs budget, biggest categories, unusual spikes, and concrete "you could save X on Y" suggestions (fed by the §7.3 modules) — the killer feature vs generic apps.
- **PWA install + quick-add form** — logging an expense must take <10 seconds on the phone.

**Later / stretch:**
- Savings goals with progress tracking; net-worth view across accounts.
- Multi-currency (PLN base) with NBP exchange rates API (free).
- Receipt photo → parsed transaction (send image to Claude API — cheap, small volume).
- Yearly report, Excel export, spending anomaly alerts ("groceries 40% above normal").
- Email/notification digest: "week summary + bills due".

**Deliberately skip:** open-banking API integrations (PSD2 licensing pain — CSV import gets you 90% of value), multi-tenant support, microservices, Kubernetes. Cheap and simple wins.

## 7. Extensibility: the savings-module architecture

The app's long-term identity: a core ledger + an open-ended set of **"save money" modules** (grocery deals, meal planning from discounts, fuel prices, subscription audits...). Design for that now, cheaply.

### 7.1 Module contracts (live in `HomeFinance.Core`)

Each module is a Razor Class Library implementing a small set of interfaces:

```csharp
public interface IFeatureModule
{
    string Name { get; }                        // "GroceryDeals"
    void ConfigureServices(IServiceCollection services);
    IEnumerable<NavItem> NavItems { get; }      // adds itself to the menu
}

public interface IDashboardWidget                // renders a card on the home dashboard
{
    string Title { get; }
    Type ComponentType { get; }                  // Blazor component to embed
}

public interface ISavingsAdvisor                 // the key abstraction
{
    // Produces suggestions like "Chicken breast 40% off at Lidl until Sunday"
    Task<IReadOnlyList<SavingsSuggestion>> GetSuggestionsAsync(HouseholdContext ctx);
}

public interface IDealSource                     // pluggable data feed
{
    Task<IReadOnlyList<Deal>> FetchDealsAsync(); // one implementation per shop/source
}
```

The host discovers modules at startup (assembly scanning or explicit registration), calls `ConfigureServices`, merges nav items and dashboard widgets. Modules own their EF entities via separate `DbContext`s or table prefixes, so they never touch core migrations. **Modules may reference `Core`; never each other.** That rule is what makes parallel agent work safe.

### 7.2 Getting deal data cheaply (the hard, honest part)

Polish grocery deals have **no public APIs**. Realistic options, cheapest first:

1. **Scheduled scraping via GitHub Actions (free)** — a cron workflow fetches promo pages/leaflet PDFs (Biedronka, Lidl, Kaufland...), parses them, and POSTs results to your app or commits JSON to the repo. This sidesteps the scale-to-zero problem (a sleeping Container App can't run background jobs) — the *scraper lives outside the app*. (Azure-native alternative: a timer-triggered Azure Function on the consumption plan, also effectively free.)
2. **Claude API as the parser** — leaflets are messy PDFs/images; classic scraping breaks weekly. Instead, send the leaflet to the Claude API: "extract products, prices, discounts as JSON". Robust to layout changes; at your volume (a few leaflets/week) expect **single dollars per month**. This is also a great skill to learn.
3. Aggregator sites (Blix, Moja Gazetka) have the data but no API and restrictive ToS — treat scraping them as fragile and legally grey. Prefer the shops' own public leaflets.

Ship the pipeline as: `IDealSource` per shop → normalized `Deal` table → `ISavingsAdvisor`s consume it.

### 7.3 Savings-module catalog (each = one agent task)

| Module | What it does | Data source |
|---|---|---|
| **GroceryDeals** | Weekly best bargains per category; price history so you know if "promo" is real | Leaflet scraping + Claude parsing (§7.2) |
| **MealPlanner** | "This week's discounted ingredients → 5 cheap, good dinner recipes + shopping list" | Deals table + one Claude API call |
| **SubscriptionAudit** | Detects recurring charges in your transactions, flags price increases and unused subs | Your own ledger — zero external data |
| **FuelPrices** | Cheapest station nearby | Public fuel-price data/scraping |
| **UtilityWatch** | Electricity/gas tariff comparison reminders, energy price alerts | Public tariff pages |
| **PharmacyDeals** | Drugstore promos (Rossmann etc.) for your usual items | Leaflet scraping |
| **PriceTracker** | Watchlist for specific products ("tell me when olive oil < 30 zł/l") | Deals table + alerts |

Start with **SubscriptionAudit** (no external data — pure logic, ships in a day) → **GroceryDeals** → **MealPlanner** (the wow feature; it's mostly prompt engineering on top of GroceryDeals).

### 7.4 Why this is the perfect multi-agent exercise

- **Contracts-first:** you + architect agent freeze the §7.1 interfaces; after that, module internals can't conflict.
- **True parallelism:** each module = own project, own worktree, own agent team (implementer + test-writer). Run three at once; merges are trivial because modules don't share files.
- **Per-module CLAUDE.md:** each `modules/X/` gets its own CLAUDE.md with module-specific rules — you learn scoped agent context.
- **Cheap failure:** a module that turns out bad gets deleted without touching the core. Perfect for experiments.

### 7.5 Revised cost picture

Core app still $0/mo. Add: Claude API for leaflet parsing + recipes ≈ **$1–5/mo** at household volume (pay-as-you-go). GitHub Actions cron: free. Total: **coffee money**.

## 8. What to do first — concrete checklist

1. Create GitHub repo `home-finance`, add this file as `PLAN.md`.
2. `claude` in the repo → ask it (in plan mode) to scaffold the solution per §2 and write the first `CLAUDE.md`. Review the plan hard before approving.
3. Get `dotnet test` + GitHub Actions CI green on the empty skeleton.
4. Deploy hello-world to Azure Container Apps (Dockerfile + azd/Bicep + GitHub Actions OIDC) + create the Azure SQL free-tier database. Prove the ~$0 pipeline end-to-end **before** writing features.
5. Create the four subagents from §4.2.
6. Start Phase 1 with the architect agent: "Design entities and pages for basic expense tracking per PLAN.md Phase 1."

Ship something you both use by the end of week 2 — real usage will drive the backlog better than any plan.
