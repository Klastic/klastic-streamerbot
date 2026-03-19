using System;
using System.Collections.Generic;

public class CPHInline
{
    // -------------------------------------------------------------------------
    // Configuration
    // -------------------------------------------------------------------------

    // Message sent to chat when a viewer uses !hydration.
    // %user% is replaced with the viewer's display name.
    // %streamer% is replaced with the broadcaster's display name.
    private const string MSG_HYDRATION = "@%streamer% 💧 %user% wants to make sure you're staying hydrated! Have you had some water recently? 🫗";

    // Global variable name holding the broadcaster's display name (set by stream start action)
    private const string GLOBAL_BROADCASTER_NAME = "broadcastUserName";

    // Cooldown in seconds between command uses (per user).
    // Recommended: 300 (5 minutes) to prevent spam.
    // Set to 0 to disable per-user cooldown (rely on Streamer.bot command cooldown instead).
    private const int COOLDOWN_SECONDS = 300;

    // Persistent user-variable key used to track the last time the command was used.
    private const string VAR_LAST_USED = "hydrationLastUsed";

    // -------------------------------------------------------------------------

    public bool Execute()
    {
        string userName = args.ContainsKey("userName") ? args["userName"].ToString() : null;
        string user     = args.ContainsKey("user")     ? args["user"].ToString()     : userName;

        if (string.IsNullOrEmpty(userName))
        {
            CPH.LogWarn("[hydration] Could not determine userName from args.");
            return false;
        }

        if (COOLDOWN_SECONDS > 0)
        {
            string lastUsedStr = CPH.GetUserVar<string>(userName, VAR_LAST_USED, true);
            if (!string.IsNullOrEmpty(lastUsedStr) &&
                DateTime.TryParse(lastUsedStr, null, System.Globalization.DateTimeStyles.RoundtripKind, out DateTime lastUsed))
            {
                double secondsElapsed = (DateTime.UtcNow - lastUsed.ToUniversalTime()).TotalSeconds;
                if (secondsElapsed < COOLDOWN_SECONDS)
                {
                    CPH.LogInfo("[hydration] " + userName + " is on cooldown (" + (int)(COOLDOWN_SECONDS - secondsElapsed) + "s remaining).");
                    return true;
                }
            }

            CPH.SetUserVar(userName, VAR_LAST_USED, DateTime.UtcNow.ToString("O"), true);
        }

        string broadcaster = CPH.GetGlobalVar<string>(GLOBAL_BROADCASTER_NAME, false);
        if (string.IsNullOrEmpty(broadcaster))
        {
            if (args.ContainsKey("broadcastUserName") && args["broadcastUserName"] != null)
                broadcaster = args["broadcastUserName"].ToString();
            else
                broadcaster = "streamer";
        }

        string message = MSG_HYDRATION
            .Replace("%user%", user)
            .Replace("%streamer%", broadcaster);

        CPH.SendMessage(message);
        return true;
    }
}
