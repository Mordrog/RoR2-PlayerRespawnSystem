using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace PlayerRespawnSystem
{
    [AssociatedRespawnType(RespawnType.SolusWing)]
    class SolusWingRespawnController : RespawnController
    {
        public static new bool IsEnabled => true;

        public void Awake()
        {
            On.EntityStates.SolusWing.SpawnState.OnEnter += SolusWing_SpawnState_OnEnter;
            On.EntityStates.SolusWing.DeathState.OnEnter += SolusWing_DeathState_OnEnter;
            On.RoR2.Run.AdvanceStage += Run_AdvanceStage;
        }

        private void DeathState_OnEnter(On.EntityStates.SolusWing.DeathState.orig_OnEnter orig, EntityStates.SolusWing.DeathState self)
        {
            throw new NotImplementedException();
        }

        public void OnDestroy()
        {
            On.EntityStates.SolusWing.SpawnState.OnEnter -= SolusWing_SpawnState_OnEnter;
            On.EntityStates.SolusWing.DeathState.OnEnter -= SolusWing_DeathState_OnEnter;
            On.RoR2.Run.AdvanceStage -= Run_AdvanceStage;
        }

        private void SolusWing_SpawnState_OnEnter(On.EntityStates.SolusWing.SpawnState.orig_OnEnter orig, EntityStates.SolusWing.SpawnState self)
        {
            orig(self);
            IsActive = true;

            if (PluginConfig.RespawnOnSolusWingStart.Value)
            {
                playerRespawner.RespawnAllUsers(this);
            }

            if (PluginConfig.BlockTimedRespawnOnSolusWingFight.Value)
            {
                RequestTimedRespawnBlock();
            }
        }

        private void SolusWing_DeathState_OnEnter(On.EntityStates.SolusWing.DeathState.orig_OnEnter orig, EntityStates.SolusWing.DeathState self)
        {
            orig(self);

            if (PluginConfig.BlockTimedRespawnOnSolusWingFight.Value)
            {
                RequestTimedRespawnUnblock();
            }

            if (PluginConfig.RespawnOnSolusWingEnd.Value)
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
    }
}
