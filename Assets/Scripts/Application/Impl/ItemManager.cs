using System;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;
using Random = System.Random;

public class ItemManager : CustomNetworkBehaviour, IItemManager
{
    [SerializeField] private TextView _textView;

    private Dictionary<PlayerSlot, List<NetworkObjectDescriptor>> _items;

    public ItemManager()
    {
        _items = typeof(PlayerSlot).GetEnumNames()
            .Select(s => Enum.TryParse(s, out PlayerSlot slot) ? slot : throw new Exception())
            .ToDictionary(k => k, v => new List<NetworkObjectDescriptor>());
    }

    public event Action<NetworkObjectDescriptor> ClientGotItem;
    public event Action<Guid> ClientRemovedItem;

    [ClientCallback]
    private void Awake()
    {
        _textView.gameObject.SetActive(false);
    }

    [Server]
    public void SetItemToPlayer(PlayerSlot player, NetworkObjectDescriptor item)
    {
        _items[player].Add(item);
        _textView.SetText(string.Join("\n", _items.Select(s => s.Value).Select(s => string.Join(",", s))));
        Debug.Log($"Added item to player in slot {player}");
        TargetSetItem(PlayersManager.GetPlayers().First(w => w.Slot == player).ConnectionInstance, item.Id,
            item.Path, item.Name);
    }

    [Server]
    public void UseItem(PlayerSlot player, Guid itemId, Guid spawnPointId)
    {
        var spawnPoint =
            SpawnPointsManager.GetSpawnPoint(spawnPointId);
        var item = _items[player].First(f => f.Id == itemId);
        var unit = UnitsDescriptor.NameUnitDescriptorDict[item.Name];
        ObjectManager.RegisterPrefab(unit);
        ObjectManager.Spawn(unit, spawnPoint.transform.position, spawnPoint.transform.rotation, o =>
        {
            var aa =
                o.GetComponent<DudeBase>();
            aa.Owner = player;
            aa.SpawnPointId = spawnPointId;
            aa.ShotDescriptor = new NetworkObjectDescriptor("prefabs/Shot", "shotkek", Guid.NewGuid());
            var r = new Random();
            var options =
                new DudeOptionsBase()
                {
                    Cd = Convert.ToSingle(r.NextDouble() * 5 + 0.1f),
                    HpRegeneration = 0,
                    InaccuracyDegrees = Convert.ToSingle(r.NextDouble() * 90),
                    MaxHp = 10,
                };
            aa.Options = options;
        }, null);
        _items[player] = _items[player].Where(w => w.Id != itemId).ToList();
        TargetRemoveItem(PlayersManager.GetPlayers().First(w => w.Slot == player).ConnectionInstance, itemId);
    }

    [TargetRpc]
    public void TargetSetItem(NetworkConnection connection, Guid id, string path, string name)
    {
        Debug.Log($"Item received event raised");
        ClientGotItem?.Invoke(new NetworkObjectDescriptor(path, name, id));
    }

    [TargetRpc]
    public void TargetRemoveItem(NetworkConnection connection, Guid id)
    {
        Debug.Log($"Item received event raised");
        ClientRemovedItem?.Invoke(id);
    }
}