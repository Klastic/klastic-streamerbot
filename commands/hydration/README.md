# `!hydration` — Hydration Reminder Command

## Summary

Lets any viewer use `!hydration` in chat to nudge the streamer to drink water. The message tags the streamer and credits the viewer who sent the reminder. A per-user cooldown prevents spam.

---

## Code

See [`hydration.cs`](hydration.cs).

---

## Streamer.bot Setup

### Create the action

1. Go to **Actions → Add**, name it `Hydration Command`
2. Add a sub-action: **Core → Execute C# Code**
3. Paste `hydration.cs` into the editor, **Compile**, then **Save**

### Create the `!hydration` command trigger

1. Go to **Commands → Add**
2. Command: `!hydration`
3. Action: `Hydration Command`
4. Recommended settings:
   - **Global Cooldown**: 60 seconds
   - **Case Insensitive**: Yes

---

## Customization

| Value | Location | Purpose |
|---|---|---|
| `MSG_HYDRATION` | Top of script | The chat message — use `%user%` for the viewer's name and `%streamer%` for the broadcaster's name |
| `COOLDOWN_SECONDS` | Top of script | Per-user cooldown in seconds (default: 300 / 5 minutes). Set to `0` to disable. |

---

## Repo Notes

Per-user cooldown is enforced via a persistent user variable (`hydrationLastUsed`) so the cooldown survives Streamer.bot restarts. You can also configure a global command cooldown in Streamer.bot's **Commands** settings for an additional layer of rate limiting.

---

## Video Notes

Worth highlighting:
- Using both a global command cooldown (Streamer.bot setting) and a per-user cooldown (script variable) together
- The `%user%` and `%streamer%` placeholder replacement pattern
