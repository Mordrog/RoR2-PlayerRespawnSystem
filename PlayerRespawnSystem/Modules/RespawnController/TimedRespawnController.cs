using RoR2;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerRespawnSystem
{
    [AssociatedRespawnType(RespawnType.Timed)]
    class TimedRespawnController : RespawnController
    {
        public static new bool IsEnabled => PluginConfig.UseTimedRespawn.Value;

        private MultiUserTimers userRespawnTimers;

        public IReadOnlyDictionary<NetworkUserId, UserTimer> UserRespawnTimers => userRespawnTimers.UserTimers;

        public void StartRespawnTimer(NetworkUser user)
        {
            if (IsActive)
            {
                if (user && playerRespawner.CheckIfControllerCanRespawn(this, user))
                {
                    var respawnTime = RespawnTimeCalculation.GetRespawnTime();

                    userRespawnTimers.StartTimer(user.id, respawnTime);
                    ChatHelper.UserWillRespawnAfter(user.userName, respawnTime);
                }
            }
        }

        public void ResetRespawnTimer(NetworkUser user)
        {
            if (user)
            {
                userRespawnTimers.ResetTimer(user.id);
            }
        }

        public void StopAllRespawnTimers()
        {
            userRespawnTimers.StopTimers();
            IsActive = false;
        }

        public void ResumeAllRespawnTimers(bool forceResume = false)
        {
            if (forceResume || !CheckIfCurrentStageIsIgnoredForTimedRespawn())
            {
                userRespawnTimers.ResumeTimers();
                IsActive = true;
            }
        }

        public void ResetAllRespawnTimers()
        {
            userRespawnTimers.ResetTimers();
        }

        public void Awake()
        {
            IsActive = true;
            userRespawnTimers = gameObject.AddComponent<MultiUserTimers>();
            userRespawnTimers.OnUserTimerEndInFiveSeconds += UserRespawnTimers_OnUserTimerEndInFiveSeconds;
            userRespawnTimers.OnUserTimerEnd += UsersRespawnTimers_OnUserTimerRespawnTimerEnd;

            On.RoR2.Run.OnUserAdded += Run_OnUserAdded;
            On.RoR2.Run.OnUserRemoved += Run_OnUserRemoved;
            On.RoR2.Run.BeginGameOver += Run_BeginGameOver;
            On.RoR2.Run.OnDestroy += Run_OnDestroy;

            On.RoR2.PlayerCharacterMasterController.OnBodyDeath += PlayerCharacterMasterController_OnBodyDeath;
            On.RoR2.CharacterMaster.Respawn += CharacterMaster_Respawn;
            On.RoR2.Run.OnServerSceneChanged += Run_OnServerSceneChanged;
            On.RoR2.Stage.BeginAdvanceStage += Stage_BeginAdvanceStage;
        }

        public void OnDestroy()
        {
            userRespawnTimers.OnUserTimerEndInFiveSeconds -= UserRespawnTimers_OnUserTimerEndInFiveSeconds;
            userRespawnTimers.OnUserTimerEnd -= UsersRespawnTimers_OnUserTimerRespawnTimerEnd;
            Destroy(userRespawnTimers);

            On.RoR2.Run.OnUserAdded -= Run_OnUserAdded;
            On.RoR2.Run.OnUserRemoved -= Run_OnUserRemoved;
            On.RoR2.Run.BeginGameOver -= Run_BeginGameOver;
            On.RoR2.Run.OnDestroy -= Run_OnDestroy;

            On.RoR2.PlayerCharacterMasterController.OnBodyDeath -= PlayerCharacterMasterController_OnBodyDeath;
            On.RoR2.CharacterMaster.Respawn -= CharacterMaster_Respawn;
            On.RoR2.Run.OnServerSceneChanged -= Run_OnServerSceneChanged;
            On.RoR2.Stage.BeginAdvanceStage -= Stage_BeginAdvanceStage;
        }

        private void UserRespawnTimers_OnUserTimerEndInFiveSeconds(NetworkUser user)
        {
            ChatHelper.UserWillRespawnAfter(user.userName, 5);
        }

        private void UsersRespawnTimers_OnUserTimerRespawnTimerEnd(NetworkUser user)
        {
            playerRespawner.RespawnUser(this, user);
        }

        private void Run_OnUserAdded(On.RoR2.Run.orig_OnUserAdded orig, Run self, NetworkUser user)
        {
            orig(self, user);

            userRespawnTimers.AddTimer(user.id);
        }

        private void Run_OnUserRemoved(On.RoR2.Run.orig_OnUserRemoved orig, Run self, NetworkUser user)
        {
            orig(self, user);

            userRespawnTimers.RemoveTimer(user.id);
        }

        private void Run_BeginGameOver(On.RoR2.Run.orig_BeginGameOver orig, Run self, GameEndingDef gameEndingDef)
        {
            orig(self, gameEndingDef);

            userRespawnTimers.ClearTimers();
        }

        private void Run_OnDestroy(On.RoR2.Run.orig_OnDestroy orig, Run self)
        {
            orig(self);

            userRespawnTimers.ClearTimers();
        }

        private void PlayerCharacterMasterController_OnBodyDeath(On.RoR2.PlayerCharacterMasterController.orig_OnBodyDeath orig, PlayerCharacterMasterController self)
        {
            orig(self);

            var user = UsersHelper.GetUser(self.master);
            if (user)
            {
                StartRespawnTimer(user);
            }
        }

        private CharacterBody CharacterMaster_Respawn(On.RoR2.CharacterMaster.orig_Respawn orig, CharacterMaster self, Vector3 footPosition, Quaternion rotation, bool wasRevivedMidStage)
        {
            var user = UsersHelper.GetUser(self);
            if (user)
            {
                ResetRespawnTimer(user);
            }

            return orig(self, footPosition, rotation, wasRevivedMidStage);
        }

        private void Run_OnServerSceneChanged(On.RoR2.Run.orig_OnServerSceneChanged orig, Run self, string sceneName)
        {
            orig(self, sceneName);

            ResetAllRespawnTimers();
            ResumeAllRespawnTimers();
            if (CheckIfCurrentStageIsIgnoredForTimedRespawn())
            {
                StopAllRespawnTimers();
                ChatHelper.TimedRespawnBlockedOnStage();
            }
        }

        private void Stage_BeginAdvanceStage(On.RoR2.Stage.orig_BeginAdvanceStage orig, Stage self, SceneDef destinationStage)
        {
            orig(self, destinationStage);

            ResetAllRespawnTimers();
            ResumeAllRespawnTimers();
        }

        private bool CheckIfCurrentStageIsIgnoredForTimedRespawn()
        {
            return PluginConfig.IgnoredMapsForTimedRespawn.Value.Contains(SceneCatalog.GetSceneDefForCurrentScene().baseSceneName);
        }
    }
}
