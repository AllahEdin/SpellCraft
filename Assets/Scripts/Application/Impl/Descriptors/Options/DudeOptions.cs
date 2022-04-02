using System;
using UnityEngine;

[Serializable]
public class DudeOptions : OptionsBase
{
    [SerializeField] public NetworkObjectDescriptor shot_descriptor;

    [SerializeField] public float health;
    [SerializeField] public float inaccuracy_angle;
    [SerializeField] public float cd;
    [SerializeField] public float regeneration;
}