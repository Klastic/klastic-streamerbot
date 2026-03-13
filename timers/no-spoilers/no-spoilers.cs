using System;
using System.Collections.Generic;

public class CPHInline
{
    // -------------------------------------------------------------------------
    // Configuration
    // -------------------------------------------------------------------------

    // Message sent to chat when spoilers are not being suppressed.
    private const string SPOILER_MESSAGE = "Please no spoilers or backseating unless I ask. I'm going through this for the first time.";

    // Games for which the spoiler reminder should be suppressed on each platform.
    // Comparison is case-insensitive and trims surrounding whitespace.
    private static readonly List<string> TWITCH_SKIP_GAMES = new List<string>
    {
        "Overwatch",
        "Dungeons and Dragons",
    };

    private static readonly List<string> KICK_SKIP_GAMES = new List<string>
    {
        "Overwatch",
        "TableTopRPGs",
    };

    private static readonly List<string> YOUTUBE_SKIP_GAMES = new List<string>
    {
        "Overwatch",
        "DnD",
    };

    // Global variable names that hold the current game for each platform.
    // These are expected to be set by your stream-start or category-change action.
    private const string GLOBAL_TWITCH_GAME  = "twitchGameName";
    private const string GLOBAL_KICK_GAME    = "kickGameName";
    private const string GLOBAL_YOUTUBE_GAME = "youtubeGameName";

    // -------------------------------------------------------------------------

    public bool Execute()
    {
        string twitchGame  = CPH.GetGlobalVar<string>(GLOBAL_TWITCH_GAME, true)  ?? string.Empty;
        string kickGame    = CPH.GetGlobalVar<string>(GLOBAL_KICK_GAME, true)    ?? string.Empty;
        string youtubeGame = CPH.GetGlobalVar<string>(GLOBAL_YOUTUBE_GAME, true) ?? string.Empty;

        bool sendTwitch  = !MatchesAny(twitchGame, TWITCH_SKIP_GAMES);
        bool sendKick    = !MatchesAny(kickGame, KICK_SKIP_GAMES);
        bool sendYouTube = !MatchesAny(youtubeGame, YOUTUBE_SKIP_GAMES);

        CPH.LogInfo(
            "[no-spoilers] Twitch='" + twitchGame + "' send=" + sendTwitch +
            " | Kick='" + kickGame + "' send=" + sendKick +
            " | YouTube='" + youtubeGame + "' send=" + sendYouTube
        );

        try
        {
            if (sendTwitch)
            {
                CPH.SendMessage(SPOILER_MESSAGE, false, true);
                CPH.LogInfo("[no-spoilers] Sent spoiler message to Twitch");
            }
            else
            {
                CPH.LogInfo("[no-spoilers] Skipped Twitch spoiler message");
            }
        }
        catch (Exception ex)
        {
            CPH.LogError("[no-spoilers] Twitch send failed: " + ex.Message);
        }

        try
        {
            if (sendKick)
            {
                CPH.SendKickMessage(SPOILER_MESSAGE, false, true);
                CPH.LogInfo("[no-spoilers] Sent spoiler message to Kick");
            }
            else
            {
                CPH.LogInfo("[no-spoilers] Skipped Kick spoiler message");
            }
        }
        catch (Exception ex)
        {
            CPH.LogError("[no-spoilers] Kick send failed: " + ex.Message);
        }

        try
        {
            if (sendYouTube)
            {
                CPH.SendYouTubeMessageToLatestMonitored(SPOILER_MESSAGE, false, true);
                CPH.LogInfo("[no-spoilers] Sent spoiler message to YouTube");
            }
            else
            {
                CPH.LogInfo("[no-spoilers] Skipped YouTube spoiler message");
            }
        }
        catch (Exception ex)
        {
            CPH.LogError("[no-spoilers] YouTube send failed: " + ex.Message);
        }

        return true;
    }

    private bool MatchesAny(string currentGame, List<string> skipList)
    {
        string normalizedCurrent = Normalize(currentGame);

        foreach (string game in skipList)
        {
            if (Normalize(game) == normalizedCurrent)
            {
                return true;
            }
        }

        return false;
    }

    private string Normalize(string value)
    {
        return (value ?? string.Empty).Trim().ToLowerInvariant();
    }
}
