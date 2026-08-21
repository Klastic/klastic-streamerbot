using System;
using System.Diagnostics;

public class CPHInline
{
    public bool Execute()
    {
        try
        {
            var broadcaster = CPH.TwitchGetExtendedUserInfoByLogin("klastic_");
            string game = broadcaster == null ? null : broadcaster.Game;
            if (IsFalloutLondon(game)) game = "Fallout: London";
            Send(string.IsNullOrWhiteSpace(game)
                ? "No current game category is available."
                : "Klastic is currently playing " + game + ".");
        }
        catch (Exception ex)
        {
            CPH.LogWarn("[game] Could not retrieve the Twitch category: " + ex.Message);
            Send("The current game category is unavailable right now.");
        }
        return true;
    }

    private bool IsFalloutLondon(string category)
    {
        if (!string.Equals(category, "Fallout 4", StringComparison.OrdinalIgnoreCase)) return false;
        try
        {
            foreach (Process process in Process.GetProcessesByName("Fallout4"))
            {
                if ((process.MainWindowTitle ?? "").IndexOf("London", StringComparison.OrdinalIgnoreCase) >= 0) return true;
                try
                {
                    foreach (ProcessModule module in process.Modules)
                    {
                        string name = module.ModuleName ?? "";
                        if (name.IndexOf("FOLON", StringComparison.OrdinalIgnoreCase) >= 0 ||
                            name.IndexOf("London", StringComparison.OrdinalIgnoreCase) >= 0) return true;
                    }
                }
                catch (Exception ex) { CPH.LogInfo("[game] Could not inspect Fallout 4 modules: " + ex.Message); }
            }
        }
        catch (Exception ex) { CPH.LogInfo("[game] Fallout: London detection was unavailable: " + ex.Message); }
        return false;
    }

    private void Send(string message)
    {
        string source;
        CPH.TryGetArg("commandSource", out source);
        source = string.IsNullOrWhiteSpace(source) ? CPH.GetSource().ToString() : source;
        if (source.Equals("YouTube", StringComparison.OrdinalIgnoreCase)) CPH.SendYouTubeMessageToLatestMonitored(message);
        else if (source.Equals("Kick", StringComparison.OrdinalIgnoreCase)) CPH.SendKickMessage(message);
        else CPH.SendMessage(message);
    }
}
