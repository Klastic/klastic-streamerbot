# `!game` Current Game Command

Reports the broadcaster's Twitch game category in the chat where the command was used.

When Twitch reports `Fallout 4`, the command inspects the running Fallout 4 process. If a loaded module contains `FOLON` or `London`, it reports `Fallout: London` instead.

## Streamer.bot setup

1. Create an action named `Game`.
2. Add an Execute C# Code sub action and paste `game.cs`.
3. Add `System.dll` and `System.Core.dll` to the code references.
4. Enable Precompile on Application Start, then save and compile.
5. Create `!game` with Twitch, YouTube, and Kick command sources enabled.
6. Set Streamer.bot's built in global cooldown to `0`. If a cooldown is wanted, implement it in code so viewers receive feedback.

The category remains sourced from Twitch because it is the shared stream category. The response is routed only to the platform that requested it.
