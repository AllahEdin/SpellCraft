using System;

public interface IItemManager
{
    event Action<NetworkObjectInstanceDescriptor> ClientGotItem;

    event Action<Guid> ClientRemovedItem;

    void SetItemToPlayer(PlayerSlot player, FullItemDescriptor descriptor);

    void UseItem(PlayerSlot player, Guid itemId, Guid spawnPointId);
}