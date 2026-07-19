# Phase 1 — MVP: expense tracking

Roadmap for the Phase 1 slices from `PLAN.md` §3: *"Add/edit/delete transactions, category list, monthly view, login for 2 seeded users."*

Each row is one independently mergeable slice (a few hundred diff lines, per `CLAUDE.md` §8). The `architect` agent produces a full detailed plan for one slice at a time; this file only tracks scope, order and dependencies.

## Status

| # | Slice | Status | Merged |
|---|---|---|---|
| 1 | Rich domain model, EF layer, validation, tests | ✅ done | `866aba3` |
| 2 | Identity wiring + 2 seeded users + login/logout | ✅ done | — |
| 3 | Seed 39 categories + read-only category list page | ✅ done | — |
| 4 | Accounts CRUD (list + create + rename + archive) | ⚪ pending | — |
| 5 | Transactions list + create form | ⚪ pending | — |
| 6 | Transaction edit + delete | ⚪ pending | — |
| 7 | Monthly view (totals + category breakdown) | ⚪ pending | — |

Deployment to Azure Container Apps + Azure SQL free tier is Phase 1's original scope per `PLAN.md` but is tracked as Phase 0's outstanding item; it does not gate the app-functionality slices.

## Slice details

### Slice 1 — Rich domain model, EF, validation, tests *(merged)*

Entities `Account`, `Category`, `Transaction`, `ApplicationUser` with `Create` factories, `<Entity>Data` records, `<Record>Validator.Invoke` normalization, `Rules.*` helpers, dedicated exceptions. `HomeFinanceDbContext` (Identity-based), configurations, initial migration. Unit tests over entities, validators, DbContext behaviour.

### Slice 2 — Identity + 2 seeded users + login/logout *(next)*

ASP.NET Core Identity wired in `Program.cs`, cookie auth, MudBlazor `/login` page, minimal-API `POST /login`/`POST /logout` endpoints, idempotent `UserSeeder` reading `Authentication:SeededUsers` from configuration. Global `FallbackPolicy` requiring authenticated user. App-bar shows current user's `DisplayName` (via custom claims-principal factory) + sign-out.

- **Depends on:** slice 1 only.
- **Enables:** any slice that needs an authenticated principal — i.e. all remaining slices (transactions need `EnteredByUserId`, accounts need `OwnerUserId`).
- **No new migration** — Identity schema already provisioned in `20260716170351_InitialCreate`.
- **Rough size:** ~350 LoC, ~10 new files.

### Slice 3 — Seed 39 categories + read-only category list

Idempotent seeder loads the 39 categories from `DECISIONS.md` 2026-07-15 with the grouped color scheme. `/categories` page renders a MudBlazor read-only table with color swatches.

- **Depends on:** slice 2 (page requires auth).
- **Enables:** slice 5 (transaction form's category picker).
- **Category CRUD is deferred** — Phase 1 ships a viewer only; management is Phase 2 polish.
- **Rough size:** ~250 LoC.

### Slice 4 — Accounts CRUD

`/accounts` page: MudTable of the current user's accounts, MudDialog create/edit form using `AccountData`. `Rename` and `Archive` mutations exposed via row actions.

- **Depends on:** slice 2 (per-user `OwnerUserId` from claims).
- **Enables:** slice 5 (transaction form's account picker).
- **Rough size:** ~350 LoC.

### Slice 5 — Transactions list + create

`/transactions` page: MudTable of transactions filtered by month picker; MudDialog create form binding `CreateTransactionData` with account + category pickers.

- **Depends on:** slices 2, 3, 4.
- **Split from edit/delete** to keep this slice within the budget.
- **Rough size:** ~400 LoC.

### Slice 6 — Transaction edit + delete

Edit dialog reuses the form component with `EditTransactionData`; delete via `MudMessageBox` confirmation.

- **Depends on:** slice 5.
- **Rough size:** ~200 LoC.

### Slice 7 — Monthly view

`/monthly` page: month picker, income vs expense totals, category breakdown table using the existing `MonthRange` type. Charts are Phase 2.

- **Depends on:** slices 5 + 6 (needs real transaction data).
- **Rough size:** ~300 LoC.

## Rules for editing this file

- Update the **Status** table when a slice merges (mark ✅, link the merge commit).
- Do not add slice detail below the level shown here — full designs live in the architect's per-slice output, not in this file.
- If slice order changes because of a discovered dependency, update this file and note the reason in `DECISIONS.md`.
