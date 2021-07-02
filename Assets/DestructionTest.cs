using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructionTest : MonoBehaviour
{
    // Start is called before the first frame update
    public GameController gc;
    public int radius;
    bool haha = false;

    private void Update() {
        gc.TerrainInterface.DestroyTerrain(transform.position,radius, out haha);
        //print(haha);
    }
}
