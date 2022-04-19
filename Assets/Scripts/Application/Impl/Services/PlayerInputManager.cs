using System;
using Mirror;

public class PlayerInputManager : CustomNetworkBehaviourBase, IPlayerInputManager
{
    public event Action<PlayerSlot, Guid, Guid> OnPlayerAttemptsToUseItem;

    [Server]
    public void RaisePlayerUseItemEvent(PlayerSlot player, Guid itemId, Guid spawnPointId)
    {
        OnPlayerAttemptsToUseItem?.Invoke(player, itemId, spawnPointId);
    }

}