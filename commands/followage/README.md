# `!followage` — User Follow-Age Lookup

## Summary

Shows how long a user has been following the channel, including the exact follow date and a human-readable duration. Moderators and the broadcaster can look up any user by passing a username (`!followage someuser`). Regular viewers are rate-limited per user in code to prevent abuse.

---

## Code

See [`followage.cs`](followage.cs).

---

## Streamer.bot Setup

### Step 1 — Store the broadcaster user ID

1. Go to **Events → Twitch → Stream Online**
2. Open or create the action for that event
3. Add a sub-action: **Variables → Set Global Variable**
   - Name: `broadcasterUserId`
   - Value: `%broadcastUserId%`
   - Persisted: **Yes** (safe to persist; it never changes)

### Step 2 — Create the command action

1. Go to **Actions → Add**, name it `!followage`
2. Add a sub-action: **Core → Execute C# Code**
3. Paste `followage.cs` into the editor, **Compile**, then **Save**

### Step 3 — Create the command trigger

1. Go to **Commands → Add**
2. Command: `!followage`
3. Action: the action above
4. Recommended settings:
   - **Global Cooldown**: 3 seconds
   - **Case Insensitive**: Yes

---

## Customization

| Value | Location | Purpose |
|---|---|---|
| `PER_USER_COOLDOWN_SECONDS` | Top of script | Seconds between a user rerunning the command |
| `GLOBAL_BROADCASTER_ID` | Top of script | Name of the global holding the broadcaster's Twitch user ID |

---

## Repo Notes

Uses `CPH.TwitchGetExtendedUserInfoByLogin` and `CPH.TwitchGetFollow` to resolve follow data. Requires the `broadcasterUserId` global to be set. Includes per-user cooldown logic and moderator lookup override.

---

## Video Notes

Worth highlighting:
- The moderator/broadcaster override pattern (`isModerator`, `isBroadcaster` args)
- Per-user cooldown stored as a user variable (vs. global cooldown)
- The `FormatFollowAge` helper and how it builds a human-readable duration from a `TimeSpan`
