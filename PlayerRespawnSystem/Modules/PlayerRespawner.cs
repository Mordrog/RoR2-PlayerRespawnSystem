using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace PlayerRespawnSystem
{
    class PlayerRespawner : NetworkBehaviour
    {
        public bool IsAdvancingStage { get; set; }

        private Queue<Tuple<RespawnController, CharacterBody>> respawnQueue = new Queue<Tuple<RespawnController, CharacterBody>>();

        public void RespawnUser(RespawnController respawnController, NetworkUser user)
        {
            if (CheckIfControllerCanRespawn(respawnController, user))
            {
                if (user.master.bodyPrefab) 
                {
                    CharacterBody body = user.master.bodyPrefab.GetComponent<CharacterBody>();

                    if (body)
                    {
                        UnityEngine.Debug.Log($"PlayerRespawnSystem: Issuing {respawnController.GetRespawnType()} respawn");
                        respawnQueue.Enqueue(new Tuple<RespawnController, CharacterBody>(respawnController, body));
                        Stage.instance.RespawnCharacter(user.master);
                    }
                }
            }
        }

        public void RespawnAllUsers(RespawnController respawnController)
        {
            foreach (var user in NetworkUser.readOnlyInstancesList)
            {
                RespawnUser(respawnController, user);
            }
        }

        public bool CheckIfControllerCanRespawn(RespawnController respawnController, NetworkUser user)
        {
            return respawnController &&
                   respawnController.IsActive &&
                   CheckIfUserCanBeRespawned(user);
        }

        public bool CheckIfUserCanBeRespawned(NetworkUser user)
        {
            return user &&
                   user.master &&
                   user.master.IsDeadAndOutOfLivesServer() &&
                   !IsAdvancingStage;
        }

        public void Awake()
        {
            On.RoR2.Stage.RespawnCharacter += Stage_RespawnCharacter;
            On.RoR2.Stage.GetPlayerSpawnTransform += Stage_GetPlayerSpawnTransform;
        }

        public void OnDestroy()
        {
            On.RoR2.Stage.RespawnCharacter -= Stage_RespawnCharacter;
            On.RoR2.Stage.GetPlayerSpawnTransform -= Stage_GetPlayerSpawnTransform;
        }

        private void Stage_RespawnCharacter(On.RoR2.Stage.orig_RespawnCharacter orig, Stage self, CharacterMaster characterMaster)
        {
            if (characterMaster.bodyPrefab)
            {
                CharacterBody body = characterMaster.bodyPrefab.GetComponent<CharacterBody>();

                if (body && (respawnQueue.Count == 0 || respawnQueue.Last().Item2 != body))
                {
                    // respawn issued outside of respawn system
                    respawnQueue.Enqueue(new Tuple<RespawnController, CharacterBody>(null, body));
                }
            }

            orig(self, characterMaster);
        }

        // Will be called after Stage_RespawnCharacter
        private Transform Stage_GetPlayerSpawnTransform(On.RoR2.Stage.orig_GetPlayerSpawnTransform orig, Stage self)
        {
            if (respawnQueue.Count > 0)
            {
                // If null, then respawn was issued outside of respawn system
                var (respawnController, body) = respawnQueue.Dequeue();
                if (respawnController && body)
                {
                    UnityEngine.Debug.Log($"PlayerRespawnSystem: Getting respawn position for {respawnController.GetRespawnType()} respawn");
                    if (respawnController.GetRespawnTransform(body, out Transform respawnTransform))
                    {
                        UnityEngine.Debug.Log($"PlayerRespawnSystem: Found respawn position at {respawnTransform.position}");
                        return respawnTransform;
                    }
                    UnityEngine.Debug.Log($"PlayerRespawnSystem: Failed to find respawn position, using default");
                }
            }

            // Default Respawn Position
            return orig(self);
        }
    }
}
