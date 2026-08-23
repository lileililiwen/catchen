## Why

Catchen is a greenfield offshore product (ASP.NET Core backend + Flutter clients) that will grow through three gated phases into a two-sided marketplace handling payments, commissions, moderation, and user-generated content. Without enforced quality gates from day one, the codebase will drift: module boundaries will erode, untested financial logic will ship, vulnerable dependencies will accumulate, and AI-generated contributions will land without review discipline. Quality must be specified and automated before the first Phase 1 feature lands, exactly as `Agents.md` requires.

## What Changes

- Add a root `.editorconfig`, shared `Directory.Build.props` with warnings-as-errors and analyzers for the C# solution, and a strict `analysis_options.yaml` for the Flutter clients, with deterministic formatting verification.
- Encode the modular-monolith boundary rules as automated architecture tests that run in CI.
- Add xUnit test projects for the backend and Flutter test coverage, with an incremental new-code coverage gate (threshold 80%) that reports but does not gate legacy lines.
- Add restore-time and CI dependency vulnerability auditing for NuGet packages and pub packages, with an explicit accept policy.
- Add Husky.Net local hooks: pre-commit formatting check, pre-push build; auto-installed on restore, skippable, with CI remaining authoritative.
- Add a CI pipeline on every push to `main` and every pull request: audit, format, build 0 warnings / 0 errors, tests, incremental coverage.
- Add AI-involvement PR markers (generated / assisted / none), an AI review checklist, and a non-blocking large-unmarked-diff warning.
- Add branch protection policy for `main`: PR-only merges, required CI checks, one approving review, plus CONTRIBUTING guidance and a PR template.

## Capabilities

### New Capabilities

- `editorconfig-and-analyzers`: Repository-wide formatting and static analysis configuration for C# and Dart with deterministic verification.
- `architecture-enforcement`: Automated tests enforcing module boundaries, allowed dependencies, and composition-root wiring.
- `coverage-gates`: Unit test suites and an incremental new-code coverage gate with defined exclusions.
- `dependency-audit`: Vulnerability scanning of NuGet and pub dependencies at restore time and in CI, with explicit risk acceptance.
- `git-hooks`: Local pre-commit/pre-push quality hooks that are auto-installed and skippable.
- `ci-pipeline`: The authoritative CI pipeline gating every push and pull request.
- `ai-code-conventions`: Recording and reviewing AI involvement in changes.
- `branch-protection`: Protected `main` merge policy and contribution documentation.

### Modified Capabilities

None. These capabilities are platform-level and precede all product phases.

## Impact

Adds repository tooling, test projects, CI workflow definitions, hook configuration, and contribution documentation. No product behavior changes. All later changes (`launch-overseas-recipe-membership` and beyond) inherit these gates; their tasks already assume automated unit/integration/security tests exist.
