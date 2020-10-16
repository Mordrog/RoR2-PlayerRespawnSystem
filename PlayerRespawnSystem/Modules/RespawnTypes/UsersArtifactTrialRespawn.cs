using UnityEngine.Networking;

namespace Mordrog
{
    class UsersArtifactTrialRespawn : NetworkBehaviour
    {
        public UsersRespawnController respawnController;

        public void Awake()
        {
            On.RoR2.ArtifactTrialMissionController.CombatState.OnEnter += ArtifactTrialMissionController_CombatState_OnEnter;
            On.RoR2.ArtifactTrialMissionController.CombatState.OnExit += CombatState_OnExit;
        }

        public void OnDestroy()
        {
            On.RoR2.ArtifactTrialMissionController.CombatState.OnEnter -= ArtifactTrialMissionController_CombatState_OnEnter;
            On.RoR2.ArtifactTrialMissionController.CombatState.OnExit -= CombatState_OnExit;
        }

        private void ArtifactTrialMissionController_CombatState_OnEnter(On.RoR2.ArtifactTrialMissionController.CombatState.orig_OnEnter orig, EntityStates.EntityState self)
        {
            orig(self);

            respawnController.RespawnType = RespawnType.Artifact;

            if (PluginConfig.RespawnOnArtifactTrialStart.Value)
            {
                respawnController.RespawnAllUsers();
            }

            if (PluginConfig.BlockTimedRespawnOnArtifactTrial.Value)
            {
                respawnController.BlockTimedRespawn();
            }
        }

        private void CombatState_OnExit(On.RoR2.ArtifactTrialMissionController.CombatState.orig_OnExit orig, EntityStates.EntityState self)
        {
            orig(self);

            if (PluginConfig.BlockTimedRespawnOnArtifactTrial.Value)
            {
                respawnController.UnblockTimedRespawn();
            }

            if (PluginConfig.RespawnOnArtifactTrialEnd.Value)
            {
                respawnController.RespawnAllUsers();
            }
        }
    }
}
