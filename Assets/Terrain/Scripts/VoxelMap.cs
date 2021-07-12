using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[SelectionBase]
public class VoxelMap : MonoBehaviour
{
    public float size = 2f;

    public int resolution = 8;

    public int chunkResolution = 2;
    public VoxelGrid GridPartition;
    private float halfsize,chunksize,voxelsize;

    private VoxelGrid[] chunks;

    public TerrainModifierStack ModifierStack;

 

    private void Awake()
    {
        
        init();

    }


    public void init()
    {
        halfsize = size * 0.5f;
        chunksize = size/chunkResolution;
        voxelsize = chunksize / resolution;    
        
        chunks = new VoxelGrid[chunkResolution * chunkResolution];
        
 
        
        for(int i = 0, y =0; y<chunkResolution; y++)
        {
            for (int x = 0; x<chunkResolution;x++, i++)
            {
                CreateChunk(i,x,y);
            }
        }
    }
    private void CreateChunk(int i,int x,int y)
    {
        VoxelGrid chunk = Instantiate(GridPartition) as VoxelGrid;
        chunk.GenerationOffsetPosition = new Vector2((x * resolution) ,(y * resolution) );
        chunk.tm_stack = ModifierStack;
        chunk.transform.parent = transform;
        chunk.initalize(resolution,chunksize);
        chunk.transform.localPosition = new Vector3(x*chunksize - halfsize,y*chunksize -halfsize);
        chunks[i] = chunk;
        if( x >0)
        {
            chunks[i -1].xNeighbor = chunk;
            //print("thing");
        }
        if(y > 0)
        {
            chunks[i - chunkResolution].yNeighbor = chunk;
            //print("thing");
                if(x > 0)
                {
                    chunks[i - chunkResolution -1].xyNeighbor = chunk;
                }
        }
    }
 
}





[CustomEditor(typeof(VoxelMap))]

//swap out for voxel map when ready
public class VoxelGenUI : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        VoxelMap mesh_Gen = (VoxelMap)target;

        if(GUILayout.Button("Generate")){
            mesh_Gen.init();

        }
    }
}