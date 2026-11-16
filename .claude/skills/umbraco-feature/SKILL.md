---
name: umbraco-feature
description: Build an Umbraco feature using the team workflow.
disable-model-invocation: true
---

Build this Umbraco feature: $ARGUMENTS

Follow these steps in order:

1. Read the uSync folder to understand the current document types.
2. Present a short plan: files to create or change, and any document type
   or property changes the developer must make in the backoffice.
   Wait for the developer to approve the plan before writing code.
3. Implement the plan, following the umbraco skill.
4. Use the umbraco-reviewer subagent to review the changes.
   Fix anything it marks as "must fix".
5. Finish with a summary: what changed, backoffice steps for the developer,
   and any "consider" items from the review you did not apply.