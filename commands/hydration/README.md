# `!hydrate` Hydration Reminder Command

## Summary

Lets viewers use `!hydrate` in Twitch, YouTube, or Kick chat to remind Klastic to drink water. The command has a ten minute global cooldown and reports the remaining wait time whenever it is on cooldown.

---

## Code

See [`hydration.cs`](hydration.cs).

---

## Streamer.bot Setup

### Create the action

1. Go to **Actions → Add**, name it `Hydration Command`
2. Add a sub-action: **Core → Execute C# Code**
3. Paste `hydration.cs` into the editor, **Compile**, then **Save**

### Create the `!hydrate` command trigger

1. Go to **Commands → Add**
2. Command: `!hydrate`
3. Action: `Hydration Command`
4. Recommended settings:
   - **Global Cooldown**: 0 seconds because the script owns the cooldown and reports remaining time
   - **Case Insensitive**: Yes

---

## Customization

| Value | Location | Purpose |
|---|---|---|
| `CooldownSeconds` | Top of script | Global cooldown length in seconds |

---

## Repo Notes

The script owns cooldown behavior so viewers never see a silent failure. Replies are routed to the platform where the command was invoked.

---

## Video Notes

Worth highlighting:
- Using both a global command cooldown (Streamer.bot setting) and a per-user cooldown (script variable) together
- The `%user%` and `%streamer%` placeholder replacement pattern
