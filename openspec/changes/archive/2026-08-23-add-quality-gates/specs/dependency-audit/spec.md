## ADDED Requirements

### Requirement: NuGet dependencies are scanned for vulnerabilities

The system SHALL run `dotnet list package --vulnerable --include-transitive` at restore time and in CI, failing on High or Critical severity findings.

#### Scenario: Vulnerable direct package

- **WHEN** a direct or transitive NuGet package has a known High/Critical vulnerability
- **THEN** the restore-time check warns and the CI audit step fails the pipeline

#### Scenario: Clean dependency graph

- **WHEN** no High/Critical findings exist
- **THEN** the audit step passes and records the scan result in the job log

### Requirement: pub dependencies are reviewed

The system SHALL run a recorded pub package review (`flutter pub outdated` plus a vulnerability review) in CI, failing on packages with known High/Critical advisories.

#### Scenario: Outdated or vulnerable pub package

- **WHEN** the pub review identifies a package with a known High/Critical advisory
- **THEN** the audit step fails the pipeline

#### Scenario: Review report recorded

- **WHEN** the pub review completes
- **THEN** its output is preserved in the CI job log for traceability

### Requirement: Risk acceptance is explicit and dated

The system SHALL allow a High/Critical finding to be accepted only via a dated, owner-signed entry in `SECURITY-ACCEPTANCES.md`, and the audit step SHALL pass only when the finding is covered by such an entry.

#### Scenario: Accepted finding

- **WHEN** a vulnerable package is covered by a current, signed acceptance entry
- **THEN** the audit step passes while logging the acceptance reference

#### Scenario: Expired or missing acceptance

- **WHEN** an acceptance entry is absent, unsigned, or past its expiry date
- **THEN** the audit step fails
