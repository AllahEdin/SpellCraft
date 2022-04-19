using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
using Mirror;
using UnityEngine;

public class ChooseSpellState : CustomNetworkBehaviourBase, IGameState
{
    [SerializeField] private NetworkObjectDescriptor _defaultPlayerPositionDescriptor;

    public event Action<IGameState> CompleteState;
    public GameState State => GameState.ChooseSpell;

    private bool _isActive;
    private bool _chooseTime;
    private float _currentTime;

    private Dictionary<PlayerSlot, bool> _pickedItems;

    private Guid[] _playerIds;
    
    private Dictionary<Guid, List<FullItemDescriptor>> _itemsToChoose;
    private ItemsRepository _itemsRepository;
    private Dictionary<PlayerSlot, List<PickableItem>> _pickableItems = new Dictionary<PlayerSlot, List<PickableItem>>();
    private List<DefaultPlayerPosition> _defaultPlayerPositions = new List<DefaultPlayerPosition>();

    private void Awake()
    {
        _itemsRepository = FindObjectOfType<ItemsRepository>();
        _pickedItems = new Dictionary<PlayerSlot, bool>();
        _itemsToChoose = new Dictionary<Guid, List<FullItemDescriptor>>();
    }

    [ServerCallback]
    private void FixedUpdate()
    {
        if (_isActive)
        {
            _currentTime += Time.deltaTime;

            if (_currentTime > 10 || !_chooseTime)
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
        PlayersManager.PlayersOnDefaultPositions += SrvPlayersManagerOnPlayersOnDefaultPositions;
        _pickedItems.Clear();
        PlayersManager.SrvMovePlayersToDefaultPositions();
        _playerIds = PlayersManager.SrvGetPlayers().Select(s => s.Id).ToArray();
        _itemsToChoose = _playerIds.ToDictionary(k => k, v => new List<FullItemDescriptor>());

        foreach (var playerId in _playerIds)
        {
            _itemsToChoose[playerId].AddRange(_itemsRepository.GetRandomDescriptorsForPlayer(3, PlayersManager.SrvGetPlayer(playerId).Slot).Select(s => new FullItemDescriptor()
            {
                Dummy = s.dummy,
                Options = s.opt
            }));
        }

        foreach (var playerAreaDescriptor in MapDescriptor.Areas)
        {
            ObjectManager.RegisterPrefab(_defaultPlayerPositionDescriptor);
            var go = ObjectManager.SrvSpawn(_defaultPlayerPositionDescriptor, new NetworkObjectOptions(new DefaulpPlayerPositionOptions(playerAreaDescriptor.Key)),
                playerAreaDescriptor.Value.Center, Quaternion.identity,
                o => { }, null);
        }
    }

    [Server]
    private void SrvPlayersManagerOnPlayersOnDefaultPositions()
    {
        PlayersManager.PlayersOnDefaultPositions -= SrvPlayersManagerOnPlayersOnDefaultPositions;
        _isActive = true;
        _chooseTime = true;
        _currentTime = 0;

        foreach (var item in _itemsToChoose)
        {
            int count = 0;
            var player = PlayersManager.SrvGetPlayer(item.Key);
            _pickableItems.Add(player.Slot, new List<PickableItem>());
            foreach (var fullItemDescription in item.Value)
            {
                var pos = MapDescriptor.SpellSpawnPoints[player.Slot][count];
                count++;

                ObjectManager.RegisterPrefab(fullItemDescription.Dummy);
                var gameObjectWithDescriptor =
                    ObjectManager.SrvSpawn(fullItemDescription.Dummy, fullItemDescription.Options, pos.Position, pos.Rotation, go =>
                    {
                        var pickableItem =
                            go.GetComponent<PickableItem>();
                        pickableItem.PickedByPlayer += PickableItemOnPickedByPlayer;
                    }, item.Key);

                _pickableItems[player.Slot].Add(gameObjectWithDescriptor.GameObject.GetComponent<PickableItem>());
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
        ItemManager.SrvSetItemToPlayer(slot, new FullItemDescriptor()
        {
            Options = pi.Options,
            Dummy = pi.Descriptor,
        });
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