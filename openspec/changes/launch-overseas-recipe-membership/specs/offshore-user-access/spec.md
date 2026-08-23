## ADDED Requirements

### Requirement: Overseas-only account eligibility
The system SHALL allow consumer registration by verified email or non-Mainland-China telephone number, MUST reject numbers with country code +86, and SHALL record acceptance of an agreement stating that the service is unavailable in Mainland China.

#### Scenario: Email registration outside Mainland China
- **WHEN** an eligible user supplies and verifies an email and accepts the current agreement
- **THEN** the system creates a consumer account and records the agreement version, timestamp, and evidence

#### Scenario: Mainland China telephone rejected
- **WHEN** a registration or profile change supplies a normalized telephone number with country code +86
- **THEN** the system rejects the number without sending an OTP or creating an account

### Requirement: Regional service controls
The system SHALL enforce configurable Mainland China access restrictions at registration, authentication, checkout, content delivery, and distribution boundaries, MUST host production systems and legal entities offshore, and MUST NOT provide a domestic APK or domestic payment integration.

#### Scenario: Restricted-region request
- **WHEN** available region signals indicate a request originates in Mainland China
- **THEN** the system denies the restricted operation, displays the availability notice, and records a privacy-minimized audit event

#### Scenario: Approved payment methods
- **WHEN** an eligible user starts checkout
- **THEN** the system offers only configured offshore methods among Stripe, PayPal, Apple Pay, and Google Pay and never offers domestic WeChat Pay or Alipay

### Requirement: Promotion-channel policy
The operations system SHALL record approved overseas campaign channels and MUST prevent staff from marking Xiaohongshu, Douyin, or domestic WeChat groups as approved distribution channels.

#### Scenario: Prohibited campaign channel
- **WHEN** an operator attempts to approve a campaign targeting a prohibited domestic channel
- **THEN** the system rejects approval and records the policy violation
