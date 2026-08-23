## Context

This Phase 3 change is gated until at least one year of stable offshore traffic and successful Phase 2 moderation, settlement, and support operations. It adds synchronous media, public user content, events, and commercial partnerships to the C# and Flutter platform.

## Goals / Non-Goals

**Goals:** safe live learning, long-form series, competitions, offline salon registration, moderated communities, disclosed sponsorships, and consented study-tour leads.

**Non-Goals:** Mainland China service or promotion, undisclosed native advertising, unmoderated public rooms, automatic reuse of community photos, or becoming tour operator/merchant of record without separate approval.

## Decisions

- Keep domain orchestration, permissions, orders, moderation, and records in ASP.NET Core; use a managed WebRTC/live-video provider behind an adapter rather than building media infrastructure.
- Use Flutter for learner/tutor live and community surfaces, with server-issued short-lived room tokens and server-authoritative roles.
- Create a unified trust-and-safety case system for live incidents, community reports, competition disputes, and appeals, with separate evidence retention and least-privilege access.
- Model online competitions and offline salons as distinct event types sharing registration, capacity, payment, consent, notification, and incident primitives.
- Model sponsorships as explicit contracts/campaigns/deliverables rather than ordinary affiliate links; require adjacent disclosure and separate compensation rules.
- Transfer study-tour lead data only after granular consent and a server-to-server auditable handoff to an approved provider.

## Risks / Trade-offs

- [Live abuse or safety incident] → Host controls, trained moderation, delayed participation options, escalation runbooks, and provider kill switches.
- [User-generated content creates privacy/copyright exposure] → Rights attestations, metadata stripping, visibility controls, reports, appeals, and takedown response.
- [Offline events add physical liability] → Jurisdiction-specific terms, capacity controls, venue and emergency processes, and insurance review.
- [Sponsorship harms trust] → Mandatory disclosure, editorial separation, campaign approval, and performance audit.
- [Tour referrals create regulated obligations] → Define platform role contractually and do not launch until legal and provider due diligence is complete.

## Migration Plan

1. Require executive/legal approval of the one-year stability gate and readiness metrics.
2. Contract and sandbox-test live media, event, notification, moderation, and partner providers.
3. Pilot invite-only long-form/live learning with trained moderators.
4. Pilot one online competition and one capacity-limited salon in approved jurisdictions.
5. Enable communities gradually, then disclosed sponsorships and consented tour leads.
6. Roll back per capability through feature flags while retaining purchased access, financial obligations, consent, and incident evidence.

## Open Questions

- What objective stability, moderation-response, refund, uptime, and settlement metrics unlock Phase 3?
- Which live provider, recording policy, age minimum, and emergency escalation regions apply?
- Is the platform organizer, marketplace, or referral agent for salons and tours in each jurisdiction?
- How are sponsorship and live-course tutor earnings calculated and taxed?
