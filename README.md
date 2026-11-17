# Experimental: AI harness sandbox project to learn how to create an AI harness

**Question:**
Can we create an AI harness together to test it locally on my Windows machine? I want to do this with you to learn how to build an AI harness so a team of developers can follow a similar path to build features for an Umbraco website (and later potentially a Kentico or Drupal website).

**Claude answer:**
Yes, let’s build it together. The best way to learn this is to build each piece by hand on a throwaway Umbraco site, see what it does, and only then package it up for a team. Here’s the path we’ll follow, one stage at a time:

1. Set up Claude Code and a sandbox Umbraco site on Windows.
2. Add the basics: a CLAUDE.md file (house rules) and settings.json (permissions).
3. Write an Umbraco skill that holds your expertise.
4. Add a hook that runs the build whenever Claude finishes, so broken code gets fixed automatically.
5. Add a slash command and a reviewer subagent.
6. Package it into its own GitHub repo with an install.ps1, so any developer can apply it to any Umbraco project. This is the step that turns your experiment into a team harness.
7. Add PR checks in GitHub Actions.
8. Later: a shared .NET skill and Kentico/Drupal skills.

At the start, everything lives inside the sandbox project’s .claude folder, so you can see cause and effect immediately. We extract it into a harness repo in stage 6, once you know what each part is for.


# AI Harness: Key Concepts and Glossary

Notes from building an AI coding harness for Umbraco (and later Kentico and Drupal) with Claude Code.

## Core concepts

**AI harness**
The software and configuration wrapped around an AI model so it can do real development work: read and edit files, run commands, check its own work and follow team conventions. The model provides the reasoning; the harness provides the tools, context, rules and guardrails.

**Claude Code**
Anthropic's coding agent, used here as the engine of the harness. It provides the agent loop, file and shell tools, context handling and permissions, so our harness is mostly configuration rather than code.

**Agent loop**
The cycle the agent repeats until a task is done: plan, act (edit files, run commands), check the result, and fix problems.

**Context / context window**
Everything the model can see while working: instructions, files it has read, and the conversation so far. The window is limited, so a good harness loads only what the current task needs.

**Headless mode**
Running Claude Code without an interactive session, for example in GitHub Actions for PR checks or issue-driven changes.

**Verification loop**
Automated checks (build, tests, linting) whose failures are fed back to the agent so it fixes its own mistakes before handing work back.

**Definition of done**
The explicit checks that must pass before work is considered finished, for example "`dotnet build` passes with no new warnings".

## Harness building blocks

**CLAUDE.md**
Instructions Claude reads at the start of every session. Keep it for facts about *this specific project*: what the site is, its commands, its quirks.

**settings.json** (`.claude/settings.json`)
Project configuration committed to the repo. Holds the permission rules and hook registrations.

**Permissions (allow / deny)**
Rules in `settings.json` controlling what Claude can do without asking. Example: allow `Bash(dotnet build:*)`, deny `Bash(git push:*)`.

**Skill** (`.claude/skills/<name>/SKILL.md`)
A package of know-how that Claude loads only when a task matches the skill's `description`. Use skills for knowledge that applies to *every* project of a type, such as our Umbraco conventions. The file must be named exactly `SKILL.md`.

**Frontmatter**
The block between `---` lines at the top of a skill or agent file, holding settings such as `name`, `description` and `tools`.

**Slash command**
A workflow you start by typing `/name`, such as `/umbraco-feature`. Commands are now a kind of skill. `disable-model-invocation: true` means only a person can trigger it, and `$ARGUMENTS` is replaced with whatever is typed after the command.

**Subagent** (`.claude/agents/<name>.md`)
A separate agent with its own context, instructions and tool limits. Our `umbraco-reviewer` is read-only and reviews changes against the Umbraco skill.

**Hook**
A script Claude Code runs automatically at a point in its lifecycle. Unlike instructions, hooks always run.
- `Stop`: when Claude finishes responding. We use it for the build check.
- `PreToolUse`: before a tool runs. Can block it, for example to protect files.
- `PostToolUse`: after a tool runs, for example to format edited files.

**Hook exit codes**
- `0`: no objection; carry on.
- `2`: block, and send the stderr message back to Claude so it can fix the problem.
- Any other code (we use `1`): non-blocking error shown to the developer.

