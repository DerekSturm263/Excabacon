using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor.Tilemaps;

using System.Threading;
public class TileTerrainTestScript : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("Options")]
    public Tilemap terrain;
    public TileProperties Tiles;
    public PerlinProperties NoiseGeneration;

    public EdgeProperties Edges;
    public TerrainDebugProperties TerrainDebug;
    
    public bool GenerateOnGameStart = true;
    
    public TerrainModifierStack modstack;
    void Start()
    {    
        if(GenerateOnGameStart)
            BuildTerrain();
        
    }
    public void BuildTerrain()
    {
        //terrain = GetComponent<Tilemap>();
        terrain.ClearAllTiles();
        
        
        //MakeBox();
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
        //terrain.BoxFill(Vector3Int.RoundToInt(transform.position),Tiles.TileToPlace,Tiles.MinTileBounds.x, Tiles.MinTileBounds.y,Tiles.MaxTileBounds.x,Tiles.MaxTileBounds.y);
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
        
        int TileSize = (Mathf.Abs(Tiles.MinTileBounds.x) + Mathf.Abs(Tiles.MaxTileBounds.x)) *(Mathf.Abs(Tiles.MinTileBounds.y) + Mathf.Abs(Tiles.MaxTileBounds.y));
        
        TileBase[] terrain_tiles = terrain.GetTilesBlock(terrain.cellBounds);
        
        for(int i =0; i <TileSize; i++)
        {
            if(count_X >= Tiles.MaxTileBounds.x -Tiles.MinTileBounds.x){
                count_X = 0;
                count_Y += 1;
                count_X += 1;
            }   else
            {
                count_X += 1;
            }
            
            TilePos_X = Tiles.MinTileBounds.x + count_X;
            Tilepos_Y = Tiles.MinTileBounds.y + count_Y;
            
            //noise_value = GenerateNoise(new Vector2(TilePos_X,Tilepos_Y),randomiser);
            noise_value = modstack.CalculateAll(new Vector2(TilePos_X,Tilepos_Y));
            
            float noise_value_minMax = Remap(noise_value,NoiseGeneration.PerlinMinMax.x,NoiseGeneration.PerlinMinMax.y);      
            float gradient_test = generateGradient(new Vector2(count_X,count_Y));
            
            float noisePlusGradient =Mathf.Clamp(noise_value_minMax,0,1) - gradient_test  ;
            if(noisePlusGradient > NoiseGeneration.NoiseThreshold && !TerrainDebug.DebugNoise)
            {
                SubtractTile(Vector2Int.RoundToInt(new Vector2(TilePos_X,Tilepos_Y)));
            }else
            {
                terrain.SetTile(Vector3Int.RoundToInt(new Vector3(TilePos_X,Tilepos_Y,0)),Tiles.TileToPlace) ;
            }
            
            
            if(TerrainDebug.DebugNoise){
                DebugNoise(Vector3Int.RoundToInt(new Vector3(TilePos_X,Tilepos_Y,0)),noisePlusGradient );
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

    float generateGradient(Vector2 position)
    {
        float gradient_value;
        
        float BottomGradient = position.y / (Mathf.Abs(Tiles.MinTileBounds.y)  + Mathf.Abs(Tiles.MaxTileBounds.y) );

        float TopGradient = 1 - BottomGradient;
        
        float LeftGradient = position.x / (Mathf.Abs(Tiles.MinTileBounds.x)  + Mathf.Abs(Tiles.MaxTileBounds.x )  );
        
        float RightGradient = 1 - LeftGradient;


        float bg_eval = Edges.GradientCurve.Evaluate(BottomGradient);
        float tg_eval = Edges.GradientCurve.Evaluate(TopGradient); 
        float lg_eval = Edges.GradientCurve.Evaluate(LeftGradient); 
        float rg_eval = Edges.GradientCurve.Evaluate(RightGradient); 
        
        gradient_value = (bg_eval + tg_eval +lg_eval + rg_eval);
        
        return gradient_value;
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
    public static float Remap(float Value ,float min,float max)
    {
        
        float remapped = 0 + (Value - min) * (1 - 0)/ (max - min);

        return remapped;
    }

    //tiles properties, stored in a class for clean editor
    [System.Serializable]
    public class TileProperties
    {
        public TileBase TileToPlace;
    
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
    public class EdgeProperties
    {
        public AnimationCurve GradientCurve;
    }
    
    [System.Serializable]
    public class TerrainDebugProperties
    {
        public bool DebugNoise;
    }
}
