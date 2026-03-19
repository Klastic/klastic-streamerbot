using System;
using System.Collections.Generic;

public class CPHInline
{
    // -------------------------------------------------------------------------
    // Configuration
    // -------------------------------------------------------------------------

    // Global variable that tracks the current rotation index
    private const string GLOBAL_INDEX_KEY = "socialReminderIndex";

    // The messages to rotate through. Add, remove, or reorder as needed.
    // %index% and %total% are replaced with the current position for context.
    private static readonly string[] MESSAGES = new[]
    {
        "📺 Enjoying the stream? Hit that follow button so you never miss a live! → https://twitch.tv/YOURCHANNEL",
        "🎬 Catch up on past streams and tutorials on YouTube → https://youtube.com/@YOURCHANNEL",
        "💬 Join the community and hang out between streams → https://discord.gg/YOURINVITE",
        "🐦 Follow on Twitter/X for stream updates and highlights → https://twitter.com/YOURHANDLE",
    };

    // Set to true to only send reminders while OBS reports streaming is active.
    // Requires OBS to be connected to Streamer.bot via the OBS WebSocket plugin.
    private const bool CHECK_OBS_STREAMING = true;

    // -------------------------------------------------------------------------

    public bool Execute()
    {
        if (CHECK_OBS_STREAMING && !CPH.ObsIsStreaming())
        {
            CPH.LogInfo("[social-reminder] OBS is not streaming — skipping.");
            return true;
        }

        if (MESSAGES.Length == 0)
        {
            CPH.LogWarn("[social-reminder] MESSAGES array is empty. Add at least one message.");
            return true;
        }

        // Advance rotation index (wraps around)
        int index = CPH.GetGlobalVar<int>(GLOBAL_INDEX_KEY, false);
        index = index % MESSAGES.Length;  // Clamp in case array was shortened

        string message = MESSAGES[index];

        // Update index for next run
        CPH.SetGlobalVar(GLOBAL_INDEX_KEY, (index + 1) % MESSAGES.Length, false);

        CPH.SendMessage(message);
        return true;
    }
}
