using System;
using System.Collections.Generic;
using System.Text.Json;

public class CPHInline
{
    // -------------------------------------------------------------------------
    // Configuration — edit these values to match your setup
    // -------------------------------------------------------------------------

    // OBS connection index. 0 = the first OBS instance linked in Streamer.bot.
    // Change if you have multiple OBS instances connected.
    private const int OBS_CONNECTION = 0;

    // Fallback: global variable name where stream start time is stored (UTC ISO-8601).
    // Set by the Stream Start action. Used only when OBS is not connected or not streaming.
    private const string GLOBAL_STREAM_START = "streamStartedAt";

    // Message sent when uptime cannot be determined from OBS or the fallback global.
    private const string MSG_NOT_LIVE = "Stream uptime is not available right now.";

    // -------------------------------------------------------------------------

    public bool Execute()
    {
        // Primary: pull stream time directly from OBS
        TimeSpan? obsUptime = GetOBSStreamTime();
        if (obsUptime.HasValue)
        {
            string broadcaster = GetBroadcasterName();
            CPH.SendMessage(broadcaster + " has been live for " + FormatUptime(obsUptime.Value) + ".");
            return true;
        }

        CPH.LogInfo("[uptime] OBS stream time not available, falling back to streamStartedAt global.");

        // Fallback: streamStartedAt global set by the Stream Start action
        string startTimeRaw = CPH.GetGlobalVar<string>(GLOBAL_STREAM_START, false);

        if (string.IsNullOrEmpty(startTimeRaw))
        {
            CPH.LogWarn("[uptime] Global '" + GLOBAL_STREAM_START + "' is also not set. Connect OBS or set up the Stream Start action.");
            CPH.SendMessage(MSG_NOT_LIVE);
            return true;
        }

        if (!DateTime.TryParse(startTimeRaw, null, System.Globalization.DateTimeStyles.RoundtripKind, out DateTime startTime))
        {
            CPH.LogWarn("[uptime] Could not parse '" + GLOBAL_STREAM_START + "' value: " + startTimeRaw);
            CPH.SendMessage(MSG_NOT_LIVE);
            return true;
        }

        TimeSpan uptime = DateTime.UtcNow - startTime.ToUniversalTime();

        if (uptime.TotalSeconds < 0)
        {
            CPH.LogWarn("[uptime] Uptime is negative — stale global from a previous session.");
            CPH.SendMessage(MSG_NOT_LIVE);
            return true;
        }

        string name = GetBroadcasterName();
        CPH.SendMessage(name + " has been live for " + FormatUptime(uptime) + ".");
        return true;
    }

    // Queries OBS WebSocket for the current stream status and extracts the uptime.
    // Returns null if OBS is not connected, not streaming, or returns an unexpected response.
    private TimeSpan? GetOBSStreamTime()
    {
        try
        {
            // Streamer.bot sends the request to OBS WebSocket v5 and returns responseData as JSON.
            string response = CPH.ObsSendRaw("GetStreamStatus", "{}", OBS_CONNECTION);

            if (string.IsNullOrWhiteSpace(response))
                return null;

            using JsonDocument doc = JsonDocument.Parse(response);
            JsonElement root = doc.RootElement;

            // The responseData may be at root level or nested under "responseData"
            JsonElement data = root;
            if (root.TryGetProperty("responseData", out JsonElement nestedData))
                data = nestedData;

            // Confirm OBS is actively streaming
            if (data.TryGetProperty("outputActive", out JsonElement activeEl) && !activeEl.GetBoolean())
            {
                CPH.LogInfo("[uptime] OBS is connected but not currently streaming.");
                return null;
            }

            // Prefer the timecode string: format is "HH:MM:SS.mmm" (e.g., "01:23:45.678")
            if (data.TryGetProperty("outputTimecode", out JsonElement timecodeEl))
            {
                string timecode = timecodeEl.GetString();
                if (!string.IsNullOrEmpty(timecode) && TimeSpan.TryParse(timecode, out TimeSpan ts))
                    return ts;
            }

            // Fallback: duration in milliseconds
            if (data.TryGetProperty("outputDuration", out JsonElement durationEl))
            {
                return TimeSpan.FromMilliseconds(durationEl.GetInt64());
            }
        }
        catch (Exception ex)
        {
            CPH.LogWarn("[uptime] OBS query failed: " + ex.Message);
        }

        return null;
    }

    private string GetBroadcasterName()
    {
        if (args.ContainsKey("broadcastUserName") && args["broadcastUserName"] != null)
            return args["broadcastUserName"].ToString();
        string stored = CPH.GetGlobalVar<string>("broadcastUserName", true);
        return !string.IsNullOrEmpty(stored) ? stored : "The stream";
    }

    private static string FormatUptime(TimeSpan span)
    {
        int hours   = (int)span.TotalHours;
        int minutes = span.Minutes;
        int seconds = span.Seconds;

        if (hours > 0)
            return hours + "h " + minutes + "m " + seconds + "s";
        if (minutes > 0)
            return minutes + "m " + seconds + "s";
        return seconds + "s";
    }
}
