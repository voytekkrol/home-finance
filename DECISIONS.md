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

## 2026-07-15 — Category taxonomy: no "Other", unified personal + sole-prop ledger
Context: One owner has a sole proprietorship (jednoosobowa działalność gospodarcza). They want a single ledger covering all money flows — personal household AND business. An accountant handles formal tax obligations and supplies ZUS/PIT amounts; this app just logs the payments.
Decision:
- **No "Other" / "Miscellaneous" category.** If a transaction has no home, add a category — "Other" hides that signal.
- **Unified ledger (Option B).** One `HomeFinanceDbContext`, one transaction list. Business income and expenses sit alongside personal ones. Categories distinguish them.
- **Transactions are always editable and deletable.** Mistakes are corrected by opening and fixing the row, not by adding correction entries.
- **"Move to Savings Account" is modeled as a signed transaction** in Phase 1 (no transfer concept yet). Phase 3/4 introduces proper account-to-account transfers.
- **Building & Community Fees is separate from Utilities.** Czynsz administracyjny / wspólnota mieszkaniowa / fundusz remontowy is structurally distinct from electricity/gas/water bills, and the household has two apartments generating both kinds of expense.
- **39 seed categories** with a grouped color scheme — similar categories share a color family, making the transaction picker scannable at a glance.
- **Extendable by design.** Categories are fully user-managed via the app (CRUD). No hard-coded category logic — only seed data.
- **No parent/child hierarchy in Phase 1.** Flat list; color families provide visual grouping. Hierarchical collapsing is Phase 2+ UI.
Consequences: `DatabaseInitializer` seeds all 39 categories with colors. Business transactions appear alongside personal — intentional.

### Seed category list (grouped by color family)

**Income — green family**

| # | Name | Type | Color | Examples |
|---|---|---|---|---|
| 1 | Business Revenue | Income | `#2E7D32` | Client invoices, services rendered, project payments |
| 2 | Salary | Income | `#388E3C` | Partner's employment, bonuses, sick pay |
| 3 | Investment Income | Income | `#66BB6A` | Savings interest, dividends, ETF/fund gains |
| 4 | Refunds & Reimbursements | Income | `#81C784` | Product returns, client expense reimbursements, tax refunds |
| 5 | Gifts Received | Income | `#A5D6A7` | Money from family, birthday gifts |

**Housing — blue family**

| # | Name | Type | Color | Examples |
|---|---|---|---|---|
| 6 | Mortgage | Expense | `#1565C0` | Monthly instalment, overpayments |
| 7 | Building & Community Fees | Expense | `#1976D2` | Czynsz administracyjny, wspólnota mieszkaniowa, fundusz remontowy |
| 8 | Property & Home Insurance | Expense | `#1E88E5` | Ubezpieczenie mieszkania, home contents insurance |
| 9 | Utilities | Expense | `#42A5F5` | Electricity, gas, water, district heating (media) |
| 10 | Internet & Phone | Expense | `#90CAF9` | Home internet, mobile plans |
| 11 | Home Maintenance | Expense | `#BBDEFB` | Repairs, renovation, contractors, tools |

**Food — orange family**

| # | Name | Type | Color | Examples |
|---|---|---|---|---|
| 12 | Groceries | Expense | `#E65100` | Supermarkets (Biedronka, Lidl, Kaufland), market, food delivery (groceries not meals) |
| 13 | Dining Out | Expense | `#FF8F00` | Restaurants, cafes, takeaway meals, work lunches |

**Transport — purple family**

| # | Name | Type | Color | Examples |
|---|---|---|---|---|
| 14 | Fuel | Expense | `#4527A0` | Petrol, diesel, EV charging |
| 15 | Vehicle Insurance | Expense | `#7B1FA2` | OC, AC, NNW |
| 16 | Vehicle Maintenance | Expense | `#AB47BC` | Servicing, tyres, repairs, car wash |
| 17 | Public Transport | Expense | `#CE93D8` | ZTM, PKP, intercity, taxi/Bolt |

