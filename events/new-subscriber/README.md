# New Subscriber — Chat Announcement

## Summary

Handles all Twitch subscription event types in a single script: new subscriptions, re-subscriptions, individual gift subs, and mass gift bombs. Each sub type gets its own configurable message with relevant placeholders. Includes deduplication to guard against Twitch firing the event twice.

---

## Code

See [`new-subscriber.cs`](new-subscriber.cs).

---

## Streamer.bot Setup

### Create the action

1. Go to **Actions → Add**, name it `New Subscriber`
2. Add a sub-action: **Core → Execute C# Code**
3. Paste `new-subscriber.cs` into the editor, **Compile**, then **Save**

### Connect Twitch sub events

Connect the same action to each of these Twitch events:

| Event path | Streamer.bot Event |
|---|---|
| Twitch → Subscribe | `subType = "sub"` |
| Twitch → Re-Subscribe | `subType = "resub"` |
| Twitch → Gift Subscription | `subType = "giftsub"` |
| Twitch → Gift Bomb | `subType = "giftbomb"` |

> All four events go to the same action. The script reads `args["subType"]` to route the correct message.

---

## Customization

| Value | Location | Purpose |
|---|---|---|
| `MSG_NEW_SUB` | Top of script | Message for a brand new sub. Supports `%user%`, `%tier%` |
| `MSG_RESUB` | Top of script | Message for resubscriptions. Supports `%months%`, `%streak%` |
| `MSG_GIFT_RECEIVED` | Top of script | Message when a viewer receives a gift sub |
| `MSG_MASS_GIFT` | Top of script | Message for a gift bomb (multiple subs at once). Supports `%count%` |
| `DEDUP_WINDOW_SECONDS` | Top of script | Deduplication window in seconds |

---

## Repo Notes

Single script for all four Twitch subscription event types. Routes via `subType` arg. Uses per-event dedup globals. Tier labels (1000/2000/3000) are mapped to human-readable strings.

---

## Video Notes

Worth highlighting:
- The `switch (subType)` routing pattern — one action handles all four sub types
- Twitch's tier values (`1000`, `2000`, `3000`) and why we map them to readable labels
- The deduplication key including `subType` so resub and new sub dedup separately
