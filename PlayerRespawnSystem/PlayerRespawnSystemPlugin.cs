using BepInEx;
using R2API;
using UnityEngine;
using UnityEngine.Networking;

namespace Mordrog
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin(ModGuid, ModName, ModVer)]
    public class PlayerRespawnSystemPlugin : BaseUnityPlugin
    {
        public const string ModVer = "2.0.1";
        public const string ModName = "PlayerRespawnSystem";
        public const string ModGuid = "com.Mordrog.PlayerRespawnSystem";

        private UsersRespawnController usersRespawnController;

        private static GameObject DeathTimerControllerPrefab;
        private GameObject deathTimerController;

        public PlayerRespawnSystemPlugin()
        {
            InitConfig();
        }

        public void Awake()
        {
            //Create prefab
            var temp = new GameObject("temp");
            temp.AddComponent<NetworkIdentity>();
            DeathTimerControllerPrefab = temp.InstantiateClone("DeathTimerController");
            Destroy(temp);
            DeathTimerControllerPrefab.AddComponent<DeathTimerController>();

            On.RoR2.NetworkUser.OnEnable += NetworkUser_OnEnable;
            On.RoR2.Run.Awake += Run_Awake;
            On.RoR2.Run.OnDestroy += Run_OnDestroy;
        }

        private void NetworkUser_OnEnable(On.RoR2.NetworkUser.orig_OnEnable orig, RoR2.NetworkUser self)
        {
            orig(self);

            if (!deathTimerController)
            {
                deathTimerController = Instantiate(DeathTimerControllerPrefab);
                deathTimerController.transform.SetParent(self.transform, false);
            }
        }

        private void Run_Awake(On.RoR2.Run.orig_Awake orig, RoR2.Run self)
        {
            orig(self);

            deathTimerController.SetActive(true);

            if (NetworkServer.active)
            {
                usersRespawnController = base.gameObject.AddComponent<UsersRespawnController>();
                NetworkServer.Spawn(deathTimerController);
            }
        }

        private void Run_OnDestroy(On.RoR2.Run.orig_OnDestroy orig, RoR2.Run self)
        {
            orig(self);

            if (deathTimerController)
                deathTimerController.SetActive(false);

            if (NetworkServer.active)
            {
                Destroy(usersRespawnController);

                if (deathTimerController)
                    NetworkServer.UnSpawn(deathTimerController);
            }
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

            PluginConfig.UsePodsOnStartOfMatch = Config.Bind<bool>(
                "Settings",
                "UsePodsOnStartOfMatch",
                false,
                "Should players spawn from pods on start of match."
            );

            PluginConfig.UseDeathTimerUI = Config.Bind<bool>(
                "Settings",
                "UseDeathTimerUI",
                true,
                "Should UI death timer show up for you on death."
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
