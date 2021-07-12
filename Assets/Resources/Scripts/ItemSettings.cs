using System.Collections.Generic;

public struct ItemSettings
{
    public List<ItemType> items;
    public float spawnRate; // Smaller numbers have lower chances of spawning. 0 has no chance of spawning, 3 is the highest.
    public float relicTime;

    public ItemSettings(List<ItemType> items, float spawnRate, float relicTime)
    {
        this.items = items;
        this.spawnRate = spawnRate;
        this.relicTime = relicTime;
    }
}
