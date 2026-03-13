# `!socials` — Social Media Links

## Summary

Posts a single formatted chat message with all configured social media links. Platforms with an empty URL are automatically hidden from output. Includes a global channel-wide cooldown enforced in code to prevent chat spam regardless of how the command is triggered (viewer command, timer, or moderator request).

---

## Code

See [`socials.cs`](socials.cs).

---

## Streamer.bot Setup

### Create the command action

1. Go to **Actions → Add**, name it `!socials`
2. Add a sub-action: **Core → Execute C# Code**
3. Paste `socials.cs` into the editor, **Compile**, then **Save**

### Create the command trigger

1. Go to **Commands → Add**
2. Command: `!socials`
3. Action: the action above
4. Recommended settings:
   - **Global Cooldown**: 60 seconds (the script also enforces 120s independently)
   - **Case Insensitive**: Yes
   - Allow: All viewers

### Optional: Reuse in a timer

This action can also be called from a timer sub-action (**Actions → Run Action**) for automatic posting every X minutes. The in-code cooldown prevents double-posting if both a viewer command and the timer fire close together.

---

## Customization

| Value | Location | Purpose |
|---|---|---|
| `TWITCH_URL` … `KICK_URL` | Top of script | Your actual social links — leave empty to hide |
| `PLATFORMS` array | Top of script | Order, labels, and emoji for each platform |
| `GLOBAL_COOLDOWN_SECONDS` | Top of script | Minimum seconds between posts (default: 120) |

---

## Repo Notes

All platform links are defined as constants at the top of the file. Empty strings are automatically excluded from output. Includes a soft global cooldown backed by a non-persisted global variable, independent of Streamer.bot's built-in cooldown.

---

## Video Notes

Worth highlighting:
- Configuring your links at the top vs. hardcoding them inline
- The "hide if empty" pattern using `string.IsNullOrWhiteSpace`
- The in-code cooldown and why it complements (not replaces) Streamer.bot's built-in cooldown
