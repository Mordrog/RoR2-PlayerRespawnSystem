using RoR2;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Mordrog
{
    public enum RespawnType : byte
    {
        Default,
        Teleporter,
        Mithrix,
        Artifact
    }

    class UsersRespawnController : NetworkBehaviour
    {
        public UsersTimedRespawn usersTimedRespawn;
        private UsersTeleporterRespawn usersTeleporterRespawn;
        private UsersMithrixRespawn usersMithrixRespawn;
        private UsersArtifactTrialRespawn usersArtifactTrialRespawn;

        private Queue<CharacterBody> respawnCharacterMaster = new Queue<CharacterBody>();

        public bool IsAdvancingStage { get; private set; }

        public bool IsTimedRespawnBlocked { get; private set; }

        public RespawnType RespawnType { get; set; } = RespawnType.Default;

        public static UsersRespawnController instance { get; private set; }

        public void Awake()
        {
            usersTimedRespawn = base.gameObject.AddComponent<UsersTimedRespawn>();
            usersTeleporterRespawn = base.gameObject.AddComponent<UsersTeleporterRespawn>();
            usersMithrixRespawn = base.gameObject.AddComponent<UsersMithrixRespawn>();
            usersArtifactTrialRespawn = base.gameObject.AddComponent<UsersArtifactTrialRespawn>();

            usersTimedRespawn.respawnController = usersTeleporterRespawn.respawnController = usersMithrixRespawn.respawnController = usersArtifactTrialRespawn.respawnController = this;

            On.RoR2.PlayerCharacterMasterController.OnBodyDeath += PlayerCharacterMasterController_OnBodyDeath;
            On.RoR2.SceneExitController.SetState += SceneExitController_SetState;
            On.RoR2.Stage.OnEnable += Stage_OnEnable;
            On.RoR2.Stage.BeginAdvanceStage += Stage_BeginAdvanceStage;
            On.RoR2.Stage.RespawnCharacter += Stage_RespawnCharacter;
            On.RoR2.Stage.GetPlayerSpawnTransform += Stage_GetPlayerSpawnTransform;
            On.RoR2.Run.OnServerSceneChanged += Run_OnServerSceneChanged;
            On.RoR2.Run.OnDestroy += Run_OnDestroy;
        }

        protected void OnEnable()
        {
            UsersRespawnController.instance = SingletonHelper.Assign<UsersRespawnController>(UsersRespawnController.instance, this);
        }

        protected void OnDisable()
        {
            UsersRespawnController.instance = SingletonHelper.Unassign<UsersRespawnController>(UsersRespawnController.instance, this);
        }

        public void OnDestroy()
        {
            On.RoR2.PlayerCharacterMasterController.OnBodyDeath -= PlayerCharacterMasterController_OnBodyDeath;
            On.RoR2.SceneExitController.SetState -= SceneExitController_SetState;
            On.RoR2.Stage.OnEnable -= Stage_OnEnable;
            On.RoR2.Stage.BeginAdvanceStage -= Stage_BeginAdvanceStage;
            On.RoR2.Stage.RespawnCharacter -= Stage_RespawnCharacter;
            On.RoR2.Stage.GetPlayerSpawnTransform -= Stage_GetPlayerSpawnTransform;
            On.RoR2.Run.OnServerSceneChanged -= Run_OnServerSceneChanged;
            On.RoR2.Run.OnDestroy -= Run_OnDestroy;

            Destroy(usersTimedRespawn);
            Destroy(usersTeleporterRespawn);
            Destroy(usersMithrixRespawn);
            Destroy(usersArtifactTrialRespawn);
        }

        private void PlayerCharacterMasterController_OnBodyDeath(On.RoR2.PlayerCharacterMasterController.orig_OnBodyDeath orig, PlayerCharacterMasterController self)
        {
            orig(self);

            Stage.instance.usePod = false;

            var user = UsersHelper.GetUser(self.master);

            if (user)
            {
                if (CheckIfCanTimedRespawn(self.master))
                {
                    usersTimedRespawn.StartTimedRespawn(user);
                }
                else if (IsTimedRespawnBlocked)
                {
                    switch(RespawnType)
                    {
                        case RespawnType.Teleporter:
                            if (PluginConfig.RespawnOnTPEnd.Value)
                                ChatHelper.UserWillRespawnAfterTPEvent(user.userName);
                            break;
                        case RespawnType.Mithrix:
                            if (PluginConfig.RespawnOnMithrixEnd.Value)
                                ChatHelper.UserWillRespawnAfterMithrixFight(user.userName);
                            break;
                        case RespawnType.Artifact:
                            if (PluginConfig.RespawnOnArtifactTrialEnd.Value)
                                ChatHelper.UserWillRespawnAfterArtifactTrial(user.userName);
                            break;
                    }
                }
            }
        }

        private void SceneExitController_SetState(On.RoR2.SceneExitController.orig_SetState orig, SceneExitController self, SceneExitController.ExitState newState)
        {
            orig(self, newState);

            if (newState == SceneExitController.ExitState.TeleportOut)
            {
                IsAdvancingStage = true;
            }
        }

        private void Stage_OnEnable(On.RoR2.Stage.orig_OnEnable orig, Stage self)
        {
            orig(self);

            if (!PluginConfig.UsePodsOnStartOfMatch.Value)
            {
                Stage.instance.usePod = false;
            }
        }

        private void Stage_BeginAdvanceStage(On.RoR2.Stage.orig_BeginAdvanceStage orig, Stage self, SceneDef destinationStage)
        {
            orig(self, destinationStage);

            UnblockTimedRespawn();
            RespawnType = RespawnType.Default;
            usersTimedRespawn.ResetAllRespawnTimers();
        }

        private void Stage_RespawnCharacter(On.RoR2.Stage.orig_RespawnCharacter orig, Stage self, CharacterMaster characterMaster)
        {
            if (characterMaster.bodyPrefab)
            {
                CharacterBody body = characterMaster.bodyPrefab.GetComponent<CharacterBody>();

                if (body)
                {
                    respawnCharacterMaster.Enqueue(body);
                }
            }

            orig(self, characterMaster);
        }

        private Transform Stage_GetPlayerSpawnTransform(On.RoR2.Stage.orig_GetPlayerSpawnTransform orig, Stage self)
        {
            if (respawnCharacterMaster.Count == 0)
            {
                return orig(self);
            }

            var body = respawnCharacterMaster.Dequeue();
            Transform spawnTransform;

            switch (RespawnType)
            {
                case RespawnType.Teleporter:
                    spawnTransform = new GameObject().transform;
                    spawnTransform.position = RespawnPosition.GetSpawnPositionAroundTeleporter(body, 0.5f, 3);
                    return spawnTransform;

                case RespawnType.Mithrix:
                    spawnTransform = new GameObject().transform;
                    spawnTransform.position = RespawnPosition.GetSpawnPositionAroundMoonBoss(body, 100, 105);
                    return spawnTransform;

                default:
                    return orig(self);
            }
        }


        private void Run_OnServerSceneChanged(On.RoR2.Run.orig_OnServerSceneChanged orig, Run self, string sceneName)
        {
            orig(self, sceneName);

            IsAdvancingStage = false;

            if (CheckIfCurrentStageIsIgnoredForTimedRespawn())
            {
                ChatHelper.TimedRespawnBlockedOnStage();
            }
        }

        private void Run_OnDestroy(On.RoR2.Run.orig_OnDestroy orig, Run self)
        {
            orig(self);

            respawnCharacterMaster.Clear();
            UnblockTimedRespawn();
            RespawnType = RespawnType.Default;
        }

        public void BlockTimedRespawn()
        {
            IsTimedRespawnBlocked = true;
            usersTimedRespawn.StopAllRespawnTimers();
        }

        public void UnblockTimedRespawn()
        {
            IsTimedRespawnBlocked = false;
            usersTimedRespawn.ResumeAllRespawnTimers();
        }

        public void RespawnUser(NetworkUser user)
        {
            if (user?.master && CheckIfCanRespawn(user.master))
            {
                Stage.instance.RespawnCharacter(user.master);
            }
        }

        public void RespawnAllUsers()
        {
            foreach(var user in NetworkUser.readOnlyInstancesList)
            {
                RespawnUser(user);
            }
        }

        public bool CheckIfCanRespawn(CharacterMaster master)
        {
            return master &&
                   master.IsDeadAndOutOfLivesServer() &&
                   !IsAdvancingStage;
        }

        public bool CheckIfCanTimedRespawn(CharacterMaster master)
        {
            return CheckIfCanRespawn(master) &&
                   !CheckIfCurrentStageIsIgnoredForTimedRespawn() &&
                   !IsTimedRespawnBlocked &&
                   PluginConfig.UseTimedRespawn.Value;
        }

        public bool CheckIfCurrentStageIsIgnoredForTimedRespawn()
        {
            return PluginConfig.IgnoredMapsForTimedRespawn.Value.Contains(SceneCatalog.GetSceneDefForCurrentScene().baseSceneName);
        }
    }
}
