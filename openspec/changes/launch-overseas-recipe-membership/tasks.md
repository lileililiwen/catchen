## 1. Foundation and Compliance

- [x] 1.1 Create the ASP.NET Core modular solution, PostgreSQL schema, background workers, object storage, and audit infrastructure.
  - Audit infrastructure: append-only AuditEvents + IAuditWriter (privacy-minimized: IP digests, truncated UAs) with a daily retention worker.
  - Object storage: intentionally deferred to the Documents capability (task 3.3) — the storage port is defined by its first consumer to avoid speculative cross-module infrastructure; dev filesystem provider lands there.
- [ ] 1.2 Create Flutter consumer and administrative shells with generated API contracts and role-aware navigation.
  - Delivered so far: `clients/consumer` + `clients/ops` Flutter 3.47 stable shells (Android/iOS/web) with boot smoke tests; both wired into hooks/CI gates.
  - Remaining: generated API contracts (OpenAPI → dart codegen) and role-aware navigation.
- [x] 1.3 Implement email/non-+86 identity, agreement evidence, regional policy evaluation, and restricted-operation tests.
  - PBKDF2 passwords, JWT auth (`/api/auth/register|login`), E.164 normalization with +86 prefix rejection before any OTP/account, reason-coded region decisions, agreement acceptance evidence (version/timestamp/IP digest), admin seeding with production fail-fast; 27 unit/integration tests.
- [x] 1.4 Add configurable payment, distribution, and promotion-channel allowlists and denylists.
  - ChannelPolicyOptions defaults (stripe/paypal/apple_pay/google_pay only; xiaohongshu/douyin/wechat_domestic_groups/domestic_apk_stores prohibited), admin approval endpoints with role gating, policy-violation auditing.

## 2. Recipe Catalog and Operations

- [ ] 2.1 Implement structured draft and immutable published recipe models with taxonomy and versioning.
- [ ] 2.2 Implement deterministic content validation for quantities, units, substitutions, equipment, cultural context, and provenance.
- [ ] 2.3 Implement author/reviewer workflow, secondary usability review, publish/unpublish, and evidence audit trails.
- [ ] 2.4 Implement catalog browse/search/filter, entitlement-aware detail, favorites, comments, and moderation.

## 3. Commerce and Documents

- [ ] 3.1 Implement provider-neutral products, orders, payments, refunds, disputes, entitlements, and signed webhook inboxes.
- [ ] 3.2 Integrate monthly membership and one-time recipe purchases with Stripe and PayPal-compatible checkout paths.
- [ ] 3.3 Implement versioned recipe PDFs and aggregated shopping-list PDFs with accessibility and entitlement tests.
- [ ] 3.4 Implement order reconciliation, refunds, and administrative reporting.

## 4. Affiliates and Verification

- [ ] 4.1 Implement allowlisted disclosed affiliate redirects, privacy-minimized click attribution, and commission statement import.
- [ ] 4.2 Add operational dashboards and exports for content, moderation, orders, and affiliate results.
- [ ] 4.3 Add automated unit, integration, contract, security, accessibility, and end-to-end tests for every specification scenario.
- [ ] 4.4 Complete offshore legal, privacy, copyright, tax, app-store, and payment-provider launch reviews.
