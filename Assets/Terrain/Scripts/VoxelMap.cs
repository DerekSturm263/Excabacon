using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.InputSystem;
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

    public GameObject TestDestructor;

    public int tempRadius = 3;

    private VoxelStencil[] stencils ={ new VoxelStencil(),new VoxelStencilCircle()};
    [Range(0,1)]
    public int TempStencilINdex = 0;

    private void Awake()
    {
        
        init();

    }

    private void Update() 
    {
        //EditVoxels(new Vector2(TestDestructor.transform.position.x,TestDestructor.transform.position.y));
    }
    
    public void EditVoxels(Vector2 point)
    {
        // can go out of range, in this event there will need to be some code to check if it is so it does not scream in the console window
        


        int CenterX = (int)((point.x + halfsize)/voxelsize);
        int CenterY = (int)((point.y + halfsize)/voxelsize);
        
        int chunkX = CenterX /resolution;
        int chunkY = CenterY /resolution;
        //print(voxelX + "_x" + voxelY +"_Y");
        //CenterX -= chunkX * resolution;
        //CenterY -= chunkY * resolution;
        
        int Xstart = (CenterX - tempRadius -1)/resolution;
        if(Xstart <0)
            Xstart =0;
        
        int Xend = (CenterX + tempRadius)/resolution;
        if(Xend >= chunkResolution)
            Xend = chunkResolution -1;
        
        int Ystart = (CenterY - tempRadius - 1)/resolution;
        if(Ystart <0)
            Ystart =0;
        
        int Yend = (CenterY + tempRadius)/resolution;
        if(Yend >= chunkResolution)
            Yend = chunkResolution -1;
        
        
        
        VoxelStencil activeStencil = stencils[TempStencilINdex];
        // probably do a thing if want to create terrain eventually too, radius should be passed in probably some sort of interface later on down the line
        activeStencil.Initialize(false,tempRadius);
        //activeStencil.setCenter(CenterX,CenterY);
        
        int voxelYoffset = Yend * resolution;
        for(int y = Yend; y >= Ystart; y-- )
        {
            int i = y * chunkResolution + Xend;
            int voxelXoffset = Xend * resolution;
            for(int x = Xend; x >= Xstart; x--,i--)
            {
                activeStencil.setCenter(CenterX - voxelXoffset,CenterY - voxelYoffset);
                chunks[i].Apply(activeStencil);
                voxelXoffset -= resolution;
            }
            voxelYoffset -= resolution;
        }
        
        
        
        
        //chunks[chunkY * chunkResolution + chunkX].Apply(activeStencil);
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