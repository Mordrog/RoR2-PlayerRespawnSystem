namespace PlayerRespawnSystem
{
    [AssociatedRespawnType(RespawnType.Artifact)]
    class ArtifactTrialRespawnController : RespawnController
    {
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

            IsActive = true;

            if (PluginConfig.RespawnOnArtifactTrialStart.Value)
            {
                playerRespawner.RespawnAllUsers(this);
            }

            if (PluginConfig.BlockTimedRespawnOnArtifactTrial.Value)
            {
                RequestTimedRespawnBlock();
            }
        }

        private void CombatState_OnExit(On.RoR2.ArtifactTrialMissionController.CombatState.orig_OnExit orig, EntityStates.EntityState self)
        {
            orig(self);

            if (PluginConfig.BlockTimedRespawnOnArtifactTrial.Value)
            {
                RequestTimedRespawnUnblock();
            }

            if (PluginConfig.RespawnOnArtifactTrialEnd.Value)
            {
                playerRespawner.RespawnAllUsers(this);
            }

            IsActive = false;
        }
    }
}
