using UnityEngine;

namespace PlayerRespawnSystem
{
    [AssociatedRespawnType(RespawnType.Mithrix)]
    class MithrixRespawnController : RespawnController
    {
        public static new bool IsEnabled => true;

        public void Awake()
        {
            On.EntityStates.Missions.BrotherEncounter.Phase1.OnEnter += BrotherEncounter_Phase1_OnEnter;
            On.EntityStates.Missions.BrotherEncounter.EncounterFinished.OnEnter += BrotherEncounter_EncounterFinished_OnEnter;
        }

        public void OnDestroy()
        {
            On.EntityStates.Missions.BrotherEncounter.Phase1.OnEnter -= BrotherEncounter_Phase1_OnEnter;
            On.EntityStates.Missions.BrotherEncounter.EncounterFinished.OnEnter -= BrotherEncounter_EncounterFinished_OnEnter;
        }

        private void BrotherEncounter_Phase1_OnEnter(On.EntityStates.Missions.BrotherEncounter.Phase1.orig_OnEnter orig, EntityStates.Missions.BrotherEncounter.Phase1 self)
        {
            orig(self);

            IsActive = true;

            if (PluginConfig.RespawnOnMithrixStart.Value)
            {
                playerRespawner.RespawnAllUsers(this);
            }

            if (PluginConfig.BlockTimedRespawnOnMithrixFight.Value)
            {
                RequestTimedRespawnBlock();
            }
        }

        private void BrotherEncounter_EncounterFinished_OnEnter(On.EntityStates.Missions.BrotherEncounter.EncounterFinished.orig_OnEnter orig, EntityStates.Missions.BrotherEncounter.EncounterFinished self)
        {
            orig(self);

            if (PluginConfig.BlockTimedRespawnOnMithrixFight.Value)
            {
                RequestTimedRespawnUnblock();
            }

            if (PluginConfig.RespawnOnMithrixEnd.Value)
            {
                playerRespawner.RespawnAllUsers(this);
            }

            IsActive = false;
        }

        public override bool GetRespawnTransform(RoR2.CharacterBody body, out Transform outRespawnTransform)
        {
            Transform respawnTransform = new GameObject().transform;
            respawnTransform.position = RespawnPosition.GetSpawnPositionAroundMoonBoss(body, 100, 105);

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
