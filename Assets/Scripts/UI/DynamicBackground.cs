using System.Collections.Generic;
using UnityEngine;

public class DynamicBackground : MonoBehaviour
{
    [SerializeField]
    List<string> strings = new();

    [SerializeField]
    int count;

    [SerializeField]
    BackgroundItem backgroundItem;

    [SerializeField]
    List<BackgroundItem> spawnedItems = new();

    private void Awake()
    {
        Spawn();
    }

    private void Spawn()
    {
        for (int i = 0; i < count; i++)
        {
            var spawnedItem = Instantiate(backgroundItem, this.transform);
            spawnedItem.Initialize(strings[Random.Range(0, strings.Count)]);
            spawnedItems.Add(spawnedItem);
        }
    }
}
