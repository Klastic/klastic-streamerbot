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

## Versioning

This repository uses [Release Please](https://github.com/googleapis/release-please) to automate semantic versioning based on conventional commits.

### How it works

- Every time a PR is merged into `main`, the Release Please GitHub Action runs automatically.
- It scans the new commits, determines the appropriate version bump (major / minor / patch), and opens a **release PR** that updates the relevant `version.txt` files and `CHANGELOG.md` entries.
- When that release PR is merged, GitHub Releases are created with the correct tags.

### Version scope

| Scope | Version file | Git tag format |
|-------|-------------|----------------|
| Entire repository | `klastic-streamerbot.csproj` | `v1.2.3` |
| Individual component (e.g. `commands/lurk`) | `commands/lurk/lurk.csproj` | `lurk-v1.2.3` |

### What triggers a version bump

| Commit type | Bump |
|-------------|------|
| `feat` | **minor** (e.g. `1.0.0` → `1.1.0`) |
| `fix`, `perf`, `refactor`, `docs`, `style`, `test`, `build`, `ci`, `chore`, `revert` | **patch** (e.g. `1.0.0` → `1.0.1`) |
| Any type with `BREAKING CHANGE` footer or `!` suffix | **major** (e.g. `1.0.0` → `2.0.0`) |

Only the component whose files were modified will have its version bumped. The root repository version is **always** bumped whenever any component is updated.

---

## General Guidelines

- Follow the existing code style in `.cs` files.
- Each new script should include a matching `README.md` with setup instructions.
- Test your scripts locally in Streamer.bot before submitting.
