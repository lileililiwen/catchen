# Contributing to Catchen

`Agents.md` is the normative contract (architecture, spec-first OpenSpec
workflow, verification standard). This file covers the day-to-day mechanics:
branches, commits, hooks, CI, AI conventions, and dependency-risk policy.

## Branches

- `main` is protected: **no direct pushes**. All changes land via pull
  request with the required check green and at least one approving review.
- Feature branches are named after the OpenSpec change they implement:
  `change/<openspec-change-name>` (e.g. `change/add-quality-gates`).
- Implement one OpenSpec change at a time, in roadmap order (`Agents.md` §7).

## Commits

Conventional style: short imperative title, blank line, body explaining
*why* and *what*. Reference the OpenSpec change name in the body.

## Local hooks (Husky.Net)

Hooks auto-install on `dotnet restore` once a solution exists. Before the
backend scaffold lands (or on a fresh clone without restore), bootstrap
manually:

```bash
dotnet tool restore && dotnet husky install
```

- **pre-commit** — format check on staged `.cs`/`.dart` files only.
- **pre-push** — full build under warnings-as-errors.

Both no-op until the backend solution / client workspace scaffolds exist.
Skipping is allowed for a single command (`git commit --no-verify`) or
entirely (`HUSKY=0`); **CI remains the authoritative gate**.

## CI and required checks

CI runs on every push to `main` and every pull request:

| Job | Steps |
|-----|-------|
| `build` | NuGet audit → `dotnet format --verify-no-changes` → build `/warnaserror` → tests with OpenCover coverage → incremental coverage gate (PRs only) |
| `flutter` | pub get → `dart format` verify → `flutter analyze` → pub audit → `flutter test --coverage` |

Dotnet/flutter steps skip gracefully until their scaffolds exist; the jobs
still run so checks are stable from day one.

**Required merge check: `build`.** Configure it once the repository is
pushed to GitHub:
*Settings → Branches → Add branch protection rule for `main`:*
require a pull request before merging, 1 approval, require status check
`build`, require branches up to date, dismiss stale approvals on new commits.

### Incremental coverage gate

New executable lines added by a PR must be covered at or above **80%**
(`scripts/check_incremental_coverage.py`). Test projects, EF migrations,
and generated code (`*.g.cs`, `*.g.dart`, designer files) are excluded.
Overall/legacy coverage is reported but never gates.

## Dependency risk policy

High/Critical advisories fail CI (NuGet: restore-time `NuGetAudit` + CI
report; pub: `dart pub audit`, falling back to a recorded outdated report).
A finding may pass only with a **dated, owner-signed entry** in
[SECURITY-ACCEPTANCES.md](SECURITY-ACCEPTANCES.md) — and for NuGet also an
ID in `NuGetAuditSuppress` inside `Directory.Build.props`.

## AI involvement convention

Every PR records AI involvement via one marker (in the PR description or any
commit message):

- `AI: generated` — blocks written by an AI with minor human edits
- `AI: assisted` — human-written with AI suggestions/refactors
- `AI: none` — human-authored

Generated/assisted PRs get the AI review checklist (in the PR template):
spec compliance, authorization/ownership checks, injection safety, test
coverage or a stated reason, no dead code. A workflow posts a **non-blocking**
soft warning when a PR adds 500+ lines with no marker.

## Verification standard

Before opening a PR, follow `Agents.md` §4–5: build clean (0 warnings / 0
errors), all test suites green, every spec scenario exercised end-to-end
(happy path AND negative cases), `tasks.md` boxes checked.
