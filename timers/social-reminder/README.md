# Social Reminder Timer

## Summary

Rotates through a list of social media reminder messages on a schedule, posting one each interval. Fires on a pure time-based schedule — no chat activity required.

The rotation index is stored as a non-persisted global so it persists across triggers within a session but resets between streams.

---

## Code

See [`social-reminder.cs`](social-reminder.cs).

---

## Streamer.bot Setup

### Step 1 — Create the action

1. Go to **Actions → Add**, name it `Social Reminder Timer`
2. Add a sub-action: **Core → Execute C# Code**
3. Paste `social-reminder.cs` into the editor, **Compile**, then **Save**

### Step 2 — Create the timer

1. Go to **Timers → Add**
2. Name: `Social Reminder`
3. Action: `Social Reminder Timer`
4. Interval: `900` seconds (15 minutes)

> **Note:** Streamer.bot timers do not have a native "only when live" option. The script handles this automatically — it checks `CPH.ObsIsStreaming()` at the start and skips the reminder if OBS is not streaming. Set `CHECK_OBS_STREAMING = false` in the script if you don't use OBS.

---

## Customization

| Value | Location | Purpose |
|---|---|---|
| `MESSAGES` array | Top of script | The messages to rotate through — add/remove/reorder freely |
| `CHECK_OBS_STREAMING` | Top of script | Set to `false` to skip the OBS live check (e.g. if you use SLOBS) |
| Timer interval | Streamer.bot Timers settings | How often the action is triggered |

---

## Repo Notes

Rotating social reminder on a pure time-based schedule. Rotation state is stored in a non-persisted global (`socialReminderIndex`).

---

## Video Notes

Worth highlighting:
- Pure time-based schedule — fires every interval regardless of chat activity
- Rotating vs. random message selection (this uses rotation for predictability)
- How the index global wraps with modulo arithmetic
