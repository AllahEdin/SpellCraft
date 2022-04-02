using System;
using UnityEngine;

[Serializable]
public class NetworkObjectOptions
{
    public string JsonOptions => _optionsJson;

    [SerializeField] private string _optionsJson;

    private NetworkObjectOptions()
    {
    }

    public NetworkObjectOptions(OptionsBase optionsBase)
    {
        _optionsJson = optionsBase.GetOptions();
    }

    public T GetOptions<T>()
    where T : OptionsBase
    {
        return JsonUtility.FromJson<T>(_optionsJson);
    }
}