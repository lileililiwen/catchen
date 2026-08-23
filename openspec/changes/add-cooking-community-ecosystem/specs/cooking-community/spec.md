## ADDED Requirements

### Requirement: Community participation
Eligible users SHALL create forum discussions, post original recipe photos, comment, react, and join tutor-fan communities subject to audience and membership controls.

#### Scenario: Private tutor community
- **WHEN** a non-member requests content from a restricted tutor community
- **THEN** the system reveals no protected posts or member data

### Requirement: Originality and privacy
Photo posters SHALL attest ownership, choose permitted visibility, and receive controls to edit or delete their content; the system SHALL strip unnecessary location metadata and MUST NOT repurpose content for advertising without separate consent.

#### Scenario: Photo upload
- **WHEN** a user uploads a recipe photo
- **THEN** the system strips configured EXIF location data before publication and records the rights attestation

### Requirement: Community moderation
Users SHALL report content or accounts, moderators SHALL apply reasoned actions and appeals, and urgent safety incidents SHALL follow an escalation policy.

#### Scenario: Report threshold reached
- **WHEN** configured high-confidence abuse signals or urgent reports are received
- **THEN** the content is temporarily limited and prioritized for human review without automatically destroying evidence
