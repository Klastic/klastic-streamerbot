# Copilot Instructions

## Project Overview

This repository contains C# scripts for [Streamer.bot](https://streamer.bot/). Scripts are organised into four top-level categories, each containing one sub-package (folder) per script:

```
commands/   – chat commands (e.g. lurk, shoutout, uptime)
events/     – stream event handlers (e.g. new-follower, raid-handler)
timers/     – timed actions (e.g. hydration-reminder, social-reminder)
utilities/  – shared helper scripts (e.g. follow-store, user-cooldown)
```

Each sub-package folder contains at minimum:
- A `.cs` source file with the Streamer.bot C# script
- A `README.md` with setup instructions
- A `.csproj` version file (see Versioning below)

---

## Versioning

This repository uses [Release Please](https://github.com/googleapis/release-please) (`googleapis/release-please-action@v4`) with the `dotnet` release type for automated semantic versioning.

### Version files

Every package — the root repository and each sub-package — has a `.csproj` file that holds its version in a `<Version>` element:

| Package | Version file |
|---------|-------------|
| Root repository | `klastic-streamerbot.csproj` |
| `commands/lurk` | `commands/lurk/lurk.csproj` |
| `events/new-follower` | `events/new-follower/new-follower.csproj` |
| *(same pattern for all sub-packages)* | `<category>/<name>/<name>.csproj` |

A minimal `.csproj` version file looks like:

```xml
<Project>
  <PropertyGroup>
    <Version>1.0.0</Version>
  </PropertyGroup>
</Project>
```

### Release Please configuration

- **`release-please-config.json`** — declares every package with `release-type: dotnet` and an explicit `version-file` path pointing to its `.csproj`.
- **`.release-please-manifest.json`** — stores the current version for every package; must be initialised with `"1.0.0"` for any new entry.
- **`.github/workflows/release-please.yml`** — triggers on every push to `main`; opens/updates a release PR automatically.

### How version bumps work

| Commit type | Version bump |
|-------------|-------------|
| `feat` | **minor** (1.0.0 → 1.1.0) |
| `fix`, `perf`, `refactor`, `docs`, `style`, `test`, `build`, `ci`, `chore`, `revert` | **patch** (1.0.0 → 1.0.1) |
| Any type with `BREAKING CHANGE` footer or `!` suffix | **major** (1.0.0 → 2.0.0) |

Only the sub-package(s) whose files changed receive a version bump. The **root** version (`klastic-streamerbot.csproj`) is **always** bumped whenever any component changes.

---

## Adding a New Sub-package

When adding a new sub-package (command, event, timer, or utility), the following files **must** be created or updated:

### 1. Create the sub-package folder and files

```
<category>/<name>/
├── <name>.cs          # Streamer.bot C# script
├── README.md          # Setup instructions
└── <name>.csproj      # Version file (start at 1.0.0)
```

The `.csproj` content:

```xml
<Project>
  <PropertyGroup>
    <Version>1.0.0</Version>
  </PropertyGroup>
</Project>
```

### 2. Register the package in `release-please-config.json`

Add a new entry under `"packages"`:

```json
"<category>/<name>": {
  "release-type": "dotnet",
  "package-name": "<name>",
  "version-file": "<category>/<name>/<name>.csproj",
  "changelog-path": "CHANGELOG.md"
}
```

### 3. Initialise the version in `.release-please-manifest.json`

Add the new package with its starting version:

```json
"<category>/<name>": "1.0.0"
```

### Checklist for new sub-packages

- [ ] `<category>/<name>/<name>.cs` — script source
- [ ] `<category>/<name>/README.md` — setup docs
- [ ] `<category>/<name>/<name>.csproj` — version file initialised at `1.0.0`
- [ ] `release-please-config.json` — new package entry added
- [ ] `.release-please-manifest.json` — new version entry added (`"1.0.0"`)

---

## Pull Request Title Format

Every pull request title **must** follow the [Conventional Commits](https://www.conventionalcommits.org/) format to pass the `Semantic Pull Request / Validate PR title` check:

```
<type>[optional scope]: <subject>
```

### Allowed types

| Type | When to use |
|------|-------------|
| `feat` | A new feature or script |
| `fix` | A bug fix |
| `docs` | Documentation changes only |
| `style` | Formatting/whitespace changes that don't affect logic |
| `refactor` | Code refactoring without feature or bug-fix changes |
| `perf` | Performance improvements |
| `test` | Adding or updating tests |
| `build` | Changes to build system or external dependencies |
| `ci` | CI/CD configuration changes |
| `chore` | Maintenance tasks that don't fit elsewhere |
| `revert` | Reverts a previous commit |

### Rules

- The **subject** (the part after `<type>: `) must **start with a lowercase letter** — not uppercase.
- The **scope** is optional. When used, it appears in parentheses: `feat(lurk): add cooldown support`.
- Breaking changes can be indicated by appending `!` after the type/scope: `feat!: redesign action API`.

### Examples

| ✅ Valid | ❌ Invalid |
|---------|----------|
| `feat: add hydration reminder timer` | `Add hydration reminder timer` *(missing type)* |
| `fix(lurk): correct cooldown logic` | `fix: Correct cooldown logic` *(subject starts with uppercase)* |
| `docs: update shoutout README` | `docs: Update shoutout README` *(subject starts with uppercase)* |
| `chore: update dependencies` | `chore(deps): Update dependencies` *(subject starts with uppercase)* |
