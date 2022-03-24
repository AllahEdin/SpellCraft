using System;
using UnityEngine;

public interface IObjectManager
{
    void RegisterPrefab(NetworkObjectDescriptor objectToSpawn);

    GameObject Spawn(NetworkObjectDescriptor objectToSpawn, Vector3 pos, Quaternion rot, Action<GameObject> configureOnServerAfterSpawn, Guid? playerAuthority);
}