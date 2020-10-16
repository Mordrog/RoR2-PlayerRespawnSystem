using UnityEngine.Networking;

namespace Mordrog
{
    class UsersMithrixRespawn : NetworkBehaviour
    {
        public UsersRespawnController respawnController;

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

            respawnController.RespawnType = RespawnType.Mithrix;

            if (PluginConfig.RespawnOnMithrixStart.Value)
            {
                respawnController.RespawnAllUsers();
            }

            if (PluginConfig.BlockTimedRespawnOnMithrixFight.Value)
            {
                respawnController.BlockTimedRespawn();
            }
        }

        private void BrotherEncounter_EncounterFinished_OnEnter(On.EntityStates.Missions.BrotherEncounter.EncounterFinished.orig_OnEnter orig, EntityStates.Missions.BrotherEncounter.EncounterFinished self)
        {
            orig(self);

            if (PluginConfig.BlockTimedRespawnOnMithrixFight.Value)
            {
                respawnController.UnblockTimedRespawn();
            }

            if (PluginConfig.RespawnOnMithrixEnd.Value)
            {
                respawnController.RespawnAllUsers();
            }
        }
    }
}
