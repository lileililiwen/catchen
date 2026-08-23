## Context

This is a greenfield offshore product for English-speaking consumers. Production hosting, legal ownership, identity, payments, storage, and analytics remain outside Mainland China. The system must support strict editorial validation, paid entitlements, PDFs, affiliate attribution, moderation, and auditability at low cold-start cost.

## Goals / Non-Goals

**Goals:**

- Deliver a C# backend and Flutter consumer/admin clients with explicit module boundaries.
- Make region policy, content validity, payment entitlement, and audit rules enforceable server-side.
- Keep Phase 1 deployable as a modular monolith with managed offshore dependencies.

**Non-Goals:**

- Tutor accounts, courses, coaching, payouts, live media, or communities.
- Mainland China availability, domestic payments, domestic promotion, or APK distribution.
- Automated copying, translation, or ingestion of third-party influencer content.

## Decisions

### ASP.NET Core modular monolith

Use ASP.NET Core on a supported .NET LTS release, PostgreSQL, background workers, and object storage. Modules cover Identity/Region Policy, Catalog, Editorial, Commerce, Documents, Affiliates, Moderation, and Reporting. This is cheaper and operationally simpler than microservices while preserving boundaries for later extraction.

Alternative: independent services from day one. Rejected because Phase 1 load and team size do not justify distributed transactions and operational overhead.

### Flutter clients with server-authoritative policy

Use Flutter for iOS, Android, and optionally web administration. All access, validation, entitlement, and moderation decisions are enforced by ASP.NET Core; clients only present results. An ASP.NET Core web admin remains acceptable if Flutter Web accessibility or operations density proves inadequate.

Alternative: separate native clients. Rejected due to duplicated delivery cost.

### Hosted identity and normalized region signals

Support verified email and E.164 overseas telephone identity. Reject +86 before OTP dispatch and at profile mutation. Combine declared country, phone country code, payment country, IP geolocation, and risk signals into a reason-coded policy decision; do not claim perfect geographic exclusion.

Alternative: IP blocking alone. Rejected because VPNs and mobile routing make it insufficient.

### Provider-neutral commerce ledger

Represent products, prices, orders, payments, refunds, disputes, and entitlements internally. Integrate Stripe first, add PayPal through the same boundary, and expose Apple Pay/Google Pay through supported provider checkout. Signed webhook inbox records provide idempotency and replay protection.

Alternative: store provider subscription state only. Rejected because it weakens reconciliation and later marketplace settlement.

### Versioned structured recipes and deterministic validation

Store recipe drafts and immutable published versions with structured ingredient quantities, units, substitutions, equipment adaptations, culture text, and asset provenance. A validation engine plus human dual review blocks invalid publication. PDFs render from immutable versions.

Alternative: rich-text-only recipes. Rejected because units, search, shopping aggregation, and standards cannot be reliably enforced.

### Privacy-minimized analytics and affiliate import

Use first-party event identifiers and consent-aware analytics. Affiliate clicks and provider commission statements remain separate datasets reconciled by supported attribution keys.

## Risks / Trade-offs

- [Regional controls can be bypassed] → Layer signals, log reasons, review abuse, and describe the restriction accurately rather than guaranteeing geolocation.
- [App-store steering rules vary] → Make checkout presentation remotely configurable and require legal/store review per storefront and release.
- [Mass/volume conversions can be unsafe] → Convert only with ingredient-specific density data; otherwise retain separate units.
- [Copyright attestations can be false] → Retain originals, metadata, contracts, reviewer evidence, and takedown workflow.
- [Flutter Web may be weak for dense admin work] → Keep APIs client-neutral and permit an ASP.NET Core admin UI.

## Migration Plan

1. Provision offshore environments, secrets, database, object storage, identity, and audit logging.
2. Deploy internal editorial and validation workflows; seed only reviewed original content.
3. Launch read-only catalog and regional controls, then enable accounts and favorites.
4. Enable sandbox payments and reconciliation, then production membership/PDF commerce.
5. Enable affiliate redirects and reporting after disclosure and privacy review.
6. Roll back individual modules using feature flags; preserve order, entitlement, and audit data during rollback.

## Open Questions

- Which offshore legal jurisdiction, supported launch countries, tax registrations, and privacy regimes apply?
- Which app stores and countries permit the intended external-checkout presentation at release time?
- Is Flutter Web or ASP.NET Core UI preferred for the operations console?
- Which affiliate networks and payout currencies are approved for Phase 1?
