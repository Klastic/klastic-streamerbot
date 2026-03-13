# New Follower — Chat Announcement

## Summary

Announces new Twitch followers in chat with a configurable welcome message and optional follower count. Includes deduplication logic to prevent double-announcements from Twitch occasionally firing the follow event more than once for the same user.

---

## Code

See [`new-follower.cs`](new-follower.cs).

---

## Streamer.bot Setup

### Create the action

1. Go to **Actions → Add**, name it `New Follower`
2. Add a sub-action: **Core → Execute C# Code**
3. Paste `new-follower.cs` into the editor, **Compile**, then **Save**

### Connect the Twitch event

1. Go to **Events → Twitch → Follow**
2. Set the Action to `New Follower`

---

## Customization

| Value | Location | Purpose |
|---|---|---|
| `MSG_NEW_FOLLOWER` | Top of script | Welcome message. `%user%` = follower name, `%followCount%` = total followers |
| `SHOW_FOLLOW_COUNT` | Top of script | Set to `false` to hide follower count from the message |
| `DEDUP_WINDOW_SECONDS` | Top of script | How long to suppress a duplicate follow event for the same user |

---

## Repo Notes

Uses a per-user dedup global (`followDedup_username`) to guard against Twitch's occasional duplicate follow event firing. Non-persisted so it resets each session.

---

## Video Notes

Worth highlighting:
- Why deduplication matters for follow events (Twitch sends duplicates more often than expected)
- How `%followCount%` is stripped from the message gracefully when not available
