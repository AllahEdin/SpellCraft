using System;

public interface IItemManager
{
    event Action<NetworkObjectDescriptor> ClientGotItem;
    event Action<Guid> ClientRemovedItem;

    void SetItemToPlayer(PlayerSlot player, NetworkObjectDescriptor item);
    void UseItem(PlayerSlot player, Guid itemId, Guid spawnPointId);
}