using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class ItemsRepository : MonoBehaviour
{
    [SerializeField] private List<NetworkObjectDescriptor> _itemDescriptors = new List<NetworkObjectDescriptor>();

    public IEnumerable<NetworkObjectDescriptor> GetRandomItems(int count)
    {
        var items =
            _itemDescriptors
                .Select(s => new NetworkObjectDescriptor(s.Path, s.Name, Guid.NewGuid())).ToArray();

        int localCount = 0;

        while (localCount < count && items.Length > 0)
        {
            localCount++;
            var val =
                Random.Range(0, items.Length);

            yield return items[val];

            var newArray =
                Enumerable.Range(0, items.Length)
                    .Where(w => w != val)
                    .Select(s => items[s])
                    .ToArray();

            items = newArray;
        }
    }
}