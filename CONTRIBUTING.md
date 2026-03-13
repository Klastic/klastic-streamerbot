# Contributing to klastic-streamerbot

Thank you for your interest in contributing! Please follow the guidelines below.

---

## Semantic Commit Messages

This repository follows the [Conventional Commits](https://www.conventionalcommits.org/) specification. All pull request titles (and ideally individual commit messages) **must** use a semantic prefix so the project history stays easy to read and can support automated tooling in the future.

### Format

```
<type>(<optional scope>): <short description>
```

- The **type** and **description** are required.
- The **scope** is optional and should be a short noun describing the area affected (e.g. `commands`, `events`, `timers`, `utilities`).
- The description must start with a **lowercase** letter.

### Allowed Types

| Type | When to use |
|------|-------------|
| `feat` | A new feature or script |
| `fix` | A bug fix in an existing script |
| `docs` | Documentation only changes (README, comments) |
| `style` | Formatting, whitespace — no logic change |
| `refactor` | Code change that is neither a fix nor a feature |
| `perf` | Performance improvement |
| `test` | Adding or correcting tests |
| `build` | Build system or dependency changes |
| `ci` | CI/CD configuration changes |
| `chore` | Maintenance tasks that don't modify source files |
| `revert` | Reverts a previous commit |

### Examples

```
feat(commands): add !clip command for saving stream clips
fix(events): correct follow-age calculation for new followers
docs(timers): update README with hydration timer setup steps
refactor(utilities): simplify per-user cooldown tracker logic
ci: add semantic PR title enforcement workflow
chore: update .gitignore to exclude OS metadata files
```

---

## Pull Requests

- PR **titles** must follow the semantic format above — this is enforced automatically by CI.
- Keep PRs focused on a single concern.
- Reference any related issue in the PR description (e.g. `Closes #12`).

---

## General Guidelines

- Follow the existing code style in `.cs` files.
- Each new script should include a matching `README.md` with setup instructions.
- Test your scripts locally in Streamer.bot before submitting.