**Health — red family**

| # | Name | Type | Color | Examples |
|---|---|---|---|---|
| 18 | Healthcare | Expense | `#C62828` | Doctor visits, dentist, pharmacy, lab tests, physio |
| 19 | Health Insurance | Expense | `#EF5350` | Medicover, LuxMed, Enel-Med monthly fee |

**Children — cyan family**

| # | Name | Type | Color | Examples |
|---|---|---|---|---|
| 20 | Childcare & Education | Expense | `#006064` | Kindergarten, school fees, tutoring, textbooks |
| 21 | Children's Activities | Expense | `#00838F` | Sports, music, arts, language classes, camps |
| 22 | Children's Needs | Expense | `#00BCD4` | Clothing, toys, school supplies, books |

**Personal — pink family**

| # | Name | Type | Color | Examples |
|---|---|---|---|---|
| 23 | Personal Care | Expense | `#AD1457` | Haircut, cosmetics, gym, wellness, toiletries |
| 24 | Clothing & Footwear | Expense | `#F06292` | Adult clothing, shoes, accessories, dry cleaning |

**Entertainment & Lifestyle — deep purple family**

| # | Name | Type | Color | Examples |
|---|---|---|---|---|
| 25 | Entertainment | Expense | `#4A148C` | Cinema, concerts, theatre, hobbies, games, sports events |
| 26 | Subscriptions | Expense | `#6A1B9A` | Netflix, Spotify, gaming, software, news |
| 27 | Travel & Holidays | Expense | `#9C27B0` | Flights, hotels, car rental, vacation spending |

**Gifts & Charity — amber family**

| # | Name | Type | Color | Examples |
|---|---|---|---|---|
| 28 | Gifts Given | Expense | `#F57F17` | Birthday/Christmas presents, wedding gifts |
| 29 | Charitable Donations | Expense | `#FF8F00` | Charity, church, crowdfunding |

**Savings & Investments — teal family**

| # | Name | Type | Color | Examples |
|---|---|---|---|---|
| 30 | Move to Savings Account | Expense | `#00695C` | Transfer to savings/emergency fund (Phase 1 workaround; proper transfers in Phase 3/4) |
| 31 | Investments | Expense | `#00897B` | Stocks, ETFs, funds purchased |

**Sole-prop obligations — deep orange family**

| # | Name | Type | Color | Examples |
|---|---|---|---|---|
| 32 | ZUS | Expense | `#BF360C` | Monthly social security (emerytalna, rentowa, chorobowa, wypadkowa, FP) — amount from accountant |
| 33 | Income Tax | Expense | `#D84315` | Quarterly PIT advance payments — amount from accountant |
| 34 | Accountant | Expense | `#FF7043` | Monthly accounting service fee |

**Business expenses — blue-grey family**

| # | Name | Type | Color | Examples |
|---|---|---|---|---|
| 35 | Business Equipment | Expense | `#37474F` | Laptop, phone, hardware, office furniture, tools |
| 36 | Business Software | Expense | `#455A64` | SaaS, cloud, hosting, domain, licences |
| 37 | Business Services | Expense | `#607D8B` | Legal, consulting, subcontractors, notary |
| 38 | Business Travel | Expense | `#78909C` | Work-related transport, accommodation, meals |

**Financial — grey**

| # | Name | Type | Color | Examples |
|---|---|---|---|---|
| 39 | Bank Fees | Expense | `#757575` | Account fees, FX commissions, card fees, wire transfer costs |

