using RoR2;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace Mordrog
{
    class UsersRespawnTimers : NetworkBehaviour
    {
        private Dictionary<NetworkUserId, UserTimer> usersRespawnTimers = new Dictionary<NetworkUserId, UserTimer>();

        public delegate void UserTimerRespawnTimerEnd(NetworkUser user);
        public event UserTimerRespawnTimerEnd OnUserTimerRespawnTimerEnd;

        public void StartRespawnTimer(NetworkUserId userId, uint respawnTime)
        {
            if (usersRespawnTimers.TryGetValue(userId, out var userTimer))
            {
                userTimer.StartTimer(respawnTime);
            }
        }

        public void InstantRespawnAll()
        {
            foreach (var userRespawnTimerEntry in usersRespawnTimers)
            {
                var user = UsersHelper.GetUser(userRespawnTimerEntry.Key);

                if (user)
                {
                    userRespawnTimerEntry.Value.Reset();
                    OnUserTimerRespawnTimerEnd?.Invoke(user);
                }
            }
        }

        public void StopAllCurrentRespawnTimers()
        {
            foreach (var userRespawnTimer in usersRespawnTimers.Values)
            {
                userRespawnTimer.Reset();
            }
        }

        public void AddUserRespawnTimer(NetworkUserId userId)
        {
            if (!usersRespawnTimers.ContainsKey(userId))
            {
                usersRespawnTimers[userId] = gameObject.AddComponent<UserTimer>();
                usersRespawnTimers[userId].UserId = userId;
                usersRespawnTimers[userId].OnRespawnInFiveSeconds += UsersRespawnTimers_OnRespawnInFiveSeconds;
                usersRespawnTimers[userId].OnTimerEnd += UsersRespawnTimers_OnTimerEnd;
            }
        }

        public void RemoveUserRespawnTimer(NetworkUserId userId)
        {
            if (usersRespawnTimers.TryGetValue(userId, out var userTimer))
            {
                userTimer.OnRespawnInFiveSeconds -= UsersRespawnTimers_OnRespawnInFiveSeconds;
                userTimer.OnTimerEnd -= UsersRespawnTimers_OnTimerEnd;
                DestroyImmediate(usersRespawnTimers[userId]);
                usersRespawnTimers.Remove(userId);
            }
        }

        public void ClearAllUsersRespawnTimers()
        {
            foreach (var userId in new List<NetworkUserId>(usersRespawnTimers.Keys))
            {
                RemoveUserRespawnTimer(userId);
            }
        }

        private void UsersRespawnTimers_OnRespawnInFiveSeconds(NetworkUserId userId)
        {
            var user = UsersHelper.GetUser(userId);

            if (user)
            {
                ChatHelper.UserWillRespawnAfter(user.userName, 5);
            }
        }

        private void UsersRespawnTimers_OnTimerEnd(NetworkUserId userId)
        {
            var user = UsersHelper.GetUser(userId);

            if (user)
            {
                OnUserTimerRespawnTimerEnd?.Invoke(user);
            }
        }
    }
}
