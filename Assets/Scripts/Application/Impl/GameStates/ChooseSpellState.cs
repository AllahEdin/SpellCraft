using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
using Mirror;
using UnityEngine;

public class ChooseSpellState : CustomNetworkBehaviour, IGameState
{
    public event Action<IGameState> CompleteState;
    public GameState State => GameState.ChooseSpell;

    private bool _isActive;
    private bool _chooseTime;
    private float _currentTime;

    private Dictionary<PlayerSlot, bool> _pickedItems;

    private Guid[] _playerIds;
    
    private Dictionary<Guid, List<NetworkObjectDescriptor>> _itemsToChoose;
    private ItemsRepository _itemsRepository;
    private Dictionary<PlayerSlot, List<PickableItem>> _pickableItems = new Dictionary<PlayerSlot, List<PickableItem>>();

    private void Awake()
    {
        _itemsRepository = FindObjectOfType<ItemsRepository>();
        _pickedItems = new Dictionary<PlayerSlot, bool>();
        _itemsToChoose = new Dictionary<Guid, List<NetworkObjectDescriptor>>();
    }

    [ServerCallback]
    private void FixedUpdate()
    {
        if (_isActive)
        {
            _currentTime += Time.deltaTime;

            if (_currentTime > 20 || !_chooseTime)
            {
                foreach (var pickableItemForPlayer in _pickableItems)
                {
                    foreach (var pickableItem in pickableItemForPlayer.Value)
                    {
                        NetworkServer.Destroy(pickableItem.gameObject);
                    }
                }

                _pickableItems.Clear();
                _chooseTime = false;
                _isActive = false;
                CompleteState?.Invoke(this);
            }
        }
    }

    [ServerCallback]
    public void StartState()
    {
        _pickedItems.Clear();

        _playerIds = PlayersManager.GetPlayers().Select(s => s.Id).ToArray();
        _itemsToChoose = _playerIds.ToDictionary(k => k, v => new List<NetworkObjectDescriptor>());

        foreach (var playerId in _playerIds)
        {
            _itemsToChoose[playerId].AddRange(_itemsRepository.GetRandomItems(3));
        }

        _isActive = true;
        _chooseTime = true;
        _currentTime = 0;

        foreach (var item in _itemsToChoose)
        {
            int count = 0;
            var player = PlayersManager.GetPlayer(item.Key);
            _pickableItems.Add(player.Slot, new List<PickableItem>());
            foreach (var networkObjectDescriptor in item.Value)
            {
                int localCount = count++;
                var pos = new Vector3(-3 - (float)item.Value.Count / 2 + localCount, 0, player.Slot == PlayerSlot.One ? -3 : 3);

                ObjectManager.RegisterPrefab(networkObjectDescriptor);
                var instance =
                    ObjectManager.Spawn(networkObjectDescriptor, pos, Quaternion.identity, go =>
                    {
                        var pickableItem =
                            go.GetComponent<PickableItem>();
                        pickableItem.RpcSetKey(networkObjectDescriptor.Name);
                        pickableItem.Key = $"{networkObjectDescriptor.Name}";
                        pickableItem.Descriptor = networkObjectDescriptor;
                        pickableItem.PickedByPlayer += PickableItemOnPickedByPlayer;
                    }, item.Key);

                _pickableItems[player.Slot].Add(instance.GetComponent<PickableItem>());
            }
        }
    }

    [ServerCallback]
    private void PickableItemOnPickedByPlayer(PickableItem pi, PlayerSlot slot)
    {
        if (_pickedItems.ContainsKey(slot) && _pickedItems[slot])
        {
            return;
        }
        _pickedItems.Add(slot, true);
        ItemManager.SetItemToPlayer(slot, pi.Descriptor);
        RemovePickableItemsForPlayer(slot);
        if (typeof(PlayerSlot).GetEnumNames().All(a => Enum.TryParse(a, out PlayerSlot ps) ? (_pickedItems.ContainsKey(ps) && _pickedItems[ps]) : throw new Exception()))
        {
            _chooseTime = false;
        }
    }

    [ServerCallback]
    private void RemovePickableItemsForPlayer(PlayerSlot slot)
    {
        foreach (var pickableItem in _pickableItems[slot])
        {
            pickableItem.PickedByPlayer -= PickableItemOnPickedByPlayer;
            NetworkServer.Destroy(pickableItem.gameObject);
        }

        _pickableItems[slot].Clear();
    }
}