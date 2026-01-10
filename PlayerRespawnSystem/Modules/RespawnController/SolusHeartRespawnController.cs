using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace PlayerRespawnSystem
{
    [AssociatedRespawnType(RespawnType.SolusHeart)]
    class SolusHeartRespawnController : RespawnController
    {
        public static new bool IsEnabled => true;

        public void Awake()
        {
            On.EntityStates.SolusHeart.Phase0.Mission0.OnEnter += SolusHeart_Phase0_Mission0_OnEnter;
            On.EntityStates.SolusHeart.Death.MissionCompleted.OnEnter += SolusHeart_Death_MissionCompleted_OnEnter; ;
            On.RoR2.Run.AdvanceStage += Run_AdvanceStage;
        }

        public void OnDestroy()
        {
            On.EntityStates.SolusHeart.Phase0.Mission0.OnEnter -= SolusHeart_Phase0_Mission0_OnEnter;
            On.EntityStates.SolusHeart.Death.MissionCompleted.OnEnter -= SolusHeart_Death_MissionCompleted_OnEnter;
            On.RoR2.Run.AdvanceStage -= Run_AdvanceStage;
        }

        private void SolusHeart_Phase0_Mission0_OnEnter(On.EntityStates.SolusHeart.Phase0.Mission0.orig_OnEnter orig, EntityStates.SolusHeart.Phase0.Mission0 self)
        {
            orig(self);
            IsActive = true;

            if (PluginConfig.RespawnOnSolusHeartStart.Value)
            {
                playerRespawner.RespawnAllUsers(this);
            }

            if (PluginConfig.BlockTimedRespawnOnSolusHeartFight.Value)
            {
                RequestTimedRespawnBlock();
            }
        }

        private void SolusHeart_Death_MissionCompleted_OnEnter(On.EntityStates.SolusHeart.Death.MissionCompleted.orig_OnEnter orig, EntityStates.SolusHeart.Death.MissionCompleted self)
        {
            orig(self);

            if (PluginConfig.BlockTimedRespawnOnSolusHeartFight.Value)
            {
                RequestTimedRespawnUnblock();
            }

            if (PluginConfig.RespawnOnSolusHeartEnd.Value)
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
