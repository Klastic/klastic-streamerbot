using System;
using System.Collections.Generic;

public class CPHInline
{
    // -------------------------------------------------------------------------
    // Configuration
    // -------------------------------------------------------------------------

    // Cooldown in seconds between a single user running this command again.
    // This is enforced in code as a per-user variable so it survives even if
    // the Streamer.bot command cooldown is misconfigured.
    private const int PER_USER_COOLDOWN_SECONDS = 30;

    // Global variable name where the broadcaster's Twitch user ID is cached.
    // Set this once in your stream-start action using the broadcasterUserId arg.
    private const string GLOBAL_BROADCASTER_ID = "broadcasterUserId";

    // -------------------------------------------------------------------------

    public bool Execute()
    {
        string userName    = args.ContainsKey("userName")    ? args["userName"].ToString()    : null;
        string userId      = args.ContainsKey("userId")      ? args["userId"].ToString()      : null;
        string rawInput    = args.ContainsKey("rawInput")    ? args["rawInput"].ToString()    : null;
        string broadcastId = CPH.GetGlobalVar<string>(GLOBAL_BROADCASTER_ID, false);

        // Allow mod/broadcaster to look up a different user: !followage username
        string targetLogin = null;
        bool isBroadcasterOrMod = false;

        if (args.ContainsKey("isSubscriber"))  // rough guard; adjust as needed
            isBroadcasterOrMod = false;
        if (args.ContainsKey("isModerator") && args["isModerator"]?.ToString() == "True")
            isBroadcasterOrMod = true;
        if (args.ContainsKey("isBroadcaster") && args["isBroadcaster"]?.ToString() == "True")
            isBroadcasterOrMod = true;

        if (isBroadcasterOrMod && !string.IsNullOrWhiteSpace(rawInput))
        {
            // Broadcaster or mod is looking up someone else
            targetLogin = rawInput.Trim().TrimStart('@').ToLower();
        }
        else
        {
            // Regular viewer — per-user cooldown check
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
            targetLogin = userName?.ToLower();
        }

        if (string.IsNullOrEmpty(targetLogin))
        {
            CPH.SendMessage("Could not determine target user for !followage.");
            return true;
        }

        // Resolve the target user's Twitch info
        var targetUser = CPH.TwitchGetExtendedUserInfoByLogin(targetLogin);
        if (targetUser == null)
        {
            CPH.SendMessage("@" + userName + " Could not find Twitch user '" + targetLogin + "'.");
            return true;
        }

        if (string.IsNullOrEmpty(broadcastId))
        {
            CPH.LogWarn("[followage] Global '" + GLOBAL_BROADCASTER_ID + "' is not set. Set it in your Stream Online action.");
            CPH.SendMessage("Follow-age lookup is not configured yet. Ask the streamer to set it up!");
            return true;
        }

        // Check follow status using the Twitch Helix follow endpoint via Streamer.bot's built-in wrapper
        var followInfo = CPH.TwitchGetFollow(targetUser.UserId, broadcastId);
        if (followInfo == null)
        {
            string label = isBroadcasterOrMod && targetLogin != userName?.ToLower()
                ? targetUser.UserName + " is"
                : "You are";
            CPH.SendMessage("@" + userName + " " + label + " not following this channel.");
            return true;
        }

        TimeSpan age       = DateTime.UtcNow - followInfo.FollowedAt.ToUniversalTime();
        string   ageStr    = FormatFollowAge(age);
        string   followDate = followInfo.FollowedAt.ToString("MMMM d, yyyy");

        string displayName = isBroadcasterOrMod && targetLogin != userName?.ToLower()
            ? targetUser.UserName + " has"
            : "You have";

        CPH.SendMessage("@" + userName + " " + displayName + " been following since " + followDate + " (" + ageStr + ").");
        return true;
    }

    private static string FormatFollowAge(TimeSpan span)
    {
        int totalDays  = (int)span.TotalDays;
        int years      = totalDays / 365;
        int months     = (totalDays % 365) / 30;
        int days       = totalDays % 30;

        if (years > 0 && months > 0)
            return years + " year" + Plural(years) + ", " + months + " month" + Plural(months);
        if (years > 0)
            return years + " year" + Plural(years);
        if (months > 0 && days > 0)
            return months + " month" + Plural(months) + ", " + days + " day" + Plural(days);
        if (months > 0)
            return months + " month" + Plural(months);
        if (totalDays > 0)
            return totalDays + " day" + Plural(totalDays);
        return "less than a day";
    }

    private static string Plural(int n) => n == 1 ? "" : "s";
}
