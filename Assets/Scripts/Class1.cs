using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAreaDescriptor
{
    public Vector3 Center => _center;

    public bool InArea(Vector3 point)
    {
        if (point.x < _leftBorder)
            return false;
        if (point.x > _rightBorder)
            return false;
        if (point.z < _bottomBorder)
            return false;
        if (point.z > _topBorder)
            return false;

        return true;
    }

    private readonly Vector3 _center;
    private readonly int _width;
    private readonly int _height;

    private readonly float _leftBorder;
    private readonly float _rightBorder;

    private readonly float _topBorder;
    private readonly float _bottomBorder;

    public PlayerAreaDescriptor(Vector2 center, int width, int height)
    {
        _center = new Vector3(center.x, 0, center.y);
        _width = width;
        _height = height;
        _leftBorder = _center.x - (float)_width / 2;
        _rightBorder = _center.x + (float)_width / 2;
        _bottomBorder = _center.z - (float)_height/ 2;
        _topBorder = _center.z + (float)_height / 2;
    }
}
public class SpawnPointDescriptor
{
    public Vector3 Position => new Vector3(_pos.x, 0, _pos.y);
    public Quaternion Rotation => _rot;

    private readonly Vector2 _pos;
    private readonly Quaternion _rot;

    public SpawnPointDescriptor(Vector2 pos, Quaternion rot)
    {
        _pos = pos;
        _rot = rot;
    }
}

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


public static class UnitsDescriptor
{
    public static Dictionary<string, NetworkObjectDescriptor> NameUnitDescriptorDict = new Dictionary<string, NetworkObjectDescriptor>()
    {
        {"A", new NetworkObjectDescriptor("prefabs/A", "unit1", Guid.NewGuid())},
        {"B", new NetworkObjectDescriptor("prefabs/A", "unit2", Guid.NewGuid())}
    };
}