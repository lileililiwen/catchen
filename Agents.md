# Agents.md

> This document is the contract for AI agents (and humans) working on the
> Catchen codebase. It is **normative**: every principle here MUST be
> followed unless explicitly overridden by a written decision in an
> OpenSpec change.

---

## 1. What is Catchen?

Catchen (茶餐 Chen) is an **offshore-only cooking platform** for English-speaking
consumers outside Mainland China: standardized original Chinese recipes with
measurable quantities, locally obtainable substitutes, and Western kitchen
equipment; monthly memberships and one-time recipe PDF sales; and — in later
phases — a verified tutor marketplace and cooking community.

**Positioning — strict non-negotiable constraints:**

- **Offshore only.** Production hosting, legal entities, identity, payments,
  storage, and analytics stay outside Mainland China. Registration MUST reject
  `+86` telephone numbers. Domestic payment channels (WeChat Pay, Alipay) and
  domestic promotion channels (Xiaohongshu, Douyin, domestic WeChat groups)
  are prohibited.
- **Original content only.** Every published recipe requires originality,
  measurement, substitution, equipment, and cultural-context validation with
  provenance evidence. No automated copying/translation of third-party
  influencer content.
- **Server-authoritative policy.** All access, entitlement, validation, and
  moderation decisions are enforced by the backend; clients present results
  only.
- **Phase gating.** Phase 1 (recipe membership) excludes tutors, courses,
  revenue sharing, live streaming, and community features. Phase 2 opens the
  tutor marketplace; Phase 3 opens live/community/partnership features. Never
  implement a later phase's capability early.

**Status:** spec-first bootstrapping. OpenSpec changes are authored before any
implementation exists.

---

## 2. Architecture — Modular Monolith + Flutter Clients

Backend: ASP.NET Core on a supported .NET LTS release, PostgreSQL, background
workers, object storage — one deployable modular monolith. Clients: Flutter
(consumer + operations console), decoupled via generated API contracts.

```
backend/
├── src/
│   ├── Catchen.Identity/        # accounts, roles, region policy, agreements
│   ├── Catchen.Catalog/         # recipes, taxonomy, discovery, favorites
│   ├── Catchen.Editorial/       # drafts, validation engine, review workflow
│   ├── Catchen.Commerce/        # products, orders, payments, entitlements
│   ├── Catchen.Documents/       # recipe/shopping-list PDF generation
│   ├── Catchen.Affiliates/      # redirects, attribution, statements
│   ├── Catchen.Moderation/      # reports, decisions, audit trails
│   ├── Catchen.Reporting/       # operational dashboards, exports
│   ├── Catchen.Data/            # central AppDbContext, migrations, seeding
│   └── Catchen.Api/             # Minimal API shell + DI composition root
├── tests/                       # xUnit unit + architecture test projects
clients/
├── consumer/                    # Flutter consumer app
└── ops/                         # Flutter operations console
```

### 2.1 Adding a new backend module (the fixed pattern)

1. Create `src/Catchen.<Module>` class library; add to the solution; reference
   only the modules it needs (never `Catchen.Data`).
2. `Models/` — entities. `Configuration/` — one `IEntityTypeConfiguration<T>`
   per entity. `Services/` — services injecting the base
   `Microsoft.EntityFrameworkCore.DbContext` and using `Set<T>()`.
   `<Module>ModuleExtensions.cs` — `AddXxxModule(IServiceCollection)`.
3. In `Catchen.Data/AppDbContext.cs`: add `DbSet`s and ONE line
   `builder.ApplyConfigurationsFromAssembly(typeof(<Entity>).Assembly);`
4. In `Catchen.Api/Program.cs`: one line `builder.Services.AddXxxModule();`.
5. Create the EF migration:
   `dotnet ef migrations add <Name> --project backend/src/Catchen.Data --startup-project backend/src/Catchen.Api`

**Dependency rules (enforced by architecture tests):**

- Modules MUST NOT reference `Catchen.Data` or `Catchen.Api`; services depend
  on the base `DbContext`.
- Cross-module navigation collections are avoided; queries go through module
  services.
- Multi-user isolation is a hard rule: every query filters by owner or by
  explicit admin scope.

---

## 3. Spec-First Development — the Fixed Workflow

**Every change goes through OpenSpec BEFORE any code is written.**

```
propose  →  validate  →  implement (apply)  →  archive  →  spec is source of truth
```

### 3.1 The four canonical docs

Every change MUST contain exactly:

```
openspec/changes/<name>/
├── proposal.md                 # WHY: motivation, what changes, capabilities, impact
├── specs/<cap>/spec.md         # WHAT: "## ADDED Requirements" with SHALL/MUST +
│                               #   "#### Scenario:" blocks (exactly 4 hashtags)
├── design.md                   # HOW: context, decisions with rationale, risks, migration
└── tasks.md                    # checkbox implementation list, grouped "## N." headings
```

- Use SHALL/MUST for normative requirements; every requirement needs at least
  one WHEN/THEN scenario.
- Behavior changes to existing capabilities use `## MODIFIED Requirements`
  (copy the FULL requirement block from `openspec/specs/<cap>/spec.md`).
