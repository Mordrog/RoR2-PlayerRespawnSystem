﻿using UnityEngine;

namespace PlayerRespawnSystem
{
    [AssociatedRespawnType(RespawnType.Teleporter)]
    class TeleporterRespawnController : RespawnController
    {
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

            IsActive = true;

            if (PluginConfig.RespawnOnTPStart.Value)
            {
                playerRespawner.RespawnAllUsers(this);
            }

            if (PluginConfig.BlockTimedRespawnOnTPEvent.Value)
            {
                RequestTimedRespawnBlock();
                ChatHelper.RespawnBlockedOnTPEvent();
            }
        }

        private void TeleporterInteraction_ChargedState_OnEnter(On.RoR2.TeleporterInteraction.ChargedState.orig_OnEnter orig, EntityStates.BaseState self)
        {
            orig(self);

            if (PluginConfig.BlockTimedRespawnOnTPEvent.Value)
            {
                RequestTimedRespawnUnblock();
            }

            if (PluginConfig.RespawnOnTPEnd.Value)
            {
                playerRespawner.RespawnAllUsers(this);
            }

            IsActive = false;
        }

        public override bool GetRespawnTransform(RoR2.CharacterBody body, out Transform outRespawnTransform)
        {
            Transform respawnTransform = new GameObject().transform;
            respawnTransform.position = RespawnPosition.GetSpawnPositionAroundTeleporter(body, 0.5f, 3);

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