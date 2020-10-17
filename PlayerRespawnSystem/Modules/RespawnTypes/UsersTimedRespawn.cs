using System.Collections.Generic;
using UnityEngine.Networking;

namespace Mordrog
{
    class UsersTimedRespawn : NetworkBehaviour
    {
        public UsersRespawnController respawnController;
        private UsersRespawnTimers usersRespawnTimers;

        public IReadOnlyDictionary<RoR2.NetworkUserId, UserTimer> readOnlyInstances => usersRespawnTimers.readOnlyInstances;

        private void Awake()
        {
            usersRespawnTimers = base.gameObject.AddComponent<UsersRespawnTimers>();

            usersRespawnTimers.OnUserTimerRespawnTimerEnd += UsersRespawnTimers_OnUserTimerRespawnTimerEnd;

            On.RoR2.Run.OnUserAdded += Run_OnUserAdded;
            On.RoR2.Run.OnUserRemoved += Run_OnUserRemoved;
            On.RoR2.Run.BeginGameOver += Run_BeginGameOver;
            On.RoR2.Run.OnDestroy += Run_OnDestroy;
        }

        public void OnDestroy()
        {
            usersRespawnTimers.OnUserTimerRespawnTimerEnd -= UsersRespawnTimers_OnUserTimerRespawnTimerEnd;

            On.RoR2.Run.OnUserAdded -= Run_OnUserAdded;
            On.RoR2.Run.OnUserRemoved -= Run_OnUserRemoved;
            On.RoR2.Run.BeginGameOver -= Run_BeginGameOver;
            On.RoR2.Run.OnDestroy -= Run_OnDestroy;
        }

        private void UsersRespawnTimers_OnUserTimerRespawnTimerEnd(RoR2.NetworkUser user)
        {
            respawnController.RespawnUser(user);
        }

        private void Run_OnUserAdded(On.RoR2.Run.orig_OnUserAdded orig, RoR2.Run self, RoR2.NetworkUser user)
        {
            orig(self, user);

            usersRespawnTimers.AddUserRespawnTimer(user.id);
        }

        private void Run_OnUserRemoved(On.RoR2.Run.orig_OnUserRemoved orig, RoR2.Run self, RoR2.NetworkUser user)
        {
            orig(self, user);

            usersRespawnTimers.RemoveUserRespawnTimer(user.id);
        }

        private void Run_BeginGameOver(On.RoR2.Run.orig_BeginGameOver orig, RoR2.Run self, RoR2.GameEndingDef gameEndingDef)
        {
            orig(self, gameEndingDef);

            usersRespawnTimers.ClearAllUsersRespawnTimers();
        }

        private void Run_OnDestroy(On.RoR2.Run.orig_OnDestroy orig, RoR2.Run self)
        {
            orig(self);

            usersRespawnTimers.ClearAllUsersRespawnTimers();
        }

        public void StartTimedRespawn(RoR2.NetworkUser user)
        {
            if (user)
            {
                var respawnTime = RespawnTimeCalculation.GetRespawnTime();

                usersRespawnTimers.StartRespawnTimer(user.id, respawnTime);
                ChatHelper.UserWillRespawnAfter(user.userName, respawnTime);
            }
        }

        public void ResetTimedRespawn(RoR2.NetworkUser user)
        {
            if (user)
            {
                usersRespawnTimers.ResetRespawnTimer(user.id);
            }
        }

        public void StopAllRespawnTimers()
        {
            usersRespawnTimers.StopAllCurrentRespawnTimers();
        }

        public void ResumeAllRespawnTimers()
        {
            usersRespawnTimers.ResumeAllCurrentRespawnTimers();
        }

        public void ResetAllRespawnTimers()
        {
            usersRespawnTimers.ResetAllCurrentRespawnTimers();
        }
    }
}
