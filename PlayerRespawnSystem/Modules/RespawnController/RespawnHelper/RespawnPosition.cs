using R2API.Utils;
using System.Reflection;
using UnityEngine;

namespace PlayerRespawnSystem
{
    static class RespawnPosition
    {
        public static Vector3 GetSpawnPositionAroundTeleporter(RoR2.CharacterBody body, float minSpawnRadius, float maxSpawnRadius)
        {
            Vector3 positionAroundTP = Vector3.zero;
            int tries = 0;

            do
            {
                if (tries++ > 1000)
                    return Vector3.zero;

                positionAroundTP = RoR2.TeleporterInteraction.instance.transform.position;
                positionAroundTP += GetRandomPositionInCircle(minSpawnRadius, maxSpawnRadius);

            } while (!TryUpdateToProperPositionOnStage(ref positionAroundTP, body.radius));

            return new Vector3(positionAroundTP.x, positionAroundTP.y + RoR2.Util.GetBodyPrefabFootOffset(body.gameObject), positionAroundTP.z);
        }
        public static Vector3 GetSpawnPositionAroundMoonBoss(RoR2.CharacterBody body, float minSpawnRadius, float maxSpawnRadius)
        {
            Vector3 positionAroundTP = Vector3.zero;
            int tries = 0;

            do
            {
                if (tries++ > 1000)
                    return Vector3.zero;

                GameObject arenaWalls = GameObject.Find("ArenaWalls");
                if (arenaWalls)
                {
                    positionAroundTP = arenaWalls.transform.position;
                    positionAroundTP += GetRandomPositionInCircle(minSpawnRadius, maxSpawnRadius);
                }

            } while (!TryUpdateToProperPositionOnStage(ref positionAroundTP, body.radius));

            return new Vector3(positionAroundTP.x, positionAroundTP.y + RoR2.Util.GetBodyPrefabFootOffset(body.gameObject), positionAroundTP.z);
        }

        public static Vector3 GetSpawnPositionForVoidBoss()
        {
            var gauntletIndexField = typeof(RoR2.VoidRaidGauntletController).GetField("gauntletIndex", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField);
            var initialDonutField = typeof(RoR2.VoidRaidGauntletController).GetField("initialDonut", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField);
            var followingDonutsField = typeof(RoR2.VoidRaidGauntletController).GetField("followingDonuts", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField);

            if (gauntletIndexField == null || initialDonutField == null || followingDonutsField == null)
            {
                Debug.LogWarning("Failed to find VoidRaidGauntletController field");
                return Vector3.zero;
            }

            RoR2.VoidRaidGauntletController.DonutInfo currentDonut;

            var currentDonutIdx = (int)gauntletIndexField.GetValue(RoR2.VoidRaidGauntletController.instance);
            if (currentDonutIdx <= 0)
            {
                currentDonut = initialDonutField.GetValue(RoR2.VoidRaidGauntletController.instance) as RoR2.VoidRaidGauntletController.DonutInfo;
                if (currentDonut == null)
                {
                    Debug.LogWarning("Failed to get VoidRaidGauntletController::initialDonut field");
                    return Vector3.zero;
                }
            }
            else
            {
                var followingDonuts = followingDonutsField.GetValue(RoR2.VoidRaidGauntletController.instance) as RoR2.VoidRaidGauntletController.DonutInfo[];
                if (followingDonuts == null)
                {
                    Debug.LogWarning("Failed to get VoidRaidGauntletController::followingDonuts field");
                    return Vector3.zero;
                }

                currentDonut = followingDonuts[currentDonutIdx - 1];
            }

            return currentDonut.returnPoint.position;
        }

        private static Vector3 GetRandomPositionInCircle(float minRadius, float maxRadius)
        {
            Vector3 position = UnityEngine.Random.insideUnitCircle.normalized * UnityEngine.Random.Range(minRadius, maxRadius);
            position.z = position.y;
            position.y = 0;

            return position;
        }

        private static bool TryUpdateToProperPositionOnStage(ref Vector3 position, float maxRadiusAroundPosition)
        {
            RaycastHit raycastHit = default(RaycastHit);
            Ray ray = new Ray(position + Vector3.up * 2f, Vector3.down);
            float maxDistance = 4f;
            if (Physics.SphereCast(ray, maxRadiusAroundPosition, out raycastHit, maxDistance, RoR2.LayerIndex.world.mask))
            {
                position.y += 2f - raycastHit.distance;
                return true;
            }

            return false;
        }
    }
}
