## ADDED Requirements

### Requirement: Unit tests cover core logic

The system SHALL ship xUnit test projects for the backend and `flutter test` suites for the clients, and both SHALL run in CI.

#### Scenario: Tests run in CI

- **WHEN** a push or PR triggers CI
- **THEN** backend and client test suites run, and any failure fails the pipeline

### Requirement: New code meets an incremental coverage threshold

The system SHALL enforce an incremental coverage gate so that executable lines added by a change are covered at or above 80%, while overall (legacy) coverage is reported but not gated.

#### Scenario: New code under threshold

- **WHEN** a change adds executable lines whose coverage is below 80%
- **THEN** the coverage gate fails

#### Scenario: New code meets threshold

- **WHEN** a change's new executable lines are covered at or above 80%
- **THEN** the gate passes

#### Scenario: Legacy lines do not gate

- **WHEN** a change touches existing code whose overall coverage remains below the threshold
- **THEN** the pipeline does not fail on those legacy lines

### Requirement: Exclusions are defined

The system SHALL exclude test projects, EF migrations, generated code (`*.g.dart`, `*.freezed.dart`, designer files), and build artifacts from the incremental gate.

#### Scenario: Excluded paths ignored

- **WHEN** a diff only adds lines under `tests/`, `Migrations/`, or generated files
- **THEN** the gate reports no new executable lines and passes

### Requirement: Coverage output is produced in a machine-readable format

The system SHALL produce OpenCover XML for .NET runs (via Coverlet collector settings) and `coverage/lcov.info` for Flutter runs, consumed by the gate script.

#### Scenario: Coverage reports produced

- **WHEN** test runs execute with the collector configuration
- **THEN** OpenCover XML and lcov reports are written to the results directory
