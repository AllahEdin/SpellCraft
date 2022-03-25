using System;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : CustomNetworkBehaviour
{
    private class ItemWithDescriptor
    {
        public ItemWithDescriptor(GameObject gameObject, NetworkObjectDescriptor descriptor)
        {
            ItemGO = gameObject;
            Descriptor = descriptor;
        }

        public GameObject ItemGO { get; }

        public NetworkObjectDescriptor Descriptor { get; }
    }

    [SerializeField] private GameObject _contentGameObject;
    [SerializeField] private GameObject _inventoryItemPrefab;

    private List<ItemWithDescriptor> _items = new List<ItemWithDescriptor>();

    public override void OnStartClient()
    {
        base.OnStartClient();
        ItemManager.ClientGotItem += AddItem;
        ItemManager.ClientRemovedItem += RemoveItem;
    }

    public void DestroyView()
    {
        Destroy(_contentGameObject);
        Destroy(this);
    }

    [ClientCallback]
    public void AddItem(NetworkObjectDescriptor objectDescriptor)
    {
        var go = Instantiate(_inventoryItemPrefab, _contentGameObject.transform);
        _items.Add(new ItemWithDescriptor(go, objectDescriptor));
        var text = go.GetComponentInChildren<Text>();
        text.text = objectDescriptor.Name;
    }

    [ClientCallback]
    public void RemoveItem(Guid id)
    {
        var target = _items.First(f => f.Descriptor.Id == id);
        Destroy(target.ItemGO);
        _items.Remove(target);
    }
}
