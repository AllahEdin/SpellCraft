using System;
using UnityEngine;

[Serializable]
public class FullItemDescriptor
{
    [SerializeField] public NetworkObjectDescriptor Dummy { get; set; }

    [SerializeField] public NetworkObjectOptions Options { get; set; }
}