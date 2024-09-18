﻿using RoR2;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace PlayerRespawnSystem
{
    class PlayerRespawnSystem : NetworkBehaviour
    {
        public static PlayerRespawnSystem instance { get; private set; }

        private Dictionary<RespawnType, RespawnController> respawnControllers = new Dictionary<RespawnType, RespawnController>();
        public IReadOnlyDictionary<RespawnType, RespawnController> RespawnControllers => respawnControllers;

        private PlayerRespawner playerRespawner;

        public bool CheckIfUserCanBeRespawned(NetworkUser user)
        {
            return playerRespawner.CheckIfUserCanBeRespawned(user);
        }

        public void BlockTimedRespawn()
        {
            var timedRespawnController = respawnControllers[RespawnType.Timed] as TimedRespawnController;
            timedRespawnController.StopAllRespawnTimers();
        }

        public void UnblockTimedRespawn()
        {
            var timedRespawnController = respawnControllers[RespawnType.Timed] as TimedRespawnController;
            timedRespawnController.ResumeAllRespawnTimers();
        }

        protected void OnEnable()
        {
            PlayerRespawnSystem.instance = SingletonHelper.Assign<PlayerRespawnSystem>(PlayerRespawnSystem.instance, this);
        }

        protected void OnDisable()
        {
            PlayerRespawnSystem.instance = SingletonHelper.Unassign<PlayerRespawnSystem>(PlayerRespawnSystem.instance, this);
        }

        public void Awake()
        {
            playerRespawner = gameObject.AddComponent<PlayerRespawner>();

            foreach (var (respawnType, respawnControllerType) in RespawnController.GetRespawnControllerTypes())
            {
                respawnControllers[respawnType] = (RespawnController)gameObject.AddComponent(respawnControllerType);
                respawnControllers[respawnType].Init(playerRespawner);
                respawnControllers[respawnType].OnRequestTimedRespawnBlock += BlockTimedRespawn;
                respawnControllers[respawnType].OnRequestTimedRespawnUnblock += UnblockTimedRespawn;
            }

            On.RoR2.Stage.OnEnable += Stage_OnEnable;
            On.RoR2.PlayerCharacterMasterController.OnBodyDeath += PlayerCharacterMasterController_OnBodyDeath;
            On.RoR2.SceneExitController.SetState += SceneExitController_SetState;
            On.RoR2.Run.OnServerSceneChanged += Run_OnServerSceneChanged;
        }

        public void OnDestroy()
        {
            foreach (var (respawnType, respawnControllerType) in respawnControllers)
            {
                respawnControllers[respawnType].OnRequestTimedRespawnBlock -= BlockTimedRespawn;
                respawnControllers[respawnType].OnRequestTimedRespawnUnblock -= UnblockTimedRespawn;
                Destroy(respawnControllers[respawnType]);
            }

            Destroy(playerRespawner);

            On.RoR2.Stage.OnEnable -= Stage_OnEnable;
            On.RoR2.PlayerCharacterMasterController.OnBodyDeath -= PlayerCharacterMasterController_OnBodyDeath;
            On.RoR2.SceneExitController.SetState -= SceneExitController_SetState;
            On.RoR2.Run.OnServerSceneChanged -= Run_OnServerSceneChanged;
        }

        private void Stage_OnEnable(On.RoR2.Stage.orig_OnEnable orig, Stage self)
        {
            orig(self);

            if (!PluginConfig.UsePodsOnStartOfMatch.Value)
            {
                Stage.instance.usePod = false;
            }
        }

        private void PlayerCharacterMasterController_OnBodyDeath(On.RoR2.PlayerCharacterMasterController.orig_OnBodyDeath orig, PlayerCharacterMasterController self)
        {
            orig(self);

            var user = UsersHelper.GetUser(self.master);
            if (user && !respawnControllers[RespawnType.Timed].IsActive)
            {
                if (respawnControllers[RespawnType.Teleporter].IsActive && PluginConfig.RespawnOnTPEnd.Value)
                {
                    ChatHelper.UserWillRespawnAfterTPEvent(user.userName);
                }
                if (respawnControllers[RespawnType.Mithrix].IsActive && PluginConfig.RespawnOnMithrixEnd.Value)
                {
                    ChatHelper.UserWillRespawnAfterMithrixFight(user.userName);
                }
                if (respawnControllers[RespawnType.Voidling].IsActive && PluginConfig.RespawnOnVoidlingEnd.Value)
                {
                    ChatHelper.UserWillRespawnAfterVoidlingFight(user.userName);
                }
                if (respawnControllers[RespawnType.FalseSon].IsActive && PluginConfig.RespawnOnFalseSonEnd.Value)
                {
                    ChatHelper.UserWillRespawnAfterFalseSonFight(user.userName);
                }
            }
        }

        private void SceneExitController_SetState(On.RoR2.SceneExitController.orig_SetState orig, SceneExitController self, SceneExitController.ExitState newState)
        {
            orig(self, newState);

            if (newState == SceneExitController.ExitState.TeleportOut)
            {
                playerRespawner.IsAdvancingStage = true;
            }
        }

        private void Run_OnServerSceneChanged(On.RoR2.Run.orig_OnServerSceneChanged orig, Run self, string sceneName)
        {
            orig(self, sceneName);

            playerRespawner.IsAdvancingStage = false;
        }
    }
}