using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;

public class CPHInline
{
    // -------------------------------------------------------------------------
    // Configuration
    // -------------------------------------------------------------------------

    // Per-user cooldown (seconds) before the same viewer can run !followage again.
    private const int PER_USER_COOLDOWN_SECONDS = 30;

    // Global variable name where the broadcaster's Twitch user ID is cached (Twitch only).
    private const string GLOBAL_BROADCASTER_ID = "broadcasterUserId";

    // Path to the cross-platform follow data file (relative to Streamer.bot executable).
    // This file is written by the New Follower event action and read here for YouTube/Kick.
    private static readonly string FOLLOW_DATA_FILE = Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory, "data", "klastic-follows.json");

    // Message when a non-Twitch user is not found in the follow data file.
    // Placeholders: %user%, %platform%
    private const string MSG_NOT_IN_STORE =
        "@%user% No follow data found for you on %platform%. You may have followed before the bot was active, or haven't followed yet.";

    // -------------------------------------------------------------------------

    public bool Execute()
    {
        string userName  = args.ContainsKey("userName")     ? args["userName"].ToString()     : null;
        string rawInput  = args.ContainsKey("rawInput")     ? args["rawInput"].ToString()     : null;
        string platform  = DetectPlatform();

        if (string.IsNullOrEmpty(userName))
        {
            CPH.LogWarn("[followage] userName arg is missing.");
            return false;
        }

        bool isMod   = args.ContainsKey("isModerator")  && args["isModerator"]?.ToString()  == "True";
        bool isBroad = args.ContainsKey("isBroadcaster") && args["isBroadcaster"]?.ToString() == "True";
        bool isBroadcasterOrMod = isMod || isBroad;

        // Mods/broadcaster can look up another user: !followage username
        string targetLogin;
        if (isBroadcasterOrMod && !string.IsNullOrWhiteSpace(rawInput))
            targetLogin = rawInput.Trim().TrimStart('@').ToLower();
        else
            targetLogin = userName.ToLower();

        // Per-user cooldown (non-mods only)
        if (!isBroadcasterOrMod)
        {
            string cooldownKey = "followageCooldown";
            string lastRunStr  = CPH.GetUserVar<string>(userName, cooldownKey, false);

            if (!string.IsNullOrEmpty(lastRunStr) &&
                DateTime.TryParse(lastRunStr, null, System.Globalization.DateTimeStyles.RoundtripKind, out DateTime lastRun))
            {
                double elapsed = (DateTime.UtcNow - lastRun).TotalSeconds;
                if (elapsed < PER_USER_COOLDOWN_SECONDS)
                {
                    int remaining = (int)(PER_USER_COOLDOWN_SECONDS - elapsed) + 1;
                    CPH.SendMessage("@" + userName + " You can use !followage again in " + remaining + "s.");
                    return true;
                }
            }

            CPH.SetUserVar(userName, cooldownKey, DateTime.UtcNow.ToString("O"), false);
        }

        // Route to appropriate lookup based on platform
        if (platform == "twitch")
            return HandleTwitch(userName, targetLogin, isBroadcasterOrMod);
        else
            return HandleNonTwitch(userName, targetLogin, platform);
    }

    // -------------------------------------------------------------------------
    // Twitch: use the live API for authoritative follow data
    // -------------------------------------------------------------------------

    private bool HandleTwitch(string callerName, string targetLogin, bool isBroadcasterOrMod)
    {
        string broadcastId = CPH.GetGlobalVar<string>(GLOBAL_BROADCASTER_ID, false);

        var targetUser = CPH.TwitchGetExtendedUserInfoByLogin(targetLogin);
        if (targetUser == null)
        {
            CPH.SendMessage("@" + callerName + " Could not find Twitch user '" + targetLogin + "'.");
            return true;
        }

        if (string.IsNullOrEmpty(broadcastId))
        {
            CPH.LogWarn("[followage] Global '" + GLOBAL_BROADCASTER_ID + "' is not set.");
            CPH.SendMessage("Follow-age lookup is not configured. Ask the streamer to set it up!");
            return true;
        }

        var followInfo = CPH.TwitchGetFollow(targetUser.UserId, broadcastId);
        if (followInfo == null)
        {
            string notFollowingLabel = isBroadcasterOrMod && targetLogin != callerName.ToLower()
                ? targetUser.UserName + " is"
                : "You are";
            CPH.SendMessage("@" + callerName + " " + notFollowingLabel + " not following this channel.");
            return true;
        }

        DateTime followedAtUtc = followInfo.FollowedAt.ToUniversalTime();
        string   ageStr        = FormatFollowAge(followedAtUtc);
        string   followDate    = followedAtUtc.ToString("MMMM d, yyyy");
        string   subjectLabel  = isBroadcasterOrMod && targetLogin != callerName.ToLower()
            ? targetUser.UserName + " has"
            : "You have";

        CPH.SendMessage("@" + callerName + " " + subjectLabel + " been following since " + followDate + " (" + ageStr + ").");
        return true;
    }

    // -------------------------------------------------------------------------
    // YouTube / Kick: look up the local follow data file
    // -------------------------------------------------------------------------

    private bool HandleNonTwitch(string callerName, string targetLogin, string platform)
    {
        string storeKey = platform + ":" + targetLogin;

        JsonObject store = LoadFollowStore();
        if (store == null || !store.ContainsKey(storeKey))
        {
            string msg = MSG_NOT_IN_STORE
                .Replace("%user%",     callerName)
                .Replace("%platform%", CapFirst(platform));
            CPH.SendMessage(msg);
            return true;
        }

        JsonNode entry = store[storeKey];
        string   displayName = entry["displayName"]?.GetValue<string>() ?? targetLogin;
        string   followedAtStr = entry["followedAt"]?.GetValue<string>() ?? null;

        if (DateTime.TryParse(followedAtStr, null, System.Globalization.DateTimeStyles.RoundtripKind, out DateTime followedAt))
        {
            string   ageStr     = FormatFollowAge(followedAt.ToUniversalTime());
            string   followDate = followedAt.ToUniversalTime().ToString("MMMM d, yyyy");

            string subjectLabel = targetLogin == callerName.ToLower()
                ? "You have"
                : displayName + " has";

            CPH.SendMessage("@" + callerName + " " + subjectLabel + " been following since " + followDate + " (" + ageStr + ").");
        }
        else
        {
            CPH.SendMessage("@" + callerName + " Follow data found but the date could not be read.");
        }

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
            if (p == "youtube")  return "youtube";
            if (p == "kick")     return "kick";
            if (p == "twitch")   return "twitch";
        }
        // Default: Twitch (most common setup)
        return "twitch";
    }

    private JsonObject LoadFollowStore()
    {
        try
        {
            if (!File.Exists(FOLLOW_DATA_FILE))
                return new JsonObject();

            string json = File.ReadAllText(FOLLOW_DATA_FILE);
            return JsonNode.Parse(json) as JsonObject ?? new JsonObject();
        }
        catch (Exception ex)
        {
            CPH.LogWarn("[followage] Failed to read follow store: " + ex.Message);
            return new JsonObject();
        }
    }

    private static string FormatFollowAge(DateTime followedAt)
    {
        DateTime now = DateTime.UtcNow;

        // Calendar-accurate: count complete years and months using actual date arithmetic
        int years = now.Year - followedAt.Year;
        if (now.Month < followedAt.Month || (now.Month == followedAt.Month && now.Day < followedAt.Day))
            years--;

        DateTime afterYears = followedAt.AddYears(years);
        int months = 0;
        while (afterYears.AddMonths(months + 1) <= now)
            months++;

        int days = (now - afterYears.AddMonths(months)).Days;

        if (years > 0 && months > 0) return years + " year" + Plural(years) + ", " + months + " month" + Plural(months);
        if (years > 0)               return years + " year" + Plural(years);
        if (months > 0 && days > 0)  return months + " month" + Plural(months) + ", " + days + " day" + Plural(days);
        if (months > 0)              return months + " month" + Plural(months);
        if (days > 0)                return days + " day" + Plural(days);
        return "less than a day";
    }

    private static string Plural(int n) => n == 1 ? "" : "s";

    private static string CapFirst(string s) =>
        string.IsNullOrEmpty(s) ? s : char.ToUpper(s[0]) + s.Substring(1);
}

