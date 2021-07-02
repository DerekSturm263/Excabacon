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

    
    public void DestroyTerrain(Vector3 position, int radius,out bool Hitblock)
    {
       if(radius == 0){
           Debug.LogError("Radius is 0! things will not work properly");
       }
        float count_X = 0;
        float count_Y = 0;
        TileBase touchtile = null;
        Hitblock = false;
        if(radius > 1)
        {
            for(int i = 0; i < radius * radius;i++ )
            {
            
                if(count_X >= radius){
                    count_X = 0;
                    count_Y += 1;
                    count_X += 1;
                }   else
                {
                    count_X += 1;
                }
                
                Vector3 radiusOffsetPosition = new Vector3(position.x - (radius/2 +1 ) + count_X,position.y - (radius/2 )+ count_Y,0);
                
                Vector3Int tile_pos = Vector3Int.RoundToInt(radiusOffsetPosition);
                if (touchtile ==null)
                    touchtile = terrain.GetTile(tile_pos);
                    Hitblock = false;
                if (touchtile !=null)
                {
                    Hitblock = true;
                }
                
                terrain.SetTile(tile_pos,null);
            
            }

        }else
        {
            Vector3Int tile_pos = Vector3Int.RoundToInt(position);
            
             if (touchtile ==null)
                    touchtile = terrain.GetTile(tile_pos);
                    Hitblock = false;
                if (touchtile !=null)
                {
                    Hitblock = true;    
                }
            terrain.SetTile(tile_pos,null);
            
        }
        
    }
}
