# `!followage` — User Follow-Age Lookup

## Summary

Shows how long a user has been following the channel. Detects the platform the command was sent from and routes to the appropriate data source:

- **Twitch**: uses the live Twitch API for authoritative follow data (real follow date, handles retroactive follows)
- **YouTube / Kick**: reads from a local JSON follow-data file (`data/klastic-follows.json`) that is populated by the [New Follower event handler](../../events/new-follower/)

Mods and the broadcaster can look up any user by passing a username (`!followage someuser`). Regular viewers have a per-user cooldown.

> **Note on YouTube/Kick:** Because these platforms do not have a public follow-date API, follow data is only available for people who followed while the bot was active. The JSON store is built incrementally over time.

---

## Code

See [`followage.cs`](followage.cs).

---

## Streamer.bot Setup

### Step 1 — Store the broadcaster's Twitch user ID (Twitch path)

1. Go to **Events → Twitch → Stream Online**
2. Open or create the action for that event
3. Add a sub-action: **Variables → Set Global Variable**
   - Name: `broadcasterUserId`
   - Value: `%broadcastUserId%`
   - Persisted: **Yes**

### Step 2 — Set up the New Follower event to populate the JSON store (YouTube/Kick path)

Follow the setup in [`events/new-follower/README.md`](../../events/new-follower/README.md) — that action writes follow data to `data/klastic-follows.json` for all platforms.

### Step 3 — Create the command action

1. Go to **Actions → Add**, name it `!followage`
2. Add a sub-action: **Core → Execute C# Code**
3. Paste `followage.cs` into the editor, **Compile**, then **Save**

### Step 4 — Create the command trigger

1. Go to **Commands → Add**
2. Command: `!followage`
3. Action: the action above
4. Recommended settings:
   - **Global Cooldown**: 3 seconds
   - **Case Insensitive**: Yes
5. Add triggers for Twitch, YouTube, and Kick chat message events

---

## Customization

| Value | Location | Purpose |
|---|---|---|
| `PER_USER_COOLDOWN_SECONDS` | Top of script | Seconds between a user rerunning the command |
| `GLOBAL_BROADCASTER_ID` | Top of script | Name of the global holding the broadcaster's Twitch user ID |
| `FOLLOW_DATA_FILE` | Top of script | Path to the cross-platform follow data JSON file |
| `MSG_NOT_IN_STORE` | Top of script | Message when a YouTube/Kick user has no follow data recorded |

---

## Platform Support

| Platform | Method | Notes |
|---|---|---|
| Twitch | Twitch Helix API | Authoritative — includes retroactive follows |
| YouTube | Local JSON file | Only followers captured while bot was active |
| Kick | Local JSON file | Only followers captured while bot was active |

---

## Repo Notes

Platform-aware follow-age command. Routes Twitch lookups through the Twitch API and YouTube/Kick lookups through a local JSON file populated by the New Follower event handler. The follow store key format is `platform:username` (e.g., `youtube:channelname`).

---

## Video Notes

Worth highlighting:
- Why Twitch can query historical follow data (Helix API) but YouTube/Kick cannot
- The JSON follow-store pattern and how the `new-follower` event builds it over time
- Platform detection via `args["platform"]` and the fallback to "twitch"
- The `HandleTwitch` / `HandleNonTwitch` split and why they're separate methods

