using UnityEngine;

public abstract class OptionsBase
{
    public string GetOptions()
    {
        return JsonUtility.ToJson(this);
    }
}