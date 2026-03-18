# Hydration & Break Reminder Timer

## Summary

Alternates between a hydration reminder and a break/stretch reminder each time the timer fires. Posts to chat so viewers can cheer the streamer on. Uses the broadcaster's display name from a global variable for a personal touch.

---

## Code

See [`hydration-reminder.cs`](hydration-reminder.cs).

---

## Streamer.bot Setup

### Create the action

1. Go to **Actions → Add**, name it `Hydration Reminder`
2. Add a sub-action: **Core → Execute C# Code**
3. Paste `hydration-reminder.cs` into the editor, **Compile**, then **Save**

### Create the timer

1. Go to **Timers → Add**
2. Name: `Hydration Reminder`
3. Action: `Hydration Reminder`
4. Interval: `1800` seconds (recommended; adjust to taste)

> **Note:** Streamer.bot timers do not have a native "only when live" option. To prevent the reminder firing while you are offline, add an OBS live check at the top of `Execute()` — see [`no-spoilers.cs`](../no-spoilers/no-spoilers.cs) for the pattern using `CPH.ObsIsStreaming()`.

### Optional: Store the broadcaster display name

The script reads a `broadcastUserName` global for the streamer's name. Set this in your Stream Online action:
- **Variables → Set Global Variable**: `broadcastUserName` = `%broadcastUserName%`

If the global is not set, the script falls back to the `broadcastUserName` arg from the action context, and finally to the string `"streamer"`.

---

## Customization

| Value | Location | Purpose |
|---|---|---|
| `MSG_HYDRATION` | Top of script | The hydration message — use `%streamer%` for their name |
| `MSG_BREAK` | Top of script | The break/stretch message |
| Timer interval | Streamer.bot Timers settings | How often reminders fire |

---

## Repo Notes

Simple alternating two-message timer. Toggle state is stored as a non-persisted global (`hydrationReminderTurn`) so it resets each stream session.

---

## Video Notes

Worth highlighting:
- The boolean toggle stored as a global and flipped each run
- How to add an OBS live check (`CPH.ObsIsStreaming()`) to prevent offline timer spam
- How to quickly swap from two messages to a longer rotation list if desired
