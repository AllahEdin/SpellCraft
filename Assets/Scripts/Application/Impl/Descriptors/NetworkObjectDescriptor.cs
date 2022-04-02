using System;
using UnityEngine;

[Serializable]
public class NetworkObjectDescriptor
{
    /// <summary>
    /// ������. ����� ���� ���������� � ������ �������
    /// </summary>
    [SerializeField] private string _path;

    /// <summary>
    /// ���������� ������������� ��� ������� ������
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