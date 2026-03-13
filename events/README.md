# Events

Event-driven actions that fire automatically in response to Twitch (and other platform) events. Each script is designed to be reliable in a live production environment with appropriate guards against duplicate triggers and spam.

| Event | File | Description |
|---|---|---|
| New Follower | [`new-follower/`](new-follower/) | Announces new followers in chat |
| New Subscriber | [`new-subscriber/`](new-subscriber/) | Celebrates new subscribers with tier-aware messaging |
| Raid Handler | [`raid-handler/`](raid-handler/) | Responds to incoming raids with size-aware messages |
| Stream Start | [`stream-start/`](stream-start/) | Initializes globals and performs stream-start tasks |
| Stream End | [`stream-end/`](stream-end/) | Cleans up globals and performs stream-end tasks |
