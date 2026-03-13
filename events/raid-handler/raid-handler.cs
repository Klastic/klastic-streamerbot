using System;
using System.Collections.Generic;

public class CPHInline
{
    // -------------------------------------------------------------------------
    // Configuration
    // -------------------------------------------------------------------------

    // Message sent when a raid arrives.
    // Placeholders: %raider% = raiding channel name, %count% = viewer count
    private const string MSG_SMALL_RAID  = "🚨 @%raider% is raiding with %count% viewers! Welcome everyone! ❤️";
    private const string MSG_MEDIUM_RAID = "🎉🎉 @%raider% is raiding with %count% viewers! Huge welcome to the raid squad! LET'S GOOO 🎉🎉";
    private const string MSG_LARGE_RAID  = "🔥🔥🔥 MASSIVE RAID! @%raider% brought %count% viewers! This is incredible — welcome to EVERYONE! 🔥🔥🔥";

    // Thresholds that determine which message to send.
    // count < MEDIUM_THRESHOLD  → small message
    // count < LARGE_THRESHOLD   → medium message
    // count >= LARGE_THRESHOLD  → large message
    private const int MEDIUM_RAID_THRESHOLD = 20;
    private const int LARGE_RAID_THRESHOLD  = 100;

    // Seconds to wait before sending the raid message (gives viewers time to arrive in chat)
    private const int DELAY_BEFORE_MESSAGE_MS = 3000;

    // Optional: fire a Twitch native shoutout to the raider automatically
    private const bool AUTO_SHOUTOUT = true;

    // Optional: set a specific OBS scene when a large raid arrives
    private const bool SWITCH_SCENE_ON_LARGE_RAID = false;
    private const string LARGE_RAID_SCENE          = "BRB Scene"; // only used if above is true

    // Dedup window — Twitch can fire a raid event twice for the same raider
    private const int DEDUP_WINDOW_SECONDS = 60;
    private const string DEDUP_KEY_PREFIX  = "raidDedup_";

    // -------------------------------------------------------------------------

    public bool Execute()
    {
        string raider      = args.ContainsKey("user")         ? args["user"].ToString()     : null;
        string raiderName  = args.ContainsKey("userName")     ? args["userName"].ToString() : raider;
        string raiderUserId = args.ContainsKey("userId")      ? args["userId"].ToString()   : null;
        int    viewerCount = 0;

        if (args.ContainsKey("viewers") && args["viewers"] != null)
            int.TryParse(args["viewers"].ToString(), out viewerCount);

        if (string.IsNullOrEmpty(raiderName))
        {
            CPH.LogWarn("[raid-handler] raider userName arg is missing.");
            return false;
        }

        // Deduplication
        string dedupKey    = DEDUP_KEY_PREFIX + raiderName.ToLower();
        string lastFireStr = CPH.GetGlobalVar<string>(dedupKey, false);
        if (!string.IsNullOrEmpty(lastFireStr) &&
            DateTime.TryParse(lastFireStr, null, System.Globalization.DateTimeStyles.RoundtripKind, out DateTime lastFire))
        {
            if ((DateTime.UtcNow - lastFire).TotalSeconds < DEDUP_WINDOW_SECONDS)
            {
                CPH.LogInfo("[raid-handler] Duplicate raid event from " + raiderName + " suppressed.");
                return true;
            }
        }
        CPH.SetGlobalVar(dedupKey, DateTime.UtcNow.ToString("O"), false);

        CPH.LogInfo("[raid-handler] Raid from " + raiderName + " with " + viewerCount + " viewers.");

        // Delay to let viewers settle in chat
        if (DELAY_BEFORE_MESSAGE_MS > 0)
            CPH.Wait(DELAY_BEFORE_MESSAGE_MS);

        // Choose message based on viewer count
        string template;
        if (viewerCount >= LARGE_RAID_THRESHOLD)
            template = MSG_LARGE_RAID;
        else if (viewerCount >= MEDIUM_RAID_THRESHOLD)
            template = MSG_MEDIUM_RAID;
        else
            template = MSG_SMALL_RAID;

        string displayRaider = !string.IsNullOrEmpty(raider) ? raider : raiderName;

        string message = template
            .Replace("%raider%", displayRaider)
            .Replace("%count%",  viewerCount.ToString());

        CPH.SendMessage(message);

        // Optional: native Twitch shoutout
        if (AUTO_SHOUTOUT && !string.IsNullOrEmpty(raiderUserId))
        {
            CPH.TwitchShoutoutUser(raiderUserId);
        }

        // Optional: OBS scene switch for large raids
        if (SWITCH_SCENE_ON_LARGE_RAID && viewerCount >= LARGE_RAID_THRESHOLD && !string.IsNullOrEmpty(LARGE_RAID_SCENE))
        {
            CPH.ObsSetScene(LARGE_RAID_SCENE);
        }

        return true;
    }
}
