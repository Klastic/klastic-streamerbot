# No Spoilers Timer

## Summary

Posts a spoiler-warning message to Twitch, Kick, and YouTube on a repeating interval. Before sending, the script checks with OBS to confirm the stream is live — if OBS is not streaming, the reminder is skipped entirely. If the current game on a given platform matches any entry in that platform's skip list, the message is silently suppressed for that platform only — useful when streaming games where spoiler-free play isn't relevant (e.g. multiplayer titles).

Game matching is case-insensitive and trims surrounding whitespace, so `"overwatch"`, `"Overwatch"`, and `" Overwatch "` all match the same entry.

> **Requires OBS** connected to Streamer.bot via the OBS WebSocket plugin. If you use SLOBS or another streaming application, set `CHECK_OBS_STREAMING = false` in the script to disable the live check.

---

## Code

See [`no-spoilers.cs`](no-spoilers.cs).

---

## Streamer.bot Setup

### Step 1 — Connect OBS

This script uses OBS to verify the stream is live before posting. Make sure OBS is connected to Streamer.bot:

1. In OBS, install the **obs-websocket** plugin (bundled with OBS 28+)
2. In Streamer.bot, go to **Stream Apps → OBS** and add your OBS WebSocket connection
3. Connect and confirm the status shows **Connected**

### Step 2 — Create the action

1. Go to **Actions → Add**, name it `No Spoilers Reminder`
2. Add a sub-action: **Core → Execute C# Code**
3. Paste `no-spoilers.cs` into the editor, **Compile**, then **Save**

### Step 3 — Create the timer

1. Go to **Timers → Add**
2. Name: `No Spoilers Reminder`
3. Action: `No Spoilers Reminder`
4. Interval: `1800` seconds (recommended; adjust to taste)

### Step 4 — Provide the current game globals

The script reads three global variables to find the current game for each platform. Set these up wherever you update your stream category (e.g. a Stream Online action or a Category Changed event):

| Global variable | Expected value |
|---|---|
| `twitchGameName` | Current Twitch category name |
| `kickGameName` | Current Kick category name |
| `youtubeGameName` | Current YouTube category name |

Use a **Variables → Set Global Variable** sub-action for each one, mapped to the relevant platform argument (e.g. `%gameName%`).

If a variable is not set or is empty, the script treats it as an empty string and will **send** the reminder for that platform (no match against the skip list).

---

## Customization

| Value | Location | Purpose |
|---|---|---|
| `SPOILER_MESSAGE` | Top of script | The message posted to chat |
| `CHECK_OBS_STREAMING` | Top of script | Set to `false` to skip the OBS live check (e.g. if you use SLOBS) |
| `TWITCH_SKIP_GAMES` | Top of script | Games that suppress the Twitch reminder |
| `KICK_SKIP_GAMES` | Top of script | Games that suppress the Kick reminder |
| `YOUTUBE_SKIP_GAMES` | Top of script | Games that suppress the YouTube reminder |
| Timer interval | Streamer.bot Timers settings | How often the reminder fires |

---

## Repo Notes

Per-platform skip lists allow different game names across Twitch, Kick, and YouTube (e.g. `"Dungeons and Dragons"` on Twitch vs `"TableTopRPGs"` on Kick). All matching is case-insensitive. Each platform send is wrapped in its own `try/catch` so a failure on one platform does not prevent the others from sending.

The OBS live check (`CPH.ObsIsStreaming()`) runs first and short-circuits the entire action when the stream is not active. This prevents the timer from posting spoiler reminders while you're offline. If you don't use OBS, set `CHECK_OBS_STREAMING = false` to bypass the check.

---

## Video Notes

Worth highlighting:
- The OBS live check (`CPH.ObsIsStreaming()`) and why it prevents offline timer spam
- Why per-platform skip lists are needed (category names differ between services)
- The `Normalize` helper and case-insensitive matching pattern
- Independent `try/catch` blocks per platform for resilience
