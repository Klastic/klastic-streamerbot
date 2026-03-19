# Break Reminder Timer

## Summary

Sends a break/stretch reminder to chat on a regular interval. Posts to chat so viewers can encourage the streamer to take a moment to stretch, rest their eyes, and recharge.

---

## Code

See [`break-reminder.cs`](break-reminder.cs).

---

## Streamer.bot Setup

### Create the action

1. Go to **Actions → Add**, name it `Break Reminder`
2. Add a sub-action: **Core → Execute C# Code**
3. Paste `break-reminder.cs` into the editor, **Compile**, then **Save**

### Create the timer

1. Go to **Timers → Add**
2. Name: `Break Reminder`
3. Action: `Break Reminder`
4. Interval: `2700` seconds (45 minutes; adjust to taste)

> **Note:** Streamer.bot timers do not have a native "only when live" option. The script handles this automatically — it checks `CPH.ObsIsStreaming()` at the start and skips the reminder if OBS is not streaming. Set `CHECK_OBS_STREAMING = false` in the script if you don't use OBS.

### Optional: Store the broadcaster display name

The script reads a `broadcastUserName` global for the streamer's name. Set this in your Stream Online action:
- **Variables → Set Global Variable**: `broadcastUserName` = `%broadcastUserName%`

If the global is not set, the script falls back to the `broadcastUserName` arg from the action context, and finally to the string `"streamer"`.

---

## Customization

| Value | Location | Purpose |
|---|---|---|
| `MSG_BREAK` | Top of script | The break/stretch message — use `%streamer%` for their name |
| `CHECK_OBS_STREAMING` | Top of script | Set to `false` to skip the OBS live check (e.g. if you use SLOBS) |
| Timer interval | Streamer.bot Timers settings | How often reminders fire (default: 2700 seconds / 45 minutes) |

---

## Repo Notes

Simple single-message break reminder timer. For a hydration reminder on a separate schedule, see [`timers/hydration-reminder`](../hydration-reminder/README.md). Viewers can also remind the streamer to drink water using the [`!hydration`](../../commands/hydration/README.md) command.

---

## Video Notes

Worth highlighting:
- The OBS live check (`CPH.ObsIsStreaming()`) and why it prevents offline timer spam
- The `%streamer%` placeholder replacement pattern for message customization
- How this pairs with the hydration-reminder timer on an independent schedule
