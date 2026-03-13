# Stream End — Cleanup Action

## Summary

Runs when the stream goes offline. Posts an optional goodbye message to chat, then clears all session-scoped global variables so they don't interfere with the next stream session. This is the companion to the Stream Start action and ensures a clean slate each session.

---

## Code

See [`stream-end.cs`](stream-end.cs).

---

## Streamer.bot Setup

### Create the action

1. Go to **Actions → Add**, name it `Stream End`
2. Add a sub-action: **Core → Execute C# Code**
3. Paste `stream-end.cs` into the editor, **Compile**, then **Save**

### Connect the Twitch Stream Offline event

1. Go to **Events → Twitch → Stream Offline**
2. Set the Action to `Stream End`

---

## Customization

| Value | Location | Purpose |
|---|---|---|
| `MSG_STREAM_OFFLINE` | Top of script | Goodbye message posted to chat. Leave empty to disable. |
| `ANNOUNCE_IN_CHAT` | Top of script | Set to `false` to suppress the chat message |
| `SESSION_GLOBALS_TO_CLEAR` | Top of script | List of non-persisted global variable names to unset on stream end. Add any new session globals here. |

---

## Repo Notes

Companion to `stream-start`. Clears all non-persisted session globals explicitly. The `SESSION_GLOBALS_TO_CLEAR` array should mirror any session globals set by other scripts in the repository.

---

## Video Notes

Worth highlighting:
- Why clearing non-persisted globals explicitly matters (Streamer.bot sessions and restart behavior)
- How `stream-start` and `stream-end` form a paired lifecycle for shared state
- The `CPH.Wait(1500)` before cleanup to ensure the chat message gets sent first
