## ADDED Requirements

### Requirement: Recipe editorial workflow
Authorized staff SHALL create, revise, submit, review, approve, publish, unpublish, and version recipes with separation between authoring and final approval.

#### Scenario: Reviewer publishes approved recipe
- **WHEN** an authorized reviewer approves a valid submitted recipe
- **THEN** the system publishes an immutable version and records author, reviewer, evidence, and timestamps

### Requirement: Copyright evidence
The system MUST require provenance and originality attestations for text and original photo/video assets, SHALL retain authorization evidence, and MUST prohibit approval of copied domestic-influencer content.

#### Scenario: Missing original asset evidence
- **WHEN** a submitted recipe lacks required provenance or original-photography evidence
- **THEN** the system blocks approval and requests remediation

### Requirement: Substitution and usability review
Operations staff SHALL perform and record a secondary review of substitutions, quantities, steps, and Western-kitchenware feasibility before publication.

#### Scenario: Unworkable substitution
- **WHEN** secondary review finds that a substitute changes the recipe without adequate instruction
- **THEN** publication remains blocked until corrected and re-reviewed

### Requirement: Moderation and blocking
Authorized moderators SHALL review, hide, restore, and permanently block inappropriate comments or repeat offenders with reason codes and audit history.

#### Scenario: User blocked for abuse
- **WHEN** a moderator blocks a user under the moderation policy
- **THEN** the user cannot submit new comments and existing treatment follows the recorded moderation decision

### Requirement: Operational analytics
Administrators SHALL view membership order statistics, individual PDF sales, affiliate clicks, imported commissions, moderation workload, and recipe publication status with exportable period and currency breakdowns.

#### Scenario: Affiliate commission import
- **WHEN** an operator imports a provider commission statement
- **THEN** the system validates, deduplicates, reconciles, and reports accepted and rejected rows
