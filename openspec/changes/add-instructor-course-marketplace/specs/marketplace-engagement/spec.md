## ADDED Requirements

### Requirement: Tutor and course discovery
Consumers SHALL browse and search approved tutor profiles, free recipes, paid courses, classification, languages, ratings, and coaching availability.

#### Scenario: Suspended tutor
- **WHEN** a tutor is suspended
- **THEN** new purchases and bookings are disabled while existing learner access follows support policy

### Requirement: Verified reviews
Only consumers with a completed qualifying purchase SHALL review that course or coaching service, and moderators SHALL hide policy-violating reviews without destroying audit history.

#### Scenario: Non-purchaser review
- **WHEN** a consumer without a qualifying order submits a review
- **THEN** the system rejects the review

### Requirement: Course questions and answers
Entitled learners SHALL ask course questions, tutors SHALL answer them, and both SHALL report inappropriate content for moderation.

#### Scenario: Reported Q&A item
- **WHEN** a participant reports a Q&A item
- **THEN** moderators can review it with course, author, reason, and conversation context
