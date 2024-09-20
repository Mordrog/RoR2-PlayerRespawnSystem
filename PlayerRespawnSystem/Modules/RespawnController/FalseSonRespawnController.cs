using UnityEngine;

namespace PlayerRespawnSystem
{
    [AssociatedRespawnType(RespawnType.FalseSon)]
    class FalseSonRespawnController : RespawnController
    {
        public static new bool IsEnabled => true;

        public void Awake()
        {
            On.RoR2.MeridianEventTriggerInteraction.MeridianEventStart.OnEnter += MeridianEventStart_OnEnter;
            On.RoR2.MeridianEventTriggerInteraction.MeridianEventCleared.OnEnter += MeridianEventCleared_OnEnter;
        }

        public void OnDestroy()
        {
            On.RoR2.MeridianEventTriggerInteraction.MeridianEventStart.OnEnter -= MeridianEventStart_OnEnter;
            On.RoR2.MeridianEventTriggerInteraction.MeridianEventCleared.OnEnter -= MeridianEventCleared_OnEnter;
        }

        private void MeridianEventStart_OnEnter(On.RoR2.MeridianEventTriggerInteraction.MeridianEventStart.orig_OnEnter orig, RoR2.MeridianEventTriggerInteraction.MeridianEventStart self)
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

        private void MeridianEventCleared_OnEnter(On.RoR2.MeridianEventTriggerInteraction.MeridianEventCleared.orig_OnEnter orig, RoR2.MeridianEventTriggerInteraction.MeridianEventCleared self)
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
