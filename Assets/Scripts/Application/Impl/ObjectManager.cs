using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class ObjectManager : CustomNetworkBehaviour, IObjectManager
{
    private readonly List<string> _registeredPrefabs =
        new List<string>();


    [TargetRpc]
    private void TargetRegisterPrefab(NetworkConnection connection, string path)
    {
        if (_registeredPrefabs.Contains(path))
        {
            return;
        }

        Debug.Log($"Target client registering prefab {path}");

        var go =
            Resources.Load<GameObject>(path);
        NetworkClient.RegisterPrefab(go);
        _registeredPrefabs.Add(path);
    }

    [Server]
    public void RegisterPrefab(NetworkObjectDescriptor objectToSpawn)
    {
        var path =
            objectToSpawn.Path;

        //Debug.Log($"Server registering prefab {path}");

        foreach (var clientToServerConnection in PlayersManager.GetPlayers())
        {
            TargetRegisterPrefab(clientToServerConnection.ConnectionInstance, path);
        }
    }

    [Server]
    public GameObjectWithDescriptor Spawn(NetworkObjectDescriptor descriptor, 
        NetworkObjectOptions options,
        Vector3 pos, Quaternion rot,
        Action<GameObject> configureOnServerAfterSpawn, 
        Guid? playerAuthority)
    {
        NetworkConnection connection = null;

        if (playerAuthority != null)
        {
            try
            {
                connection = PlayersManager.GetPlayer(playerAuthority.Value).ConnectionInstance;
            }
            catch (Exception)
            {
                return null;
            }
        }

        var path =
            descriptor.Path;

        Debug.Log($"Server spawning prefab {path}");

        var go =
            Resources.Load<GameObject>(path);

        var instance = Instantiate(go, pos, rot);

        var instanceDescriptor = new NetworkObjectInstanceDescriptor(Guid.NewGuid(), descriptor, options);

        if (playerAuthority != null)
        {
            NetworkServer.Spawn(instance, connection);
        }
        else
        {
            NetworkServer.Spawn(instance);
        }

        instance.GetComponent<CustomNetworkBehaviour>().SrvApplyOptions(options);

        configureOnServerAfterSpawn?.Invoke(instance);

        return new GameObjectWithDescriptor(instanceDescriptor, instance);
    }

    public override void SrvApplyOptions(NetworkObjectOptions options)
    {
        throw new NotImplementedException();
    }
}