## ADDED Requirements

### Requirement: Online cooking competitions
Administrators SHALL configure eligibility, rules, deadlines, judging criteria, prizes, rights terms, regions, and moderation; eligible users SHALL submit original entries and receive auditable results.

#### Scenario: Plagiarized entry
- **WHEN** review finds an entry lacks required originality evidence
- **THEN** the entry is disqualified with a reason and retained appeal evidence

### Requirement: Offline cooking salon registration
Eligible overseas users SHALL register for capacity-limited offline salons with venue, timezone, price, cancellation, health/safety, media-consent, and privacy terms.

#### Scenario: Salon capacity reached
- **WHEN** the final place is confirmed
- **THEN** additional users cannot be charged as confirmed attendees and may join a waitlist if enabled

### Requirement: Event administration
Authorized staff SHALL manage entries, judges, conflicts of interest, attendees, waitlists, cancellations, refunds, incidents, and exportable rosters under least-privilege access.

#### Scenario: Judge conflict disclosed
- **WHEN** a judge declares a conflict with an entry
- **THEN** the system prevents that judge from scoring it and records reassignment
