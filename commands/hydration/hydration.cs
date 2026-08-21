using System;

public class CPHInline
{
    private const string CooldownKey = "hydrate_command_last_response";
    private const int CooldownSeconds = 600;

    public bool Execute()
    {
        long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        long last = CPH.GetGlobalVar<long>(CooldownKey, false);
        if (last > 0 && now - last < CooldownSeconds)
        {
            int remaining = (int)(CooldownSeconds - (now - last));
            Send("Hydrate is on cooldown for another " + FormatWait(remaining) + ".");
            return true;
        }

        CPH.SetGlobalVar(CooldownKey, now, false);
        Send("Hydration check! Klastic, grab some water when you get a chance.");
        return true;
    }

    private static string FormatWait(int seconds)
    {
        int minutes = seconds / 60;
        int remainder = seconds % 60;
        return minutes > 0 ? minutes + "m " + remainder + "s" : remainder + "s";
    }

    private void Send(string message)
    {
        string source;
        CPH.TryGetArg("commandSource", out source);
        if (string.IsNullOrWhiteSpace(source)) CPH.TryGetArg("platform", out source);
        source = string.IsNullOrWhiteSpace(source) ? CPH.GetSource().ToString() : source;
        if (source.Equals("YouTube", StringComparison.OrdinalIgnoreCase)) CPH.SendYouTubeMessageToLatestMonitored(message);
        else if (source.Equals("Kick", StringComparison.OrdinalIgnoreCase)) CPH.SendKickMessage(message);
        else CPH.SendMessage(message);
    }
}
