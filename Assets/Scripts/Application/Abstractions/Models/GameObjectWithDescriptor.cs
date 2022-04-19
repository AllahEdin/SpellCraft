using UnityEngine;

public class GameObjectWithDescriptor
{
    public GameObjectWithDescriptor(NetworkObjectInstanceDescriptor instanceDescriptor, GameObject go)
    {
        InstanceDescriptor = instanceDescriptor;
        GameObject = go;
    }

    public NetworkObjectInstanceDescriptor InstanceDescriptor { get; }

    public GameObject GameObject { get; }
}