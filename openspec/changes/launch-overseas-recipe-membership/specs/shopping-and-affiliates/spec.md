## ADDED Requirements

### Requirement: Shopping-list PDF export
An entitled user SHALL select recipes, aggregate exact ingredient quantities into a shopping list, preserve unit and substitution information, and export an accessible PDF.

#### Scenario: Combined shopping list
- **WHEN** a user exports two recipes containing compatible versions of the same ingredient
- **THEN** the PDF combines convertible quantities and keeps non-convertible variants separate with their units and substitutions

### Requirement: Recipe PDF integrity
Generated recipe and shopping-list PDFs SHALL include document version, generation timestamp, locale, allergen disclaimer, and only content the user is entitled to access.

#### Scenario: PDF generated after recipe revision
- **WHEN** an entitled user requests a PDF after a published recipe is revised
- **THEN** the PDF identifies the current published recipe version

### Requirement: Affiliate link attribution
The system SHALL label affiliate links clearly, route clicks only to allowlisted overseas merchants, attach supported campaign attribution, and aggregate click and imported commission records without claiming a sale from a click alone.

#### Scenario: Affiliate click
- **WHEN** a user follows an Amazon kitchenware or spice affiliate link
- **THEN** the system records privacy-compliant click attribution and redirects to the allowlisted destination

#### Scenario: Unapproved affiliate destination
- **WHEN** content references a non-allowlisted affiliate destination
- **THEN** publication or redirection is blocked
