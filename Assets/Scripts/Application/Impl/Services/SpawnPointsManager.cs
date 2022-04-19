    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Mirror;
    using UnityEngine;

    public class SpawnPointsManager : CustomNetworkBehaviourBase, ISpawnPointsManager
    {
        [SerializeField] private NetworkObjectDescriptor _spawnPointDescriptor;

        private readonly List<GameObject> _spawnPoints = new List<GameObject>();

        private readonly Dictionary<PlayerSlot, List<SpawnPoint>> _spawnPointsDictionary;

        private readonly Dictionary<PlayerSlot, List<Guid>> _spawnPointsIds;

        private readonly Dictionary<PlayerSlot, List<Guid>> _slotsUsed;

        public SpawnPointsManager()
        {
            _slotsUsed =
                typeof(PlayerSlot).GetEnumNames().Select(s =>
                        Enum.TryParse(s, out PlayerSlot slot) ? slot : throw new Exception())
                    .ToDictionary(k => k, v => new List<Guid>());

            _spawnPointsIds = typeof(PlayerSlot).GetEnumNames().Select(s =>
                    Enum.TryParse(s, out PlayerSlot slot) ? slot : throw new Exception())
                .ToDictionary(k => k, v => new List<Guid>());

            _spawnPointsDictionary =
                typeof(PlayerSlot).GetEnumNames().Select(s =>
                        Enum.TryParse(s, out PlayerSlot slot) ? slot : throw new Exception())
                    .ToDictionary(k => k, v => new List<SpawnPoint>());
        }


        [Server]
        public void SrvAddSpawnPointsToPlayers()
        {
            foreach (var playerSlot in typeof(PlayerSlot).GetEnumNames().Select(s =>
                         Enum.TryParse(s, out PlayerSlot slot) ? slot : throw new Exception()))
            {
                var idCounter = 0;
                ObjectManager.RegisterPrefab(_spawnPointDescriptor);

                using IEnumerator<Vector3> enumerator = MapDescriptor.UnitSpawnPoints[playerSlot].Select(s => s.Position).GetEnumerator();
                
                while (enumerator.MoveNext())
                {
                    var localCounter = idCounter;
                    if (_spawnPointsIds[playerSlot].Count <= idCounter)
                    {
                        _spawnPointsIds[playerSlot].Add(Guid.NewGuid());
                    }

                    var gameObjectWithDescriptor =
                        ObjectManager.SrvSpawn(_spawnPointDescriptor, new NetworkObjectOptions(new SpawnPointOptions()
                            {
                                id = _spawnPointsIds[playerSlot][localCounter].ToString()
                            }) , enumerator.Current,
                            playerSlot == PlayerSlot.One ? Quaternion.identity : Quaternion.Euler(0, 180, 0),
                            o => { }, PlayersManager.SrvGetPlayers().First(f => f.Slot == playerSlot).Id);

                    var spawnPoint = gameObjectWithDescriptor.GameObject.GetComponent<SpawnPoint>();
                    _spawnPointsDictionary[playerSlot].Add(spawnPoint);
                    _spawnPoints.Add(gameObjectWithDescriptor.GameObject);
                    idCounter++;
                }
            }
        }

        [Server]
        public void SrvRemoveSpawnPointsToPlayers()
        {
            foreach (var spawnPoint in _spawnPoints)
            {
                NetworkServer.Destroy(spawnPoint);
            }
            _spawnPoints.Clear();
            foreach (var spdItem in _spawnPointsDictionary)
            {
                spdItem.Value.Clear();
            }
        }

        [Server]
        public SpawnPoint SrvGetSpawnPoint(Guid id)
        {
            return _spawnPointsDictionary.SelectMany(s => s.Value).First(f => f.Id == id);
        }

        [Server]
        public bool SrvIsEmpty(Guid id)
        {
            return _slotsUsed.SelectMany(s => s.Value).FirstOrDefault(f => f == id).Equals(Guid.Empty);
        }

        [Server]
        public void SrvSetIsEmpty(PlayerSlot playerSlot, Guid id, bool isEmpty)
        {
            if (!isEmpty)
            {
                _slotsUsed[playerSlot].Add(id);
            }
            else
            {
                _slotsUsed[playerSlot].Remove(id);
            }
        }
    }