## 2026-07-16 — Rich domain model for Phase 1 entities
Context: The first pass at `Account` / `Category` / `Transaction` was an anemic model — every property publicly settable, no constructors, no validation. Business invariants (non-empty name, non-zero amount, valid color, valid currency, non-future dates) lived only in the EF configuration or nowhere at all. Anyone could `new Transaction { Amount = 0 }` and rely on the DB check constraint to catch it.
Decision: Every domain entity uses:
- a private parameterless constructor for EF hydration only,
- a public static `Create(...)` factory that validates every argument and sets `Id` + `CreatedUtc`,
- `init` accessors for values that never change (`Id`, audit fields, immutable FKs like `OwnerUserId` and `EnteredByUserId`),
- `private set` + explicit mutation methods (`Rename`, `Archive`, `ChangeColor`, `Edit`, …) for legitimate state changes,
- `sealed class`, no domain events, no aggregate roots, no unit-of-work.
`Transaction` mutation uses a single coarse-grained `Edit(...)` because the UI dialog edits all fields at once.
Consequences: Object initializers no longer construct valid entities — tests, seed code, and any future service code must go through factories. The `CK_Transaction_AmountNonZero` DB constraint becomes belt-and-braces (kept for defense-in-depth, no dedicated test since no production code path can violate it). Repositories are still not introduced; DI still hands out `HomeFinanceDbContext` directly. If a future requirement needs field-level audit or partial edits, mutation methods can be split — reversing that direction is easier than starting fine-grained.

## 2026-07-16 — Identity user PK stays `string`
Context: `Account.OwnerUserId` and `Transaction.EnteredByUserId` are `string`, which felt off — GUIDs stored as text, no strong typing.
Decision: Keep `string`. ASP.NET Core Identity's default `IdentityUser` uses `string Id` (a stringified GUID). Switching to `IdentityUser<Guid>` would ripple through every FK, every migration, `UserManager<T>`, `SignInManager<T>`, and every seeded row — for a two-user app where the FK is opaque anyway. Not worth the churn.
Consequences: All entities that reference a user hold `string`, not `Guid`. Validation is `ArgumentException.ThrowIfNullOrWhiteSpace`, not `Guid != Guid.Empty`.

## 2026-07-16 — Request DTOs for multi-parameter factories and mutations
Context: `Account.Create(string name, string ownerUserId, AccountType type, string currency, decimal openingBalance)` — five positional parameters, three of them `string`, in a swap-friendly order. C# named arguments help at the call site but are not enforced; a caller can trivially pass `Account.Create(ownerUserId, name, ...)` and the compiler stays silent. `Transaction.Create` and `Transaction.Edit` had the same shape and the same risk.
Decision: Any factory (`Entity.Create`) or mutation method taking two or more parameters takes a single `sealed record` request DTO with `required init` properties. Callers must use object initializer syntax, forcing named-property assignment.
- `CreateAccountRequest`, `CreateCategoryRequest`, `CreateTransactionRequest`, `EditTransactionRequest` live in `src/HomeFinance.Core/Contracts/<Aggregate>/`.
- Single-parameter methods (`Rename(string)`, `ChangeColor(string)`, `Archive()`) stay direct — no DTO needed when there's nothing to swap.
- Rule applies to future entities and service methods, not just Phase 1.
Consequences: Slightly more typing at call sites, in exchange for compiler-verifiable named argument passing. Test-writer's factory tests construct request objects; the seeder in Slice 2 does the same. If future update/patch methods need only a subset of fields, they get their own request type (`EditTransactionRequest` omits `EnteredByUserId` because it is immutable).

## 2026-07-17 — Rename write DTOs to `<Entity>Data`; extract validation into a per-record `<Record>Validator.Invoke` + `Rules` + dedicated exceptions
Context: Two smells surfaced once the entity factories were sitting side by side:
1. Every write DTO ended in `Request`, but `Account.Create(new CreateAccountRequest { … })` stutters — the method already says "Create". `Request` is a web-layer word; these records are pure domain input.
2. Factory bodies opened with 10–15 lines of `ArgumentException.ThrowIfNullOrWhiteSpace`, `Enum.IsDefined`, regex checks, and hand-rolled length limits. A reader had to scroll past the validation to find the object being built, identical rules were repeated across entities, and `Transaction` even had a private `ValidateEditableFields` helper — a smell that the pattern needed a name. A per-field guard call inside `Create` (one line per property) was rejected as still noisy: the factory should orchestrate construction, not enumerate rules.
Decision:
- **Rename convention.** Drop the `Request` suffix.
  - Single write shape → `<Entity>Data` (`AccountData`, `CategoryData`, `ApplicationUserData`).
  - Multiple write shapes → match the entity method verb: `CreateTransactionData` (for `Transaction.Create`), `EditTransactionData` (for `Transaction.Edit`). A future `Update` gets `UpdateXData`.
