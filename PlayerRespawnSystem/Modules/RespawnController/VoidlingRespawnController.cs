using UnityEngine;

namespace PlayerRespawnSystem
{
    [AssociatedRespawnType(RespawnType.Voidling)]
    class VoidlingRespawnController : RespawnController
    {
        public static new bool IsEnabled => true;

        public void Awake()
        {
            On.RoR2.ScriptedCombatEncounter.BeginEncounter += ScriptedCombatEncounter_BeginEncounter;
            On.RoR2.VoidRaidGauntletController.SpawnOutroPortal += VoidRaidGauntletController_SpawnOutroPortal;
            On.RoR2.Run.AdvanceStage += Run_AdvanceStage;
        }

        public void OnDestroy()
        {
            On.RoR2.ScriptedCombatEncounter.BeginEncounter -= ScriptedCombatEncounter_BeginEncounter;
            On.RoR2.VoidRaidGauntletController.SpawnOutroPortal -= VoidRaidGauntletController_SpawnOutroPortal;
            On.RoR2.Run.AdvanceStage -= Run_AdvanceStage;
        }

        private void ScriptedCombatEncounter_BeginEncounter(On.RoR2.ScriptedCombatEncounter.orig_BeginEncounter orig, RoR2.ScriptedCombatEncounter self)
        {
            orig(self);
            if (IsActive || RoR2.SceneCatalog.GetSceneDefForCurrentScene().baseSceneName.ToLower() != "voidraid")
            {
                return;
            }

            IsActive = true;

            if (PluginConfig.RespawnOnVoidlingStart.Value)
            {
                playerRespawner.RespawnAllUsers(this);
            }

            if (PluginConfig.BlockTimedRespawnOnVoidlingFight.Value)
            {
                RequestTimedRespawnBlock();
            }
        }

        private void VoidRaidGauntletController_SpawnOutroPortal(On.RoR2.VoidRaidGauntletController.orig_SpawnOutroPortal orig, RoR2.VoidRaidGauntletController self)
        {
            orig(self);

            if (PluginConfig.BlockTimedRespawnOnVoidlingFight.Value)
            {
                RequestTimedRespawnUnblock();
            }

            if (PluginConfig.RespawnOnVoidlingEnd.Value)
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
            respawnTransform.position = RespawnPosition.GetSpawnPositionForVoidBoss();

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
