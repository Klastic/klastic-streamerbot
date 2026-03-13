using System;
using System.Collections.Generic;

public class CPHInline
{
    // -------------------------------------------------------------------------
    // Configuration
    // -------------------------------------------------------------------------

    // Minimum seconds between shoutouts to the SAME target (prevents double-SO spam)
    private const int PER_TARGET_COOLDOWN_SECONDS = 300;

    // Channel URL template — the script appends the target login
    private const string TWITCH_URL_BASE = "https://twitch.tv/";

    // Message templates.
    // Available placeholders: %targetName%, %targetUrl%, %targetGame%
    // Use %targetGame% only if the target's last game should be included.
    private const string MSG_WITH_GAME    = "🎮 Go show some love to %targetName%! They were last seen playing %targetGame% → %targetUrl%";
    private const string MSG_WITHOUT_GAME = "🎮 Go show some love to %targetName%! → %targetUrl%";

    // Sent when the target user cannot be found on Twitch
    private const string MSG_USER_NOT_FOUND = "Could not find a Twitch channel for '%input%'. Check the spelling and try again.";

    // Whether non-moderators can use this command (set false to restrict to mods+)
    private const bool ALLOW_NON_MODS = false;

    // -------------------------------------------------------------------------

    public bool Execute()
    {
        string callerName = args.ContainsKey("userName")     ? args["userName"].ToString()   : "unknown";
        bool   isMod      = args.ContainsKey("isModerator")  && args["isModerator"]?.ToString()  == "True";
        bool   isBroad    = args.ContainsKey("isBroadcaster") && args["isBroadcaster"]?.ToString() == "True";
        string rawInput   = args.ContainsKey("rawInput")     ? args["rawInput"]?.ToString()?.Trim() : null;

        // Permission check
        if (!ALLOW_NON_MODS && !isMod && !isBroad)
        {
            // Silently ignore — don't tell chat that non-mods tried it
            return true;
        }

        if (string.IsNullOrWhiteSpace(rawInput))
        {
            CPH.SendMessage("@" + callerName + " Usage: !so <username>");
            return true;
        }

        string targetLogin = rawInput.TrimStart('@').ToLower();

        // Per-target cooldown check (stored as a non-persisted global)
        string cooldownKey = "soCooldown_" + targetLogin;
        string lastSOStr   = CPH.GetGlobalVar<string>(cooldownKey, false);
        if (!string.IsNullOrEmpty(lastSOStr) &&
            DateTime.TryParse(lastSOStr, null, System.Globalization.DateTimeStyles.RoundtripKind, out DateTime lastSO))
        {
            double elapsed = (DateTime.UtcNow - lastSO).TotalSeconds;
            if (elapsed < PER_TARGET_COOLDOWN_SECONDS)
            {
                int remaining = (int)(PER_TARGET_COOLDOWN_SECONDS - elapsed) + 1;
                CPH.SendMessage("@" + callerName + " " + targetLogin + " was already shouted out recently. Try again in " + remaining + "s.");
                return true;
            }
        }

        // Resolve Twitch user info
        var userInfo = CPH.TwitchGetExtendedUserInfoByLogin(targetLogin);
        if (userInfo == null)
        {
            string notFound = MSG_USER_NOT_FOUND.Replace("%input%", rawInput);
            CPH.SendMessage("@" + callerName + " " + notFound);
            return true;
        }

        // Stamp the cooldown before sending (prevents race conditions with double-triggers)
        CPH.SetGlobalVar(cooldownKey, DateTime.UtcNow.ToString("O"), false);

        string targetName = userInfo.UserName;
        string targetUrl  = TWITCH_URL_BASE + userInfo.UserLogin;
        string targetGame = userInfo.Game;   // May be empty if channel has no category set

        string message;
        if (!string.IsNullOrWhiteSpace(targetGame))
        {
            message = MSG_WITH_GAME
                .Replace("%targetName%", targetName)
                .Replace("%targetUrl%",  targetUrl)
                .Replace("%targetGame%", targetGame);
        }
        else
        {
            message = MSG_WITHOUT_GAME
                .Replace("%targetName%", targetName)
                .Replace("%targetUrl%",  targetUrl);
        }

        CPH.SendMessage(message);

        // Also trigger Twitch's native shoutout feature (requires editor permissions)
        CPH.TwitchShoutoutUser(userInfo.UserId);

        return true;
    }
}
