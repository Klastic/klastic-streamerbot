# `!followage` — User Follow-Age Lookup

## Summary

Shows how long a user has been following the channel. Detects the platform the command was sent from and routes to the appropriate data source:

- **Twitch**: reads follow age from args populated by the built-in **Get Follow Age Info for Target** sub-action — includes authoritative follow date and human-readable age directly from Twitch
- **YouTube / Kick**: reads from a local JSON follow-data file (`data/klastic-follows.json`) that is populated by the [New Follower event handler](../../events/new-follower/)

Mods and the broadcaster can look up any user by passing a username (`!followage someuser`). Regular viewers have a per-user cooldown.

> **Note on YouTube/Kick:** Because these platforms do not have a public follow-date API, follow data is only available for people who followed while the bot was active. The JSON store is built incrementally over time.

---

## Code

See [`followage.cs`](followage.cs).

---

## Streamer.bot Setup

### Step 1 — Set up the New Follower event to populate the JSON store (YouTube/Kick)

Follow the setup in [`events/new-follower/README.md`](../../events/new-follower/README.md) — that action writes follow data to `data/klastic-follows.json` for all platforms.

### Step 2 — Create the command action

1. Go to **Actions → Add**, name it `!followage`
2. Add a sub-action: **Twitch → Followers → Get Follow Age Info for Target**
   - **Source Type**: `From Input`
   - This reads the username typed after `!followage` (or defaults to the command invoker when no username is given)
3. Add a sub-action: **Core → Execute C# Code**
4. Paste `followage.cs` into the editor, **Compile**, then **Save**

> **Important:** The **Get Follow Age Info for Target** sub-action must be added **before** the C# code sub-action. It populates args like `isFollowing`, `followDate`, and `followAgeLong` that the script reads for the Twitch path.

### Step 3 — Create the command trigger

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
| `FOLLOW_DATA_FILE` | Top of script | Path to the cross-platform follow data JSON file |
| `MSG_NOT_IN_STORE` | Top of script | Message when a user has no follow data recorded |

---

## Platform Support

| Platform | Method | Notes |
|---|---|---|
| Twitch | Get Follow Age Info for Target sub-action | Authoritative — includes retroactive follows |
| YouTube | Local JSON file | Only followers captured while bot was active |
| Kick | Local JSON file | Only followers captured while bot was active |

---

## Repo Notes

Platform-aware follow-age command. Twitch path reads follow age from args populated by the built-in "Get Follow Age Info for Target" sub-action (authoritative, includes retroactive follows). YouTube/Kick paths read from the local JSON follow store (`data/klastic-follows.json`) populated by the New Follower event handler. The follow store key format is `platform:username` (e.g., `kick:viewername`).

---

## Video Notes

Worth highlighting:
- Why Twitch uses the built-in sub-action for authoritative follow data (retroactive follows) but YouTube/Kick cannot
- The JSON follow-store pattern and how the `new-follower` event builds it over time
- Platform detection via `args["platform"]` and the fallback to "twitch"
- The `HandleTwitch` / `HandleNonTwitch` split and why they're separate methods
- Why the "Get Follow Age Info for Target" sub-action must run **before** the C# inline code

