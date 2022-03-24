using System;

public class PlayerInputManager : CustomNetworkBehaviour, IPlayerInputManager
{
    public event Action<PlayerSlot, Guid, Guid> OnPlayerAttemptsToUseItem;

    public void RaisePlayerUseItemEvent(PlayerSlot player, Guid itemId, Guid spawnPointId)
    {
        OnPlayerAttemptsToUseItem?.Invoke(player, itemId, spawnPointId);
    }
}