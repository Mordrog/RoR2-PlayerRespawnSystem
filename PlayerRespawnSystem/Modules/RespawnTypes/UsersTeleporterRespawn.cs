using UnityEngine.Networking;

namespace Mordrog
{
    class UsersTeleporterRespawn : NetworkBehaviour
    {
        public UsersRespawnController respawnController;

        public void Awake()
        {
            On.RoR2.TeleporterInteraction.ChargingState.OnEnter += TeleporterInteraction_ChargingState_OnEnter;
            On.RoR2.TeleporterInteraction.ChargedState.OnEnter += TeleporterInteraction_ChargedState_OnEnter;
        }

        public void OnDestroy()
        {
            On.RoR2.TeleporterInteraction.ChargingState.OnEnter -= TeleporterInteraction_ChargingState_OnEnter;
            On.RoR2.TeleporterInteraction.ChargedState.OnEnter -= TeleporterInteraction_ChargedState_OnEnter;
        }

        private void TeleporterInteraction_ChargingState_OnEnter(On.RoR2.TeleporterInteraction.ChargingState.orig_OnEnter orig, EntityStates.BaseState self)
        {
            orig(self);

            respawnController.RespawnType = RespawnType.Teleporter;

            if (PluginConfig.RespawnOnTPStart.Value)
            {
                respawnController.RespawnAllUsers();
            }

            if (PluginConfig.BlockTimedRespawnOnTPEvent.Value)
            {
                respawnController.BlockTimedRespawn();
                ChatHelper.RespawnBlockedOnTPEvent();
            }
        }

        private void TeleporterInteraction_ChargedState_OnEnter(On.RoR2.TeleporterInteraction.ChargedState.orig_OnEnter orig, EntityStates.BaseState self)
        {
            orig(self);

            if (PluginConfig.BlockTimedRespawnOnTPEvent.Value)
            {
                respawnController.UnblockTimedRespawn();
            }

            if (PluginConfig.RespawnOnTPEnd.Value)
            {
                respawnController.RespawnAllUsers();
            }
        }
    }
}
