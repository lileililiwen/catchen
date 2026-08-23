## ADDED Requirements

### Requirement: Membership subscription
The system SHALL sell a monthly membership through configured offshore payment providers, update access only from verified provider events, and handle renewals, cancellation, failure, refund, and dispute states idempotently.

#### Scenario: Verified subscription payment
- **WHEN** a valid signed provider event confirms a successful monthly payment
- **THEN** the system records the order and grants or extends membership exactly once

#### Scenario: Forged webhook
- **WHEN** a payment callback fails signature or replay validation
- **THEN** the system makes no entitlement change and records a security event

### Requirement: Individual recipe purchase
The system SHALL support one-time purchase of a recipe PDF and grant durable access to the purchased PDF subject to refund and dispute policy.

#### Scenario: Recipe purchase completed
- **WHEN** a verified payment event completes an individual recipe order
- **THEN** the user can download the versioned purchased PDF and the order is visible in purchase history

### Requirement: Payment-channel presentation
The product SHALL make web checkout the primary purchase path while treating native in-app purchase as optional and platform-policy-dependent; it MUST NOT use interface text or behavior that violates an app store's current payment rules.

#### Scenario: Web checkout from an allowed surface
- **WHEN** an eligible user chooses to purchase on a surface where external checkout is permitted
- **THEN** the system opens a secure Stripe or PayPal web checkout and reconciles completion through verified events

### Requirement: Order reporting and refunds
Administrators SHALL view reconciled membership and recipe orders by period, provider, currency, product, status, refund, and dispute state.

#### Scenario: Refunded order report
- **WHEN** a provider confirms a refund
- **THEN** the report reflects the refund and the applicable entitlement follows configured refund policy
