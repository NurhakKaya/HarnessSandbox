# HarnessSandbox

Sandbox Umbraco 19 site (release candidate, .NET 11) used to develop and test
our AI harness. SQLite for local development.

Follow the umbraco skill for all Umbraco work.

## Commands
- Build: `dotnet build`
- Run: `dotnet run --project HarnessSandbox`

## Project facts
- The content model is exported to the `uSync` folder (Settings only).
- The root node is "Home", which uses the "Site settings" composition.