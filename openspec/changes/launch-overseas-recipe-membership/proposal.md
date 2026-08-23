## Why

Non-Chinese users outside Mainland China need an English-first way to cook authentic Chinese food with measurable quantities, locally obtainable substitutes, and Western kitchen equipment. The cold-start product must validate paid demand without incurring the cost or operational complexity of a tutor marketplace.

## What Changes

- Add an English consumer experience for browsing, searching, favoriting, and purchasing standardized Chinese recipes.
- Add monthly membership access, one-time recipe PDF sales, shopping-list PDF export, and attributable outbound affiliate links.
- Add an operations console for recipe workflow, order and affiliate reporting, and comment moderation.
- Enforce offshore-only registration and service eligibility, including rejection of Mainland China phone numbers and exclusion of domestic payment and promotion channels.
- Establish mandatory originality, measurement, substitution, equipment, and cultural-context validation for every published recipe.
- Explicitly exclude tutor onboarding, courses, revenue sharing, live streaming, and community functionality from Phase 1.

## Capabilities

### New Capabilities

- `offshore-user-access`: Overseas-only registration, service eligibility, consent, and regional restrictions.
- `standardized-recipe-catalog`: Recipe taxonomy, discovery, strict content schema, favorites, comments, and access control.
- `recipe-commerce`: Memberships, individual recipe PDF purchases, payment handling, entitlements, and refunds.
- `shopping-and-affiliates`: Shopping-list PDF export and attributable affiliate-link redirection/reporting.
- `recipe-operations`: Administrative recipe workflow, content-quality evidence, moderation, and commercial reporting.

### Modified Capabilities

None.

## Impact

Introduces an ASP.NET Core (C#) backend and operations APIs, Flutter consumer and administrative clients, offshore hosting and storage, email/overseas-phone identity, PDF generation, Stripe/PayPal and wallet-compatible checkout, affiliate attribution, and auditable moderation. No tutor-facing surface is introduced.
