using System.Collections.Generic;
using UnityEngine;

public static class MapDescriptor
{
    public static Dictionary<PlayerSlot, List<SpawnPointDescriptor>> UnitSpawnPoints =>
        new Dictionary<PlayerSlot, List<SpawnPointDescriptor>>()
        {
            {
                PlayerSlot.One, new List<SpawnPointDescriptor>()
                {
                    new SpawnPointDescriptor(new Vector2(-1 ,-3), Quaternion.identity),
                    new SpawnPointDescriptor(new Vector2(0, -3), Quaternion.identity),
                    new SpawnPointDescriptor(new Vector2(1, -3), Quaternion.identity)
                }
            },
            {
                PlayerSlot.Two, new List<SpawnPointDescriptor>()
                {
                    new SpawnPointDescriptor(new Vector2(-1, 3), Quaternion.Euler(0,180,0)),
                    new SpawnPointDescriptor(new Vector2(0, 3), Quaternion.Euler(0,180,0)),
                    new SpawnPointDescriptor(new Vector2(1,3), Quaternion.Euler(0,180,0))
                }
            }
        };

    public static Dictionary<PlayerSlot, List<SpawnPointDescriptor>> SpellSpawnPoints =>
        new Dictionary<PlayerSlot, List<SpawnPointDescriptor>>()
        {
            {
                PlayerSlot.One, new List<SpawnPointDescriptor>()
                {
                    new SpawnPointDescriptor(new Vector2(-1 ,-4), Quaternion.identity),
                    new SpawnPointDescriptor(new Vector2(0, -4), Quaternion.identity),
                    new SpawnPointDescriptor(new Vector2(1, -4), Quaternion.identity)
                }
            },
            {
                PlayerSlot.Two, new List<SpawnPointDescriptor>()
                {
                    new SpawnPointDescriptor(new Vector2(-1, 4), Quaternion.identity),
                    new SpawnPointDescriptor(new Vector2(0, 4), Quaternion.identity),
                    new SpawnPointDescriptor(new Vector2(1,4), Quaternion.identity)
                }
            }
        };

    public static Dictionary<PlayerSlot, PlayerAreaDescriptor> Areas =>
        new Dictionary<PlayerSlot, PlayerAreaDescriptor>()
        {
            {
                PlayerSlot.One, new PlayerAreaDescriptor(new Vector2(0, -5), 5, 4)
            },
            {
                PlayerSlot.Two, new PlayerAreaDescriptor(new Vector2(0, 5), 5, 4)
            }
        };
}

