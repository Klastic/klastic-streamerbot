# `!uptime` — Stream Uptime Display

## Summary

Displays how long the current stream has been live. Pulls the stream time **directly from OBS** via the OBS WebSocket integration in Streamer.bot (primary), and falls back to a `streamStartedAt` global variable if OBS is not connected or not streaming.

Works on Twitch, YouTube, and Kick — the source is OBS, not a platform API, so it's fully platform-agnostic.

Handles edge cases: OBS not connected, OBS connected but not streaming, missing fallback global, malformed timestamp, and stale values from a previous session.

---

## Code

See [`uptime.cs`](uptime.cs).

---

## Requirements

- **OBS WebSocket** connected in Streamer.bot (preferred — used as primary source)
  - In Streamer.bot, go to **Settings → OBS WebSocket** and connect
- **Stream Start action** sets `streamStartedAt` global (used as fallback when OBS is not available)

---

## Streamer.bot Setup

### Step 1 — Connect OBS (required for primary source)

1. In Streamer.bot go to **Settings → OBS WebSocket**
2. Configure the host/port (default: `localhost:4455`) and click **Connect**
3. Confirm the connection status shows as connected

### Step 2 — Set up the fallback (optional but recommended)

The `streamStartedAt` global from the [`stream-start`](../../events/stream-start/) action acts as a fallback when OBS is unavailable.

### Step 3 — Create the command action

1. Go to **Actions → Add**, name it `!uptime`
2. Add a sub-action: **Core → Execute C# Code**
3. Paste the contents of `uptime.cs` into the editor
4. Click **Compile**, then **Save**

### Step 4 — Create the command trigger

1. Go to **Commands → Add**
2. Set the command to `!uptime`
3. Set the **Action** to the action created above
4. Recommended settings:
   - **Global Cooldown**: 5–10 seconds (prevents spam)
   - **Case Insensitive**: Yes
5. Add triggers for each platform you stream on (Twitch Chat Message, YouTube Chat Message, Kick Chat Message)

---

## Customization

| Value | Location | Purpose |
|---|---|---|
| `OBS_CONNECTION` | Top of script | OBS connection index (0 = first OBS instance in Streamer.bot) |
| `GLOBAL_STREAM_START` | Top of script | Name of the fallback global variable |
| `MSG_NOT_LIVE` | Top of script | Message sent when uptime cannot be determined |

---

## Platform Support

| Platform | Source | Notes |
|---|---|---|
| Twitch | OBS → global fallback | Fully supported |
| YouTube | OBS → global fallback | Fully supported |
| Kick | OBS → global fallback | Fully supported |

---

## Repo Notes

OBS-primary uptime command. Uses `CPH.ObsSendRaw("GetStreamStatus")` to get the live stream timecode from OBS WebSocket v5. Falls back to the `streamStartedAt` global if OBS is unavailable. Platform-agnostic.

---

## Video Notes

Worth highlighting:
- Why OBS is a better source than a manually-stored global (OBS tracks the exact stream start internally)
- The `CPH.ObsSendRaw("GetStreamStatus")` call and parsing the `outputTimecode` field
- The fallback chain: OBS → global → "unavailable" message
- The `responseData` nesting behavior difference between OBS WebSocket versions

