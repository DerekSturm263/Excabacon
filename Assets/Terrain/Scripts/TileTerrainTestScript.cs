using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Threading;
public class TileTerrainTestScript : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("Options")]
    Tilemap terrain;
    public TileProperties Tiles;
    public PerlinProperties NoiseGeneration;
    public TerrainDebugProperties TerrainDebug;
    
    public bool GenerateOnGameStart = true;
  
    void Start()
    {    
        if(GenerateOnGameStart)
            BuildTerrain();
        
    }
    public void BuildTerrain()
    {
        terrain = GetComponent<Tilemap>();
        terrain.ClearAllTiles();
        
        
        MakeBox();
        TileLoop();
    }
    
    private void MakeBox()
    {
        //establishes bound tiles,needed for proper tile bounds to fill with box
        terrain.SetTile(new Vector3Int(Tiles.MinTileBounds.x,Tiles.MinTileBounds.y,0),Tiles.TileToPlace);
       
        terrain.SetTile(new Vector3Int(Tiles.MinTileBounds.x,Tiles.MaxTileBounds.y,0),Tiles.TileToPlace);
        
        terrain.SetTile(new Vector3Int(Tiles.MaxTileBounds.x,Tiles.MinTileBounds.y,0),Tiles.TileToPlace);
        
        terrain.SetTile(new Vector3Int(Tiles.MaxTileBounds.x,Tiles.MaxTileBounds.y,0),Tiles.TileToPlace);

        // fills it
        terrain.BoxFill(Vector3Int.RoundToInt(transform.position),Tiles.TileToPlace,Tiles.MinTileBounds.x, Tiles.MinTileBounds.y,Tiles.MaxTileBounds.x,Tiles.MaxTileBounds.y);
    }
    //loops through all tiles after box is created to subtract holes for caves
    private void TileLoop()
    {
        float count_X = 0;
        float count_Y = 0;

        float TilePos_X =0;
        float Tilepos_Y = 0;
        float randomiser = Random.Range(-5000,5000); 
        
        float noise_value;
        

        TileBase[] terrain_tiles = terrain.GetTilesBlock(terrain.cellBounds);
        
        for(int i =0; i <terrain_tiles.Length; i++)
        {
            if(count_X >= Tiles.MaxTileBounds.x -Tiles.MinTileBounds.x){
                count_X = 0;
                count_Y += 1;
            }   else
            {
                count_X += 1;
            }
            
            TilePos_X = Tiles.MinTileBounds.x + count_X;
            Tilepos_Y = Tiles.MinTileBounds.y + count_Y;
            
            noise_value = GenerateNoise(new Vector2(TilePos_X,Tilepos_Y),randomiser);
            
            float noise_value_minMax = Remap(noise_value,NoiseGeneration.PerlinMinMax.x,NoiseGeneration.PerlinMinMax.y);      
            
            if(noise_value_minMax> NoiseGeneration.NoiseThreshold && !TerrainDebug.DebugNoise)
            {
                SubtractTile(Vector2Int.RoundToInt(new Vector2(TilePos_X,Tilepos_Y)));
            }
            if(TerrainDebug.DebugNoise){
                DebugNoise(Vector3Int.RoundToInt(new Vector3(TilePos_X,Tilepos_Y,0)),noise_value_minMax);
            }
        }
    }
    //generates perlin noise
     float GenerateNoise(Vector2 position,float seed)
    {
   
        float noise_x = position.x / NoiseGeneration.PerlinScaleX *NoiseGeneration.PerlinNoiseScaleModifier +seed ;
        float noise_y = position.y /NoiseGeneration.PerlinScaleY *NoiseGeneration.PerlinNoiseScaleModifier +seed;
        
        float noise_value;
        
        noise_value = Mathf.PerlinNoise(noise_x,noise_y);

        return noise_value;
    }
    
    private void DebugNoise(Vector3Int position,float Value)
    {
            Color tiledebugcolor = new Color(Value,Value,Value,1);
            terrain.SetTileFlags(Vector3Int.RoundToInt(new Vector3(position.x,position.y,0)),TileFlags.None);
            terrain.SetColor(Vector3Int.RoundToInt(new Vector3(position.x,position.y,0)),tiledebugcolor);
    }
    
    
    
    //code for subtracting a tile which should also allow for use within interfaces for when mining maybe?
    private void SubtractTile(Vector2Int tile_to_delete)
    {
        terrain.SetTile(new Vector3Int(tile_to_delete.x,tile_to_delete.y,0),null);
    }
    
    private void OnDrawGizmos() 
    {
        //draws points to points to make bounding box
        Gizmos.DrawLine(new Vector3(Tiles.MinTileBounds.x + 0.5f,Tiles.MinTileBounds.y + 0.5f,0),new Vector3(Tiles.MinTileBounds.x + 0.5f,Tiles.MaxTileBounds.y + 0.5f,0));
       
        Gizmos.DrawLine(new Vector3(Tiles.MinTileBounds.x + 0.5f,Tiles.MaxTileBounds.y + 0.5f,0),new Vector3(Tiles.MaxTileBounds.x + 0.5f,Tiles.MaxTileBounds.y + 0.5f,0));
        
        Gizmos.DrawLine(new Vector3(Tiles.MaxTileBounds.x + 0.5f,Tiles.MaxTileBounds.y + 0.5f,0),new Vector3(Tiles.MaxTileBounds.x + 0.5f,Tiles.MinTileBounds.y + 0.5f,0));
        
        Gizmos.DrawLine(new Vector3(Tiles.MinTileBounds.x + 0.5f,Tiles.MinTileBounds.y + 0.5f,0),new Vector3(Tiles.MaxTileBounds.x + 0.5f,Tiles.MinTileBounds.y + 0.5f,0));
    }

    // remap function, used for remapping the perlin noise to allow for higher or lower scales but could also be turned into a function library thingy maybe later if needed?
    float Remap(float Value ,float min,float max)
    {
        
        float remapped = 0 + (Value - min) * (1 - 0)/ (max - min);

        return remapped;
    }

    //tiles properties, stored in a class for clean editor
    [System.Serializable]
    public class TileProperties
    {
        public Tile TileToPlace;
    
        public Vector2Int MinTileBounds = new Vector2Int(-4,-4);
        public Vector2Int MaxTileBounds = new Vector2Int(4,4);
    }
    
    
    
    //Perlin Noise properties, stored in a class for clean editor
    [System.Serializable]
    public class PerlinProperties
    {
        [Range(1,1024)]
        [SerializeField]
        public float PerlinScaleX = 256;

        [Range(1,1024)]
        public float PerlinScaleY = 256;
        [Range(0,250)]
        public float PerlinNoiseScaleModifier = 20;

        public Vector2 PerlinMinMax = new Vector2(0,1);
        [Range(0,2f)]
        public float NoiseThreshold = 0.5f;

    }
    
    [System.Serializable]
    public class TerrainDebugProperties
    {
        public bool DebugNoise;
    }
}
