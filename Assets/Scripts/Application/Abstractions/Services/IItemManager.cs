using System;

public interface IItemManager
{
    event Action<NetworkObjectInstanceDescriptor> ClientGotItem;

    event Action<Guid> ClientRemovedItem;

    void SrvSetItemToPlayer(PlayerSlot player, FullItemDescriptor descriptor);

    void SrvUseItem(PlayerSlot player, Guid itemId, Guid spawnPointId);
}