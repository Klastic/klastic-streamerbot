# `!lurk` / `!unlurk` — Lurk Tracking System

## Summary

Handles viewer lurk mode with both `!lurk` and `!unlurk` commands. When a viewer lurks, their start time is saved to a persistent user variable. When they return with `!unlurk`, the script reads the stored time, calculates how long they were lurking, and posts an appropriate welcome-back message.

This is a single script wired to two separate commands — Streamer.bot passes the triggered command name via the `command` arg, so no duplicate code is needed.

---

## Code

See [`lurk.cs`](lurk.cs).

---

## Streamer.bot Setup

### Create the action

1. Go to **Actions → Add**, name it `Lurk Commands`
2. Add a sub-action: **Core → Execute C# Code**
3. Paste `lurk.cs` into the editor, **Compile**, then **Save**

### Create the `!lurk` command trigger

1. Go to **Commands → Add**
2. Command: `!lurk`
3. Action: `Lurk Commands`
4. Recommended settings:
   - **Global Cooldown**: 0 (per-user state prevents double-lurks)
   - **Case Insensitive**: Yes

### Create the `!unlurk` command trigger

1. Go to **Commands → Add**
2. Command: `!unlurk`
3. Action: `Lurk Commands` (same action as above)
4. Same recommended settings

> **Note:** Both commands point to the same action. The script checks `args["command"]` to determine which flow to run.

---

## Customization

| Value | Location | Purpose |
|---|---|---|
| `MSG_LURK` | Top of script | Chat message when a viewer enters lurk. Use `%user%` for their name. |
| `MSG_UNLURK` | Top of script | Chat message when a viewer returns from lurk |
| `MSG_NOT_LURKING` | Top of script | Message when `!unlurk` is used without a prior `!lurk` |
| `SHOW_LURK_DURATION` | Top of script | Set to `false` to hide the "(lurked for X)" suffix |

---

## Repo Notes

Single script handling both `!lurk` and `!unlurk` via the `command` arg. Lurk state is stored in a persisted user variable, so it survives Streamer.bot restarts. Duration formatting is shown only if the viewer lurked for more than 60 seconds.

---

## Video Notes

Worth highlighting:
- The "one action, two commands" pattern using `args["command"]`
- Persistent vs. non-persisted user variables and why lurk time should be persisted
- The `%user%` placeholder replacement pattern for message customization
