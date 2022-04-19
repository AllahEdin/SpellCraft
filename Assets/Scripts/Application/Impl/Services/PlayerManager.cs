using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
using Mirror;
using UnityEngine;


public class PlayerManager : CustomNetworkBehaviourBase, IPlayersManager<NetworkConnection>
{
    public event Action AllPlayersReady;

    public event Action PlayersOnDefaultPositions;

    private Dictionary<PlayerSlot, PlayerStatus> _playerStatuses = new Dictionary<PlayerSlot, PlayerStatus>();
    private Dictionary<PlayerSlot, Player> _playerGameObjects = new Dictionary<PlayerSlot, Player>();

    private Dictionary<PlayerSlot, IClientToServerConnection<NetworkConnection>> _connections = new Dictionary<PlayerSlot, IClientToServerConnection<NetworkConnection>>();

    private Dictionary<PlayerSlot, bool> _defaultPositionsReady;

    private void Awake()
    {
        _playerGameObjects = new Dictionary<PlayerSlot, Player>();

        _defaultPositionsReady = typeof(PlayerSlot).GetEnumNames()
            .Select(s => Enum.TryParse<PlayerSlot>(s, out var slot)
                ? slot
                : throw new Exception())
            .ToDictionary(k => k, v => false);

        _playerStatuses = typeof(PlayerSlot).GetEnumNames()
            .Select(s => Enum.TryParse<PlayerSlot>(s, out var slot)
                ? slot
                : throw new Exception())
            .ToDictionary(k => k, v => PlayerStatus.NotReady);
        _connections = new Dictionary<PlayerSlot, IClientToServerConnection<NetworkConnection>>();
    }

    public void SrvReserveSlot(IClientToServerConnection<NetworkConnection> connection, PlayerSlot slot)
    {
        Debug.Log($"Trying to add connection {connection.Id}");

        _connections.Add(slot, connection);
    }

    public PlayerSlot SrvGetEmptySlot()
    {
        if (_connections.Count < typeof(PlayerSlot).GetEnumNames().Length)
        {
            var values = new List<PlayerSlot>();

            var enumerator =
                typeof(PlayerSlot).GetEnumValues().GetEnumerator();

            while (enumerator.MoveNext())
            {
                values.Add((PlayerSlot)(enumerator.Current ?? throw new Exception()));
            }

            var number =
                values.First(s => !_connections.Select(ss => ss.Key).Contains(s));

            return number;
        }

        throw new Exception("No slots left");

    }

    public void SrvSetPlayerReady(PlayerSlot slot, Player player)
    {
        player.OnDefaultPosition += SrvSetPlayerDefaultPositionReady;

        _playerStatuses[slot] = PlayerStatus.Ready;
        _playerGameObjects.Add(slot, player);
        CheckForAllReady();
    }

    public void SrvClearConnectionSlot(PlayerSlot slot)
    {
        Debug.Log($"Trying to remove connection from slot {slot}");

        if (_connections.TryGetValue(slot, out var conn))
        {
            Debug.Log($"Removing connection {conn.Id}");
            _connections.Remove(slot);
        }
    }

    [Server]
    public IClientToServerConnection<NetworkConnection> SrvGetPlayer(Guid id)
    {
        return _connections.First(f => f.Value.Id.Equals(id)).Value;
    }

    [Server]
    public IEnumerable<IClientToServerConnection<NetworkConnection>> SrvGetPlayers()
    {
        return _connections.Select(clientToServerConnection => clientToServerConnection.Value);
    }

    [Server]
    public void SrvMovePlayersToDefaultPositions()
    {
        _defaultPositionsReady = typeof(PlayerSlot).GetEnumNames()
            .Select(s => Enum.TryParse<PlayerSlot>(s, out var slot)
                ? slot
                : throw new Exception())
            .ToDictionary(k => k, v => false);

        foreach (var player in SrvGetPlayers())
        {
            _playerGameObjects[player.Slot].TargetMoveToPosition(player.ConnectionInstance ,MapDescriptor.Areas[player.Slot].Center);
        }
    }

    [Server]
    private void SrvSetPlayerDefaultPositionReady(PlayerSlot player)
    {
        Debug.Log($"Player {player} default position ready");
        _defaultPositionsReady[player] = true;
        if (_defaultPositionsReady.All(a => a.Value))
        {
            _defaultPositionsReady = typeof(PlayerSlot).GetEnumNames()
                .Select(s => Enum.TryParse<PlayerSlot>(s, out var slot)
                    ? slot
                    : throw new Exception())
                .ToDictionary(k => k, v => false);

            Debug.Log($"All players default position ready");
            PlayersOnDefaultPositions?.Invoke();
        }
    }

    [Server]
    private void CheckForAllReady()
    {
        if (_playerStatuses.All(a => a.Value == PlayerStatus.Ready))
        {
            AllPlayersReady?.Invoke();
        }
    }
}
