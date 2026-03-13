using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;

// ---------------------------------------------------------------------------
// Cross-Platform Follow Store — Standalone Read/Write Utility
//
// Use this script as a standalone action called from other actions, or copy
// the helper methods (ReadEntry, WriteEntry, DeleteEntry) into any script
// that needs direct follow data access.
//
// Supported operations (set args["storeAction"] before calling this action):
//   "write"  — write or update a follow entry
//   "read"   — look up a single entry (sets output args)
//   "delete" — remove an entry
//   "exists" — check if an entry exists (sets "followExists" output arg)
//
// Required input args for all operations:
//   storeAction  — one of the above operations
//   storePlatform — "twitch", "youtube", or "kick"
//   storeUserName — the target user's login (lowercase)
//
// Additional input args for "write":
//   storeDisplayName — display name to record
//   storeFollowedAt  — (optional) ISO-8601 timestamp; defaults to UtcNow
//
// Output args set by "read":
//   followFound       — "true" or "false"
//   followDisplayName — stored display name
//   followedAt        — stored follow date (ISO-8601)
//   followPlatform    — stored platform label
//   followAgeFormatted — human-readable age string (e.g., "1 year, 3 months")
// ---------------------------------------------------------------------------

public class CPHInline
{
    // -------------------------------------------------------------------------
    // Configuration
    // -------------------------------------------------------------------------

    private static readonly string FOLLOW_DATA_FILE = Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory, "data", "klastic-follows.json");

    // -------------------------------------------------------------------------

    public bool Execute()
    {
        string action      = GetArg("storeAction",      "",             toLower: true);
        string platform    = GetArg("storePlatform",    "",             toLower: true);
        string userName    = GetArg("storeUserName",    "",             toLower: true);
        string displayName = GetArg("storeDisplayName", userName);
        string followedAt  = GetArg("storeFollowedAt",  DateTime.UtcNow.ToString("O"));

        if (string.IsNullOrEmpty(platform) || string.IsNullOrEmpty(userName))
        {
            CPH.LogWarn("[follow-store] storePlatform and storeUserName are required.");
            return false;
        }

        string key = platform + ":" + userName;

        switch (action)
        {
            case "write":
                WriteEntry(key, userName, displayName, followedAt, platform);
                break;

            case "read":
                ReadEntry(key);
                break;

            case "delete":
                DeleteEntry(key);
                break;

            case "exists":
                bool exists = EntryExists(key);
                CPH.SetArgument("followExists", exists ? "true" : "false");
                break;

            default:
                CPH.LogWarn("[follow-store] Unknown storeAction: '" + action + "'. Use write/read/delete/exists.");
                return false;
        }

        return true;
    }

    // -------------------------------------------------------------------------
    // Operations
    // -------------------------------------------------------------------------

    private void WriteEntry(string key, string userName, string displayName, string followedAt, string platform)
    {
        JsonObject store = LoadStore();

        // Preserve original follow date — do not overwrite if already recorded
        bool isNew = !store.ContainsKey(key);
        if (!isNew)
        {
            CPH.LogInfo("[follow-store] Entry already exists for '" + key + "' — skipping overwrite.");
            return;
        }

        store[key] = new JsonObject
        {
            ["userName"]    = userName,
            ["displayName"] = displayName,
            ["followedAt"]  = followedAt,
            ["platform"]    = CapFirst(platform),
        };

        SaveStore(store);
        CPH.LogInfo("[follow-store] Wrote new entry: " + key);
    }

    private void ReadEntry(string key)
    {
        JsonObject store = LoadStore();

        if (!store.ContainsKey(key))
        {
            CPH.SetArgument("followFound", "false");
            CPH.LogInfo("[follow-store] No entry found for key: " + key);
            return;
        }

        JsonNode entry = store[key];
        string followedAtStr  = entry["followedAt"]?.GetValue<string>() ?? "";
        string storedDisplay  = entry["displayName"]?.GetValue<string>() ?? "";
        string storedPlatform = entry["platform"]?.GetValue<string>() ?? "";

        CPH.SetArgument("followFound",       "true");
        CPH.SetArgument("followDisplayName", storedDisplay);
        CPH.SetArgument("followedAt",        followedAtStr);
        CPH.SetArgument("followPlatform",    storedPlatform);

        if (!string.IsNullOrEmpty(followedAtStr) &&
            DateTime.TryParse(followedAtStr, null, System.Globalization.DateTimeStyles.RoundtripKind, out DateTime followedAt))
        {
            string ageStr = FormatFollowAge(followedAt.ToUniversalTime());
            CPH.SetArgument("followAgeFormatted", ageStr);
        }
        else
        {
            CPH.SetArgument("followAgeFormatted", "unknown");
        }
    }

    private void DeleteEntry(string key)
    {
        JsonObject store = LoadStore();
        if (store.ContainsKey(key))
        {
            store.Remove(key);
            SaveStore(store);
            CPH.LogInfo("[follow-store] Deleted entry: " + key);
        }
        else
        {
            CPH.LogInfo("[follow-store] No entry to delete for key: " + key);
        }
    }

    private bool EntryExists(string key)
    {
        JsonObject store = LoadStore();
        return store.ContainsKey(key);
    }

    // -------------------------------------------------------------------------
    // File I/O helpers
    // -------------------------------------------------------------------------

    private JsonObject LoadStore()
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
            CPH.LogWarn("[follow-store] Failed to load store: " + ex.Message);
            return new JsonObject();
        }
    }

    private void SaveStore(JsonObject store)
    {
        try
        {
            string dir = Path.GetDirectoryName(FOLLOW_DATA_FILE);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            var options = new JsonSerializerOptions { WriteIndented = true };
            File.WriteAllText(FOLLOW_DATA_FILE, store.ToJsonString(options));
        }
        catch (Exception ex)
        {
            CPH.LogWarn("[follow-store] Failed to save store: " + ex.Message);
        }
    }

    // -------------------------------------------------------------------------
    // Formatting helpers
    // -------------------------------------------------------------------------

    // Calendar-accurate follow age using DateTime arithmetic instead of TimeSpan approximations.
    private static string FormatFollowAge(DateTime followedAt)
    {
        DateTime now = DateTime.UtcNow;

        // Count complete years elapsed
        int years = now.Year - followedAt.Year;
        if (now.Month < followedAt.Month || (now.Month == followedAt.Month && now.Day < followedAt.Day))
            years--;

        // Count complete months elapsed after removing full years
        DateTime afterYears = followedAt.AddYears(years);
        int months = 0;
        while (afterYears.AddMonths(months + 1) <= now)
            months++;

        // Remaining days
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

    private string GetArg(string key, string defaultValue = null, bool toLower = false)
    {
        if (!args.ContainsKey(key) || args[key] == null)
            return defaultValue;
        string value = args[key].ToString();
        return toLower ? value.ToLower() : value;
    }
}
