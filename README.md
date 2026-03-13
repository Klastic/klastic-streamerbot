# klastic-streamerbot

A collection of production-ready [Streamer.bot](https://streamer.bot) commands, C# inline scripts, timers, event handlers, and automations built for real livestream use.

This repository supports my personal stream setup and serves as a public reference for my YouTube tutorials. Everything here is meant to be copied, imported, and used directly.

---

## Contents

| Category | What's Inside |
|---|---|
| [`commands/`](commands/) | Chat commands: `!uptime`, `!followage`, `!socials`, `!lurk`, `!shoutout` |
| [`timers/`](timers/) | Recurring chat messages: social reminders, hydration prompts |
| [`events/`](events/) | Event-driven responses: new follower, subscriber, raid, stream start/end |
| [`utilities/`](utilities/) | Reusable helpers: per-user cooldown tracker |

---

## Requirements

- **Streamer.bot 0.2.x** or later
- Twitch account connected in Streamer.bot (for Twitch-specific features)
- OBS or SLOBS connected via Streamer.bot plugin (for scene-switching features)
- Basic familiarity with Streamer.bot actions and sub-actions

---

## How to Use

Each folder contains:
- A **`.cs` file** — the C# inline script to paste into an _Execute C# Code_ sub-action
- A **`README.md`** — setup instructions, customization guide, and notes

### General import steps

1. Open Streamer.bot and navigate to **Actions**
2. Create a new Action with an appropriate name
3. Add a **Sub-Action → Execute C# Code**
4. Paste the contents of the `.cs` file into the code editor
5. Click **Compile** to verify, then **Save**
6. Follow the individual `README.md` for trigger setup and configuration

---

## Versioning and Compatibility

All scripts target **Streamer.bot 0.2.x**. The `CPH` inline API used throughout is the standard `IInlineInvokeProxy` interface injected at runtime — no additional assemblies required unless noted.

---

## Contributing

This is a personal reference repository. Feel free to fork and adapt for your own stream. Issues and suggestions are welcome.

---

## License

MIT — use freely, attribution appreciated but not required.
