## ADDED Requirements

### Requirement: Tutor application and qualification
The system SHALL provide a Simplified Chinese tutor application requiring overseas eligibility, identity and qualification evidence, an actual original cooking video, amateur-or-professional classification, tax-responsibility acknowledgement, and a signed copyright authorization agreement.

#### Scenario: Complete tutor application
- **WHEN** an overseas applicant submits all required evidence and agreements
- **THEN** the application enters review without granting publishing or earnings privileges

#### Scenario: Mainland China phone supplied
- **WHEN** an applicant supplies a +86 telephone number
- **THEN** the application is rejected under the offshore-only policy

### Requirement: Tutor approval and profile
Authorized staff SHALL approve, reject, suspend, or request changes with reason codes; approved tutors SHALL maintain a Chinese-language profile, English consumer-facing biography, portfolio, classification, and course catalog.

#### Scenario: Approved profile publication
- **WHEN** an approved tutor submits a complete profile and translations pass review
- **THEN** the public tutor profile becomes discoverable

### Requirement: Launch incentive eligibility
Administrators SHALL pre-recruit tutors and record a contractual three-month launch incentive that sets the platform commission to 10% for eligible orders, with explicit effective dates and precedence.

#### Scenario: Incentive expires
- **WHEN** an eligible tutor's incentive end time passes
- **THEN** new orders use the otherwise applicable commission rule without changing prior order calculations
