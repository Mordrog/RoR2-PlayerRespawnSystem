using System;
using UnityEngine;

namespace Mordrog
{
    static class TeleporterSpawnPosition
    {
        public static Vector3 GetSpawnPositionAroundTeleporter(RoR2.CharacterBody body, float spawnRadius)
        {
            Vector3 positionAroundTP = Vector3.zero;

            do
            {
                positionAroundTP = RoR2.TeleporterInteraction.instance.transform.position;
                positionAroundTP += GetRandomPositionAroundTeleporter(spawnRadius);

            } while (!TryUpdateToProperPositionOnStage(ref positionAroundTP, body.radius));

            return new Vector3(positionAroundTP.x, positionAroundTP.y + RoR2.Util.GetBodyPrefabFootOffset(body.gameObject), positionAroundTP.z);
        }

        private static Vector3 GetRandomPositionAroundTeleporter(float radius)
        {
            Vector3 position = UnityEngine.Random.insideUnitCircle * radius;
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
