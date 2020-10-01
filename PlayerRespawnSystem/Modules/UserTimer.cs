using UnityEngine.Networking;

namespace Mordrog
{
    class UserTimer : NetworkBehaviour
    {
        private float timeRemaining = 0;
        private bool spawnInFiveSecondsFired = false;

        public delegate void RespawnInFiveSeconds(RoR2.NetworkUserId userId);
        public event RespawnInFiveSeconds OnRespawnInFiveSeconds;

        public delegate void TimerEnd(RoR2.NetworkUserId userId);
        public event TimerEnd OnTimerEnd;

        public RoR2.NetworkUserId UserId { get; set; }

        public bool IsRunning { get; private set; } = false;

        public void StartTimer(float time)
        {
            timeRemaining = time;
            spawnInFiveSecondsFired = false;
            IsRunning = true;
        }

        public void Reset()
        {
            IsRunning = false;
            timeRemaining = 0;
        }

        public void Update()
        {
            if (IsRunning)
            {
                if (timeRemaining > 0)
                {
                    timeRemaining -= UnityEngine.Time.deltaTime;

                    if (timeRemaining <= 5 && !spawnInFiveSecondsFired)
                    {
                        spawnInFiveSecondsFired = true;
                        OnRespawnInFiveSeconds?.Invoke(UserId);
                    }
                }
                else
                {
                    Reset();
                    OnTimerEnd?.Invoke(UserId);
                }
            }
        }
    }
}