**`stop_hook_active`**
A flag in the Stop hook's input that is `true` when Claude is already continuing because of a Stop hook. Our build check uses it to give Claude one fix attempt, then report to the developer instead of looping. Claude Code also overrides a Stop hook after eight consecutive blocks.

**`CLAUDE_PROJECT_DIR`**
Environment variable pointing at the project root, used by hook scripts to find files.

**Plugin**
A bundle of skills, agents, hooks and commands that can be installed and shared, such as Umbraco's official backoffice skills.

**MCP (Model Context Protocol)**
A standard way to connect Claude to external tools and services, such as the Umbraco MCP server.

**Project vs user-level configuration**
`.claude` in the repo applies to that project. `~/.claude` in your user folder applies to every project on your machine, so a company harness installed there also affects sandbox experiments.

## Architecture terms

**Platform pack**
Everything CMS-specific (instructions, conventions, commands, templates), kept separate from the generic harness. In Claude Code, packs are implemented as skills. Adding a new CMS means adding a new pack, not changing the core.

**Shared .NET base pack / skill**
Common .NET knowledge (build, test, NuGet, formatting) shared by the Umbraco and Kentico packs.

**Project environment / sandbox**
Where the agent builds and tests code. Locally this is the developer's machine; in CI it can be a container.

**PR checks**
Automated checks run on pull requests via GitHub Actions (`action.yml`), so agent and human changes are verified before merging. The agent works on a branch and never pushes to `main`.

**Evals**
A fixed set of tasks rerun whenever instructions or models change, to check whether a change actually improved results.

## Umbraco terms used in the harness

**uSync**
Exports Umbraco configuration from the database to files, so document types are visible to Claude and versioned in Git. Use the **Settings** export (document types, data types, templates); **Everything** also exports content and media, which adds noise.

**Models Builder**
Generates strongly typed C# models for document types. Claude should use these models, not `Value("alias")` strings.

**Unattended install**
Installs Umbraco from configuration without the setup wizard, so the site can boot headlessly for builds and tests.

**Document types live in the database**
Claude cannot create them by writing files. For now it describes the exact changes, the developer makes them in the backoffice, and uSync exports them.

## Principles we learned

- **Advice vs enforcement.** CLAUDE.md and skills are advice that Claude usually follows. Hooks and permissions are enforcement that always applies. Anything where a mistake is costly belongs in a hook or permission rule.
- **Skills are knowledge, subagents are roles.** Use a skill when Claude needs to *know* something. Use a subagent when you need a separate perspective with its own job and limits, such as a reviewer.
- **CLAUDE.md for this project, skills for every project.** Keeping rules in one place stops them drifting apart.
- **Encode repeated questions.** If Claude asks something that would have the same answer in every project, the answer belongs in CLAUDE.md or a skill.
- **Test hooks by hand before trusting them.** Pipe sample JSON into the script and check the exit code.
- **The agent should stop, not improvise.** If a required piece (such as the reviewer) is missing, the workflow should report it rather than silently substitute something else.
- **Own the guardrails.** Developers change hooks and settings; the agent should not edit its own enforcement.

## Our harness layout

```
D:\repos\HarnessSandbox\            <- repo root: start `claude` here
├── .claude\
│   ├── settings.json               <- permissions and hooks
│   ├── hooks\build-check.ps1       <- Stop hook: build must pass
│   ├── agents\umbraco-reviewer.md  <- read-only reviewer subagent
│   └── skills\
│       ├── umbraco\SKILL.md        <- Umbraco conventions
│       └── umbraco-feature\SKILL.md <- /umbraco-feature workflow
├── CLAUDE.md                       <- project facts
├── HarnessSandbox.sln
└── HarnessSandbox\                 <- Umbraco project
```

## Useful Claude Code commands

| Command | What it does |
|---|---|
| `/clear` | Clears the conversation for a fresh start on the next task |
| `/exit` | Exits Claude Code (restart to pick up new skills, agents or settings) |
| `/hooks` | Lists the configured hooks |
| `/agents` | Lists the available subagents |
| `/plugin` | Manages installed plugins |
| `/config` | Opens settings, such as turning off session recaps |
