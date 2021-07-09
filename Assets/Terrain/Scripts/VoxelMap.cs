using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        halfsize = size * 0.5f;
        chunksize = size/chunkResolution;
        voxelsize = chunksize / resolution;    

        chunks = new VoxelGrid[chunkResolution * chunkResolution];
        for(int i = 0, y =0; y<chunkResolution; y++)
        {
            for (int x = 0; x<chunkResolution;x++, i++){
                CreateChunk(i,x,y);
            }
        }


    }
    private void CreateChunk(int i,int x,int y)
    {
        VoxelGrid chunk = Instantiate(GridPartition) as VoxelGrid;
        chunk.initalize(resolution,chunksize);
        chunk.transform.parent = transform;
        chunk.transform.localPosition = new Vector3(x*chunksize - halfsize,y*chunksize -halfsize);
        chunk.tm_stack = ModifierStack;
        chunks[i] = chunk;
    }
}
