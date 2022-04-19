using System;
using Mirror;
using UnityEngine;

public class DefaultPlayerPosition : SpawnableCustomNetworkBehaviourBase
{
    private PlayerSlot? _playerSlot;

    public override void OnStartServer()
    {
        GetComponent<Collider>().enabled = true;
        base.OnStartClient();
    }

    [Server]
    public PlayerSlot SrvGetPlayerSlot() => _playerSlot ?? throw new Exception();

    public override void SrvApplyOptions(NetworkObjectOptions options)
    {
        var opt = options.GetOptions<DefaulpPlayerPositionOptions>();
        _playerSlot = opt.slot;
    }
}