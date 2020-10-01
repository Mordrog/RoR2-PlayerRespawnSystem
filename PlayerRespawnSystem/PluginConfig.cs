using BepInEx.Configuration;

namespace Mordrog
{
    public static class PluginConfig
    {
        public static ConfigEntry<string>
            IgnoredMaps
            ;

        public static ConfigEntry<RespawnTimeType>
            RespawnTimeType;

        public static ConfigEntry<uint>
            StartingRespawnTime,
            MaxRespawnTime,
            UpdateCurrentRespawnTimeEveryXSeconds,
            UpdateCurrentRepsawnTimeByXSeconds;

        public static ConfigEntry<bool>
            UseTimeRespawn,
            BlockRespawningOnTPEvent,
            RespawnOnTPStart,
            RespawnOnTPEnd;
    }
}