- The **source of truth** for any capability is `openspec/specs/<cap>/spec.md`.
  Code that drifts from it is a bug.

### 3.2 Sequential processing rule

Implement pending changes one at a time, in roadmap order. A change is finished
only when implemented AND archived AND merged to `main` via a reviewed PR. Only
then may the next change start. Never interleave changes.

### 3.3 "Serious code" standard

Implementation is expected to be **production-quality, not stubs**: real
working code per spec scenarios — no `TODO`, no placeholder returns, no
`NotImplementedException`. Security care on every endpoint: authorization,
ownership filters (`currentUserId != owner` → 404), server-side validation
(never trust the client), signed webhook inboxes with idempotency, no secrets
in logs/URLs, input length limits. Financial logic (commissions, ledgers,
payouts) MUST be immutable-append and reconcilable. Every feature MUST be
exercised end-to-end over HTTP before declaring done.

---

## 4. Build, Run & Verify

```bash
dotnet build backend                      # MUST finish 0 warnings / 0 errors
dotnet test backend                       # unit + architecture tests
flutter analyze && flutter test           # client analysis + tests
dotnet format --verify-no-changes         # formatting gate
dart format --set-exit-if-changed --output=none
```

Verification standard: build clean, migrations apply, seed runs, every spec
scenario exercised via HTTP (curl with JWT from `/api/auth/login`), role gating
and multi-user isolation checked, negative scenarios (invalid regions, missing
fields, non-owners, webhook replays) covered.

---

## 5. Agent Workflow Checklist

When asked to implement a feature or spec:

1. **Read** the change folder (`proposal.md`, `specs/<cap>/spec.md`,
   `design.md`, `tasks.md`) plus the source-of-truth specs in
   `openspec/specs/`.
2. **Check the roadmap order** (§7). One change at a time, never jump ahead.
3. **Explore & reuse** *(mandatory)* — search the tree for existing entities,
   services, endpoints, and patterns before planning; name what you reuse in
   `design.md`.
4. **Plan** by walking `tasks.md` top-to-bottom.
5. **Implement** in the module pattern (§2).
6. **Smoke-test** every scenario at the HTTP layer before declaring done.
7. **Build & test** — 0 warnings / 0 errors; all suites green.
8. **Update** `tasks.md` — every box checked.
9. **Archive** — `openspec archive <name> -y`.
10. **Commit & land via PR** — conventional message, feature branch named after
    the change, required CI checks green, one approving review.

> **Global-view rule:** if you cannot point at the existing module or utility
> your change depends on, stop and explore before writing code.

---

## 6. Anti-Patterns (do not do these)

- **Don't** write code before the spec change exists and validates.
- **Don't** implement two changes at once or skip roadmap order.
- **Don't** edit files outside your module except the two composition-root
  lines (AppDbContext scanning, Program.cs registration).
- **Don't** create circular references: modules never reference
  `Catchen.Data`; services inject the base `DbContext`.
- **Don't** leave stubs, TODOs, or unhandled error paths in shipped code.
- **Don't** trust client input — enforce ownership/roles/region server-side.
- **Don't** introduce domestic (Mainland China) payment or promotion channels,
  or accept `+86` numbers anywhere.
- **Don't** hardcode provider API keys (Stripe/PayPal/LLM) — config only.
- **Don't** publish recipes without the full originality/validation evidence.
- **Don't** archive a change whose spec scenarios are not verified.

---

## 7. Current State & Roadmap

### 7.1 Shipped & archived

None yet. Quality gates land first so every later change merges under full
gating.

### 7.2 Pending changes (implement in this order)

| Order | Change | Capabilities | One-line summary |
|---|---|---|---|
| 1 | `add-quality-gates` | editorconfig-and-analyzers, architecture-enforcement, coverage-gates, dependency-audit, git-hooks, ci-pipeline, ai-code-conventions, branch-protection | Formatting/analyzers, architecture tests, incremental coverage, dependency audit, hooks, CI, AI conventions, branch protection |
| 2 | `launch-overseas-recipe-membership` | offshore-user-access, standardized-recipe-catalog, recipe-commerce, shopping-and-affiliates, recipe-operations | Phase 1: offshore consumer membership, recipe catalog/commerce, PDFs, affiliates, ops console |
| 3 | `add-instructor-course-marketplace` | tutor-onboarding, tutor-content, coaching-bookings, marketplace-engagement, commissions-and-payouts | Phase 2: verified tutor marketplace, courses/coaching, commissions/payouts |
| 4 | `add-cooking-community-ecosystem` | live-learning, events-and-competitions, cooking-community, commercial-partnerships | Phase 3: live learning, competitions, communities, brand partnerships |

### 7.3 Deferred

E-signatures; SSO; batch generation; RBAC organizations; third-party content
ingestion.

---

## 8. References

- `openspec/specs/*/spec.md` — source-of-truth capability specs
- `openspec/changes/<name>/` — active changes (proposal/design/specs/tasks)
- `openspec/config.yaml` — OpenSpec project context (points here as normative)
- `CONTRIBUTING.md` — contribution flow, PR template, AI conventions
