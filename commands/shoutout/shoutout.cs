using System;
using System.Collections.Generic;

public class CPHInline
{
    // -------------------------------------------------------------------------
    // Configuration
    // -------------------------------------------------------------------------

    // Minimum seconds before the same target can be shouted out again
    private const int PER_TARGET_COOLDOWN_SECONDS = 300;

    // Whether non-moderators can use this command (false = mods+ only)
    private const bool ALLOW_NON_MODS = false;

    // ---- Twitch message templates ----
    // Placeholders: %targetName%, %targetUrl%, %targetGame%
    private const string MSG_TWITCH_WITH_GAME    = "🎮 Go show some love to %targetName%! They were last seen playing %targetGame% → %targetUrl%";
    private const string MSG_TWITCH_WITHOUT_GAME = "🎮 Go show some love to %targetName%! → %targetUrl%";

    // ---- YouTube message template ----
    // Placeholders: %targetName%, %targetUrl%
    private const string MSG_YOUTUBE = "📺 Go check out %targetName% on YouTube! → %targetUrl%";

    // ---- Kick message template ----
    private const string MSG_KICK = "🟩 Go check out %targetName% on Kick! → %targetUrl%";

    // ---- Generic fallback (unknown or unsupported platform) ----
    private const string MSG_GENERIC = "❤️ Go show some love to %targetName%! → %targetUrl%";

    // Sent when the target cannot be found or a name was not provided
    private const string MSG_USAGE = "Usage: !so <username>";

    // -------------------------------------------------------------------------

    public bool Execute()
    {
        string callerName = args.ContainsKey("userName")      ? args["userName"].ToString()   : "unknown";
        bool   isMod      = args.ContainsKey("isModerator")   && args["isModerator"]?.ToString()  == "True";
        bool   isBroad    = args.ContainsKey("isBroadcaster") && args["isBroadcaster"]?.ToString() == "True";
        string rawInput   = args.ContainsKey("rawInput")      ? args["rawInput"]?.ToString()?.Trim() : null;
        string platform   = DetectPlatform();

        // Permission check
        if (!ALLOW_NON_MODS && !isMod && !isBroad)
            return true;  // Silently ignore

        if (string.IsNullOrWhiteSpace(rawInput))
        {
            CPH.SendMessage("@" + callerName + " " + MSG_USAGE);
            return true;
        }

        string targetLogin = rawInput.TrimStart('@').ToLower();

        // Per-target cooldown (shared across platforms — same key)
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

        // Stamp cooldown before sending to prevent race condition on double-trigger
        CPH.SetGlobalVar(cooldownKey, DateTime.UtcNow.ToString("O"), false);

        if (platform == "twitch")
            return HandleTwitchShoutout(callerName, rawInput, targetLogin);
        else
            return HandleNonTwitchShoutout(callerName, targetLogin, platform);
    }

    // -------------------------------------------------------------------------
    // Twitch: resolve channel info via API, fire native /shoutout
    // -------------------------------------------------------------------------

    private bool HandleTwitchShoutout(string callerName, string rawInput, string targetLogin)
    {
        var userInfo = CPH.TwitchGetExtendedUserInfoByLogin(targetLogin);
        if (userInfo == null)
        {
            CPH.SendMessage("@" + callerName + " Could not find Twitch channel '" + rawInput + "'. Check the spelling.");
            return true;
        }

        string targetName = userInfo.UserName;
        string targetUrl  = "https://twitch.tv/" + userInfo.UserLogin;
        string targetGame = userInfo.Game;

        string message = !string.IsNullOrWhiteSpace(targetGame)
            ? MSG_TWITCH_WITH_GAME
                .Replace("%targetName%", targetName)
                .Replace("%targetUrl%",  targetUrl)
                .Replace("%targetGame%", targetGame)
            : MSG_TWITCH_WITHOUT_GAME
                .Replace("%targetName%", targetName)
                .Replace("%targetUrl%",  targetUrl);

        CPH.SendMessage(message);

        // Fire Twitch's native /shoutout (requires editor permission)
        CPH.TwitchShoutoutUser(userInfo.UserId);

        return true;
    }

    // -------------------------------------------------------------------------
    // YouTube / Kick / other: best-effort URL from username, no API lookup
    // -------------------------------------------------------------------------

    private bool HandleNonTwitchShoutout(string callerName, string targetLogin, string platform)
    {
        // Build the best-effort channel URL from the provided username
        string targetUrl;
        string template;

        switch (platform)
        {
            case "youtube":
                // YouTube channels use @handle format; fall back to /c/name for legacy
                targetUrl = "https://youtube.com/@" + targetLogin;
                template  = MSG_YOUTUBE;
                break;
            case "kick":
                targetUrl = "https://kick.com/" + targetLogin;
                template  = MSG_KICK;
                break;
            default:
                targetUrl = targetLogin;   // Unknown platform — just use the name
                template  = MSG_GENERIC;
                break;
        }

        // Use provided rawInput display form for the target name (keep original casing)
        string displayName = args.ContainsKey("rawInput") ? args["rawInput"].ToString().TrimStart('@') : targetLogin;

        string message = template
            .Replace("%targetName%", displayName)
            .Replace("%targetUrl%",  targetUrl);

        CPH.SendMessage(message);
        return true;
    }

    // -------------------------------------------------------------------------
    // Helpers
    // -------------------------------------------------------------------------

    private string DetectPlatform()
    {
        if (args.ContainsKey("platform") && args["platform"] != null)
        {
            string p = args["platform"].ToString().ToLower();
            if (p == "youtube") return "youtube";
            if (p == "kick")    return "kick";
            if (p == "twitch")  return "twitch";
        }
        return "twitch";
    }
}

