using System;
using System.Collections.Generic;

public class CPHInline
{
    // -------------------------------------------------------------------------
    // Configuration — edit these values to match your setup
    // -------------------------------------------------------------------------

    // Global variable name where the stream start time is stored (UTC ISO-8601 string).
    // This must be set by your "Stream Online" event action using:
    //   Set Global Variable → streamStartedAt → %utcNow% (or equivalent)
    private const string GLOBAL_STREAM_START = "streamStartedAt";

    // Message sent when the stream start time has not been recorded.
    private const string MSG_NOT_LIVE = "Stream uptime is not available right now.";

    // -------------------------------------------------------------------------

    public bool Execute()
    {
        string startTimeRaw = CPH.GetGlobalVar<string>(GLOBAL_STREAM_START, false);

        if (string.IsNullOrEmpty(startTimeRaw))
        {
            CPH.LogWarn("[uptime] Global '" + GLOBAL_STREAM_START + "' is not set. Make sure your Stream Online event stores the start time.");
            CPH.SendMessage(MSG_NOT_LIVE);
            return true;
        }

        if (!DateTime.TryParse(startTimeRaw, null, System.Globalization.DateTimeStyles.RoundtripKind, out DateTime startTime))
        {
            CPH.LogWarn("[uptime] Could not parse value in global '" + GLOBAL_STREAM_START + "': " + startTimeRaw);
            CPH.SendMessage(MSG_NOT_LIVE);
            return true;
        }

        TimeSpan uptime = DateTime.UtcNow - startTime.ToUniversalTime();

        // Guard against negative uptime (e.g., stale global from a previous stream)
        if (uptime.TotalSeconds < 0)
        {
            CPH.LogWarn("[uptime] Calculated uptime is negative. The global may contain a stale value.");
            CPH.SendMessage(MSG_NOT_LIVE);
            return true;
        }

        string formatted = FormatUptime(uptime);

        // Prefer the broadcaster display name from args; fall back to "the stream"
        string broadcaster = "the stream";
        if (args.ContainsKey("broadcastUserName") && args["broadcastUserName"] != null)
            broadcaster = args["broadcastUserName"].ToString();

        CPH.SendMessage(broadcaster + " has been live for " + formatted + ".");
        return true;
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
