using System;
using Mirror;

public class PlayerInputManager : CustomNetworkBehaviour, IPlayerInputManager
{
    public event Action<PlayerSlot, Guid, Guid> OnPlayerAttemptsToUseItem;

    public void RaisePlayerUseItemEvent(PlayerSlot player, Guid itemId, Guid spawnPointId)
    {
        OnPlayerAttemptsToUseItem?.Invoke(player, itemId, spawnPointId);
    }

    [Server]
    public override void SrvApplyOptions(NetworkObjectOptions options)
    {
        throw new NotImplementedException();
    }
}