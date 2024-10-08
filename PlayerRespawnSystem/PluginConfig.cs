﻿using BepInEx.Configuration;

namespace PlayerRespawnSystem
{
    public static class PluginConfig
    {
        public static ConfigEntry<string>
            IgnoredMapsForTimedRespawn,
            IgnoredGameModes;

        public static ConfigEntry<RespawnTimeType>
            RespawnTimeType;

        public static ConfigEntry<uint>
            StartingRespawnTime,
            MaxRespawnTime,
            UpdateCurrentRespawnTimeEveryXSeconds,
            UpdateCurrentRepsawnTimeByXSeconds;

        public static ConfigEntry<bool>
            UsePodsOnStartOfMatch,
            UseDeathTimerUI,
            UseTimedRespawn,
            BlockTimedRespawnOnTPEvent,
            RespawnOnTPStart,
            RespawnOnTPEnd,
            BlockTimedRespawnOnMithrixFight,
            RespawnOnMithrixStart,
            RespawnOnMithrixEnd,
            BlockTimedRespawnOnArtifactTrial,
            RespawnOnArtifactTrialStart,
            RespawnOnArtifactTrialEnd,
            BlockTimedRespawnOnVoidlingFight,
            RespawnOnVoidlingStart,
            RespawnOnVoidlingEnd,
            BlockTimedRespawnOnFalseSonFight,
            RespawnOnFalseSonStart,
            RespawnOnFalseSonEnd;
    }
}
