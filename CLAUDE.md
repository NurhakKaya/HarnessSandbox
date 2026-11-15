# HarnessSandbox

Umbraco site on .NET, SQLite for local development.

## Commands
- Build: `dotnet build`
- Run: `dotnet run --project HarnessSandbox`

## Conventions
- Use view components for reusable rendering, not partials with logic.
- No business logic in Razor views; put it in services registered via composers.
- Use the strongly typed Models Builder models, never `Value("alias")` strings.

## Definition of done
- `dotnet build` passes with no new warnings.
- Explain any new document types and how to create them.

## Content model
- Site-wide settings (site name, logo, footer links) live on the root node,
  via a "Site settings" composition. Fall back to the root node name.
- Read site settings from the root of the current content tree, so multi-site works.
- Document types are created in the backoffice for now: describe the exact
  document type, properties and compositions needed, rather than writing code for them.