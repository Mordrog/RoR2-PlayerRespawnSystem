# RoR2-PlayerRespawnSystem
RoR2 mod - provides customizable players respawn system for your games

Mod will provide respawn system which can be configured to your play style. It allows for time respawn, respawn on start of teleporter event, respawn on end of teleporter event. 
You can customize respawn timer settings to scale with stage or game time. Respawns can be blocked at certain stages and during the teleporter to maintain the challenge that the game should provide.

## New UI Death Timer

Mod should now add new Death Timer panel when you die, so you will know exactly when you are going to respawn.

**This feature is client dependent** meaning, clients will only have this panel shown if they have mod.

### Default Config Settings
| Setting                                               | Default Value                                                             |
| :-------------------------------------                | :-----------------------------------------------------------------------: |
| IgnoredMapsForTimedRespawn                            |           "bazaar,arena,goldshores,moon,artifactworld,mysteryspace,limbo" |
| RespawnTimeType                                       |                                                            StageTimeBased |
| StartingRespawnTime                                   |                                                                        30 |
| MaxRespawnTime                                        |                                                                       180 |
| UpdateCurrentRepsawnTimeByXSeconds                    |                                                                         5 |
| UpdateCurrentRespawnTimeEveryXSeconds                 |                                                                        10 |
| UsePodsOnStartOfMatch                                 |                                                                     false |
| UseDeathTimerUI                                       |                                                                      true |
| UseTimedRespawn                                       |                                                                      true |
| BlockTimedRespawnOnTPEvent                            |                                                                      true |
| RespawnOnTPStart                                      |                                                                      true |
| RespawnOnTPEnd                                        |                                                                      true |
| BlockTimedRespawnOnMithrixFight                       |                                                                      true |
| RespawnOnMithrixStart                                 |                                                                      true |
| RespawnOnMithrixEnd                                   |                                                                     false |
| BlockTimedRespawnOnArtifactTrial                      |                                                                      true |
| RespawnOnArtifactTrialStart                           |                                                                      true |
| RespawnOnArtifactTrialEnd                             |                                                                      true |

## More

Find my other mods here: https://thunderstore.io/package/Mordrog/

### Changelog
#### 2.0.0
- Update icon
- Added plentiful options to control respawning on certain events, regardless of timed respawn
- Added option to not use pods respawn on start of match
- Added Death Timer UI for dead players (client dependent)