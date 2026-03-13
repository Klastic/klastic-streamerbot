# Raid Handler

## Summary

Responds to incoming Twitch raids with a size-aware chat message. Raids are grouped into three tiers (small, medium, large) based on configurable viewer count thresholds. Includes a configurable delay before posting so the raiding viewers have time to arrive, an optional automatic native Twitch shoutout, an optional OBS scene switch for large raids, and deduplication to prevent double-handling.

---

## Code

See [`raid-handler.cs`](raid-handler.cs).

---

## Streamer.bot Setup

### Create the action

1. Go to **Actions → Add**, name it `Raid Handler`
2. Add a sub-action: **Core → Execute C# Code**
3. Paste `raid-handler.cs` into the editor, **Compile**, then **Save**

### Connect the Twitch raid event

1. Go to **Events → Twitch → Raid**
2. Set the Action to `Raid Handler`

---

## Customization

| Value | Location | Purpose |
|---|---|---|
| `MSG_SMALL_RAID` | Top of script | Message for raids below `MEDIUM_RAID_THRESHOLD` viewers |
| `MSG_MEDIUM_RAID` | Top of script | Message for mid-sized raids |
| `MSG_LARGE_RAID` | Top of script | Message for raids at or above `LARGE_RAID_THRESHOLD` viewers |
| `MEDIUM_RAID_THRESHOLD` | Top of script | Viewer count where medium message kicks in (default: 20) |
| `LARGE_RAID_THRESHOLD` | Top of script | Viewer count where large message kicks in (default: 100) |
| `DELAY_BEFORE_MESSAGE_MS` | Top of script | Milliseconds to wait before sending the welcome (default: 3000) |
| `AUTO_SHOUTOUT` | Top of script | Set to `false` to disable the automatic Twitch `/shoutout` |
| `SWITCH_SCENE_ON_LARGE_RAID` | Top of script | Set to `true` to switch OBS scene on large raids |
| `LARGE_RAID_SCENE` | Top of script | The OBS scene name to switch to on large raids |

---

## Repo Notes

Three-tier raid messaging system (small / medium / large) with configurable thresholds. Optional native Twitch shoutout and OBS scene switch. Deduplication using a non-persisted global keyed by raider login.

---

## Video Notes

Worth highlighting:
- The delay before sending (`CPH.Wait`) and why it improves the experience
- The three-tier threshold approach — easily adjustable to your audience size
- The optional `CPH.TwitchShoutoutUser` call and when you might want to turn it off
- OBS scene switching as a real production feature (hype scene on big raids)
