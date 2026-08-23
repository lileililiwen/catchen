## ADDED Requirements

### Requirement: Repository-wide formatting and analysis configuration

The system SHALL provide a root `.editorconfig` covering C#, Dart/Flutter, YAML, JSON, and Markdown, a shared `Directory.Build.props` applying `TreatWarningsAsErrors`, nullable reference types, latest analyzers, and deterministic builds to every C# project, and a Flutter `analysis_options.yaml` with strict lint options.

#### Scenario: Analyzer warning fails the build

- **WHEN** any C# project produces a compiler or analyzer warning
- **THEN** the build fails with that warning as an error

#### Scenario: Dart lint violation surfaces

- **WHEN** `flutter analyze` runs against Dart code violating the configured lints
- **THEN** the violation is reported and fails the analysis step

### Requirement: Formatting is deterministically verifiable

The system SHALL verify formatting with `dotnet format --verify-no-changes` for C# and `dart format --set-exit-if-changed --output=none` for Dart, producing identical results on repeated runs.

#### Scenario: Format drift detected

- **WHEN** committed code deviates from the configured formatting
- **THEN** the verification command exits non-zero naming the offending files

#### Scenario: Clean tree passes

- **WHEN** all files match the configured formatting
- **THEN** both verification commands exit zero
