using UnityEngine;

namespace PlayerRespawnSystem
{
    [AssociatedRespawnType(RespawnType.FalseSon)]
    class FalseSonRespawnController : RespawnController
    {
        public static new bool IsEnabled => true;

        public void Awake()
        {
            On.EntityStates.MeridianEvent.MeridianEventStart.OnEnter += MeridianEventStart_OnEnter;
            On.EntityStates.MeridianEvent.MeridianEventCleared.OnEnter += MeridianEventCleared_OnEnter;
            On.RoR2.Run.AdvanceStage += Run_AdvanceStage;
        }

        public void OnDestroy()
        {
            On.EntityStates.MeridianEvent.MeridianEventStart.OnEnter -= MeridianEventStart_OnEnter;
            On.EntityStates.MeridianEvent.MeridianEventCleared.OnEnter -= MeridianEventCleared_OnEnter;
            On.RoR2.Run.AdvanceStage -= Run_AdvanceStage;
        }

        private void MeridianEventStart_OnEnter(On.EntityStates.MeridianEvent.MeridianEventStart.orig_OnEnter orig, EntityStates.MeridianEvent.MeridianEventStart self)
        {
            orig(self);

            IsActive = true;

            if (PluginConfig.RespawnOnFalseSonStart.Value)
            {
                playerRespawner.RespawnAllUsers(this);
            }

            if (PluginConfig.BlockTimedRespawnOnFalseSonFight.Value)
            {
                RequestTimedRespawnBlock();
            }
        }

        private void MeridianEventCleared_OnEnter(On.EntityStates.MeridianEvent.MeridianEventCleared.orig_OnEnter orig, EntityStates.MeridianEvent.MeridianEventCleared self)
        {
            orig(self);

            if (PluginConfig.BlockTimedRespawnOnFalseSonFight.Value)
            {
                RequestTimedRespawnUnblock();
            }

            if (PluginConfig.RespawnOnFalseSonEnd.Value)
            {
                playerRespawner.RespawnAllUsers(this);
            }

            IsActive = false;
        }

        private void Run_AdvanceStage(On.RoR2.Run.orig_AdvanceStage orig, RoR2.Run self, RoR2.SceneDef nextScene)
        {
            orig(self, nextScene);
            IsActive = false;
        }

        public override bool GetRespawnTransform(RoR2.CharacterBody body, out Transform outRespawnTransform)
        {
            Transform respawnTransform = new GameObject().transform;
            respawnTransform.position = RespawnPosition.GetSpawnPositionForStormBoss();

            if (respawnTransform.position != Vector3.zero)
            {
                outRespawnTransform = respawnTransform;
                return true;
            }

            Debug.Log($"GetRespawnTransform: Failed to find better respawn position for '{GetRespawnType()}' respawn type");
            outRespawnTransform = null;
            return false;
        }
    }
}
