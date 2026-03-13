# Cross-Platform Follow Store Utility

## Summary

A standalone utility action for reading and writing the cross-platform follow data file (`data/klastic-follows.json`). This file is the shared data source that powers `!followage` on YouTube and Kick, and is written to automatically by the [New Follower event handler](../../events/new-follower/).

Use this utility when you need to manually manage follow data — for example, bulk-importing existing followers, removing stale entries, or checking follow data from a custom action.

### Supported operations

| `storeAction` value | What it does |
|---|---|
| `write` | Writes a new follow entry (preserves existing entries — won't overwrite) |
| `read` | Reads a follow entry and sets output args for downstream sub-actions |
| `delete` | Removes a follow entry |
| `exists` | Checks if an entry exists and sets `followExists` output arg |

---

## Code

See [`follow-store.cs`](follow-store.cs).

---

## File Format

The follow store is a flat JSON object at `data/klastic-follows.json` (relative to the Streamer.bot executable):

```json
{
  "twitch:viewerlogin": {
    "userName": "viewerlogin",
    "displayName": "ViewerLogin",
    "followedAt": "2024-06-01T20:30:00.0000000Z",
    "platform": "Twitch"
  },
  "youtube:ythandle": {
    "userName": "ythandle",
    "displayName": "YTHandle",
    "followedAt": "2024-06-02T18:00:00.0000000Z",
    "platform": "YouTube"
  },
  "kick:kickuser": {
    "userName": "kickuser",
    "displayName": "KickUser",
    "followedAt": "2024-06-03T14:00:00.0000000Z",
    "platform": "Kick"
  }
}
```

The key format is `platform:username` (both lowercase). Original follow dates are never overwritten by new writes.

---

## Streamer.bot Setup

### Create the utility action

1. Go to **Actions → Add**, name it `Follow Store`
2. Add a sub-action: **Core → Execute C# Code**
3. Paste `follow-store.cs` into the editor, **Compile**, then **Save**

### Using it from another action

1. Set the required input args before calling this action:

   | Arg | Required for | Description |
   |---|---|---|
   | `storeAction` | All | `write`, `read`, `delete`, or `exists` |
   | `storePlatform` | All | `twitch`, `youtube`, or `kick` |
   | `storeUserName` | All | The user's login name (lowercase) |
   | `storeDisplayName` | `write` | Display name to record |
   | `storeFollowedAt` | `write` (optional) | ISO-8601 follow date; defaults to now |

2. Add a sub-action: **Core → Run Action** → `Follow Store`

3. Use the output args set by `read`:

   | Output Arg | Description |
   |---|---|
   | `followFound` | `"true"` or `"false"` |
   | `followDisplayName` | Stored display name |
   | `followedAt` | Stored follow date (ISO-8601) |
   | `followPlatform` | Platform label |
   | `followAgeFormatted` | Human-readable age (e.g., `"1 year, 3 months"`) |
   | `followExists` | `"true"` or `"false"` (set by `exists` operation) |

---

## Customization

| Value | Location | Purpose |
|---|---|---|
| `FOLLOW_DATA_FILE` | Top of script | Path to the JSON file — must match the path in `new-follower.cs` and `followage.cs` |

---

## Repo Notes

Standalone utility for managing the cross-platform follow store. The `write` operation is idempotent (won't overwrite). The `read` operation sets formatted output args for use in downstream sub-actions. File path must be consistent with `new-follower.cs` and `followage.cs`.

---

## Video Notes

Worth highlighting:
- The shared JSON file pattern — a "database" built incrementally by the New Follower event
- How `storeAction` makes one script handle four operations (same "one action, many behaviors" pattern as `!lurk`)
- The output arg pattern from `read` — sets `%followFound%`, `%followAgeFormatted%` etc. for use in condition groups
