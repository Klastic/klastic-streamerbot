using System;
using System.Collections.Generic;

public class CPHInline
{
    // -------------------------------------------------------------------------
    // Configuration
    // -------------------------------------------------------------------------

    // Message sent when a viewer enters lurk. Use %user% as a placeholder — the
    // script replaces it with the viewer's display name at runtime.
    private const string MSG_LURK   = "%user% is now lurking in the shadows. Thanks for the silent support! 👻";

    // Message sent when a viewer returns from lurk.
    private const string MSG_UNLURK = "👋 %user% has returned from the shadows! Welcome back!";

    // Message sent when !unlurk is used by someone who wasn't lurking.
    private const string MSG_NOT_LURKING = "@%user% You weren't in lurk mode, but welcome back anyway! 👋";

    // Persistent user-variable key used to store lurk state and timestamp.
    private const string VAR_LURK_TIME  = "lurkStartedAt";

    // Whether to show how long the viewer was lurking when they return.
    private const bool SHOW_LURK_DURATION = true;

    // -------------------------------------------------------------------------

    public bool Execute()
    {
        string command  = args.ContainsKey("command")      ? args["command"].ToString().ToLower()  : "";
        string userName = args.ContainsKey("userName")     ? args["userName"].ToString()            : null;
        string user     = args.ContainsKey("user")         ? args["user"].ToString()                : userName;

        if (string.IsNullOrEmpty(userName))
        {
            CPH.LogWarn("[lurk] Could not determine userName from args.");
            return false;
        }

        if (command == "!lurk")
        {
            HandleLurk(user, userName);
        }
        else if (command == "!unlurk")
        {
            HandleUnlurk(user, userName);
        }
        else
        {
            CPH.LogWarn("[lurk] Unexpected command value: " + command);
        }

        return true;
    }

    private void HandleLurk(string user, string userName)
    {
        // Record the lurk start time (overwrites any existing lurk entry)
        CPH.SetUserVar(userName, VAR_LURK_TIME, DateTime.UtcNow.ToString("O"), true);

        string msg = MSG_LURK.Replace("%user%", user);
        CPH.SendMessage(msg);
    }

    private void HandleUnlurk(string user, string userName)
    {
        string lurkStartStr = CPH.GetUserVar<string>(userName, VAR_LURK_TIME, true);

        if (string.IsNullOrEmpty(lurkStartStr))
        {
            // Viewer wasn't tracked as lurking
            string msg = MSG_NOT_LURKING.Replace("%user%", user);
            CPH.SendMessage(msg);
            return;
        }

        // Clear the lurk state
        CPH.UnsetUserVar(userName, VAR_LURK_TIME, true);

        string durationStr = "";
        if (SHOW_LURK_DURATION &&
            DateTime.TryParse(lurkStartStr, null, System.Globalization.DateTimeStyles.RoundtripKind, out DateTime lurkStart))
        {
            TimeSpan duration = DateTime.UtcNow - lurkStart.ToUniversalTime();
            if (duration.TotalSeconds >= 60)
                durationStr = " (lurked for " + FormatDuration(duration) + ")";
        }

        string returnMsg = MSG_UNLURK.Replace("%user%", user) + durationStr;
        CPH.SendMessage(returnMsg);
    }

    private static string FormatDuration(TimeSpan span)
    {
        int hours   = (int)span.TotalHours;
        int minutes = span.Minutes;

        if (hours > 0 && minutes > 0)
            return hours + "h " + minutes + "m";
        if (hours > 0)
            return hours + "h";
        return minutes + "m";
    }
}
