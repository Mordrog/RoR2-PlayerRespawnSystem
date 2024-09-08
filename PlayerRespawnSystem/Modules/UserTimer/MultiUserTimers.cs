using RoR2;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace PlayerRespawnSystem
{
    class MultiUserTimers : NetworkBehaviour
    {
        private Dictionary<NetworkUserId, UserTimer> userTimers = new Dictionary<NetworkUserId, UserTimer>();
        public IReadOnlyDictionary<NetworkUserId, UserTimer> UserTimers => userTimers;

        public delegate void UserTimerEndInFiveSeconds(NetworkUser user);
        public event UserTimerEndInFiveSeconds OnUserTimerEndInFiveSeconds;

        public delegate void UserTimerEnd(NetworkUser user);
        public event UserTimerEnd OnUserTimerEnd;

        public void OnDestroy()
        {
            ClearTimers();
        }

        public void StartTimer(NetworkUserId userId, uint time)
        {
            if (userTimers.TryGetValue(userId, out var userTimer))
            {
                userTimer.StartTimer(time);
            }
        }

        public void ResetTimer(NetworkUserId userId)
        {
            if (userTimers.TryGetValue(userId, out var userTimer))
            {
                userTimer.Reset();
            }
        }

        public void StopTimers()
        {
            foreach (var userTimer in userTimers.Values)
            {
                userTimer.Stop();
            }
        }

        public void ResumeTimers()
        {
            foreach (var userTimer in userTimers.Values)
            {
                userTimer.Resume();
            }
        }

        public void ResetTimers()
        {
            foreach (var userTimer in userTimers.Values)
            {
                userTimer.Reset();
            }
        }

        public void ClearTimers()
        {
            foreach (var userId in new List<NetworkUserId>(userTimers.Keys))
            {
                RemoveTimer(userId);
            }
        }

        public void AddTimer(NetworkUserId userId)
        {
            if (!userTimers.ContainsKey(userId))
            {
                userTimers[userId] = gameObject.AddComponent<UserTimer>();
                userTimers[userId].UserId = userId;
                userTimers[userId].OnTimerEndInFiveSeconds += UserTimer_OnTimerEndInFiveSeconds;
                userTimers[userId].OnTimerEnd += UserTimer_OnTimerEnd;
            }
        }

        public void RemoveTimer(NetworkUserId userId)
        {
            if (userTimers.TryGetValue(userId, out var userTimer))
            {
                userTimer.OnTimerEndInFiveSeconds -= UserTimer_OnTimerEndInFiveSeconds;
                userTimer.OnTimerEnd -= UserTimer_OnTimerEnd;
                DestroyImmediate(userTimers[userId]);
                userTimers.Remove(userId);
            }
        }

        private void UserTimer_OnTimerEndInFiveSeconds(NetworkUserId userId)
        {
            var user = UsersHelper.GetUser(userId);

            if (user)
            {
                OnUserTimerEndInFiveSeconds?.Invoke(user);
            }
        }

        private void UserTimer_OnTimerEnd(NetworkUserId userId)
        {
            var user = UsersHelper.GetUser(userId);

            if (user)
            {
                OnUserTimerEnd?.Invoke(user);
            }
        }
    }
}
