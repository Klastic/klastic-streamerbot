# `!shoutout` / `!so` — Cross-Platform Broadcaster Shoutout

## Summary

Gives a shoutout to another streamer, with behavior that differs by platform:

- **Twitch**: resolves the target's Twitch display name and last-played game via the Twitch API, then fires Twitch's native `/shoutout` command in addition to the chat message
- **YouTube**: posts a shoutout message with a best-effort `https://youtube.com/@username` link
- **Kick**: posts a shoutout message with a `https://kick.com/username` link

Per-target cooldown (default 5 min) is shared across all platforms — if someone was just shouted out on Twitch, they can't be immediately shouted out on YouTube either. Non-mods are blocked by default.

---

## Code

See [`shoutout.cs`](shoutout.cs).

---

## Streamer.bot Setup

### Create the command action

1. Go to **Actions → Add**, name it `!shoutout`
2. Add a sub-action: **Core → Execute C# Code**
3. Paste `shoutout.cs` into the editor, **Compile**, then **Save**

### Create the command triggers

Create **two** command triggers pointing to the same action, for each platform:

| Command | Platform | Notes |
|---|---|---|
| `!shoutout` | All platforms | Long form |
| `!so` | All platforms | Short alias |

Recommended settings:
- **Global Cooldown**: 0 (script handles per-target cooldown)
- **Case Insensitive**: Yes
- Restrict to **Mods+** at the command level if `ALLOW_NON_MODS = false`

### Required Twitch permissions (Twitch path only)

The `CPH.TwitchShoutoutUser()` call requires your bot account to have **Editor** permission on the channel. If you don't have editor rights, the chat message will still be sent — only the native platform SO will be skipped (it logs a warning).

---

## Customization

| Value | Location | Purpose |
|---|---|---|
| `PER_TARGET_COOLDOWN_SECONDS` | Top of script | Minimum seconds before the same target can be shouted out again |
| `MSG_TWITCH_WITH_GAME` | Top of script | Twitch message when target has a last-played game |
| `MSG_TWITCH_WITHOUT_GAME` | Top of script | Twitch message when target has no category |
| `MSG_YOUTUBE` | Top of script | YouTube shoutout message |
| `MSG_KICK` | Top of script | Kick shoutout message |
| `ALLOW_NON_MODS` | Top of script | Set to `true` to allow all viewers |

---

## Platform Support

| Platform | Channel Info Lookup | Native SO | Fallback URL |
|---|---|---|---|
| Twitch | ✅ Twitch API (display name + game) | ✅ `/shoutout` command | N/A |
| YouTube | ❌ No API available | N/A | `youtube.com/@username` |
| Kick | ❌ No API available | N/A | `kick.com/username` |

---

## Repo Notes

Platform-aware shoutout command. Twitch path uses `CPH.TwitchGetExtendedUserInfoByLogin` for display name/game and fires `CPH.TwitchShoutoutUser`. YouTube/Kick paths construct best-effort URLs from the provided username with no API lookup.

---

## Video Notes

Worth highlighting:
- Why Twitch can look up channel info but YouTube/Kick cannot (API access differences)
- The `HandleTwitchShoutout` / `HandleNonTwitchShoutout` routing pattern
- Why the cooldown key is shared across platforms (prevents double-SO across platforms)
- The best-effort `youtube.com/@handle` URL construction and its limitations

