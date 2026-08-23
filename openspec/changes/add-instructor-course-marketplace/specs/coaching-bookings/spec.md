## ADDED Requirements

### Requirement: Tutor availability and booking
Tutors SHALL manage timezone-aware availability and consumers SHALL purchase an available one-to-one online coaching slot without double booking.

#### Scenario: Concurrent booking attempts
- **WHEN** two consumers attempt to pay for the same slot
- **THEN** at most one confirmed booking is created and the other attempt receives no charge or an automatic reversal

### Requirement: Booking lifecycle
The system SHALL track pending-payment, confirmed, rescheduled, completed, cancelled, no-show, refunded, and disputed states under a published cancellation policy.

#### Scenario: Tutor-approved reschedule
- **WHEN** both parties accept a valid replacement slot
- **THEN** the booking moves atomically and retains the complete history

### Requirement: Session privacy
Meeting access SHALL be limited to the booked parties and authorized support staff, with expiring join credentials and no recording unless explicit consent is captured.

#### Scenario: Expired join link
- **WHEN** a join credential is used outside its allowed window
- **THEN** access is denied and the attempt is audited
