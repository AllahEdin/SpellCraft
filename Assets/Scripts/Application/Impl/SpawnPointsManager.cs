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

        public SpawnPointsManager()
        {
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
                ObjectManager.RegisterPrefab(_spawnPointPrefab);

                IEnumerator<Vector3> enumerator = playerSlot switch
                {
                    PlayerSlot.One => _player1SpawnPoints.GetEnumerator(),
                    PlayerSlot.Two => _player2SpawnPoints.GetEnumerator(),
                    _ => throw new ArgumentOutOfRangeException()
                };

                while (enumerator.MoveNext())
                {
                    var instance =
                        ObjectManager.Spawn(_spawnPointPrefab, enumerator.Current,
                            Quaternion.identity,
                            o =>
                            {
                                var sp = o.GetComponent<SpawnPoint>();
                                sp.Id = Guid.NewGuid();
                            }, PlayersManager.GetPlayers().First(f => f.Slot == playerSlot).Id);

                    var spawnPoint = instance.GetComponent<SpawnPoint>();
                    _spawnPointsDictionary[playerSlot].Add(spawnPoint);
                    _spawnPoints.Add(instance);
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
        }

        public SpawnPoint GetSpawnPoint(PlayerSlot playerSlot, Guid id)
        {
            return _spawnPointsDictionary[playerSlot].First(f => f.Id == id);
        }
    }

