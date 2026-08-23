## ADDED Requirements

### Requirement: Immutable commission calculation
For each eligible paid course or coaching order, the system SHALL snapshot the applicable rule: 25% platform and 75% tutor by default, 15% platform for verified tutor-originated traffic, or 10% platform during a valid launch incentive. Free recipes SHALL never create tutor revenue share.

#### Scenario: Tutor-attributed order
- **WHEN** a paid order has valid, unexpired, fraud-screened tutor attribution and no stronger incentive
- **THEN** the ledger records a 15% platform share and 85% tutor share using the net basis defined by contract

#### Scenario: Launch incentive order
- **WHEN** an eligible paid order occurs within the tutor's three-month incentive window
- **THEN** the ledger records a 10% platform share and 90% tutor share and preserves the rule version

### Requirement: Attribution integrity
Tutor-originated attribution SHALL use signed campaign identifiers, an explicit attribution window, last-qualified-source rules, and fraud controls; tutors MUST NOT self-refer or overwrite attribution after checkout begins.

#### Scenario: Invalid referral token
- **WHEN** an order contains an invalid, expired, or self-referral token
- **THEN** the order uses the default commission rule and records the rejection reason

### Requirement: Monthly statements and batch payout
The system SHALL generate monthly tutor statements from immutable ledger entries, with sales, fees, commissions, refunds, disputes, reserves, FX rate/source/time, prior adjustments, and payable balance; approved balances SHALL be paid in batches through a configured cross-border provider rather than instant withdrawal.

#### Scenario: Monthly payout approved
- **WHEN** finance approves a reconciled statement above the payout threshold after its hold period
- **THEN** the amount enters one idempotent payout batch and its provider outcome updates the ledger

### Requirement: Tutor tax responsibility records
The tutor agreement and each statement SHALL state that the tutor is responsible for local personal income tax and that the platform does not withhold or pay it, except where applicable law requires otherwise.

#### Scenario: Required withholding jurisdiction
- **WHEN** legal configuration says withholding is mandatory for a tutor jurisdiction
- **THEN** payout is held until compliant handling is configured rather than applying the no-withholding default

### Requirement: Financial administration and disputes
Authorized finance staff SHALL review statements, hold payouts, record reasoned adjustments, manage failed payouts, and resolve disputes under dual control with a complete audit trail.

#### Scenario: Refund after statement close
- **WHEN** a refund is confirmed after its original statement closed
- **THEN** the system posts a linked adjustment to the next open statement without mutating the closed statement
