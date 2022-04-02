using System;
using UnityEngine;

[Serializable]
public class NetworkObjectInstanceDescriptor
{
    [SerializeField] private Guid _id;
    [SerializeField] private NetworkObjectDescriptor _descriptor;
    [SerializeField] private NetworkObjectOptions _options;

    private NetworkObjectInstanceDescriptor()
    {

    }

    public NetworkObjectInstanceDescriptor(Guid id, NetworkObjectDescriptor descriptor, NetworkObjectOptions options)
    {
        _descriptor = descriptor;
        _options = options;
        _id = id;
    }

    public Guid Id => _id;

    public NetworkObjectDescriptor Descriptor => _descriptor;
    public NetworkObjectOptions Options => _options;
}