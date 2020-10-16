using UnityEngine.Networking;

namespace Mordrog
{
    class UserTimer : NetworkBehaviour
    {
        
        private bool spawnInFiveSecondsFired = false;

        public delegate void TimerEndInFiveSeconds(RoR2.NetworkUserId userId);
        public event TimerEndInFiveSeconds OnTimerEndInFiveSeconds;

        public delegate void TimerEnd(RoR2.NetworkUserId userId);
        public event TimerEnd OnTimerEnd;

        public float TimeRemaining { get; private set; } = 0;

        public RoR2.NetworkUserId UserId { get; set; }

        public bool IsRunning { get; private set; } = false;

        public void StartTimer(float time)
        {
            TimeRemaining = time;
            spawnInFiveSecondsFired = false;
            IsRunning = true;
        }

        public void Reset()
        {
            IsRunning = false;
            TimeRemaining = 0;
        }

        public void Stop()
        {
            if (TimeRemaining > 0)
                IsRunning = false;
        }

        public void Resume()
        {
            if (TimeRemaining > 0)
                IsRunning = true;
        }

        public void Update()
        {
            if (IsRunning)
            {
                if (TimeRemaining > 0)
                {
                    TimeRemaining -= UnityEngine.Time.deltaTime;

                    if (TimeRemaining <= 5 && !spawnInFiveSecondsFired)
                    {
                        spawnInFiveSecondsFired = true;
                        OnTimerEndInFiveSeconds?.Invoke(UserId);
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
