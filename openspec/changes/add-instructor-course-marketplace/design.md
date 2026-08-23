## Context

This Phase 2 change activates only after verified consumer count exceeds 10,000 and Phase 1 is operational. It adds a regulated two-sided marketplace to the existing ASP.NET Core modular backend and Flutter clients.

## Goals / Non-Goals

**Goals:** Chinese tutor operations, original paid video courses, coaching bookings, reviews/Q&A, deterministic commissions, and monthly cross-border payouts.

**Non-Goals:** live streaming, forums, competitions, offline salons, instant withdrawals, or revenue sharing on free recipes.

## Decisions

- Extend the C# modular monolith with Tutor, Learning, Scheduling, Marketplace, Ledger, and Settlement modules; do not introduce microservices until measured scale requires them.
- Provide Flutter consumer and Simplified Chinese tutor experiences backed by the same server-authoritative RBAC and policy APIs.
- Store original uploads in private offshore object storage, transcode asynchronously, and serve protected segments through short-lived signed access.
- Use database-enforced slot holds and idempotent payment workflows to prevent double booking.
- Maintain append-only double-entry-style subledgers. Snapshot contract, attribution, commission basis, rate, currency, and rule version per order; never recalculate closed history from current rules.
- Prefer Stripe-hosted web checkout and a vetted Connect/cross-border payout provider, but keep provider adapters because supported countries, FX, tax, and split-payment capabilities vary.
- Treat the 10,000-user threshold as a release gate based on verified eligible accounts, not a hard-coded feature behavior.

## Risks / Trade-offs

- [Cross-border payouts and tax representations vary] → Obtain jurisdiction-specific counsel, configure country eligibility, and hold unsupported payouts.
- [External checkout may conflict with store rules] → Remotely configure surfaces and complete store-specific review.
- [Video storage and delivery costs grow] → Apply encoding ladders, quotas, lifecycle rules, and measured CDN budgets.
- [Attribution is contested or gamed] → Sign referrals, define one window and precedence, expose evidence, and screen self-referrals.
- [Tutors submit copied work] → Require original files/contracts, human review, fingerprints where lawful, and takedown controls.

## Migration Plan

1. Approve legal/payment countries, contracts, commission basis, tax handling, and launch gate.
2. Deploy modules and tutor portal behind feature flags; onboard the pre-recruited cohort.
3. Validate media, moderation, booking concurrency, ledger, statement, and payout flows in sandbox.
4. Publish approved profiles/free content, then enable courses, coaching, and monthly payouts progressively.
5. Disable new marketplace purchases on rollback while preserving learner access, bookings, ledger, and obligations.

## Open Questions

- Which countries, currencies, payout provider, KYC/KYB process, reserve, refund allocation, and minimum payout apply?
- Does the commission basis exclude processor fees, taxes, discounts, refunds, and chargebacks, and in what order?
- What attribution window and source precedence qualify tutor-originated traffic?
- Which video provider and coaching meeting provider satisfy residency and privacy requirements?
