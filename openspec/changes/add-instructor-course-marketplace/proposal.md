## Why

After the consumer service exceeds 10,000 users, verified overseas Chinese tutors can expand the catalog into paid video learning and private coaching. The platform needs controlled onboarding, quality review, deterministic commissions, and low-frequency cross-border settlement before opening this two-sided marketplace.

## What Changes

- Add a Simplified Chinese tutor portal with identity/qualification review, profiles, portfolios, recipe and course publishing, earnings, and coaching schedules.
- Add tutor discovery, paid segmented-video courses, reviews, Q&A, and one-to-one coaching booking to the client.
- Add auditable 25%/75% base commissions and 15% platform commissions for qualified tutor-attributed orders.
- Add a configurable three-month launch incentive with a 10% platform commission for eligible recruited tutors.
- Add monthly statement generation, payout review, foreign-exchange evidence, and batch cross-border payouts.
- Extend content review and copyright controls to tutor-created recipes, videos, and courses.

## Capabilities

### New Capabilities

- `tutor-onboarding`: Chinese-language applications, qualification evidence, agreements, roles, and profile publication.
- `tutor-content`: Original recipe/course authoring, segmented video delivery, moderation, and learner access.
- `coaching-bookings`: Tutor availability, paid one-to-one booking, lifecycle management, and conflict prevention.
- `marketplace-engagement`: Tutor discovery, verified-purchase reviews, and course Q&A.
- `commissions-and-payouts`: Attribution, commission calculation, ledgers, statements, FX, disputes, and monthly batch payouts.

### Modified Capabilities

None. This change depends on, but does not redefine, the Phase 1 capabilities; its new capabilities specify their integration constraints explicitly.

## Impact

Extends the ASP.NET Core backend and Flutter applications with tutor RBAC, media storage/transcoding, scheduling, marketplace orders, immutable financial ledgers, monthly settlement jobs, and cross-border payout-provider integration. This change depends on the Phase 1 capabilities.
