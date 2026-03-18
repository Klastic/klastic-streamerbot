using System;
using System.Collections.Generic;

public class CPHInline
{
    // -------------------------------------------------------------------------
    // Configuration
    // -------------------------------------------------------------------------

    // The reminder is sent to chat so viewers can encourage the streamer.
    // %streamer% is replaced with the broadcaster's display name.
    private const string MSG_HYDRATION = "💧 Hey %streamer%, have you had some water recently? Stay hydrated! 🫗";
    private const string MSG_BREAK     = "🧘 Stream break reminder: stretch, rest your eyes, and take a moment. You've got this! 💪";

    // Alternate between hydration and break messages each time the timer fires.
    // true  = current trigger is a hydration message
    // false = current trigger is a break message
    private const string GLOBAL_HYDRATION_TURN = "hydrationReminderTurn";

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
            CPH.LogInfo("[hydration-reminder] OBS is not streaming — skipping.");
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

        // Toggle between hydration and break messages
        bool isHydrationTurn = CPH.GetGlobalVar<bool>(GLOBAL_HYDRATION_TURN, false);
        CPH.SetGlobalVar(GLOBAL_HYDRATION_TURN, !isHydrationTurn, false);

        string message = isHydrationTurn
            ? MSG_HYDRATION.Replace("%streamer%", broadcaster)
            : MSG_BREAK.Replace("%streamer%", broadcaster);

        CPH.SendMessage(message);
        return true;
    }
}
