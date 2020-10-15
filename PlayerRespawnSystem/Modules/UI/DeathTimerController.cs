using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace Mordrog
{
    class DeathTimerController : NetworkBehaviour
    {
        private DeathTimerPanel deathTimerPanel;

        public readonly int UpdateTimersEveryXFrames = 5;

        private int frameCount = 0;

        public void Awake()
        {
            On.RoR2.UI.HUD.Awake += HUD_Awake;
            On.RoR2.Run.OnDestroy += Run_OnDestroy;
        }

        public void OnDestroy()
        {
            On.RoR2.UI.HUD.Awake -= HUD_Awake;
            On.RoR2.Run.OnDestroy -= Run_OnDestroy;
        }

        private void HUD_Awake(On.RoR2.UI.HUD.orig_Awake orig, RoR2.UI.HUD self)
        {
            orig(self);

            var deathTimerGameobject = new GameObject("death_timer_box");
            deathTimerGameobject.transform.SetParent(self.mainContainer.transform);
            deathTimerGameobject.transform.SetAsFirstSibling();

            deathTimerPanel = deathTimerGameobject.AddComponent<DeathTimerPanel>();
        }

        private void Run_OnDestroy(On.RoR2.Run.orig_OnDestroy orig, RoR2.Run self)
        {
            orig(self);

            deathTimerPanel = null;
        }

        public void FixedUpdate()
        {
            if (NetworkServer.active && hasAuthority)
            {
                frameCount++;
                if (frameCount >= UpdateTimersEveryXFrames)
                {
                    CmdUpdateAllDeathTimers();
                    frameCount = 0;
                }
            }
        }

        [Command]
        public void CmdUpdateAllDeathTimers()
        {
            if (UsersRespawnController.instance)
            {
                foreach (var user in NetworkUser.readOnlyInstancesList)
                {
                    if (UsersRespawnController.instance.usersTimedRespawn.readOnlyInstances.TryGetValue(user.id, out var userTimer))
                    {
                        float respawnTime = userTimer.TimeRemaining;
                        bool canTimedRespawn = UsersRespawnController.instance.CheckIfCanTimedRespawn(user?.master);
                        bool canRespawnAfter = UsersRespawnController.instance.CheckIfCanRespawn(user?.master) && UsersRespawnController.instance.IsTimedRespawnBlocked;
                        RespawnType respawnType = UsersRespawnController.instance.RespawnType;

                        TargetUpdateDeathTimer(user.connectionToClient, respawnTime, canTimedRespawn, canRespawnAfter, respawnType);
                    }
                }
            }
        }

        [TargetRpc]
        public void TargetUpdateDeathTimer(NetworkConnection target, float respawnTime, bool canTimedRespawn, bool canRespawnAfter, RespawnType respawnType)
        {
            if (deathTimerPanel && PluginConfig.UseDeathTimerUI.Value)
            {
                if (canTimedRespawn)
                {
                    deathTimerPanel.show = true;

                    deathTimerPanel.textContext2.text = $"in <color=red>{Mathf.CeilToInt(respawnTime)}</color> seconds";
                }
                else if (canRespawnAfter)
                {
                    switch (respawnType)
                    {
                        case RespawnType.Default:
                            deathTimerPanel.show = false;
                            break;

                        case RespawnType.Teleporter:
                            if (PluginConfig.RespawnOnTPEnd.Value)
                            {
                                deathTimerPanel.textContext2.text = $"after <color=red>teleporter</color> event";
                                deathTimerPanel.show = true;
                            }
                            break;

                        case RespawnType.Mithrix:
                            if (PluginConfig.RespawnOnMithrixEnd.Value)
                            {
                                deathTimerPanel.textContext2.text = $"after <color=red>Mithrix</color> fight";
                                deathTimerPanel.show = true;
                            }
                            break;

                        case RespawnType.Artifact:
                            if (PluginConfig.RespawnOnArtifactTrialEnd.Value)
                            {
                                deathTimerPanel.textContext2.text = $"after <color=red>artifact trial</color> ends";
                                deathTimerPanel.show = true;
                            }
                            break;
                    }
                }
                else
                {
                    deathTimerPanel.show = false;
                }
            }
        }
    }
}
