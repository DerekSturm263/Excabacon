using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public class InteracteabkeTerrain : MonoBehaviour, ModifyTerrain
{
    private Tilemap terrain;
    
    // Start is called before the first frame update
    void Start()
    {
        terrain = GetComponent<Tilemap>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void DestroyTerrain(Vector3 position, int radius)
    {
        print("not currently implemented");
    }
}
