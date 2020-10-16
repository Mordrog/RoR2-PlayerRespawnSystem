using RoR2;

namespace Mordrog
{
    public enum RespawnTimeType
    {
        StageTimeBased,
        GameTimeBased,
    }

    static class RespawnTimeCalculation
    {
        public static uint GetRespawnTime()
        {
            uint respawnTime = PluginConfig.StartingRespawnTime.Value;

            switch(PluginConfig.RespawnTimeType.Value)
            {
                case RespawnTimeType.StageTimeBased:
                    respawnTime += RespawnTimeBasedOnPassedStageTime();
                    break;
                case RespawnTimeType.GameTimeBased:
                    respawnTime += RespawnTimeBasedOnPassedGameTime();
                    break;
            }

            return (uint) UnityEngine.Mathf.Min(respawnTime, PluginConfig.MaxRespawnTime.Value);
        }

        private static uint RespawnTimeBasedOnPassedStageTime()
        {
            var secondsPassed = GameSecondsPassed() - StageEntrySeconds();
            var updateIntervalsPassed = (secondsPassed / PluginConfig.UpdateCurrentRespawnTimeEveryXSeconds.Value);

            return updateIntervalsPassed * PluginConfig.UpdateCurrentRepsawnTimeByXSeconds.Value;
        }


        private static uint RespawnTimeBasedOnPassedGameTime()
        {
            var secondsPassed = GameSecondsPassed();
            var updateIntervalsPassed = (secondsPassed / PluginConfig.UpdateCurrentRespawnTimeEveryXSeconds.Value);

            return updateIntervalsPassed * PluginConfig.UpdateCurrentRepsawnTimeByXSeconds.Value;
        }

        private static uint StageEntrySeconds()
        {
            return (uint)(Stage.instance ? Stage.instance.Network_entryStopwatchValue : 0f);
        }

        private static uint GameSecondsPassed()
        {
            return (uint)(Run.instance ? Run.instance.GetRunStopwatch() : 0f);
        }
    }
}
