# Stream Start — Initialization Action

## Summary

Runs when the stream goes online. Sets the global variables that other scripts in this repository depend on: stream start time (for `!uptime`), broadcaster user ID (for `!followage`), broadcaster display name (for timers and event messages), and resets the chat line counter (for the social reminder activity guard). Optionally posts a "we are live" message to chat.

This is the foundation action that bootstraps the shared global state used throughout the session.

---

## Code

See [`stream-start.cs`](stream-start.cs).

---

## Streamer.bot Setup

### Create the action

1. Go to **Actions → Add**, name it `Stream Start`
2. Add a sub-action: **Core → Execute C# Code**
3. Paste `stream-start.cs` into the editor, **Compile**, then **Save**

### Connect the Twitch Stream Online event

1. Go to **Events → Twitch → Stream Online**
2. Set the Action to `Stream Start`

---

## Customization

| Value | Location | Purpose |
|---|---|---|
| `MSG_STREAM_ONLINE` | Top of script | The chat message posted when going live. Leave empty to disable. |
| `ANNOUNCE_IN_CHAT` | Top of script | Set to `false` to suppress the chat message entirely |
| Global variable names | Top of script | Names must match what other scripts expect — change all in sync if renaming |

---

## Repo Notes

Initializes all shared globals for the session. Used as a dependency by `!uptime`, `!followage`, social-reminder, and hydration-reminder. Broadcaster identity globals are persisted so they survive restarts; session-specific globals (start time, chat count) are non-persisted.

---

## Video Notes

Worth highlighting:
- Why globals are the right tool for sharing state across independent actions
- The difference between persisted and non-persisted globals (identity vs. session data)
- How this one action bootstraps everything else — the "hub" pattern
