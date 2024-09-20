using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace PlayerRespawnSystem
{
    class UIDeathTimerController : NetworkBehaviour
    {
        private UIDeathTimerPanel deathTimerPanel;

        public readonly float UpdateUIEveryXSeconds = 0.5f;
        private float deltaCount = 0.0f;

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

            deathTimerPanel = deathTimerGameobject.AddComponent<UIDeathTimerPanel>();
        }

        private void Run_OnDestroy(On.RoR2.Run.orig_OnDestroy orig, RoR2.Run self)
        {
            orig(self);

            Destroy(deathTimerPanel);
        }

        public void Update()
        {
            if (NetworkServer.active && hasAuthority)
            {
                deltaCount += Time.deltaTime;
                if (deltaCount >= UpdateUIEveryXSeconds)
                {
                    CmdUpdateAllDeathTimers();
                    deltaCount = 0.0f;
                }
            }
        }

        [Command]
        public void CmdUpdateAllDeathTimers()
        {
            if (PlayerRespawnSystem.instance && PlayerRespawnSystem.instance.RespawnControllers.ContainsKey(RespawnType.Timed))
            {
                RespawnType activeRespawnType = RespawnType.Timed;
                TimedRespawnController timedRespawnController = null;

                foreach (var (respawnType, respawnController) in PlayerRespawnSystem.instance.RespawnControllers)
                {
                    if (respawnController is TimedRespawnController)
                    {
                        timedRespawnController = respawnController as TimedRespawnController;
                    }
                    else if (respawnController.IsActive)
                    {
                        activeRespawnType = respawnType;
                    }
                }

                if (timedRespawnController)
                {
                    foreach (var user in NetworkUser.readOnlyInstancesList)
                    {
                        if (timedRespawnController.UserRespawnTimers.TryGetValue(user.id, out var userTimer))
                        {
                            float respawnTime = userTimer.TimeRemaining;
                            bool canRespawn = PlayerRespawnSystem.instance.CheckIfUserCanBeRespawned(user);
                            bool canTimedRespawn = canRespawn && timedRespawnController.IsActive;

                            TargetUpdateDeathTimer(user.connectionToClient, respawnTime, canRespawn, canTimedRespawn, activeRespawnType);
                        }
                    }
                }
            }
        }

        [TargetRpc]
        public void TargetUpdateDeathTimer(NetworkConnection target, float respawnTime, bool canRespawn, bool canTimedRespawn, RespawnType activeRespawnType)
        {
            if (!deathTimerPanel || !PluginConfig.UseDeathTimerUI.Value)
            {
                return;
            }

            if (canRespawn)
            {
                if (canTimedRespawn)
                {
                    deathTimerPanel.textContext2.text = $"in <color=red>{Mathf.CeilToInt(respawnTime)}</color> seconds";
                    deathTimerPanel.show = true;
                }
                else
                {
                    switch (activeRespawnType)
                    {
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
                        default:
                            deathTimerPanel.show = false;
                            break;
                    }
                }
            }
            else
            {
                deathTimerPanel.show = false;
            }
        }
    }
}
