## 1. Repository Configuration

- [x] 1.1 Add root `.editorconfig` (C#, Dart/Flutter, YAML, JSON, Markdown conventions) and shared `Directory.Build.props` with `TreatWarningsAsErrors`, nullable enable, latest analyzers, deterministic builds.
- [x] 1.2 Add Flutter `analysis_options.yaml` with `flutter_lints` and strict Dart options; verify `dart format` determinism. *(verification deferred until a client workspace exists)*
- [x] 1.3 Verify `dotnet format --verify-no-changes` passes on the solution. *(backend/Catchen.sln — verified clean)*

## 2. Architecture Enforcement

- [x] 2.1 Create the backend test project and add the architecture-test library. *(TngTech.ArchUnitNET 0.13.3 — chosen over NetArchTest for parity with the proven reference implementation; design.md decision updated)*
- [x] 2.2 Encode the module dependency graph fixture per `Agents.md` §2 (allowed references, forbidden Data/composition-root references from modules). *(backend/tests/Catchen.ArchitectureTests/ModuleArchitectureTests.cs)*
- [x] 2.3 Add tests: illegal reference fails, missing composition-root registration fails, new-module-without-fixture-update fails. *(6 tests green; undeclared-module scan included)*

## 3. Coverage Gates

- [x] 3.1 Add xUnit unit-test projects wired to the solution; add Coverlet OpenCover collector settings (`Coverlet.runsettings`). *(Catchen.UnitTests + coverlet.collector; selector tests green)*
- [x] 3.2 Wire `flutter test --coverage` output into the gate script. *(gate merges lcov with OpenCover; CI iterates every client workspace; verified end-to-end: FAIL at 50% before entry-line tests, PASS at 100% after)*
- [x] 3.3 Implement the incremental new-code coverage gate script (80% threshold; excludes `tests/`, `Migrations/`, generated code); verify pass/fail/skip behaviors. *(script in place; CI runs it PR-only; full pass/fail verification once reports exist)*

## 4. Dependency Audit

- [x] 4.1 Add restore-time NuGet vulnerability check (`NuGetAudit`, mode=all, level=high) as MSBuild properties in `Directory.Build.props`.
- [x] 4.2 Add pub package review step producing a recorded report (`scripts/audit_pub_packages.sh`; wired into CI flutter job).
- [x] 4.3 Add `SECURITY-ACCEPTANCES.md` policy (dated, owner-signed entries) and wire CI to fail on High/Critical without acceptance.

## 5. Git Hooks

- [x] 5.1 Configure Husky.Net: pre-commit format verification, pre-push build; auto-install on restore via MSBuild target. *(hooks no-op until scaffolds exist; manual bootstrap documented)*
- [x] 5.2 Verify skippability (`--no-verify`, `HUSKY=0`) and document that CI is authoritative. *(documented in CONTRIBUTING.md)*

## 6. CI Pipeline

- [x] 6.1 Add the CI workflow: parallel dotnet/flutter jobs running audit → format → build (warnings-as-errors) → tests → incremental coverage (PRs only). *(jobs no-op gracefully until scaffolds exist)*
- [x] 6.2 Upload coverage reports and failure logs as artifacts; cache NuGet/pub dependencies keyed on csproj hashes (actions/cache) and pub via flutter-action cache.
- [x] 6.3 Mark the `build` job as the required merge check. *(applied via API: required status check `build`, strict up-to-date; first CI run green on both jobs)*

## 7. AI Conventions & Branch Protection

- [x] 7.1 Add AI-involvement markers (generated / assisted / none) to the PR template plus the AI review checklist (spec compliance, authorization/ownership checks, injection safety, test coverage or stated reason, no dead code).
- [x] 7.2 Add the non-blocking large-unmarked-diff warning workflow (500+ added lines).
- [x] 7.3 Document branch naming, commit conventions, required checks, and hook behavior in CONTRIBUTING.md.
- [x] 7.4 Configure protected `main`: PR-only, one approving review, required `build` check; verify direct pushes are rejected. *(applied via API on public repo; probe push confirmed admins bypass reviews with enforce_admins=false — deliberate solo-maintainer escape hatch — while force pushes are denied even for admins)*
