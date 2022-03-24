using System;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;


public class PlayerManager : NetworkBehaviour, IPlayersManager<NetworkConnection>
{
    public event Action AllPlayersReady;

    public event Action<IClientToServerConnection<NetworkConnection>> PlayerAdded;

    public event Action<IClientToServerConnection<NetworkConnection>> PlayerRemoved;

    private Dictionary<PlayerSlot, PlayerStatus> _playerStatuses = new Dictionary<PlayerSlot, PlayerStatus>();

    private Dictionary<PlayerSlot, IClientToServerConnection<NetworkConnection>> _connections = new Dictionary<PlayerSlot, IClientToServerConnection<NetworkConnection>>();

    [SerializeField] private int _maxPlayers;

    private void Awake()
    {
        _playerStatuses = typeof(PlayerSlot).GetEnumNames()
            .Select(s => Enum.TryParse<PlayerSlot>(s, out var slot)
                ? slot
                : throw new Exception())
            .ToDictionary(k => k, v => PlayerStatus.NotReady);
        _connections = new Dictionary<PlayerSlot, IClientToServerConnection<NetworkConnection>>();
    }

    public void ReserveSlot(IClientToServerConnection<NetworkConnection> connection, PlayerSlot slot)
    {
        Debug.Log($"Trying to add connection {connection.Id}");

        _connections.Add(slot, connection);

        PlayerAdded?.Invoke(connection);
    }

    public PlayerSlot GetEmptySlot()
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

    public void SetPlayerReady(PlayerSlot player)
    {
        _playerStatuses[player] = PlayerStatus.Ready;
        CheckForAllReady();
    }

    private void CheckForAllReady()
    {
        if (_playerStatuses.All(a => a.Value == PlayerStatus.Ready))
        {
            AllPlayersReady?.Invoke();
        }
    }

    public void ClearConnectionSlot(PlayerSlot slot)
    {
        Debug.Log($"Trying to remove connection from slot {slot}");

        if (_connections.TryGetValue(slot, out var conn))
        {
            Debug.Log($"Removing connection {conn.Id}");
            _connections.Remove(slot);
            PlayerRemoved?.Invoke(conn);
        }
    }

    public IClientToServerConnection<NetworkConnection> GetPlayer(Guid id)
    {
        return _connections.First(f => f.Value.Id.Equals(id)).Value as IClientToServerConnection<NetworkConnection>;
    }

    public IEnumerable<IClientToServerConnection<NetworkConnection>> GetPlayers()
    {
        foreach (var clientToServerConnection in _connections)
        {
            yield return clientToServerConnection.Value;
        }
    }
}
