using System;
using Mirror;
using UnityEngine;

public class SpawnPoint : CustomNetworkBehaviour
{
    [SyncVar(hook = nameof(SetId))] 
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

    [ClientCallback]
    void SetId(Guid oldId, Guid newId)
    {
        if (!newId.Equals(Guid.Empty))
        {
            GetComponent<Collider>().enabled = true;
        }
    }
}
