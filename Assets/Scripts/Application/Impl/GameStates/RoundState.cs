using System;
using Mirror;
using UnityEngine;

public class RoundState : CustomNetworkBehaviourBase, IGameState
{
    public event Action<IGameState> CompleteState;
    public GameState State => GameState.Round;

    private bool _isActive;
    private float _currentTime;

    [ServerCallback]
    private void FixedUpdate()
    {
        if (_isActive)
        {
            _currentTime += Time.deltaTime;

            if (_currentTime > 10)
            {
                PlayerInputManager.OnPlayerAttemptsToUseItem -= PlayerInputManagerOnOnPlayerAttemptsToUseItem;
                SpawnPointsManager.SrvRemoveSpawnPointsToPlayers();
                CompleteState?.Invoke(this);
                _isActive = false;
            }
        }
    }

    public void StartState()
    {
        _isActive = true;
        _currentTime = 0;
        PlayerInputManager.OnPlayerAttemptsToUseItem += PlayerInputManagerOnOnPlayerAttemptsToUseItem;
        SpawnPointsManager.SrvAddSpawnPointsToPlayers();
    }

    [Server]
    private void PlayerInputManagerOnOnPlayerAttemptsToUseItem(PlayerSlot player, Guid itemId, Guid spawnPointId)
    {
        Debug.Log($"Server checks if target spawn point {spawnPointId} is empty");
        var isEmpty = SpawnPointsManager.SrvIsEmpty(spawnPointId);

        Debug.Log($"Spawn point {spawnPointId} {(isEmpty ? "empty" : "not empty")}");
        if (isEmpty)
        {
            ItemManager.SrvUseItem(player, itemId, spawnPointId);
        }
    }

}