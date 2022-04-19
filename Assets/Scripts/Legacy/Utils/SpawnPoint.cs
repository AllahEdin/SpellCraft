using System;
using Mirror;
using UnityEngine;

public class SpawnPoint : SpawnableCustomNetworkBehaviourBase
{
    public Guid Id;

    public override void OnStartServer()
    {
        GetComponent<Collider>().enabled = false;
        base.OnStartClient();
    }

    public override void OnStartClient()
    {
        GetComponent<Collider>().enabled = false;
        base.OnStartClient();
    }

    [ClientRpc]
    void RpcSetId(Guid newId)
    {
        Debug.Log($"Spawn point id has set to {newId.ToString()}");

        Id = newId;
        if (!newId.Equals(Guid.Empty))
        {
            GetComponent<Collider>().enabled = true;
        }
    }
    
    [Server]
    public override void SrvApplyOptions(NetworkObjectOptions options)
    {
        Debug.Log($"Spawn point with options : {options.JsonOptions}");
        var spawnPointOptions = options.GetOptions<SpawnPointOptions>();
        var guidId = Guid.Parse(spawnPointOptions.id);
        Id = guidId;
        RpcSetId(guidId);
    }
}