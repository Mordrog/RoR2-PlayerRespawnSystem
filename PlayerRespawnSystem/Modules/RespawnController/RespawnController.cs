using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Networking;

namespace PlayerRespawnSystem
{
    public enum RespawnType : byte
    {
        Timed,
        Teleporter,
        Mithrix,
        Artifact,
        Voidling
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class AssociatedRespawnType : Attribute
    {
        public RespawnType RespawnType { get; }

        public AssociatedRespawnType(RespawnType respawnType)
        {
            RespawnType = respawnType;
        }
    }

    interface IRespawnController
    {
        public delegate void RequestTimedRespawnBlock();
        public event RequestTimedRespawnBlock OnRequestTimedRespawnBlock;

        public delegate void RequestTimedRespawnUnblock();
        public event RequestTimedRespawnBlock OnRequestTimedRespawnUnblock;
    }

    abstract class RespawnController : NetworkBehaviour, IRespawnController
    {
        public virtual bool IsActive { get; protected set; } = false;

        protected PlayerRespawner playerRespawner;

        public void Init(PlayerRespawner playerRespawner)
        {
            this.playerRespawner = playerRespawner;
        }

        public RespawnType GetRespawnType()
        {
            return GetType().GetCustomAttribute<AssociatedRespawnType>().RespawnType;
        }

        public virtual bool GetRespawnTransform(RoR2.CharacterBody body, out Transform outRespawnTransform)
        {
            outRespawnTransform = null;
            return false;
        }

        public event IRespawnController.RequestTimedRespawnBlock OnRequestTimedRespawnBlock;
        public event IRespawnController.RequestTimedRespawnBlock OnRequestTimedRespawnUnblock;

        protected void RequestTimedRespawnBlock() { OnRequestTimedRespawnBlock?.Invoke(); }
        protected void RequestTimedRespawnUnblock() { OnRequestTimedRespawnUnblock?.Invoke(); }

        public static Dictionary<RespawnType, Type> GetRespawnControllerTypes()
        {
            Dictionary<RespawnType, Type> respawnControllers = new Dictionary<RespawnType, Type>();

            foreach (var respawnController in Assembly.GetExecutingAssembly().GetTypes().Where(t => t.GetCustomAttribute<AssociatedRespawnType>() != null))
            {
                var respawnType = respawnController.GetCustomAttribute<AssociatedRespawnType>().RespawnType;
                UnityEngine.Assertions.Assert.IsFalse(respawnControllers.ContainsKey(respawnType));
                respawnControllers[respawnType] = respawnController;
            }

            return respawnControllers;
        }
    }
}
