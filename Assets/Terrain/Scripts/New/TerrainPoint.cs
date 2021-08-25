using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainPoint
{
    public bool On = true;
    Vector2 Position;

    public TerrainPoint(bool on,Vector2 position)
    {   
        On = on;
        Position = position;
    }
}
