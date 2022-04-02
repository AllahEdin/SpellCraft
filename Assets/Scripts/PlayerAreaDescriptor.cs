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