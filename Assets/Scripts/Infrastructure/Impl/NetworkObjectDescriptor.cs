using System;
using UnityEngine;

[Serializable]
public class NetworkObjectDescriptor
{
    [SerializeField] private string _path;
    [SerializeField] private string _name;
    private Guid _id;

    private NetworkObjectDescriptor()
    {

    }

    public NetworkObjectDescriptor(string path, string name, Guid id)
    {
        _path = path;
        _name = name;
        _id = id;
    }

    public Guid Id => _id;

    public string Path => _path;

    public string Name => _name;
}