using System;
using System.Collections.Generic;

public class CPHInline
{
    // -------------------------------------------------------------------------
    // Configuration
    // -------------------------------------------------------------------------

    // Optional: message sent to chat when the stream goes live
    private const string MSG_STREAM_ONLINE = "🔴 We are LIVE! Welcome everyone — let's get started! 👋";

    // Set to true to post the online message to chat automatically
    private const bool ANNOUNCE_IN_CHAT = true;

    // Global variable names written by this action and read by other scripts
    private const string GLOBAL_STREAM_START    = "streamStartedAt";       // read by !uptime
    private const string GLOBAL_BROADCASTER_ID  = "broadcasterUserId";     // read by !followage
    private const string GLOBAL_BROADCASTER_NAME = "broadcastUserName";    // read by timers and events

    // Chat line counter global — reset to 0 each stream for the social reminder activity guard
    private const string GLOBAL_CHAT_COUNT       = "chatLineCount";

    // -------------------------------------------------------------------------

    public bool Execute()
    {
        // Record stream start time in UTC ISO-8601 format
        string nowStr = DateTime.UtcNow.ToString("O");
        CPH.SetGlobalVar(GLOBAL_STREAM_START, nowStr, false);
        CPH.LogInfo("[stream-start] streamStartedAt set to " + nowStr);

        // Cache broadcaster identity (used by multiple scripts)
        if (args.ContainsKey("broadcastUserId") && args["broadcastUserId"] != null)
        {
            string broadcasterId = args["broadcastUserId"].ToString();
            CPH.SetGlobalVar(GLOBAL_BROADCASTER_ID, broadcasterId, true);
            CPH.LogInfo("[stream-start] broadcasterUserId set to " + broadcasterId);
        }

        if (args.ContainsKey("broadcastUserName") && args["broadcastUserName"] != null)
        {
            string broadcasterName = args["broadcastUserName"].ToString();
            CPH.SetGlobalVar(GLOBAL_BROADCASTER_NAME, broadcasterName, true);
            CPH.LogInfo("[stream-start] broadcastUserName set to " + broadcasterName);
        }

        // Reset the chat line counter for the activity guard used in social-reminder
        CPH.SetGlobalVar(GLOBAL_CHAT_COUNT, 0, false);

        // Optional: announce in chat
        if (ANNOUNCE_IN_CHAT && !string.IsNullOrEmpty(MSG_STREAM_ONLINE))
        {
            CPH.SendMessage(MSG_STREAM_ONLINE);
        }

        return true;
    }
}
