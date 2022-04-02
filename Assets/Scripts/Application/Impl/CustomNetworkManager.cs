 using System;
 using System.Collections.Generic;
 using Assets.Scripts;
 using Mirror;
 using UnityEngine;

 public class CustomNetworkManager : NetworkManager
 {
    private IPlayersManager<NetworkConnection> PlayersManager =>
        _playersManager ??= FindObjectOfType<PlayerManager>();

     private IPlayersManager<NetworkConnection> _playersManager;
     private Dictionary<int, Guid> _connectionMappings;
     private Dictionary<Guid, PlayerSlot> _slotMappings;

    public override void Awake()
    {
        base.Awake();
        _connectionMappings = new Dictionary<int, Guid>();
        _slotMappings = new Dictionary<Guid, PlayerSlot>();
    }

    [Server]
    public override void OnServerConnect(NetworkConnection conn)
    {
        Debug.Log($"Player connected");

        var slot =
            PlayersManager.GetEmptySlot();

        Debug.Log($"Got empty slot {slot}");

        var connection = new MirrorConnection(conn, slot);

        Debug.Log($"Connection created {connection.Id}");

        PlayersManager.ReserveSlot(connection, slot);

        Debug.Log($"Reserved slot {slot}");

        _connectionMappings.Add(conn.connectionId, connection.Id);
        _slotMappings.Add(connection.Id, slot);

        base.OnServerConnect(conn);
    }

    [Server]
    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        if (_connectionMappings.TryGetValue(conn.connectionId, out var existingConnection))
        {
            if (_slotMappings.TryGetValue(existingConnection, out var slot))
            {
                GameObject player = Instantiate(playerPrefab, MapDescriptor.Areas[slot].Center, Quaternion.identity);

                // instantiating a "Player" prefab gives it the name "Player(clone)"
                // => appending the connectionId is WAY more useful for debugging!
                player.name = $"{playerPrefab.name} [connId={conn.connectionId}]";
                NetworkServer.AddPlayerForConnection(conn, player);

                var playerComponent =
                    player.GetComponent<Player>();
                playerComponent.SetSlot(slot);
                playerComponent.TargetSetSlot(conn, slot);
                PlayersManager.SetPlayerReady(slot);
            }
        }
    }

    [Server]
    public override void OnServerDisconnect(NetworkConnection conn)
    {
        if (_connectionMappings.TryGetValue(conn.connectionId, out var existingConnection))
        {
            if (_slotMappings.TryGetValue(existingConnection, out var slot))
            {
                PlayersManager.ClearConnectionSlot(slot);
                _slotMappings.Remove(existingConnection);
                _connectionMappings.Remove(conn.connectionId);
            }
        }

        base.OnServerDisconnect(conn);
    }
 }