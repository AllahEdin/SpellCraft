using System;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;

namespace Assets.Scripts
{
    public class Player : NetworkBehaviour
    {
        public float speed = 1;

        public PlayerSlot Slot { get; private set; }

        private IPlayersManager<NetworkConnection> _playersManager;
        private IItemManager _itemManager;
        private PlayerInputManager _playerInputManager;

        private List<NetworkObjectInstanceDescriptor> _items = new List<NetworkObjectInstanceDescriptor>();

        [SerializeField] private Inventory _clientInventory;

        [SerializeField] private TextView _textView;

        public override void OnStartServer()
        {
            Debug.Log("Awake server player");
            base.OnStartServer();
            _itemManager = FindObjectOfType<ItemManager>();
            
            _playersManager = FindObjectOfType<PlayerManager>();
            _playersManager.MoveToDefaultPositions += PlayersManagerOnMoveToDefaultPositions;

            _playerInputManager = FindObjectOfType<PlayerInputManager>();
            _clientInventory = FindObjectOfType<Inventory>();
            DestroyInventoryView();
        }

        [Server]
        private void PlayersManagerOnMoveToDefaultPositions()
        {
            foreach (var player in _playersManager.GetPlayers())
            {
                TargetMoveToPosition(player.ConnectionInstance, MapDescriptor.Areas[player.Slot].Center);   
            }
        }

        [TargetRpc]
        public void TargetMoveToPosition(NetworkConnection connection, Vector3 pos)
        {
            if (isLocalPlayer)
            {
                gameObject.transform.position = pos;
            }
        }

        public override void OnStartLocalPlayer()
        {
            Debug.Log("Awake local player");
            base.OnStartLocalPlayer();
            _itemManager = FindObjectOfType<ItemManager>();
            _clientInventory = FindObjectOfType<Inventory>();
            _itemManager.ClientGotItem += ItemManagerOnClientGotItem;
            _itemManager.ClientRemovedItem += ItemManagerOnClientRemovedItem;
        }

        [ServerCallback]
        private void DestroyInventoryView()
        {
            _clientInventory?.DestroyView();
        }

        [Server]
        public void SetSlot(PlayerSlot playerSlot)
        {
            Slot = playerSlot;
        }

        [TargetRpc]
        public void TargetSetSlot(NetworkConnection connection, PlayerSlot playerSlot)
        {
            Slot = playerSlot;
        }

        [ClientCallback]
        private void ItemManagerOnClientGotItem(NetworkObjectInstanceDescriptor obj)
        {
            _items.Add(obj);
            _textView.SetText("");
        }

        [ClientCallback]
        private void ItemManagerOnClientRemovedItem(Guid id)
        {
            _items = _items.Where(w => w.Id != id).ToList();
        }

        [Command]
        private void CmdPlayerAttemptsToUseItem(Guid itemId, Guid spawnPointId)
        {
            Debug.Log($"Server uses item {itemId} on sp {spawnPointId}");
            _playerInputManager.RaisePlayerUseItemEvent(Slot, itemId, spawnPointId);
        }

        void FixedUpdate()
        {
            if (isLocalPlayer)
            {
                var axis =
                    new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

                var newPos =
                    gameObject.transform.position + new Vector3(axis.x * speed * Time.fixedDeltaTime,
                        0, axis.y * speed * Time.fixedDeltaTime);

                if ((newPos - gameObject.transform.position).sqrMagnitude > 0)
                {
                    if (MapDescriptor.Areas[Slot].InArea(newPos))
                    {
                        gameObject.transform.position = newPos;
                    }
                }
            }
        }

        void Update()
        {
            if (isLocalPlayer)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Debug.Log($"Mouse hit");

                    var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    var hits = Physics.RaycastAll(ray, 100f);

                    foreach (var hit in hits)
                    {
                        var spawnPoint = hit.transform.GetComponent<SpawnPoint>();
                        if (spawnPoint == null)
                        {
                            continue;
                        }
                        else
                        {
                            Debug.Log($"Mouse hit spawn point");

                            if (Input.anyKeyDown)
                            {
                                Debug.Log($"Mouse hit spawn point and key pressed");

                                var key =
                                    Enumerable.Range(1, 9).FirstOrDefault(f => Input.GetKey(f.ToString()));

                                if (key > 0)
                                {
                                    Debug.Log($"Mouse hit spawn point and key {key} pressed.");

                                    if (_items.Count > key - 1)
                                    {
                                        if (_items[key - 1] != null)
                                        {
                                            if (spawnPoint.netIdentity.hasAuthority)
                                            {
                                                Debug.Log($"{_items[key - 1].Id} Used");
                                                CmdPlayerAttemptsToUseItem(_items[key - 1].Id, spawnPoint.Id);
                                            }
                                        }
                                    }
                                }
                            }

                            break;
                        }
                    }
                }
            }
        }
    }
}