using System;
using System.Collections.Generic;

public class CPHInline
{
    // -------------------------------------------------------------------------
    // Configuration
    // -------------------------------------------------------------------------

    // Optional: message sent to chat when the stream ends
    private const string MSG_STREAM_OFFLINE = "Stream is ending — thanks for hanging out! See you next time! 👋";

    // Set to true to post the offline message before ending
    private const bool ANNOUNCE_IN_CHAT = true;

    // Session globals to clear on stream end (non-persisted, but clearing explicitly
    // ensures they don't bleed into the next session if Streamer.bot doesn't restart)
    private static readonly string[] SESSION_GLOBALS_TO_CLEAR = new[]
    {
        "streamStartedAt",
        "chatLineCount",
        "socialReminderIndex",
        "socialReminderLastChatCount",
        "hydrationReminderTurn",
    };

    // -------------------------------------------------------------------------

    public bool Execute()
    {
        // Post the offline announcement before clearing state
        if (ANNOUNCE_IN_CHAT && !string.IsNullOrEmpty(MSG_STREAM_OFFLINE))
        {
            CPH.SendMessage(MSG_STREAM_OFFLINE);

            // Brief delay so the message posts before any further actions
            CPH.Wait(1500);
        }

        // Clear session-scoped globals so they don't interfere with the next stream
        foreach (string key in SESSION_GLOBALS_TO_CLEAR)
        {
            CPH.UnsetGlobalVar(key, false);
            CPH.LogInfo("[stream-end] Cleared global: " + key);
        }

        CPH.LogInfo("[stream-end] Stream-end cleanup complete at " + DateTime.UtcNow.ToString("O"));
        return true;
    }
}
