# RoR2-PlayerRespawnSystem
RoR2 mod - provides customizable players respawn system for your games

Mod will provide respawn system which can be configured to your play style. It allows for time respawn, respawn on start of teleporter event, respawn on end of teleporter event. 
You can customize respawn timer settings to scale with stage or game time. Respawns can be blocked at certain stages and during the teleporter to maintain the challenge that the game should provide.

## New UI Death Timer

Mod should now add new Death Timer panel when you die, so you will know exactly when you are going to respawn.

**This feature is client dependent** meaning, clients will only have this panel shown if they have mod.

### Default Config Settings
| Setting                                               | Default Value                                                                   |
| :-------------------------------------                | :-----------------------------------------------------------------------------: |
| IgnoredMapsForTimedRespawn                            |           "bazaar,arena,goldshores,moon,moon2,artifactworld,mysteryspace,limbo" |
| IgnoredGameModes                                      |                                                              "IgnoredGameModes" |
| RespawnTimeType                                       |                                                            StageTimeBased        |
| StartingRespawnTime                                   |                                                                        30        |
| MaxRespawnTime                                        |                                                                       180        |
| UpdateCurrentRepsawnTimeByXSeconds                    |                                                                         5        |
| UpdateCurrentRespawnTimeEveryXSeconds                 |                                                                        10        |
| UsePodsOnStartOfMatch                                 |                                                                     false        |
| UseDeathTimerUI                                       |                                                                      true        |
| UseTimedRespawn                                       |                                                                      true        |
| BlockTimedRespawnOnTPEvent                            |                                                                      true        |
| RespawnOnTPStart                                      |                                                                      true        |
| RespawnOnTPEnd                                        |                                                                      true        |
| BlockTimedRespawnOnMithrixFight                       |                                                                      true        |
| RespawnOnMithrixStart                                 |                                                                      true        |
| RespawnOnMithrixEnd                                   |                                                                     false        |
| BlockTimedRespawnOnArtifactTrial                      |                                                                      true        |
| RespawnOnArtifactTrialStart                           |                                                                      true        |
| RespawnOnArtifactTrialEnd                             |                                                                      true        |

## More

Find my other mods here: https://thunderstore.io/package/Mordrog/

### Changelog
#### 2.0.4
- Update manifest + rebuild on new patch

#### 2.0.3
- Fix for Survivors of the Void changes
- Fix bug where sometimes respawn system would not correctly destroy it's instance resulting in it's duplication
- Added new void boss stage to ignored by default
- Added option to turn off respawn on selected game modes

#### 2.0.2
- Fix for Anniversary Update
- Fix issue where game would freeze if no proper repsawn point was found
- Fix missing R2Api "PrefabApi" reference
- Added new Moon stage to ignored Stages for timed respawn

#### 2.0.1
- Reset respawn timer after RoR2 respawn method is called to eleminate conflicts with other respawn mods

#### 2.0.0
- Update icon
- Added plentiful options to control respawning on certain events, regardless of timed respawn
- Added option to not use pods respawn on start of match
- Added Death Timer UI for dead players (client dependent)