## ADDED Requirements

### Requirement: Gated live and long-form learning
Approved tutors SHALL publish moderated long-form series and schedule paid or entitled live cooking sessions with capacity, timezone, equipment, ingredient, language, and safety information.

#### Scenario: Entitled learner joins live session
- **WHEN** an entitled overseas learner joins during the allowed window
- **THEN** the system issues expiring room access with the learner's authorized role

### Requirement: Interactive media safety
Live rooms SHALL support host controls, participant mute/remove, reporting, consent notices, and auditable incident escalation; recording MUST be disabled unless all required consent and retention policies are satisfied.

#### Scenario: Participant removed
- **WHEN** a host or moderator removes a disruptive participant
- **THEN** the participant loses room access and an incident record is created

### Requirement: Live earnings integration
Eligible live-course orders SHALL use a versioned contractual commission rule and flow through the existing monthly statement and batch payout process.

#### Scenario: Live order settled
- **WHEN** a live-course order becomes settlement-eligible
- **THEN** its immutable earnings entries appear in the tutor's next applicable monthly statement
