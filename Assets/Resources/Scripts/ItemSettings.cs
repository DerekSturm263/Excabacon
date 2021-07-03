using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public struct ItemSettings
{
    public List<ItemType> items;
    public float itemSpawnRate; // Smaller numbers have lower chances of spawning. 0 has no chance of spawning, 3 is the highest.
}
