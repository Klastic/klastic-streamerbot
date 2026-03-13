# Per-User Cooldown Helper

## Summary

A reusable utility script that enforces per-user cooldowns for any command. Run this as a sub-action before your main command logic and gate the subsequent steps on the output argument `cooldownPassed == "true"`.

The cooldown key and duration can be overridden at runtime via action arguments, making this a single shared utility usable across all your commands.

---

## Code

See [`user-cooldown.cs`](user-cooldown.cs).

---

## Streamer.bot Setup

### Create the utility action

1. Go to **Actions → Add**, name it `Per-User Cooldown Check`
2. Add a sub-action: **Core → Execute C# Code**
3. Paste `user-cooldown.cs` into the editor, **Compile**, then **Save**

### Use it in another action

In any command action that needs a per-user cooldown:

1. Add a sub-action: **Core → Run Action** → `Per-User Cooldown Check`
   - Set argument `cooldownKey` = `"mycommand"` (use a unique key per command)
   - Set argument `cooldownSeconds` = `"30"` (or your desired duration)

2. After the Run Action sub-action, add your main command logic sub-actions

3. Wrap your main sub-actions in a **Sub-action Group** with the condition:
   - `%cooldownPassed% == "true"`

   > In Streamer.bot, use **Right-click → Add Condition Group** on the group, or use the built-in "Only run if" condition on individual sub-actions.

### Alternatively: embed inline

For commands where you prefer self-contained scripts, copy the `CheckCooldown` pattern from this utility directly into your command script. This is the approach used in `followage.cs`.

---

## Customization

| Value | Location | Purpose |
|---|---|---|
| `DEFAULT_COOLDOWN_SECONDS` | Top of script | Fallback cooldown when no runtime override is provided |
| `DEFAULT_COOLDOWN_KEY` | Top of script | Fallback key when no `cooldownKey` arg is set |
| `cooldownSeconds` arg | Caller action | Override cooldown duration per command |
| `cooldownKey` arg | Caller action | Override key per command (use the command name) |

---

## Output Arguments

| Argument | Values | Description |
|---|---|---|
| `cooldownPassed` | `"true"` / `"false"` | Whether the cooldown has elapsed |
| `cooldownRemaining` | integer string | Seconds remaining on cooldown (0 if passed) |

---

## Repo Notes

Standalone cooldown utility designed to be called as a sub-action before any command logic. Cooldown state stored in non-persisted user variables keyed by `"cooldown_<key>"`. Supports runtime argument overrides for `cooldownKey` and `cooldownSeconds`.

---

## Video Notes

Worth highlighting:
- The "utility action" pattern in Streamer.bot — a shared action called by many others
- How `CPH.SetArgument` passes data to downstream sub-actions
- The condition group gating pattern: `%cooldownPassed% == "true"`
- Why per-user cooldowns (user variables) are better than global cooldowns for rate limiting
