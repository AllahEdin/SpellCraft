using UnityEngine;

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