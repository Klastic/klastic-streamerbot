using System;
using System.Collections.Generic;

public class CPHInline
{
    // -------------------------------------------------------------------------
    // Configuration
    // -------------------------------------------------------------------------

    // Message posted in chat for a new follower.
    // Placeholders: %user% = follower display name, %followCount% = total follower count
    private const string MSG_NEW_FOLLOWER = "❤️ Thank you for the follow, %user%! Welcome to the community! (%followCount% followers)";

    // Minimum seconds between processing the same user again.
    // Twitch can fire the follow event more than once for the same user in rare cases.
    private const int DEDUP_WINDOW_SECONDS = 60;

    // Prefix used for dedup global keys: "followDedup_username"
    private const string DEDUP_KEY_PREFIX = "followDedup_";

    // Whether to announce follow count in the message (requires follower count arg)
    private const bool SHOW_FOLLOW_COUNT = true;

    // -------------------------------------------------------------------------

    public bool Execute()
    {
        string user     = args.ContainsKey("user")         ? args["user"].ToString()     : null;
        string userName = args.ContainsKey("userName")     ? args["userName"].ToString() : null;

        if (string.IsNullOrEmpty(userName))
        {
            CPH.LogWarn("[new-follower] userName arg is missing. Check event trigger configuration.");
            return false;
        }

        // Deduplication: guard against Twitch firing the event twice for the same user
        string dedupKey   = DEDUP_KEY_PREFIX + userName.ToLower();
        string lastFireStr = CPH.GetGlobalVar<string>(dedupKey, false);

        if (!string.IsNullOrEmpty(lastFireStr) &&
            DateTime.TryParse(lastFireStr, null, System.Globalization.DateTimeStyles.RoundtripKind, out DateTime lastFire))
        {
            if ((DateTime.UtcNow - lastFire).TotalSeconds < DEDUP_WINDOW_SECONDS)
            {
                CPH.LogInfo("[new-follower] Skipping duplicate follow event for " + userName);
                return true;
            }
        }

        CPH.SetGlobalVar(dedupKey, DateTime.UtcNow.ToString("O"), false);

        // Build the message
        string displayName = !string.IsNullOrEmpty(user) ? user : userName;
        string message     = MSG_NEW_FOLLOWER.Replace("%user%", displayName);

        if (SHOW_FOLLOW_COUNT)
        {
            string countStr = args.ContainsKey("followCount") ? args["followCount"].ToString() : "";
            if (!string.IsNullOrEmpty(countStr))
                message = message.Replace("%followCount%", countStr);
            else
                message = message.Replace(" (%followCount% followers)", "").Replace("(%followCount% followers)", "");
        }
        else
        {
            message = message.Replace(" (%followCount% followers)", "").Replace("(%followCount% followers)", "");
        }

        CPH.SendMessage(message);
        return true;
    }
}
