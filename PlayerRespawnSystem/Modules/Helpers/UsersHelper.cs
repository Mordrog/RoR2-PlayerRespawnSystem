using RoR2;
using System.Linq;

namespace PlayerRespawnSystem
{
    public static class UsersHelper
    {
        public static NetworkUser GetUser(NetworkUserId userId)
        {
            return NetworkUser.readOnlyInstancesList.FirstOrDefault(u => u.id.Equals(userId));
        }

        public static NetworkUser GetUser(CharacterMaster master)
        {
            return NetworkUser.readOnlyInstancesList.FirstOrDefault(u => u.master == master);
        }
    }
}
