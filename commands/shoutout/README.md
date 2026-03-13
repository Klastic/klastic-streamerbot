# `!shoutout` / `!so` — Broadcaster Shoutout

## Summary

Gives a shoutout to another Twitch streamer. Resolves the target's current channel info (display name and last game played) via Streamer.bot's built-in Twitch API and posts a formatted chat message. Also fires Twitch's native `/shoutout` command for the on-platform SO notification.

Includes a per-target cooldown (defaults to 5 minutes) to prevent double-SO spam. Non-mods are blocked by default; this is configurable.

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

Create **two** command triggers pointing to the same action:

| Command | Notes |
|---|---|
| `!shoutout` | Long form |
| `!so` | Short alias |

Recommended settings for both:
- **Global Cooldown**: 0 (script handles per-target cooldown)
- **Case Insensitive**: Yes
- If `ALLOW_NON_MODS = false`, you can also restrict at the command level to **Mods+**

### Required Twitch permissions

- Your Twitch bot/editor account needs **Editor** permission on the channel to fire the native `/shoutout` via `CPH.TwitchShoutoutUser()`
- If you don't have editor rights, remove or comment out that line

---

## Customization

| Value | Location | Purpose |
|---|---|---|
| `PER_TARGET_COOLDOWN_SECONDS` | Top of script | Minimum seconds before the same target can be shouted out again |
| `MSG_WITH_GAME` | Top of script | Message when the target has a last-played game set |
| `MSG_WITHOUT_GAME` | Top of script | Message when the target's category is empty |
| `ALLOW_NON_MODS` | Top of script | Set to `true` to allow all viewers to use `!so` |

---

## Repo Notes

Uses `CPH.TwitchGetExtendedUserInfoByLogin` for display name and last game. Per-target cooldown is stored as a non-persisted global (resets on Streamer.bot restart). Also calls `CPH.TwitchShoutoutUser` for the native Twitch shoutout feature.

---

## Video Notes

Worth highlighting:
- The difference between the chat message SO and the native Twitch `/shoutout` command
- How the `MSG_WITH_GAME` / `MSG_WITHOUT_GAME` fallback handles streamers with no category
- Why per-target cooldowns use globals keyed by login (`soCooldown_username`) vs. a single global
