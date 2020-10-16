using BepInEx;

namespace Mordrog
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin(ModGuid, ModName, ModVer)]
    public class PlayerRespawnSystemPlugin : BaseUnityPlugin
    {
        public const string ModVer = "1.0.0";
        public const string ModName = "PlayerRespawnSystem";
        public const string ModGuid = "com.Mordrog.PlayerRespawnSystem";

        public PlayerRespawnSystemPlugin()
        {
            InitConfig();
        }

        public void Awake()
        {
            base.gameObject.AddComponent<UsersRespawnController>();
        }

        private void InitConfig()
        {
            PluginConfig.IgnoredMapsForTimedRespawn = Config.Bind<string>(
                "Settings",
                "IgnoredMapsForTimedRespawn",
                "bazaar,arena,goldshores,moon,artifactworld,mysteryspace,limbo",
                "Maps on which respawning is ignored."
            );

            PluginConfig.RespawnTimeType = Config.Bind<RespawnTimeType>(
                "Settings",
                "RespawnTimeType",
                RespawnTimeType.StageTimeBased,
                "Type of time to consider when calculating respawn time."
            );

            PluginConfig.StartingRespawnTime = Config.Bind<uint>(
                "Settings",
                "StartingRespawnTime",
                30,
                "Starting respawn time."
            );

            PluginConfig.MaxRespawnTime = Config.Bind<uint>(
                "Settings",
                "MaxRespawnTime",
                180,
                "Max respawn time."
            );

            PluginConfig.UpdateCurrentRepsawnTimeByXSeconds = Config.Bind<uint>(
                "Settings",
                "UpdateCurrentRepsawnTimeByXSeconds",
                5,
                "Value by which current respawn time is updated every UpdateCurrentRespawnTimeEveryXSeconds."
            );

            PluginConfig.UpdateCurrentRespawnTimeEveryXSeconds = Config.Bind<uint>(
                "Settings",
                "UpdateCurrentRespawnTimeEveryXSeconds",
                10,
                "Time interval between updates of the UpdateCurrentRepsawnTimeByXSeconds."
            );

            PluginConfig.UseTimedRespawn = Config.Bind<bool>(
                "Settings",
                "UseTimedRespawn",
                true,
                "Should players be respawned on timed based system."
            );

            PluginConfig.BlockTimedRespawnOnTPEvent = Config.Bind<bool>(
                "Settings",
                "BlockTimedRespawnOnTPEvent",
                true,
                "Should players be blocked from respawning after teleporter event is started."
            );

            PluginConfig.RespawnOnTPStart = Config.Bind<bool>(
                "Settings",
                "RespawnOnTPStart",
                true,
                "Should players be respawned on start of teleporter event (regardless of BlockSpawningOnTPEvent)."
            );

            PluginConfig.RespawnOnTPEnd = Config.Bind<bool>(
                "Settings",
                "RespawnOnTPEnd",
                true,
                "Should players be respawned on end of teleporter event (regardless of BlockSpawningOnTPEvent)."
            );

            PluginConfig.BlockTimedRespawnOnMithrixFight = Config.Bind<bool>(
                "Settings",
                "BlockTimedRespawnOnMithrixFight",
                true,
                "Should players be blocked from respawning after Mithrix fight is started."
            );

            PluginConfig.RespawnOnMithrixStart = Config.Bind<bool>(
                "Settings",
                "RespawnOnMithrixStart",
                true,
                "Should players be respawned on start of Mithrix fight (regardless of BlockRespawningOnMithrixFight or map being ignored)."
            );

            PluginConfig.RespawnOnMithrixEnd = Config.Bind<bool>(
                "Settings",
                "RespawnOnMithrixEnd",
                false,
                "Should players be respawned on end of Mithrix fight (regardless of BlockRespawningOnMithrixFight or map being ignored)."
            );

            PluginConfig.BlockTimedRespawnOnArtifactTrial = Config.Bind<bool>(
                "Settings",
                "BlockTimedRespawnOnArtifactTrial",
                true,
                "Should players be blocked from respawning after Artifact trial is started."
            );

            PluginConfig.RespawnOnArtifactTrialStart = Config.Bind<bool>(
                "Settings",
                "RespawnOnArtifactTrialStart",
                true,
                "Should players be respawned on start of Artifact trial (regardless of BlockTimedRespawningOnArtifactTrial or map being ignored)."
            );

            PluginConfig.RespawnOnArtifactTrialEnd = Config.Bind<bool>(
                "Settings",
                "RespawnOnArtifactTrialEnd",
                true,
                "Should players be respawned on end of Artifact trial (regardless of BlockTimedRespawningOnArtifactTrial or map being ignored)."
            );
        }
    }
}
