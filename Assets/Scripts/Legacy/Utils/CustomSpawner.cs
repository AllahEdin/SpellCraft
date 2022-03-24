using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace Assets.Scripts.Utils
{
    public class CustomSpawner : NetworkBehaviour
    {
        private readonly List<string> _registeredPrefabs =
            new List<string>();

        [Server]
        public void RegisterAndSpawn(string path, Vector3 pos, Quaternion rot, NetworkConnection connection = null, Action<GameObject> afterActionOnClient = null)
        {
            if (!_registeredPrefabs.Contains(path))
            {
                _registeredPrefabs.Add(path);
            }

            RpcRegisterPrefab(path);
            
            var go =
                Resources.Load<GameObject>(path);
            var instance = Instantiate(go, pos, rot);

            if (connection != null)
            {
                NetworkServer.Spawn(instance, connection);
            }
            else
            {
                NetworkServer.Spawn(instance);
            }

            afterActionOnClient?.Invoke(instance);
        }

        [ClientRpc]
        public void RpcRegisterPrefab(string path)
        {
            if (_registeredPrefabs.Contains(path))
            {
                return;
            }

            var go =
                Resources.Load<GameObject>(path);
            NetworkClient.RegisterPrefab(go);
            _registeredPrefabs.Add(path);
        }
    }
}