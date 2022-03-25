    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Mirror;
    using UnityEngine;

    public class SpawnPointsManager : CustomNetworkBehaviour, ISpawnPointsManager
    {
        [SerializeField] private NetworkObjectDescriptor _spawnPointPrefab;
        [SerializeField] private List<Vector3> _player1SpawnPoints;
        [SerializeField] private List<Vector3> _player2SpawnPoints;

        private readonly List<GameObject> _spawnPoints = new List<GameObject>();

        private readonly Dictionary<PlayerSlot, List<SpawnPoint>> _spawnPointsDictionary;

        private readonly Dictionary<PlayerSlot, List<Guid>> _spawnPointsIds;

        public SpawnPointsManager()
        {
            _spawnPointsIds = typeof(PlayerSlot).GetEnumNames().Select(s =>
                    Enum.TryParse(s, out PlayerSlot slot) ? slot : throw new Exception())
                .ToDictionary(k => k, v => new List<Guid>());

            _spawnPointsDictionary =
                typeof(PlayerSlot).GetEnumNames().Select(s =>
                        Enum.TryParse(s, out PlayerSlot slot) ? slot : throw new Exception())
                    .ToDictionary(k => k, v => new List<SpawnPoint>());
        }

        [Server]
        public void AddSpawnPointsToPlayers()
        {
            foreach (var playerSlot in typeof(PlayerSlot).GetEnumNames().Select(s =>
                         Enum.TryParse(s, out PlayerSlot slot) ? slot : throw new Exception()))
            {
                var idCounter = 0;
                ObjectManager.RegisterPrefab(_spawnPointPrefab);

                IEnumerator<Vector3> enumerator = playerSlot switch
                {
                    PlayerSlot.One => _player1SpawnPoints.GetEnumerator(),
                    PlayerSlot.Two => _player2SpawnPoints.GetEnumerator(),
                    _ => throw new ArgumentOutOfRangeException()
                };

                while (enumerator.MoveNext())
                {
                    var localCounter = idCounter;
                    if (_spawnPointsIds[playerSlot].Count <= idCounter)
                    {
                        _spawnPointsIds[playerSlot].Add(Guid.NewGuid());
                    }
                    var instance =
                        ObjectManager.Spawn(_spawnPointPrefab, enumerator.Current,
                            playerSlot == PlayerSlot.One ? Quaternion.identity : Quaternion.Euler(0, 180, 0),
                            o =>
                            {
                                var sp = o.GetComponent<SpawnPoint>();
                                sp.Id = _spawnPointsIds[playerSlot][localCounter];
                            }, PlayersManager.GetPlayers().First(f => f.Slot == playerSlot).Id);

                    var spawnPoint = instance.GetComponent<SpawnPoint>();
                    _spawnPointsDictionary[playerSlot].Add(spawnPoint);
                    _spawnPoints.Add(instance);
                    idCounter++;
                }
            }
        }

        [Server]
        public void RemoveSpawnPointsToPlayers()
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

        public SpawnPoint GetSpawnPoint(PlayerSlot playerSlot, Guid id)
        {
            return _spawnPointsDictionary[playerSlot].First(f => f.Id == id);
        }
    }

