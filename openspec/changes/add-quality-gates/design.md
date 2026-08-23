## Context

Catchen has three pending OpenSpec changes (Phase 1 recipe membership, Phase 2 tutor marketplace, Phase 3 community ecosystem) and no implementation yet. The stack is an ASP.NET Core modular monolith (C#, .NET LTS, PostgreSQL) with Flutter consumer/admin clients. This change is modeled on the proven quality-gate set used by the OpenDockify project (editorconfig-and-analyzers, architecture-enforcement, coverage-gates, nuget-audit, git-hooks, ci-pipeline, ai-code-conventions, branch-protection), adapted to a dual C#/Flutter codebase and to Catchen's offshore-only compliance posture.

## Goals / Non-Goals

**Goals:**

- Make quality gates executable and authoritative in CI before any product feature lands.
- Enforce the modular-monolith boundaries described in `Agents.md` §2 by tests, not convention.
- Keep the gate fast: incremental coverage on new code only; legacy coverage reported, not gated.
- Cover both halves of the stack (dotnet + flutter) with equivalent discipline.

**Non-Goals:**

- Any product capability from Phase 1–3.
- E2E device-farm testing infrastructure (deferred until Phase 1 clients exist).
- Load/performance testing, chaos testing.
- Blocking gates on overall legacy coverage.

## Decisions

### One change, eight capabilities

Ship all eight quality capabilities as a single change so the first product commit already lands under full gating. Splitting them would leave windows where merges bypass some gates.

Alternative: sequence them as eight changes. Rejected — they are small, independent of product work, and only valuable together.

### Warnings are errors, everywhere

`Directory.Build.props` sets `TreatWarningsAsErrors=true`, `Nullable=enable`, latest analyzers, and deterministic builds for all C# projects; `analysis_options.yaml` enables `flutter_lints` plus strict-mode Dart options. Formatting is verified (`dotnet format --verify-no-changes`, `dart format --set-exit-if-changed --output=none`) rather than auto-applied in CI.

Alternative: warnings-as-notices. Rejected — notice-level warnings accumulate silently and become permanent debt.

### Architecture tests via NetArchTest.Rules

Use NetArchTest.Rules (lightweight, xUnit-native) to assert: modules never reference the Data/composition-root projects; services depend on abstractions; no cross-module entity navigation shortcuts; every module is registered in the composition root. A fixture lists the allowed dependency graph per `Agents.md`; adding a module without updating the fixture fails the suite.

Alternative: ArchUnitNET. Rejected — heavier API for the same assertions at this solution size.

### Incremental coverage via diff-based gate script

Backend: xUnit + Coverlet (OpenCover collector). Client: `flutter test --coverage`. A script diffs the PR base ref, computes covered lines among added executable lines (excluding `tests/`, `Migrations/`, generated `.g.dart`/`.freezed.dart`, designer files), and fails below 80%. Runs only where a base ref exists (PRs), skipped on direct pushes to `main`.

Alternative: gate total coverage. Rejected — punishes contributors for pre-existing gaps and stalls Phase 1 velocity.

### Dual-source dependency audit

`dotnet list package --vulnerable --include-transitive` for NuGet; `dart pub outdated`/audit-equivalent review for pub packages (recorded output checked into CI logs). High/Critical findings fail CI unless a dated, owner-signed acceptance entry exists in `SECURITY-ACCEPTANCES.md`.

Alternative: Dependabot only. Rejected — PR noise without a merge-blocking gate.

### Husky.Net hooks mirror CI

Pre-commit runs formatting verification; pre-push runs build. Auto-installed via MSBuild target on restore. Skippable (`--no-verify`, `HUSKY=0`) because CI is authoritative.

## Risks / Trade-offs

- [Dual-stack CI is slower than single-stack] → Parallel jobs (dotnet / flutter) with path filters; cache NuGet and pub caches keyed on lockfiles.
- [Incremental coverage can be gamed by trivial tests] → AI/human review checklist requires meaningful scenario coverage; architecture tests keep structure honest.
- [NetArchTest rules drift from documented graph] → The fixture IS the graph; `Agents.md` links to it, and a test asserts every module appears in the fixture.
- [pub has no first-party vulnerability audit] → Manual review step documented in CONTRIBUTING; upgrade to a third-party scanner if ecosystem tooling matures.
- [Hooks annoy contributors] → Hooks are skippable and fast (<30s); CI remains the real gate.

## Migration Plan

1. Land repository config (`.editorconfig`, `Directory.Build.props`, `analysis_options.yaml`, hook wiring) — nothing else depends on it.
2. Add test projects and the architecture-test fixture alongside the empty solution scaffold.
3. Add the coverage gate script and dependency audit scripts.
4. Add the CI workflow last, enabling all steps once each local equivalent passes locally.
5. Configure branch protection after the first green CI run on `main`.

## Open Questions

- Exact .NET LTS minor version and Flutter stable channel pinning policy (patch-pin vs minor-pin)?
- Coverage threshold 80% confirmed, or start lower (70%) for Phase 1 bootstrap?
