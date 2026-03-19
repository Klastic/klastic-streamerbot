using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;

public class CPHInline
{
    // -------------------------------------------------------------------------
    // Configuration
    // -------------------------------------------------------------------------

    // Per-user cooldown (seconds) before the same viewer can run !followage again.
    private const int PER_USER_COOLDOWN_SECONDS = 30;

    // Path to the cross-platform follow data file (relative to Streamer.bot executable).
    // This file is written by the New Follower event action and read here.
    private static readonly string FOLLOW_DATA_FILE = Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory, "data", "klastic-follows.json");

    // Message when a user is not found in the follow data file.
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
            string cooldownKey = "followageCooldown_" + platform + "_" + userName.ToLower();
            string lastRunStr  = CPH.GetGlobalVar<string>(cooldownKey, false);

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

            CPH.SetGlobalVar(cooldownKey, DateTime.UtcNow.ToString("O"), false);
        }

        // Route to appropriate lookup based on platform
        if (platform == "twitch")
            return HandleTwitch(userName, targetLogin, isBroadcasterOrMod);
        else
            return HandleNonTwitch(userName, targetLogin, platform);
    }

    // -------------------------------------------------------------------------
    // Twitch: read follow age data from args populated by the
    // "Get Follow Age Info for Target" sub-action (must run before this script)
    // -------------------------------------------------------------------------

    private bool HandleTwitch(string callerName, string targetLogin, bool isBroadcasterOrMod)
    {
        // isFollowing is set by the "Get Follow Age Info for Target" sub-action
        if (!args.ContainsKey("isFollowing"))
        {
            CPH.LogWarn("[followage] Twitch follow age info not found in args. Ensure the 'Get Follow Age Info for Target' sub-action runs before this script.");
            CPH.SendMessage("@" + callerName + " Could not retrieve follow info — action is misconfigured.");
            return true;
        }

        bool isFollowing = args["isFollowing"] is bool boolVal
            ? boolVal
            : string.Equals(args["isFollowing"]?.ToString(), "true", StringComparison.OrdinalIgnoreCase);

        string callerNameLower = callerName.ToLower();
        bool lookingUpOther = isBroadcasterOrMod && !string.IsNullOrEmpty(targetLogin)
                              && targetLogin != callerNameLower;
        string displayName  = args.ContainsKey("followUser") ? args["followUser"].ToString() : targetLogin;

        if (!isFollowing)
        {
            string notFollowingLabel = lookingUpOther ? displayName + " is" : "You are";
            CPH.SendMessage("@" + callerName + " " + notFollowingLabel + " not following this channel.");
            return true;
        }

        string followDate    = args.ContainsKey("followDate")    ? args["followDate"].ToString()    : "unknown date";
        string followAgeLong = args.ContainsKey("followAgeLong") ? args["followAgeLong"].ToString() : null;
        string subjectLabel  = lookingUpOther ? displayName + " has" : "You have";

        string agePart = !string.IsNullOrEmpty(followAgeLong) ? " (" + followAgeLong + ")" : "";
        CPH.SendMessage("@" + callerName + " " + subjectLabel + " been following since " + followDate + agePart + ".");
        return true;
    }

    // -------------------------------------------------------------------------
    // YouTube / Kick: look up the local follow data file
    // -------------------------------------------------------------------------

    private bool HandleNonTwitch(string callerName, string targetLogin, string platform)
    {
        string storeKey = platform + ":" + targetLogin;

        JObject store = LoadFollowStore();
        if (store == null || !store.ContainsKey(storeKey))
        {
            string msg = MSG_NOT_IN_STORE
                .Replace("%user%",     callerName)
                .Replace("%platform%", CapFirst(platform));
            CPH.SendMessage(msg);
            return true;
        }

        JToken entry = store[storeKey];
        string   displayName = entry["displayName"]?.Value<string>() ?? targetLogin;
        string   followedAtStr = entry["followedAt"]?.Value<string>() ?? null;

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

    private JObject LoadFollowStore()
    {
        try
        {
            if (!File.Exists(FOLLOW_DATA_FILE))
                return new JObject();

            string json = File.ReadAllText(FOLLOW_DATA_FILE);
            return JToken.Parse(json) as JObject ?? new JObject();
        }
        catch (Exception ex)
        {
            CPH.LogWarn("[followage] Failed to read follow store: " + ex.Message);
            return new JObject();
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

