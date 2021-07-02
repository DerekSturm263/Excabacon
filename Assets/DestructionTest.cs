using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructionTest : MonoBehaviour
{
    // Start is called before the first frame update
    public int radius;

    private void Update() {
        GameController.TerrainInterface.DestroyTerrain(transform.position,radius);
    }
}
