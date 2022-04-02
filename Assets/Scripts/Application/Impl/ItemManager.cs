using System;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;
using Random = System.Random;

public class ItemManager : CustomNetworkBehaviour, IItemManager
{
    [SerializeField] private TextView _textView;

    private Dictionary<PlayerSlot, List<NetworkObjectInstanceDescriptor>> _items;

    public ItemManager()
    {
        _items = typeof(PlayerSlot).GetEnumNames()
            .Select(s => Enum.TryParse(s, out PlayerSlot slot) ? slot : throw new Exception())
            .ToDictionary(k => k, v => new List<NetworkObjectInstanceDescriptor>());
    }

    public event Action<NetworkObjectInstanceDescriptor> ClientGotItem;
    public event Action<Guid> ClientRemovedItem;

    [ClientCallback]
    private void Awake()
    {
        _textView.gameObject.SetActive(false);
    }

    [Server]
    public void SetItemToPlayer(PlayerSlot player, FullItemDescriptor descriptor)
    {
        var itemId = Guid.NewGuid();
        Debug.Log($"Item Id set to {itemId}");
        _items[player].Add(new NetworkObjectInstanceDescriptor(itemId, descriptor.Dummy, descriptor.Options));
        _textView.SetText(string.Join("\n", _items.Select(s => s.Value).Select(s => string.Join(",", s))));
        Debug.Log($"Added item to player in slot {player}");
        TargetSetItem(PlayersManager.GetPlayers().First(w => w.Slot == player).ConnectionInstance,
            itemId,
            descriptor);
    }

    [Server]
    public void UseItem(PlayerSlot player, Guid itemId, Guid spawnPointId)
    {
        Debug.Log($"Server start using an item {itemId} for player {player} on spawn poitn {spawnPointId}");
        var spawnPoint =
            SpawnPointsManager.SrvGetSpawnPoint(spawnPointId);
        var item = _items[player].First(f => f.Id == itemId);
        ObjectManager.RegisterPrefab(item.Descriptor);
        ObjectManager.Spawn(item.Descriptor, item.Options, spawnPoint.transform.position, spawnPoint.transform.rotation, o =>
        {
            var aa =
                o.GetComponent<DudeBase>();
            aa.SrvSetOwner(player);
            aa.SrvSetSpawnPoint(spawnPointId);
        }, null);
        _items[player] = _items[player].Where(w => w.Id != itemId).ToList();
        TargetRemoveItem(PlayersManager.GetPlayers().First(w => w.Slot == player).ConnectionInstance, itemId);
    }

    [TargetRpc]
    public void TargetSetItem(NetworkConnection connection, Guid id, FullItemDescriptor descriptor)
    {
        Debug.Log($"Item received event raised");
        ClientGotItem?.Invoke(new NetworkObjectInstanceDescriptor(id, descriptor.Dummy, descriptor.Options));
    }

    [TargetRpc]
    public void TargetRemoveItem(NetworkConnection connection, Guid id)
    {
        Debug.Log($"Item received event raised");
        ClientRemovedItem?.Invoke(id);
    }

    public override void SrvApplyOptions(NetworkObjectOptions options)
    {
        throw new NotImplementedException();
    }
}