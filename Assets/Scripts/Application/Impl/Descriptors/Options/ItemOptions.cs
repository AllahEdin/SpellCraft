using System;
using UnityEngine;

[Serializable]
public class ItemOptions : OptionsBase
{
    [SerializeField] public NetworkObjectDescriptor dudeDescriptor;
    [SerializeField] public DudeOptions dudeOptionsToSpawn;
}