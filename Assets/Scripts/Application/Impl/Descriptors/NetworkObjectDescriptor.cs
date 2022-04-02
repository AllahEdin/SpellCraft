using System;
using UnityEngine;

[Serializable]
public class NetworkObjectDescriptor
{
    /// <summary>
    /// Префаб. Может быть одинаковый у разных чуваков
    /// </summary>
    [SerializeField] private string _path;

    /// <summary>
    /// Уникальный идентификатор для каждого чувака
    /// </summary>
    [SerializeField] private string _key;

    private NetworkObjectDescriptor()
    {

    }

    public NetworkObjectDescriptor(string path, string key)
    {
        _path = path;
        _key = key;
    }

    public string Key => _key;

    public string Path => _path;
}