# mf-harness — Team Claude Config Repo

This repo distributes the Manifesto agency team's shared Claude Code configuration. It is not a product codebase.

## Purpose

Running `./update.sh` (or `./claude-config/install.sh` directly) copies the team's `CLAUDE.md`, subagents, skills, and settings into `~/.claude/` so they apply globally across all projects on the developer's machine.

## Contributing

- **Orchestration protocol / role narrative** → edit `claude-config/CLAUDE.md`
- **A specific subagent (prompt, model, description)** → edit the matching file in `claude-config/agents/`. These are real Claude Code agents: YAML frontmatter (`name`, `description`, `model`) plus a system-prompt body. They install to `~/.claude/agents/` (same-named files are overwritten; personal agents are left alone). Keep the `model` frontmatter aligned with the alias table in `claude-config/CLAUDE.md` (`opus` / `sonnet` / `haiku`)
- **Settings changes** → edit `claude-config/settings.json` (only keys in the allow-list in `install.sh` are merged; personal settings are never overwritten)
- **New skill installs** → vendor the skill under `claude-config/skills/` as plain files and add a `cp -R` block to `claude-config/install.sh` (and `install.ps1`). For a third-party open-source skill, copy the upstream files in (drop its `.git` dir) and record the upstream URL and commit in the Supply chain table in `README.md` — never a live `git clone` at install time, so upstream changes only land via a reviewed PR.
- **New plugin / marketplace installs** → plugins (superpowers, Umbraco, .NET) are *not* vendored skills and are not touched by `install.sh`, so the vendoring bullet above does not apply. Adding one is three edits in this repo: register the marketplace under `extraKnownMarketplaces` and set the plugin to `true` under `enabledPlugins` in `claude-config/settings.json`; add the `/plugin install` line to the "Install plugins" step in `README.md`; and add rows to the "What gets installed" and "Not covered" plugin tables in `README.md`. Running the `/plugin` commands stays a manual one-time step for the user by design (it keeps the installer offline and deterministic) — pre-configuring `settings.json` so they only need `/plugin install` is the part that belongs in a PR here.
- **This file** → describes this repo only; changes here do not affect the global team config

## Updating your local install

```bash
./update.sh
```

All changes go to `~/.claude/` — nothing in this repo's working directory is affected by the install.
