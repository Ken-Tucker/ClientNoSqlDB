---
name: code-review
description: Review code changes for correctness, regressions, and security issues.
---

# Code Review

Review the proposed changes in the context of the surrounding code and the stated requirements.

- Prioritize concrete bugs, regressions, and security issues over style or preference.
- Verify suspected issues against relevant call sites, tests, and existing behavior.
- Report only actionable findings introduced by the changes. Include severity, file, and line for each finding, and explain the impact and a practical fix.
- Do not make changes during a review unless explicitly asked to implement a fix.
- If there are no actionable findings, say so and mention any important validation gaps.
