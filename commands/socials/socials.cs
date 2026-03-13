using System;
using System.Collections.Generic;

public class CPHInline
{
    // -------------------------------------------------------------------------
    // Configuration — update these to match your actual links and handles
    // -------------------------------------------------------------------------

    private const string TWITCH_URL   = "https://twitch.tv/YOURCHANNEL";
    private const string YOUTUBE_URL  = "https://youtube.com/@YOURCHANNEL";
    private const string DISCORD_URL  = "https://discord.gg/YOURINVITE";
    private const string TWITTER_URL  = "https://twitter.com/YOURHANDLE";
    private const string TIKTOK_URL   = "";   // Leave empty to hide from output
    private const string KICK_URL     = "";   // Leave empty to hide from output

    // Prefix shown before each platform in chat. Adjust emoji to taste.
    private static readonly (string Label, string Url)[] PLATFORMS = new[]
    {
        ("🎮 Twitch",   TWITCH_URL),
        ("🎬 YouTube",  YOUTUBE_URL),
        ("💬 Discord",  DISCORD_URL),
        ("🐦 Twitter",  TWITTER_URL),
        ("🎵 TikTok",   TIKTOK_URL),
        ("🟩 Kick",     KICK_URL),
    };

    // Global cooldown in seconds enforced in code (supplements Streamer.bot's built-in cooldown)
    private const int GLOBAL_COOLDOWN_SECONDS = 120;
    private const string COOLDOWN_GLOBAL_KEY  = "socialsCooldownLastRun";

    // -------------------------------------------------------------------------

    public bool Execute()
    {
        // Enforce global channel-wide cooldown
        string lastRunStr = CPH.GetGlobalVar<string>(COOLDOWN_GLOBAL_KEY, false);
        if (!string.IsNullOrEmpty(lastRunStr) &&
            DateTime.TryParse(lastRunStr, null, System.Globalization.DateTimeStyles.RoundtripKind, out DateTime lastRun))
        {
            double elapsed = (DateTime.UtcNow - lastRun).TotalSeconds;
            if (elapsed < GLOBAL_COOLDOWN_SECONDS)
            {
                // Silently ignore — don't clutter chat with "on cooldown" for a socials command
                return true;
            }
        }

        CPH.SetGlobalVar(COOLDOWN_GLOBAL_KEY, DateTime.UtcNow.ToString("O"), false);

        // Build the message from non-empty platform entries
        var parts = new System.Text.StringBuilder();
        foreach (var (label, url) in PLATFORMS)
        {
            if (!string.IsNullOrWhiteSpace(url))
            {
                if (parts.Length > 0)
                    parts.Append(" | ");
                parts.Append(label + ": " + url);
            }
        }

        if (parts.Length == 0)
        {
            CPH.LogWarn("[socials] No platform URLs are configured. Edit the constants at the top of socials.cs.");
            return true;
        }

        CPH.SendMessage("📣 Find me here → " + parts.ToString());
        return true;
    }
}
