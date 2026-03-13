using System;
using System.Collections.Generic;

public class CPHInline
{
    // -------------------------------------------------------------------------
    // Configuration
    // -------------------------------------------------------------------------

    // Message for a new subscriber (Tier 1).
    // Placeholders: %user%, %tier%, %months% (cumulative months), %streak% (streak months)
    private const string MSG_NEW_SUB = "🎉 %user% just subscribed! Thank you so much for the support! 🙏";

    // Message for a gift sub received by a viewer.
    // %gifter% = person who gifted, %user% = recipient
    private const string MSG_GIFT_RECEIVED = "🎁 %user% just received a gift sub from %gifter%! Thank you %gifter% for the generosity!";

    // Message for a mass gift sub event (gifter sends to N random viewers).
    // %gifter%, %count% = number of subs gifted
    private const string MSG_MASS_GIFT = "🎁🎁 %gifter% just gifted %count% subs to the community! What a legend!";

    // Message for a re-subscription. %months% = cumulative months.
    private const string MSG_RESUB = "🔁 %user% just resubscribed for %months% months! Thank you for sticking around! ❤️";

    // Tier labels used in log output (Twitch sends "1000", "2000", "3000")
    private static readonly Dictionary<string, string> TIER_LABELS = new Dictionary<string, string>
    {
        { "1000", "Tier 1" },
        { "2000", "Tier 2" },
        { "3000", "Tier 3" },
        { "Prime", "Prime" },
    };

    // Dedup window to guard against duplicate sub events
    private const int DEDUP_WINDOW_SECONDS = 30;
    private const string DEDUP_KEY_PREFIX  = "subDedup_";

    // -------------------------------------------------------------------------

    public bool Execute()
    {
        string user      = args.ContainsKey("user")         ? args["user"].ToString()      : null;
        string userName  = args.ContainsKey("userName")     ? args["userName"].ToString()  : null;
        string subType   = args.ContainsKey("subType")      ? args["subType"].ToString()   : "sub";   // sub, resub, giftsub, giftbomb
        string tier      = args.ContainsKey("tier")         ? args["tier"].ToString()      : "1000";
        string months    = args.ContainsKey("cumulativeMonths") ? args["cumulativeMonths"].ToString() : "1";
        string streak    = args.ContainsKey("monthStreak")  ? args["monthStreak"].ToString() : "0";

        string gifter     = null;
        string recipient  = null;
        int    giftCount  = 1;

        if (string.IsNullOrEmpty(userName))
        {
            CPH.LogWarn("[new-subscriber] userName arg is missing.");
            return false;
        }

        string displayName = !string.IsNullOrEmpty(user) ? user : userName;

        // Deduplication
        string dedupKey    = DEDUP_KEY_PREFIX + subType + "_" + userName.ToLower();
        string lastFireStr = CPH.GetGlobalVar<string>(dedupKey, false);
        if (!string.IsNullOrEmpty(lastFireStr) &&
            DateTime.TryParse(lastFireStr, null, System.Globalization.DateTimeStyles.RoundtripKind, out DateTime lastFire))
        {
            if ((DateTime.UtcNow - lastFire).TotalSeconds < DEDUP_WINDOW_SECONDS)
            {
                CPH.LogInfo("[new-subscriber] Duplicate sub event skipped for " + userName + " (" + subType + ")");
                return true;
            }
        }
        CPH.SetGlobalVar(dedupKey, DateTime.UtcNow.ToString("O"), false);

        string message;

        switch (subType.ToLower())
        {
            case "resub":
                message = MSG_RESUB
                    .Replace("%user%",    displayName)
                    .Replace("%months%",  months)
                    .Replace("%streak%",  streak)
                    .Replace("%tier%",    ResolveTierLabel(tier));
                break;

            case "giftsub":
                // Single gift: args contain gifter and recipient info
                gifter    = args.ContainsKey("isGift") && args["isGift"].ToString() == "True"
                              && args.ContainsKey("gifterUserName") ? args["gifterUserName"].ToString() : displayName;
                recipient = args.ContainsKey("recipientUserName") ? args["recipientUserName"].ToString()
                           : args.ContainsKey("recipientUser")    ? args["recipientUser"].ToString()
                           : displayName;

                message = MSG_GIFT_RECEIVED
                    .Replace("%user%",   recipient)
                    .Replace("%gifter%", !string.IsNullOrEmpty(gifter) ? gifter : "anonymous");
                break;

            case "giftbomb":
                // Mass gift: userName is the gifter, giftCount is total subs given
                int.TryParse(args.ContainsKey("gifts") ? args["gifts"].ToString() : "1", out giftCount);
                message = MSG_MASS_GIFT
                    .Replace("%gifter%", displayName)
                    .Replace("%count%",  giftCount.ToString());
                break;

            default: // "sub" — new subscription
                message = MSG_NEW_SUB
                    .Replace("%user%",   displayName)
                    .Replace("%tier%",   ResolveTierLabel(tier))
                    .Replace("%months%", months)
                    .Replace("%streak%", streak);
                break;
        }

        CPH.SendMessage(message);
        return true;
    }

    private static string ResolveTierLabel(string tier)
    {
        if (TIER_LABELS.TryGetValue(tier, out string label))
            return label;
        return tier;
    }
}
