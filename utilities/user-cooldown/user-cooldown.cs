using System;
using System.Collections.Generic;

// ---------------------------------------------------------------------------
// Per-User Cooldown Helper
//
// This script is designed to be called as a sub-action BEFORE your main
// command logic. It sets an output argument "cooldownPassed" to "true" or
// "false". Your subsequent sub-actions should be gated on cooldownPassed == "true"
// using Streamer.bot's "Run Action If" or a condition group.
//
// Alternatively, embed the CheckCooldown / SetCooldown methods directly
// into any other script that needs per-user rate limiting.
// ---------------------------------------------------------------------------

public class CPHInline
{
    // -------------------------------------------------------------------------
    // Configuration
    // -------------------------------------------------------------------------

    // The cooldown duration in seconds.
    // Override this by setting a "cooldownSeconds" argument before calling this action,
    // or hardcode your desired value here.
    private const int DEFAULT_COOLDOWN_SECONDS = 30;

    // The key name used to namespace cooldowns for this specific command.
    // Set a "cooldownKey" arg to override this at runtime (e.g., "uptime", "followage").
    private const string DEFAULT_COOLDOWN_KEY = "genericCommand";

    // Name of the output argument set by this script for downstream sub-actions.
    private const string OUTPUT_ARG = "cooldownPassed";

    // -------------------------------------------------------------------------

    public bool Execute()
    {
        string userName = args.ContainsKey("userName") ? args["userName"].ToString() : null;

        if (string.IsNullOrEmpty(userName))
        {
            CPH.LogWarn("[user-cooldown] userName arg is missing — cooldown check skipped.");
            CPH.SetArgument(OUTPUT_ARG, "true");
            return true;
        }

        // Allow runtime override of cooldown duration
        int cooldownSeconds = DEFAULT_COOLDOWN_SECONDS;
        if (args.ContainsKey("cooldownSeconds") &&
            int.TryParse(args["cooldownSeconds"].ToString(), out int overrideSecs) &&
            overrideSecs > 0)
        {
            cooldownSeconds = overrideSecs;
        }

        // Allow runtime override of the cooldown key (e.g., the command name)
        string cooldownKey = DEFAULT_COOLDOWN_KEY;
        if (args.ContainsKey("cooldownKey") && !string.IsNullOrEmpty(args["cooldownKey"]?.ToString()))
        {
            cooldownKey = args["cooldownKey"].ToString();
        }

        string varName    = "cooldown_" + cooldownKey;
        string lastRunStr = CPH.GetUserVar<string>(userName, varName, false);

        if (!string.IsNullOrEmpty(lastRunStr) &&
            DateTime.TryParse(lastRunStr, null, System.Globalization.DateTimeStyles.RoundtripKind, out DateTime lastRun))
        {
            double elapsed = (DateTime.UtcNow - lastRun).TotalSeconds;

            if (elapsed < cooldownSeconds)
            {
                int remaining = (int)(cooldownSeconds - elapsed) + 1;
                CPH.LogInfo("[user-cooldown] " + userName + " on cooldown for '" + cooldownKey + "' (" + remaining + "s remaining)");
                CPH.SetArgument(OUTPUT_ARG, "false");
                CPH.SetArgument("cooldownRemaining", remaining.ToString());
                return true;
            }
        }

        // Cooldown has passed — stamp the time and allow the action to continue
        CPH.SetUserVar(userName, varName, DateTime.UtcNow.ToString("O"), false);
        CPH.SetArgument(OUTPUT_ARG, "true");
        CPH.SetArgument("cooldownRemaining", "0");
        return true;
    }
}
