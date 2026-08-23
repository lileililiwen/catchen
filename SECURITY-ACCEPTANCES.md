# SECURITY-ACCEPTANCES.md

Accepted dependency-risk entries for the Catchen repository.

## Policy

- CI fails on any direct or transitive dependency with a known **High** or
  **Critical** advisory (NuGet via `NuGetAudit` + `dotnet list package
  --vulnerable --include-transitive`; pub via `dart pub audit`, falling back
  to a recorded `flutter pub outdated` report).
- A finding may pass **only** while it is covered by a row below that is:
  1. **dated** (added and expiry filled in),
  2. **owner-signed** (a named person, not a team alias),
  3. carrying a reviewed rationale and a remediation plan.
- Accepted advisories must ALSO be listed in the `NuGetAuditSuppress`
  property of `Directory.Build.props` (NuGet only) so restore-time audit
  stays consistent with this file.
- Expired entries are removed; the audit then fails until the package is
  upgraded or a new entry is signed.

## Accepted entries

| Date | Package | Advisory / ID | Severity | Expiry | Owner | Rationale & remediation plan |
|------|---------|---------------|----------|--------|-------|------------------------------|
| _(none)_ | | | | | | |
direct-push probe
