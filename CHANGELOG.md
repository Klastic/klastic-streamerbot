# Changelog

## [1.2.1](https://github.com/Klastic/klastic-streamerbot/compare/klastic-streamerbot-v1.2.0...klastic-streamerbot-v1.2.1) (2026-03-19)


### Bug Fixes

* remove chat activity guard from social-reminder, make time-based only ([ebbf8e1](https://github.com/Klastic/klastic-streamerbot/commit/ebbf8e166e9c6e9d6585923f0460e24014f9545f))
* **social-reminder:** remove chat activity guard, post on pure time schedule ([ff2e4dd](https://github.com/Klastic/klastic-streamerbot/commit/ff2e4dd2bc1870067f6ffe15a4f1ff12de36b881))

## [1.2.0](https://github.com/Klastic/klastic-streamerbot/compare/klastic-streamerbot-v1.1.0...klastic-streamerbot-v1.2.0) (2026-03-19)


### Features

* split hydration and break timers ([39e0101](https://github.com/Klastic/klastic-streamerbot/commit/39e01014663f614cd68e558f94be9546a8506848))
* split hydration and break timers ([b986ba6](https://github.com/Klastic/klastic-streamerbot/commit/b986ba6d0d344780154db31c1d5cd71fec8beaf6))


### Bug Fixes

* **break-reminder:** tone down cheesy break message ([1b47030](https://github.com/Klastic/klastic-streamerbot/commit/1b4703075bf107dfdeb35065b1c3ce5094f58d75))

## [1.1.0](https://github.com/Klastic/klastic-streamerbot/compare/klastic-streamerbot-v1.0.0...klastic-streamerbot-v1.1.0) (2026-03-18)


### Features

* add no-spoilers timer ([214d74b](https://github.com/Klastic/klastic-streamerbot/commit/214d74b9b9b3ff103db7eebd03aac4f344735ce2))
* add no-spoilers timer with per-platform game skip lists ([48ebc6b](https://github.com/Klastic/klastic-streamerbot/commit/48ebc6bc627f0ad709e3ad9ba6fbc1ecae5d616d))
* **no-spoilers:** add OBS live check before sending spoiler reminders ([69375d2](https://github.com/Klastic/klastic-streamerbot/commit/69375d2334e67b49979c445a2185423ece510cd0))
* **no-spoilers:** gate spoiler reminders behind OBS streaming check ([a7196a2](https://github.com/Klastic/klastic-streamerbot/commit/a7196a2a3bc24b1d42229aad9d5906fc40bed82f))


### Bug Fixes

* add OBS streaming check to hydration and social reminder timers ([60458e2](https://github.com/Klastic/klastic-streamerbot/commit/60458e23a2b6d5af16de7829d965d2e83df08642))
* Bump project version from 1.0.0 to 1.1.1 ([e0535f6](https://github.com/Klastic/klastic-streamerbot/commit/e0535f65e3f4d2c3d477a885273f0a7ccd9a1163))
* Bump timers/no-spoilers version to 1.1.1 ([556d297](https://github.com/Klastic/klastic-streamerbot/commit/556d29747baaef37ebb4bf8bdc1fb29aa3147d4c))
* correct timer interval units and implement OBS live check in timer scripts ([9dbe1bd](https://github.com/Klastic/klastic-streamerbot/commit/9dbe1bd8bba7bb91ba304b8101e793e3a49fa90c))
* replace dotnet with simple release type for release-please ([3c929a7](https://github.com/Klastic/klastic-streamerbot/commit/3c929a7ecc4923b87768b73781c7db4dcab3b510))
* replace unsupported `dotnet` release type with `simple` ([13ad977](https://github.com/Klastic/klastic-streamerbot/commit/13ad9777d2340338981a8cd3de0b7130daa96e4b))
* timer README intervals and live-check documentation ([a5dded2](https://github.com/Klastic/klastic-streamerbot/commit/a5dded2a3f05b7c99414a63f2490c51de3c2746b))
* use PAT instead of GITHUB_TOKEN for release-please PR creation ([ddc5408](https://github.com/Klastic/klastic-streamerbot/commit/ddc5408720e480ae3c5830cbdb8769b1514d9345))
* use RELEASE_TOKEN PAT in release-please workflow to allow PR creation ([b881491](https://github.com/Klastic/klastic-streamerbot/commit/b8814919a175b8adc4b7b2aa34dbd817d0ab1862))
