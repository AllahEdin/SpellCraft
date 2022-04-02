using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class ItemsRepository : MonoBehaviour
{
    [SerializeField] private List<NetworkObjectDescriptor> _itemDescriptors = new List<NetworkObjectDescriptor>();

    public IEnumerable<(NetworkObjectDescriptor dummy, NetworkObjectOptions opt)> GetRandomDescriptorsForPlayer(int count, PlayerSlot owner)
    {
        var items =
            _itemDescriptors
                .Select(s => new NetworkObjectDescriptor(s.Path, s.Key)).ToArray();

        int localCount = 0;

        while (localCount < count && items.Length > 0)
        {
            localCount++;
            var val =
                Random.Range(0, items.Length);

            yield return (items[val], new NetworkObjectOptions(new ItemOptions()
            {
                dudeDescriptor = new NetworkObjectDescriptor("prefabs/dude", "supadude"),
                dudeOptionsToSpawn = new DudeOptions() 
                { 
                    cd = 1,
                    health = 100,
                    regeneration = 1,
                    shot_descriptor = new NetworkObjectDescriptor("prefabs/Shot", "simpleShot"),
                    inaccuracy_angle = Random.Range(0, 50),
                },
            }));

            var newArray =
                Enumerable.Range(0, items.Length)
                    .Where(w => w != val)
                    .Select(s => items[s])
                    .ToArray();

            items = newArray;
        }
    }
}