# New Follower — Chat Announcement

## Summary

Announces new followers in chat and writes their follow data to a local JSON file for cross-platform follow-age lookups. Works on Twitch, YouTube, and Kick.

- **Chat announcement**: sends a configurable welcome message to whatever platform the follow came from
- **Follow store**: writes `platform:username → { followedAt, displayName, platform }` to `data/klastic-follows.json`; the [`!followage` command](../../commands/followage/) reads this file for YouTube/Kick lookups
- **Deduplication**: prevents double-announcements when a platform fires the follow event more than once
- **Follow count** (Twitch only): shown in the message if the arg is available

---

## Code

See [`new-follower.cs`](new-follower.cs).

---

## Streamer.bot Setup

### Create the action

1. Go to **Actions → Add**, name it `New Follower`
2. Add a sub-action: **Core → Execute C# Code**
3. Paste `new-follower.cs` into the editor, **Compile**, then **Save**

### Connect platform events

| Platform | Event path |
|---|---|
| Twitch | Events → Twitch → Follow |
| YouTube | Events → YouTube → Subscriber (maps to "follow" concept) |
| Kick | Events → Kick → Follow |

Connect the same `New Follower` action to each platform event.

---

## Customization

| Value | Location | Purpose |
|---|---|---|
| `MSG_NEW_FOLLOWER` | Top of script | Welcome message. `%user%` = follower name, `%followCount%` = total (Twitch only) |
| `SHOW_FOLLOW_COUNT` | Top of script | Show follower count in message (Twitch only; stripped on other platforms) |
| `DEDUP_WINDOW_SECONDS` | Top of script | Suppression window for duplicate follow events |
| `FOLLOW_DATA_FILE` | Top of script | Path to the JSON follow store |

---

## Follow Store File

The file is written to `data/klastic-follows.json` relative to the Streamer.bot executable. Example content:

```json
{
  "twitch:viewername": {
    "userName": "viewername",
    "displayName": "ViewerName",
    "followedAt": "2024-06-01T20:30:00.0000000Z",
    "platform": "Twitch"
  },
  "youtube:ythandle": {
    "userName": "ythandle",
    "displayName": "YTHandle",
    "followedAt": "2024-06-02T18:00:00.0000000Z",
    "platform": "YouTube"
  }
}
```

Existing entries are never overwritten — the original follow date is always preserved.

---

## Platform Support

| Platform | Chat Announcement | Follow Store Write |
|---|---|---|
| Twitch | ✅ | ✅ |
| YouTube | ✅ | ✅ |
| Kick | ✅ | ✅ |

---

## Repo Notes

Multi-platform follow handler. Dedup key now includes platform (`followDedup_platform_username`) so Twitch and YouTube dedup state don't collide. Follow count is stripped from the message for non-Twitch platforms.

---

## Video Notes

Worth highlighting:
- Why the JSON follow store is needed (YouTube/Kick have no follow-date API)
- The dedup key including platform name — a subtle but important change
- The `WriteFollowToStore` method: directory creation, existing-entry guard, and non-critical error handling
- How this action pairs with the `!followage` command to provide a unified follow-age experience

