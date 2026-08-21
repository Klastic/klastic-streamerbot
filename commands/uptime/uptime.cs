using System;

public class CPHInline
{
    public bool Execute()
    {
        bool active;
        if (!TryReadBool("obsRaw.outputActive", out active) || !active)
        {
            Send("The stream is not live right now.");
            return true;
        }

        string timecode;
        if (!CPH.TryGetArg("obsRaw.outputTimecode", out timecode) || string.IsNullOrWhiteSpace(timecode))
        {
            Send("The stream is live, but its uptime is unavailable right now.");
            return true;
        }

        TimeSpan uptime;
        if (!TimeSpan.TryParse(timecode.Split('.')[0], out uptime))
        {
            Send("The stream is live, but its uptime is unavailable right now.");
            return true;
        }

        string formatted = uptime.Hours > 0
            ? uptime.Hours + "h " + uptime.Minutes + "m " + uptime.Seconds + "s"
            : uptime.Minutes > 0 ? uptime.Minutes + "m " + uptime.Seconds + "s" : uptime.Seconds + "s";
        Send("We have been live for " + formatted + ".");
        return true;
    }

    private bool TryReadBool(string key, out bool value)
    {
        if (CPH.TryGetArg(key, out value)) return true;
        string text;
        if (CPH.TryGetArg(key, out text)) return bool.TryParse(text, out value);
        value = false;
        return false;
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
