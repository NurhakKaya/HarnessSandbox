**Experimental: AI harness sandbox project to learn how to create an AI harness**

**Question:**
Can we create an AI harness together to test it locally on my Windows machine? I want to do this with you to learn how to build an AI harness so a team of developers can follow a similar path to build features for an Umbraco website (and later potentially a Kentico or Drupal website).

**Claude answer:**
Yes, let’s build it together. The best way to learn this is to build each piece by hand on a throwaway Umbraco site, see what it does, and only then package it up for a team. Here’s the path we’ll follow, one stage at a time:

Set up Claude Code and a sandbox Umbraco site on Windows.
Add the basics: a CLAUDE.md file (house rules) and settings.json (permissions).
Write an Umbraco skill that holds your expertise.
Add a hook that runs the build whenever Claude finishes, so broken code gets fixed automatically.
Add a slash command and a reviewer subagent.
Package it into its own GitHub repo with an install.ps1, so any developer can apply it to any Umbraco project. This is the step that turns your experiment into a team harness.
Add PR checks in GitHub Actions.
Later: a shared .NET skill and Kentico/Drupal skills.

At the start, everything lives inside the sandbox project’s .claude folder, so you can see cause and effect immediately. We extract it into a harness repo in stage 6, once you know what each part is for.
