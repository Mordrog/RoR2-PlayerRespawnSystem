using UnityEngine.Networking;

namespace PlayerRespawnSystem
{
    class UserTimer : NetworkBehaviour
    {
        public RoR2.NetworkUserId UserId { get; set; }

        public float TimeRemaining { get; private set; } = 0;

        public bool IsRunning { get; private set; } = false;

        public delegate void TimerEndInFiveSeconds(RoR2.NetworkUserId userId);
        public event TimerEndInFiveSeconds OnTimerEndInFiveSeconds;

        public delegate void TimerEnd(RoR2.NetworkUserId userId);
        public event TimerEnd OnTimerEnd;

        private bool hasTimerEndInFiveSecondsFired = false;

        public UserTimer(RoR2.NetworkUserId userId)
        {
            UserId = userId;
        }

        public void StartTimer(float time)
        {
            TimeRemaining = time;
            hasTimerEndInFiveSecondsFired = false;
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
            {
                IsRunning = false;
            }
        }

        public void Resume()
        {
            if (TimeRemaining > 0)
            {
                IsRunning = true;
            }
        }

        public void Update()
        {
            if (IsRunning)
            {
                if (TimeRemaining > 0)
                {
                    TimeRemaining -= UnityEngine.Time.deltaTime;

                    if (TimeRemaining <= 5 && !hasTimerEndInFiveSecondsFired)
                    {
                        hasTimerEndInFiveSecondsFired = true;
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
