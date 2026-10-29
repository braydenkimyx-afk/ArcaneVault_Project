# ArcaneVault

## Authentication & API Identity Scoping

This project uses a prototype-level approach for identity and authentication between the Web Client (`ArcaneVault.Web`) and the API backend (`ArcaneVaultAPI`).

### Prototype Approach
Currently, the Web Client securely transmits the identity of the authenticated user to the API on each request using custom headers:
- `X-User-Name`
- `X-User-Role`

To prevent malicious forgery internally (i.e. to prevent someone from sending an HTTP request directly to the API with `X-User-Role: Staff`), an `X-Api-Key` shared secret header is also required. The API rejects requests (401 Unauthorized) if this key does not strictly match the shared secret stored in `appsettings.json`.

### Password Reset Simplification
Because the prototype application lacks an integrated mail server, password reset currently returns the raw reset token in the HTTP response body for immediate use. In production environments, this token would never be exposed in the response; a dedicated background service would generate an email containing the token link and dispatch it to the registered user to ensure security.

### Production Equivalent
In a production web/microservices environment, this header-based approach would be replaced by **Signed JWTs (JSON Web Tokens)**. With JWTs:
- The token holds cryptographically signed claims regarding identity and roles.
- The API verifies the cryptographic signature instead of relying on a shared API Key secret and raw string headers.
- This entirely mitigates forgery, as unauthorized parties cannot sign tokens matching the trusted authorities without access to the signing certificates.

## Public vs. Private Architectural Split

ArcaneVault separates internal data management from global platform discovery by explicitly isolating API endpoints:

- **Private Management (`/api/collectionitems`)**: Endpoints are strictly scoped and ownership-bound. They filter reads against the `X-User-Name` header and enforce ownership checks upon mutate (returning 403 Forbidden if a user attempts to edit data they do not own).
- **Public Browse (`/api/collectionitems/browse`)**: An explicitly unscoped, read-only endpoint that completely bypasses user-header requirements. It utilizes specifically crafted Response DTOs to ensure sensitive properties (IsDeleted tracking, passwords, owner emails, or large image payloads) are statically stripped before sending the data back to an anonymous or authenticated Web platform visitor.


## Design Decisions

### Staff Oversight UI
The management page (`/CollectionItems`) explicitly presents all items to Staff members as an administrative oversight feature, labelled cleanly as "All Collections" rather than the potentially confusing "My Collection." We considered strictly isolating Staff exclusively to their own items. However, enabling platform-wide oversight on the management screen (combined with an explicit role-aware warning banner and owner-filtering) simplifies administrative workflows, avoids duplicating page templates, and clarifies that Staff act on behalf of the whole system without fundamentally altering the private scoped identity system.