- **Per-record validator, one call site.** Each data record has a paired `<Record>Validator` static class in the same folder. Its only public member is `Invoke(<Record> data) => <Record>`, returning the *normalized* record (trimmed strings, upper-cased ISO code / hex). The entity's `Create` / `Edit` calls it once and then assigns fields directly. The method is named `Invoke` — `Validate` would stutter against the class name. No other public API surface on the validator; no private validate helpers on entities.
- **Atomic rules in `HomeFinance.Core.Validation.Rules`.** Static class of pure helpers, one per rule: `RequireLabel(value, maxLength, paramName)`, `RequireOptionalLabel`, `RequireIsoCurrencyCode`, `RequireHexColor`, `RequireDefined<TEnum>`, `RequireIdentityUserId`, `RequireNonEmptyGuid`, `RequireNonZero`, `RequireNotFarFuture`. Each returns the normalized value or throws a dedicated exception. Validators are one-liners of `data with { X = Rules.RequireX(...), ... }`. When a new rule appears, extend `Rules` — never re-implement inline.
- **Dedicated, strongly typed, const-messaged exceptions.** Under `HomeFinance.Core.Validation`, all `sealed`, all inherit `DomainValidationException : ArgumentException`. Each defines `public const string Message = "..."` and passes it (plus `paramName`) to the base ctor. Concrete types: `MissingRequiredValueException`, `LabelTooLongException`, `InvalidCurrencyCodeException`, `InvalidHexColorException`, `InvalidEnumValueException`, `InvalidIdentityUserIdException`, `EmptyGuidException`, `ZeroAmountException`, `FutureDateException`. Tests assert on exception *type* and `paramName`, not on message text — the const lets consumers reference `LabelTooLongException.Message` if they ever need the string, without duplicating it.
Consequences: All existing DTO names change (source-level breaking, but Phase 1 is on `phase-1-slice-1` — only the entities themselves and the test factories consume them). `Transaction.ValidateEditableFields` and `Category.Create`'s positional parameters both go away — `Category.Create(new CategoryData { … })` matches the pattern. The `Rules` + validator + exception setup adds ~11 new small files up front, and pays back the moment a second aggregate reuses `RequireLabel` / `RequireHexColor` (already true today). Reviewer must reject any new `if (…) throw new ArgumentException(...)` inside an entity or factory — that pattern is now a code smell, not a style preference. Exception messages are hard-coded consts, so localization stays out of the domain (fine — this app is single-locale).

## 2026-07-18 — Tests reference `HomeFinance.Web`; unit-tests-only
Context: Slice 2 adds `Authentication/*` types (UserSeeder, DisplayNameClaimsPrincipalFactory, DependencyInjection, AuthenticationEndpoints) that the test-writer needs to unit-test. The test project previously referenced only `Core` and `Data`. Options: (a) add a `<ProjectReference>` to `HomeFinance.Web` in the single test project, or (b) create a separate `HomeFinance.Web.Tests` project.
Decision: Option (a) — add `<ProjectReference Include="..\..\src\HomeFinance.Web\HomeFinance.Web.csproj" />` to `HomeFinance.Tests`. Single test project only. No `WebApplicationFactory`, no integration tests of any kind in `HomeFinance.Tests`.
Consequences: The test project now transitively pulls in ASP.NET Core + MudBlazor assemblies, slightly increasing restore and startup time. A separate `HomeFinance.Web.IntegrationTests` project should be created when integration-test need arises. Reviewer must reject any integration-style test (e.g. `WebApplicationFactory`, full HTTP round-trips) added to `HomeFinance.Tests` — those belong in the future integration project.
