using System;
using System.Collections.Generic;

public class CPHInline
{
    // -------------------------------------------------------------------------
    // Configuration
    // -------------------------------------------------------------------------

    // Message sent to chat when the break timer fires.
    // %streamer% is replaced with the broadcaster's display name.
    private const string MSG_BREAK = "Hey %streamer%, good time for a quick break — stretch and rest your eyes for a moment.";

    // Global variable name holding the broadcaster's display name (set by stream start action)
    private const string GLOBAL_BROADCASTER_NAME = "broadcastUserName";

    // Set to true to only send reminders while OBS reports streaming is active.
    // Requires OBS to be connected to Streamer.bot via the OBS WebSocket plugin.
    private const bool CHECK_OBS_STREAMING = true;

    // -------------------------------------------------------------------------

    public bool Execute()
    {
        if (CHECK_OBS_STREAMING && !CPH.ObsIsStreaming())
        {
            CPH.LogInfo("[break-reminder] OBS is not streaming — skipping.");
            return true;
        }

        string broadcaster = CPH.GetGlobalVar<string>(GLOBAL_BROADCASTER_NAME, false);
        if (string.IsNullOrEmpty(broadcaster))
        {
            // Fall back to args if the global isn't set
            if (args.ContainsKey("broadcastUserName") && args["broadcastUserName"] != null)
                broadcaster = args["broadcastUserName"].ToString();
            else
                broadcaster = "streamer";
        }

        CPH.SendMessage(MSG_BREAK.Replace("%streamer%", broadcaster));
        return true;
    }
}
