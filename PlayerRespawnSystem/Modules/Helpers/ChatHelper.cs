namespace Mordrog
{
    public static class ChatHelper
    {
        private const string GrayColor = "7e91af";
        private const string RedColor = "ff0000";
        private const string YellowColor = "ffff00";
        private const string GreenColor = "32cd32";
        private const string SilverColor = "c0c0c0";

        public static void RespawnBlockedOnStage()
        {
            var message = $"<color=#{GrayColor}>Respawns are blocked on this stage.</color>";
            RoR2.Chat.SendBroadcastChat(new RoR2.Chat.SimpleChatMessage { baseToken = message });
        }

        public static void RespawnBlockedOnTPEvent()
        {
            var message = $"<color=#{GrayColor}>Respawns are blocked on teleporter event.</color>";
            RoR2.Chat.SendBroadcastChat(new RoR2.Chat.SimpleChatMessage { baseToken = message });
        }

        public static void UserWillRespawnAfterTPEvent(string userName)
        {
            var message = $"<color=#{GreenColor}>{userName}</color> <color=#{GrayColor}>will respawn after teleporter event</color>";
            RoR2.Chat.SendBroadcastChat(new RoR2.Chat.SimpleChatMessage { baseToken = message });
        }

        public static void UserWillRespawnAfter(string userName, uint respawnTime)
        {
            var message = $"<color=#{GreenColor}>{userName}</color> <color=#{GrayColor}>will respawn in {respawnTime} seconds</color>";
            RoR2.Chat.SendBroadcastChat(new RoR2.Chat.SimpleChatMessage { baseToken = message });
        }
    }
}
