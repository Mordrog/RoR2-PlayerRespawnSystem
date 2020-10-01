using RoR2;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Mordrog
{
    class UsersRespawnController : NetworkBehaviour
    {
        private UsersRespawnTimers usersRespawnTimers;

        public bool blockRespawningOnTP = false;
        public bool respawnNearTP = false;

        public Queue<CharacterBody> respawnCharacterMaster = new Queue<CharacterBody>();

        public void Awake()
        {
            usersRespawnTimers = base.gameObject.AddComponent<UsersRespawnTimers>();

            usersRespawnTimers.OnUserTimerRespawnTimerEnd += UsersRespawnTimers_OnUserTimerRespawnTimerEnd;

            On.RoR2.TeleporterInteraction.ChargingState.OnEnter += TeleporterInteraction_ChargingState_OnEnter;
            On.RoR2.TeleporterInteraction.ChargedState.OnEnter += TeleporterInteraction_ChargedState_OnEnter;
            On.RoR2.Stage.Start += Stage_Start;
            On.RoR2.Stage.BeginAdvanceStage += Stage_BeginAdvanceStage;
            On.RoR2.Stage.RespawnCharacter += Stage_RespawnCharacter;
            On.RoR2.Stage.GetPlayerSpawnTransform += Stage_GetPlayerSpawnTransform;
            On.RoR2.PlayerCharacterMasterController.OnBodyDeath += PlayerCharacterMasterController_OnBodyDeath;
            On.RoR2.Run.OnUserAdded += Run_OnUserAdded;
            On.RoR2.Run.OnUserRemoved += Run_OnUserRemoved;
            On.RoR2.Run.BeginGameOver += Run_BeginGameOver;
            On.RoR2.Run.OnDestroy += Run_OnDestroy;
            On.RoR2.Run.OnServerSceneChanged += Run_OnServerSceneChanged;
        }

        private void UsersRespawnTimers_OnUserTimerRespawnTimerEnd(NetworkUser user)
        {
            if (CheckIfCanRespawn(user.master))
            {
                Stage.instance.RespawnCharacter(user.master);
            }
        }

        private void TeleporterInteraction_ChargingState_OnEnter(On.RoR2.TeleporterInteraction.ChargingState.orig_OnEnter orig, EntityStates.BaseState self)
        {
            orig(self);

            respawnNearTP = true;

            if (PluginConfig.RespawnOnTPStart.Value)
            {
                usersRespawnTimers.InstantRespawnAll();
            }

            if (PluginConfig.BlockRespawningOnTPEvent.Value)
            {
                usersRespawnTimers.StopAllCurrentRespawnTimers();
                blockRespawningOnTP = true;
                ChatHelper.RespawnBlockedOnTPEvent();
            }
        }

        private void TeleporterInteraction_ChargedState_OnEnter(On.RoR2.TeleporterInteraction.ChargedState.orig_OnEnter orig, EntityStates.BaseState self)
        {
            orig(self);

            blockRespawningOnTP = false;

            if (PluginConfig.RespawnOnTPEnd.Value)
            {
                usersRespawnTimers.InstantRespawnAll();
            }
        }

        private void Stage_Start(On.RoR2.Stage.orig_Start orig, Stage self)
        {
            orig(self);

            Stage.instance.usePod = false;
        }

        private void Stage_BeginAdvanceStage(On.RoR2.Stage.orig_BeginAdvanceStage orig, Stage self, SceneDef destinationStage)
        {
            orig(self, destinationStage);

            respawnNearTP = false;
            usersRespawnTimers.StopAllCurrentRespawnTimers();
        }

        private void Stage_RespawnCharacter(On.RoR2.Stage.orig_RespawnCharacter orig, Stage self, CharacterMaster characterMaster)
        {
            CharacterBody body = characterMaster?.bodyPrefab?.GetComponent<CharacterBody>();

            if (body)
            {
                respawnCharacterMaster.Enqueue(body);
            }

            orig(self, characterMaster);
        }

        private Transform Stage_GetPlayerSpawnTransform(On.RoR2.Stage.orig_GetPlayerSpawnTransform orig, Stage self)
        {
            if (respawnNearTP && respawnCharacterMaster.Count > 0)
            {
                var body = respawnCharacterMaster.Dequeue();
                var spawnTransform = new GameObject().transform;
                spawnTransform.position = TeleporterSpawnPosition.GetSpawnPositionAroundTeleporter(body, 3);

                return spawnTransform;
            }
            else
            {
                return orig(self);
            }
        }

        private void PlayerCharacterMasterController_OnBodyDeath(On.RoR2.PlayerCharacterMasterController.orig_OnBodyDeath orig, PlayerCharacterMasterController self)
        {
            orig(self);

            var user = UsersHelper.GetUser(self.master);

            if (user)
            {
                if (CheckIfCanRespawn(self.master) && PluginConfig.UseTimeRespawn.Value)
                {
                    var respawnTime = RespawnTimeCalculation.GetRespawnTime();

                    usersRespawnTimers.StartRespawnTimer(user.id, respawnTime);
                    ChatHelper.UserWillRespawnAfter(user.userName, respawnTime);
                }
                else if (blockRespawningOnTP && PluginConfig.RespawnOnTPEnd.Value)
                {
                    ChatHelper.UserWillRespawnAfterTPEvent(user.userName);
                }
            }
        }

        private void Run_OnUserAdded(On.RoR2.Run.orig_OnUserAdded orig, Run self, NetworkUser user)
        {
            orig(self, user);

            usersRespawnTimers.AddUserRespawnTimer(user.id);
        }

        private void Run_OnUserRemoved(On.RoR2.Run.orig_OnUserRemoved orig, Run self, NetworkUser user)
        {
            orig(self, user);

            usersRespawnTimers.RemoveUserRespawnTimer(user.id);
        }

        private void Run_BeginGameOver(On.RoR2.Run.orig_BeginGameOver orig, Run self, GameEndingDef gameEndingDef)
        {
            orig(self, gameEndingDef);

            blockRespawningOnTP = false;
            respawnNearTP = false;
            usersRespawnTimers.ClearAllUsersRespawnTimers();
        }

        private void Run_OnDestroy(On.RoR2.Run.orig_OnDestroy orig, Run self)
        {
            orig(self);

            blockRespawningOnTP = false;
            respawnNearTP = false;
            usersRespawnTimers.ClearAllUsersRespawnTimers();
        }

        private void Run_OnServerSceneChanged(On.RoR2.Run.orig_OnServerSceneChanged orig, Run self, string sceneName)
        {
            orig(self, sceneName);

            if (CheckIfCurrentStageIsIgnored())
            {
                ChatHelper.RespawnBlockedOnStage();
            }
        }

        private bool CheckIfCanRespawn(CharacterMaster master)
        {
            return master &&
                   master.IsDeadAndOutOfLivesServer() &&
                   !CheckIfCurrentStageIsIgnored() &&
                   !blockRespawningOnTP;
        }

        private bool CheckIfCurrentStageIsIgnored()
        {
            return PluginConfig.IgnoredMaps.Value.Contains(SceneCatalog.GetSceneDefForCurrentScene().baseSceneName);
        }
    }
}
