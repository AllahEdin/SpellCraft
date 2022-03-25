using System;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;

public class RoundState : CustomNetworkBehaviour, IGameState
{
    public event Action<IGameState> CompleteState;
    public GameState State => GameState.Round;

    private bool _isActive;
    private float _currentTime;

    private readonly Dictionary<PlayerSlot, List<Guid>> _slotsUsed;

    public RoundState()
    {
        _slotsUsed =
            typeof(PlayerSlot).GetEnumNames().Select(s =>
                    Enum.TryParse(s, out PlayerSlot slot) ? slot : throw new Exception())
                .ToDictionary(k => k, v => new List<Guid>());
    }

    [ServerCallback]
    private void FixedUpdate()
    {
        if (_isActive)
        {
            _currentTime += Time.deltaTime;

            if (_currentTime > 10)
            {
                PlayerInputManager.OnPlayerAttemptsToUseItem -= PlayerInputManagerOnOnPlayerAttemptsToUseItem;
                SpawnPointsManager.RemoveSpawnPointsToPlayers();
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
        SpawnPointsManager.AddSpawnPointsToPlayers();
    }

    private void PlayerInputManagerOnOnPlayerAttemptsToUseItem(PlayerSlot player, Guid itemId, Guid spawnPointId)
    {
        if (!_slotsUsed[player].Contains(spawnPointId))
        {
            ItemManager.UseItem(player, itemId, spawnPointId);
            _slotsUsed[player].Add(spawnPointId);
        }
    }
}