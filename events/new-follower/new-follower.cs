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

    // Message posted in chat for a new follower.
    // Placeholders: %user% = follower display name, %followCount% = total follower count
    private const string MSG_NEW_FOLLOWER = "❤️ Thank you for the follow, %user%! Welcome to the community! (%followCount% followers)";

    // Minimum seconds between processing the same user again.
    // Platforms can fire the follow event more than once for the same user in rare cases.
    private const int DEDUP_WINDOW_SECONDS = 60;

    // Prefix used for dedup global keys: "followDedup_platform_username"
    private const string DEDUP_KEY_PREFIX = "followDedup_";

    // Whether to announce follow count in the message (requires followCount arg; Twitch only)
    private const bool SHOW_FOLLOW_COUNT = true;

    // Path to the cross-platform follow data file (relative to Streamer.bot executable).
    // Used by !followage for YouTube and Kick lookups.
    private static readonly string FOLLOW_DATA_FILE = Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory, "data", "klastic-follows.json");

    // -------------------------------------------------------------------------

    public bool Execute()
    {
        string user     = args.ContainsKey("user")      ? args["user"].ToString()      : null;
        string userName = args.ContainsKey("userName")  ? args["userName"].ToString()  : null;
        string platform = DetectPlatform();

        if (string.IsNullOrEmpty(userName))
        {
            CPH.LogWarn("[new-follower] userName arg is missing. Check event trigger configuration.");
            return false;
        }

        // Deduplication: guard against the same follow event firing twice
        string dedupKey   = DEDUP_KEY_PREFIX + platform + "_" + userName.ToLower();
        string lastFireStr = CPH.GetGlobalVar<string>(dedupKey, false);

        if (!string.IsNullOrEmpty(lastFireStr) &&
            DateTime.TryParse(lastFireStr, null, System.Globalization.DateTimeStyles.RoundtripKind, out DateTime lastFire))
        {
            if ((DateTime.UtcNow - lastFire).TotalSeconds < DEDUP_WINDOW_SECONDS)
            {
                CPH.LogInfo("[new-follower] Skipping duplicate follow event for " + userName + " on " + platform);
                return true;
            }
        }

        CPH.SetGlobalVar(dedupKey, DateTime.UtcNow.ToString("O"), false);

        string displayName = !string.IsNullOrEmpty(user) ? user : userName;

        // Write follow data to JSON store — enables !followage for YouTube and Kick
        WriteFollowToStore(userName.ToLower(), displayName, platform);

        // Build and send the chat message
        string message = MSG_NEW_FOLLOWER.Replace("%user%", displayName);

        if (SHOW_FOLLOW_COUNT && platform == "twitch")
        {
            string countStr = args.ContainsKey("followCount") ? args["followCount"].ToString() : "";
            if (!string.IsNullOrEmpty(countStr))
                message = message.Replace("%followCount%", countStr);
            else
                message = StripFollowCount(message);
        }
        else
        {
            message = StripFollowCount(message);
        }

        CPH.SendMessage(message);
        return true;
    }

    // -------------------------------------------------------------------------
    // Helpers
    // -------------------------------------------------------------------------

    private void WriteFollowToStore(string userNameLower, string displayName, string platform)
    {
        try
        {
            string dir = Path.GetDirectoryName(FOLLOW_DATA_FILE);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            JsonObject store = new JsonObject();

            // Load existing data
            if (File.Exists(FOLLOW_DATA_FILE))
            {
                try
                {
                    string existing = File.ReadAllText(FOLLOW_DATA_FILE);
                    store = JsonNode.Parse(existing) as JsonObject ?? new JsonObject();
                }
                catch (Exception ex)
                {
                    CPH.LogWarn("[new-follower] Could not read existing follow store (" + ex.Message + ") — starting fresh.");
                    store = new JsonObject();
                }
            }

            string key = platform + ":" + userNameLower;

            // Only write if not already recorded (preserves the original follow date)
            if (!store.ContainsKey(key))
            {
                store[key] = new JsonObject
                {
                    ["userName"]    = userNameLower,
                    ["displayName"] = displayName,
                    ["followedAt"]  = DateTime.UtcNow.ToString("O"),
                    ["platform"]    = CapFirst(platform),
                };

                var options = new JsonSerializerOptions { WriteIndented = true };
                File.WriteAllText(FOLLOW_DATA_FILE, store.ToJsonString(options));
                CPH.LogInfo("[new-follower] Wrote follow entry for " + key);
            }
        }
        catch (Exception ex)
        {
            // Follow store write is non-critical — log and continue
            CPH.LogWarn("[new-follower] Failed to write follow store: " + ex.Message);
        }
    }

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

    private static string StripFollowCount(string msg) =>
        msg.Replace(" (%followCount% followers)", "").Replace("(%followCount% followers)", "");

    private static string CapFirst(string s) =>
        string.IsNullOrEmpty(s) ? s : char.ToUpper(s[0]) + s.Substring(1);
}

