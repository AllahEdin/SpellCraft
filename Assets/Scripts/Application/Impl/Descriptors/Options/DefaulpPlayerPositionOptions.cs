using System;
using UnityEngine;

[Serializable]
public class DefaulpPlayerPositionOptions : OptionsBase
{
    [SerializeField] public PlayerSlot slot;

    public DefaulpPlayerPositionOptions(PlayerSlot slot)
    {
        this.slot = slot;
    }
}