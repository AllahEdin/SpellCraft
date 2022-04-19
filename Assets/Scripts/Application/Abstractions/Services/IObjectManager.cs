using System;
using UnityEngine;

public interface IObjectManager
{
    void RegisterPrefab(NetworkObjectDescriptor objectToSpawn);

    GameObjectWithDescriptor SrvSpawn(NetworkObjectDescriptor descriptor, NetworkObjectOptions options, Vector3 pos, Quaternion rot, Action<GameObject> configureOnServerAfterSpawn, Guid? playerAuthority);
}