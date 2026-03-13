# `!uptime` — Stream Uptime Display

## Summary

Displays how long the current stream has been live. It reads a UTC start-time stored as a global variable when the stream goes online, calculates the elapsed duration, and posts a formatted message in chat.

Handles edge cases: missing global, malformed timestamp, and stale values from a previous stream session.

---

## Code

See [`uptime.cs`](uptime.cs).

---

## Streamer.bot Setup

### Step 1 — Store the stream start time

1. Go to **Events → Twitch → Stream Online**
2. Create (or open) the action linked to that event
3. Add a sub-action: **Variables → Set Global Variable**
   - Name: `streamStartedAt`
   - Value: `%utcNow%`
   - Persisted: **No** (stream time should reset each session)

### Step 2 — Create the command action

1. Go to **Actions → Add**
2. Name it `!uptime`
3. Add a sub-action: **Core → Execute C# Code**
4. Paste the contents of `uptime.cs` into the editor
5. Click **Compile**, then **Save**

### Step 3 — Create the command trigger

1. Go to **Commands → Add**
2. Set the command to `!uptime`
3. Set the **Action** to the action created above
4. Recommended settings:
   - **Global Cooldown**: 5–10 seconds (prevents spam)
   - **Case Insensitive**: Yes

---

## Customization

| Value | Location | Purpose |
|---|---|---|
| `GLOBAL_STREAM_START` | Top of script | Name of the global variable holding the start time |
| `MSG_NOT_LIVE` | Top of script | Message sent when the stream start time is unavailable |
| Global cooldown | Streamer.bot command settings | Reduce chat spam |

---

## Repo Notes

Self-contained uptime command. Depends on a `streamStartedAt` global that must be set by your Stream Online event. No external API calls required.

---

## Video Notes

Worth highlighting:
- The split between **storing the start time** (event action) and **reading it** (command action) — this is the key pattern
- The `%utcNow%` variable in Streamer.bot and why UTC is used
- The `FormatUptime` helper and how it handles sub-minute, sub-hour, and multi-hour streams
