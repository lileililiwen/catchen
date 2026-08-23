# ci-pipeline Specification

## Purpose
TBD - created by archiving change add-quality-gates. Update Purpose after archive.
## Requirements
### Requirement: Every push and PR is verified by CI

The system SHALL run a CI pipeline on every push to `main` and every pull request that: audits dependencies, checks formatting, builds with warnings-as-errors, runs the test suites, and runs the incremental coverage gate on pull requests.

#### Scenario: Format violation fails

- **WHEN** a change deviates from the configured formatting
- **THEN** CI fails with a format error before build/tests run

#### Scenario: Build warning fails

- **WHEN** a change introduces a compiler or analyzer warning
- **THEN** CI fails on the build step

#### Scenario: Test failure fails

- **WHEN** a backend or client test fails
- **THEN** CI reports the failing test and the pipeline is red

#### Scenario: Vulnerability found

- **WHEN** a dependency has a known High/Critical vulnerability without an accepted entry
- **THEN** the audit step fails the pipeline

### Requirement: Incremental coverage runs on PRs only

The system SHALL run the incremental new-code coverage gate on pull requests (where a base ref exists) and skip it on direct pushes to `main`.

#### Scenario: PR below threshold

- **WHEN** a PR adds new executable lines covered below 80%
- **THEN** CI fails on the coverage step

#### Scenario: Push without PR

- **WHEN** CI runs for a push to `main`
- **THEN** the incremental coverage step is skipped

### Requirement: Results gate merges and failures are diagnosable

The system SHALL surface the `build` job as a required check on pull requests and SHALL upload coverage reports and failure logs as artifacts.

#### Scenario: Required check

- **WHEN** a pull request is considered for merge
- **THEN** the required `build` check must pass first

#### Scenario: Artifacts uploaded

- **WHEN** a CI run completes (success or failure)
- **THEN** coverage reports (and build/test logs on failure) are uploaded as artifacts

