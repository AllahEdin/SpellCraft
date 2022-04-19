using System;
using System.Linq;
using JetBrains.Annotations;
using Mirror;
using UnityEngine;

public class GameUiNetworkManager : NetworkManager
{
    private readonly NetworkConnection[] _networkConnections = new NetworkConnection[2];

    private GameUiGameManager _gameManager;

    private GameUiGameManager GameManager =>
        _gameManager ??= FindObjectOfType<GameUiGameManager>();

    [Server]
    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);
        for (var i = 0; i < _networkConnections.Length; i++)
        {
            if (_networkConnections[i] == conn)
            {
                _networkConnections[i] = null;
            }
        }
    }

    [Server]
    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        Transform startPos = GetStartPosition();
        GameObject player = startPos != null
            ? Instantiate(playerPrefab, startPos.position, startPos.rotation)
            : Instantiate(playerPrefab);

        // instantiating a "Player" prefab gives it the name "Player(clone)"
        // => appending the connectionId is WAY more useful for debugging!
        player.name = $"{playerPrefab.name} [connId={conn.connectionId}]";
        NetworkServer.AddPlayerForConnection(conn, player);

        var playerComponent = player.GetComponent<GameUiPlayer>();
        if (playerComponent != null)
        {
            var number = _networkConnections[0] == null ? 0
                : _networkConnections[1] == null ? 1
                : throw new Exception();

            _networkConnections[number] = conn;
            playerComponent.TargetSetPlayerNumber(number);
            GameManager.TargetSetPlayerNumber(conn, number);
        }

        if (_networkConnections.All(a => a != null))
        {
            GameManager.ServerStart();
        }
    }

    [Server]
    public NetworkConnection GetPlayer(int number)
    {
        return _networkConnections[number];
    }
}