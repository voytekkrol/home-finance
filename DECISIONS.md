# Decisions

Append-only log of non-obvious architectural or process decisions. One entry per decision, newest at the bottom. Only record things a future reader (or agent) would ask "why?" about — routine choices belong in the code.

Format:

```
## YYYY-MM-DD — Short title
Context: what problem or situation forced a choice.
Decision: what we picked.
Consequences: what this locks in or rules out.
```

---

## 2026-07-14 — Stack: .NET 10 + Blazor Web App + MudBlazor
Context: Household app for two users, ~$0/month, built as a multi-agent learning exercise. Owner works with Azure daily.
Decision: .NET 10, Blazor Web App (unified model), MudBlazor for UI, EF Core with SQLite locally and Azure SQL free tier in prod, ASP.NET Core Identity with two seeded users.
Consequences: Server interactivity holds a SignalR/WebSocket connection → Static Web Apps is not an option; hosting must be Container Apps or App Service. No OAuth complexity. Agents can rely on the stack being fixed — swapping any layer requires an explicit go-ahead.

## 2026-07-14 — Modular monolith, not microservices
Context: Long-term identity is a core ledger + open-ended "save money" modules (grocery deals, meal planning, subscription audit…).
Decision: One deployable Blazor Web App; every feature beyond the core ledger is a plugin module in its own Razor Class Library, implementing contracts from `HomeFinance.Core`. Modules may reference `Core`; never each other.
Consequences: Extensibility costs nothing at runtime (assemblies + DI). Parallel agent work on modules is safe because they can't share files. Rules out separate APIs, service buses, Kubernetes.

## 2026-07-14 — Hosting: Container Apps + Azure SQL free tier
Context: Target cost ~$0/month, owner familiar with Azure.
Decision: Azure Container Apps (consumption, scale-to-zero) + Azure SQL Database free tier (serverless, auto-pause). Upgrade path to App Service B1 (~$13/mo) if cold starts become annoying in daily use.
Consequences: First page load after idle takes ~5–15 s — accepted for now. GitHub Actions with OIDC federated credentials, no stored secrets. Connection strings via Key Vault + managed identity.

## 2026-07-14 — No open-banking / PSD2, CSV import only
Context: Bank data ingestion is required for the app to survive real use.
Decision: Import from bank-exported CSV files, one importer per Polish bank behind a common `IBankImporter` interface. No PSD2 / open-banking integrations.
Consequences: Avoids licensing pain and per-bank API contracts. Loses ~10% of the value (real-time sync). Each new bank format is an isolated parallel-agent task in Phase 3.

## 2026-07-14 — Multi-agent workflow with four roles
Context: The project's second goal is learning to orchestrate Claude Code agents, not typing code.
Decision: Feature loop is `architect` → `implementer` → `test-writer` → `reviewer` → user arbitrates. Definitions live in `.claude/agents/`. When an agent goes wrong, fix the instruction (agent definition, `CLAUDE.md`, or prompt) — not the output.
Consequences: `CLAUDE.md` is the shared contract every agent reads. Keeps tasks small and independently mergeable. Reviewer findings drive updates to `CLAUDE.md` over time.
