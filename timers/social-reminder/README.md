# Social Reminder Timer

## Summary

Rotates through a list of social media reminder messages on a schedule, posting one each interval. Includes an activity guard that skips posting if fewer than N chat messages have been sent since the last reminder — so the timer stays quiet during slow or empty chat periods.

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
4. Interval: `20` minutes (or your preferred interval)
5. **Only when live**: Yes ✓

### Step 3 — Track chat activity (required for activity guard)

The script reads a global `chatLineCount` that increments with each chat message. Set this up:

1. Go to **Events → Twitch → Chat Message**
2. Create (or open) the action for that event
3. Add a sub-action: **Variables → Increment Global Variable**
   - Name: `chatLineCount`
   - Amount: `1`
   - Persisted: **No**

> If you skip this step, `chatLineCount` will always be 0 and the reminder will never post. Set `MIN_CHAT_LINES_SINCE_LAST` to `0` in the script to disable the activity guard.

---

## Customization

| Value | Location | Purpose |
|---|---|---|
| `MESSAGES` array | Top of script | The messages to rotate through — add/remove/reorder freely |
| `MIN_CHAT_LINES_SINCE_LAST` | Top of script | Minimum chat activity required before posting (0 to disable) |
| Timer interval | Streamer.bot Timers settings | How often the action is triggered |

---

## Repo Notes

Rotating social reminder with activity guard. Requires a `chatLineCount` global incremented per chat message. Rotation state is stored in a non-persisted global (`socialReminderIndex`).

---

## Video Notes

Worth highlighting:
- The activity guard pattern and why posting into dead chat is worth avoiding
- Rotating vs. random message selection (this uses rotation for predictability)
- How the index global wraps with modulo arithmetic
