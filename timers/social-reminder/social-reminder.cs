using System;
using System.Collections.Generic;

public class CPHInline
{
    // -------------------------------------------------------------------------
    // Configuration
    // -------------------------------------------------------------------------

    // Minimum number of chat messages that must have been sent since the last
    // reminder before the next one fires. Prevents posting into a dead chat.
    private const int MIN_CHAT_LINES_SINCE_LAST = 10;

    // Global variable that tracks the current rotation index
    private const string GLOBAL_INDEX_KEY       = "socialReminderIndex";

    // Global variable that stores the chat line count at the last reminder post
    private const string GLOBAL_LAST_CHAT_COUNT = "socialReminderLastChatCount";

    // Global variable maintained by your chat logger or a Chat Message event
    // that increments a counter each time someone sends a message.
    // See the utilities/chat-line-counter section for setup instructions.
    private const string GLOBAL_CHAT_COUNT      = "chatLineCount";

    // The messages to rotate through. Add, remove, or reorder as needed.
    // %index% and %total% are replaced with the current position for context.
    private static readonly string[] MESSAGES = new[]
    {
        "📺 Enjoying the stream? Hit that follow button so you never miss a live! → https://twitch.tv/YOURCHANNEL",
        "🎬 Catch up on past streams and tutorials on YouTube → https://youtube.com/@YOURCHANNEL",
        "💬 Join the community and hang out between streams → https://discord.gg/YOURINVITE",
        "🐦 Follow on Twitter/X for stream updates and highlights → https://twitter.com/YOURHANDLE",
    };

    // -------------------------------------------------------------------------

    public bool Execute()
    {
        if (MESSAGES.Length == 0)
        {
            CPH.LogWarn("[social-reminder] MESSAGES array is empty. Add at least one message.");
            return true;
        }

        // Activity check: only post if chat has been active
        int chatCount     = CPH.GetGlobalVar<int>(GLOBAL_CHAT_COUNT, false);
        int lastChatCount = CPH.GetGlobalVar<int>(GLOBAL_LAST_CHAT_COUNT, false);
        int linesSinceLast = chatCount - lastChatCount;

        if (linesSinceLast < MIN_CHAT_LINES_SINCE_LAST)
        {
            CPH.LogInfo("[social-reminder] Skipped — only " + linesSinceLast + " chat line(s) since last post (min: " + MIN_CHAT_LINES_SINCE_LAST + ").");
            return true;
        }

        // Advance rotation index (wraps around)
        int index = CPH.GetGlobalVar<int>(GLOBAL_INDEX_KEY, false);
        index = index % MESSAGES.Length;  // Clamp in case array was shortened

        string message = MESSAGES[index];

        // Update index for next run
        CPH.SetGlobalVar(GLOBAL_INDEX_KEY, (index + 1) % MESSAGES.Length, false);

        // Update the chat count baseline
        CPH.SetGlobalVar(GLOBAL_LAST_CHAT_COUNT, chatCount, false);

        CPH.SendMessage(message);
        return true;
    }
}
