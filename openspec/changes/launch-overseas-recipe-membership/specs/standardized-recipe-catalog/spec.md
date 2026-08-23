## ADDED Requirements

### Requirement: Standardized recipe content
Every publishable recipe SHALL have English title and instructions, cuisine category, difficulty, exact Imperial and Metric quantities, Western-supermarket substitutions, Asian-grocery-only labels, Western-kitchenware instructions, dish origin or holiday context, and original visual assets. Vague quantities including "a little", "appropriate amount", "a dash", and "a pinch" MUST fail validation.

#### Scenario: Complete recipe passes validation
- **WHEN** a reviewer validates a recipe containing every mandatory field with exact convertible measurements
- **THEN** the system permits approval and retains the validation evidence

#### Scenario: Vague measurement blocks publication
- **WHEN** any ingredient or instruction contains a prohibited vague quantity
- **THEN** the system blocks approval and identifies the offending content

### Requirement: Catalog discovery
The client SHALL browse published recipes by Sichuan, Cantonese, flour-based, vegetarian, and quick home-style categories and SHALL search or filter by ingredient, difficulty, and cuisine type.

#### Scenario: Combined filter
- **WHEN** a user selects an ingredient, difficulty, and cuisine type
- **THEN** the system returns only published recipes matching all selected criteria

### Requirement: Favorites and comments
Authenticated consumers SHALL add and remove favorites and SHALL submit comments subject to moderation; blocked comments MUST not be publicly visible.

#### Scenario: Favorite persists
- **WHEN** an authenticated user favorites a recipe
- **THEN** the recipe appears in that user's favorites across authenticated devices

#### Scenario: Moderated comment hidden
- **WHEN** an administrator blocks a comment
- **THEN** the comment is removed from public views while retained for audit

### Requirement: Entitlement-aware recipe access
The system SHALL expose preview metadata publicly and full recipe content only when the recipe is free, the consumer has an active membership, or the consumer owns that recipe.

#### Scenario: Locked premium recipe
- **WHEN** a consumer without a qualifying entitlement requests full premium content
- **THEN** the system returns preview data and purchase options without protected instructions or assets
