---
name: umbraco
description: House conventions for building Umbraco sites. Use for any work on
  Umbraco document types, views, view components, controllers, composers or content.
---

# Umbraco conventions

## Before writing code
- Read the uSync folder to understand the existing document types and compositions.
- Use the strongly typed Models Builder models. Never use `Value("alias")` strings.

## Content modelling
- Site-wide settings live on the root node via a "Site settings" composition.
- Prefer compositions over inheritance for shared properties.
- Do not create document types in code. Describe the exact document type,
  properties, aliases and compositions needed, so the developer can add them
  in the backoffice and export them with uSync.

## Rendering
- Reusable rendering goes in view components, not partials with logic.
- No business logic in Razor views. Put it in services, registered in a composer.
- Read site settings from the root of the current content tree, so multi-site works.

## Never
- Never hardcode content IDs or keys.
- Never edit files in the umbraco/ or wwwroot/umbraco folders.

## Done means
- `dotnet build` passes with no new warnings.
- New document types or property changes are listed for the developer.