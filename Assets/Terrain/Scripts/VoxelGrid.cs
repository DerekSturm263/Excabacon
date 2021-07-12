using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.InputSystem;

public class VoxelGrid : MonoBehaviour
{
    public TerrainModifierStack tm_stack;
    
    
    public int resolution;
    
    private voxel[] voxels;
    private Mesh mesh;

    public float voxelSize,gridsize;

    private List<Vector3> vertices;

    private List<int> triangles; 

    public Vector2 GenerationOffsetPosition;

    public VoxelGrid xNeighbor,yNeighbor,xyNeighbor;

    private voxel dummyX,dummyY,dummyT;

   /*public void initialize()
   {
       
       refresh();
   }*/

   //private void Awake() {
       //initalize(resolution,1);
  // }

  
    public void InitializeFromOther()
    {
        initalize(resolution,1);
    }
    
    public void initalize(int vresolution, float size)
    {
        this.resolution = vresolution;
        gridsize = size;
        voxelSize = size/resolution;
        voxels = new voxel[resolution * resolution];


        dummyX = new voxel();
        dummyY = new voxel();
        dummyT = new voxel();
        
		for (int i = 0, y = 0; y < vresolution; y++) {
			for (int x = 0; x < vresolution; x++, i++) {
				CreateVoxel(i, x, y);
                 if(tm_stack.CalculateAll(new Vector2(x + GenerationOffsetPosition.x,y + GenerationOffsetPosition.y)) > 0.5){
                    voxels[i].state = false;
                 }
                  
			}
		}
        
        
        
        
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "Grid mesh";
        vertices = new List<Vector3>();
        triangles = new List<int>();
        
       
       
       //Refresh();
        StartCoroutine(DelayedRefresh());
        
    }
    
    // probably see if there is a way of doing this that does not require this treacherous peice of code lol
    public IEnumerator DelayedRefresh()
    {
        yield return new WaitForSeconds(0.01f);
        Refresh();
    }
    private void CreateVoxel(int i,int x,int y)
    {
        voxels[i] = new voxel(x,y,voxelSize);
    }
    
    public void Apply(VoxelStencil stencil)
    {
        int Xstart = stencil.Xstart;
        if(Xstart <0)
            Xstart =0;
        int Xend = stencil.Xend;
        if(Xend >= resolution)
            Xend = resolution -1;
        int Ystart = stencil.Ystart;
        if(Ystart<0)
            Ystart =0;
        int Yend = stencil.Yend;
        if(Yend >= resolution)
            Yend = resolution -1;




        for(int y = Ystart; y <= Yend; y++)
        {
            int i = y * resolution +Xstart;
            for(int x = Xstart; x <= Xend; x++,i++)
            {
                voxels[i].state = stencil.Apply(x,y,voxels[i].state);
            }
        }
      
      
      
       // voxels[y *resolution + x ].state = stencil.Apply(stencil);
       
       
       
        Refresh();
    }
    
    public void Refresh()
    {
        Triangulate();
    }
    private void Triangulate()
    {
        vertices.Clear();
        triangles.Clear();
        mesh.Clear();
        
        if(xNeighbor != null)
        {
            dummyX.BecomeXdummyOf(xNeighbor.voxels[0],gridsize);
        }
        TriangulateCellRows();
        if(yNeighbor != null){
                TriangulateGapRow();
            }
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
    }
    private void TriangulateCellRows()
    {
        int cells_x = resolution -1;
        int cells_y = resolution -1;
        for(int i = 0,y = 0; y <cells_y ; y++, i++)
        {
            for(int x = 0; x < cells_x; x++, i++)
            {
                TriangulateCell(voxels[i],voxels[i +1],voxels[i+ resolution],voxels[i + resolution + 1]);
            }
            if(xNeighbor !=null)
            {
                TriangulateGapCell(i);
            }
            
        }

    }
    private void TriangulateGapCell(int i)
    {
        voxel dummyswap = dummyT;
        dummyswap.BecomeXdummyOf(xNeighbor.voxels[i +1],gridsize);
        dummyT = dummyX;
        dummyX = dummyswap;
        TriangulateCell(voxels[i],dummyT,voxels[i +resolution],dummyX);
    }

    private void TriangulateGapRow()
    {
        dummyY.BecomeYdummyOf(yNeighbor.voxels[0],gridsize);
        int cells = resolution -1;
        int offset = cells * resolution;
    
        for(int x = 0; x <cells; x++)
        {
            voxel dummySwap = dummyT;
            dummySwap.BecomeYdummyOf(yNeighbor.voxels[x+1],gridsize);
            dummyT = dummyY;
            dummyY = dummySwap;
            TriangulateCell(voxels[x + offset],voxels[x + offset +1],dummyT,dummyY);
        }
        
        if(xNeighbor !=null){
            dummyT.BecomeXYDummyOF(xyNeighbor.voxels[0],gridsize);
            TriangulateCell(voxels[voxels.Length - 1],dummyX ,dummyY ,dummyT);
        }
    }
    private void TriangulateCell(voxel a, voxel b, voxel c, voxel d)
    {
        int celltype = 0;
        if(a.state){
            celltype |= 1;
        }
        if(b.state){
            celltype |= 2;
        }
        if(c.state){
            celltype |=4;
        }
        if (d.state){
            celltype |=8;
        }

        switch(celltype)
        {
            case 0:
                return;
            case 1:
                AddTriangle(a.position,a.yEdgePosition,a.xEdgePosition);
                break;
            case 2:
                AddTriangle(b.position,a.xEdgePosition,b.yEdgePosition);
                break;
            case 3:
                AddQuad(a.position,a.yEdgePosition,b.yEdgePosition,b.position);
                break;
            case 4:
                AddTriangle(c.position,c.xEdgePosition,a.yEdgePosition);
                break;
            case 5:
                AddQuad(a.position,c.position,c.xEdgePosition,a.xEdgePosition);
                break;
            case 6:
                AddTriangle(b.position,a.xEdgePosition,b.yEdgePosition);
                AddTriangle(c.position,c.xEdgePosition,a.yEdgePosition);
                break; 
            case 7:
                AddPentagon(a.position,c.position,c.xEdgePosition,b.yEdgePosition,b.position);
                break;
            case 8:
                AddTriangle(d.position,b.yEdgePosition,c.xEdgePosition);
                break;
            case 9:
                AddTriangle(a.position,a.yEdgePosition,a.xEdgePosition);
                AddTriangle(d.position,b.yEdgePosition,c.xEdgePosition);
                break;

            case 10:
                AddQuad(a.xEdgePosition,c.xEdgePosition,d.position,b.position);
                break;
            case 11:
                AddPentagon(b.position,a.position,a.yEdgePosition,c.xEdgePosition,d.position);
                break;
            case 12:
                AddQuad(a.yEdgePosition,c.position,d.position,b.yEdgePosition);
                break;
            case 13:
                AddPentagon(c.position,d.position,b.yEdgePosition,a.xEdgePosition,a.position);
                break;
            case 14:
                AddPentagon(d.position,b.position,a.xEdgePosition,a.yEdgePosition,c.position);
                break;
            case 15:
                AddQuad(a.position,c.position,d.position,b.position);
                break;

        }
    }

    private void AddTriangle(Vector3 a, Vector3 b, Vector3 c)
    {
        
        int vertexIndex = vertices.Count;
        vertices.Add(a);
        vertices.Add(b);
        vertices.Add(c);
        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex +1);
        triangles.Add(vertexIndex +2);
    }
    private void AddQuad(Vector3 a,Vector3 b,Vector3 c,Vector3 d)
    {
        int vertexIndex = vertices.Count;
        vertices.Add(a);
        vertices.Add(b);
        vertices.Add(c);
        vertices.Add(d);

        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 2);
        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex +2);
        triangles.Add(vertexIndex + 3);
    }

    private void AddPentagon(Vector3 a,Vector3 b, Vector3 c, Vector3 d, Vector3 e)
    {
        int vertexIndex = vertices.Count;
        vertices.Add(a);
        vertices.Add(b);
        vertices.Add(c);
        vertices.Add(d);
        vertices.Add(e);
        
        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 2 );
        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 2);
        triangles.Add(vertexIndex + 3);
        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 3);
        triangles.Add(vertexIndex + 4);
    }
}


[System.Serializable]
public class voxel
{
    public bool state = true;
    public Vector2 position,xEdgePosition,yEdgePosition;
    
    public voxel (){}
    
    
    public voxel(int x, int y, float size)
    {
        position.x = (x + 0.5f) * size;
        position.y = (y + 0.5f) * size;

		xEdgePosition = position;
		xEdgePosition.x += size * 0.5f;
		yEdgePosition = position;
		yEdgePosition.y += size * 0.5f;
    }

    public void BecomeXdummyOf(voxel voxel, float offset)
    {
        state = voxel.state;
        position = voxel.position;
        xEdgePosition = voxel.xEdgePosition;
        yEdgePosition = voxel.yEdgePosition;
        position.x +=offset;
        xEdgePosition.x += offset;
        yEdgePosition.x +=offset;
    }

        public void BecomeYdummyOf(voxel voxel, float offset)
    {
        state = voxel.state;
        position = voxel.position;
        xEdgePosition = voxel.xEdgePosition;
        yEdgePosition = voxel.yEdgePosition;
        position.y +=offset;
        xEdgePosition.y += offset;
        yEdgePosition.y += offset;
    }

    public void BecomeXYDummyOF(voxel voxel,float offset)
    {
        state = voxel.state;
        position = voxel.position;
        xEdgePosition = voxel.xEdgePosition;
        yEdgePosition = voxel.yEdgePosition;
        position.x += offset;
        position.y += offset;
        xEdgePosition.x +=offset;
        xEdgePosition.y +=offset;
        yEdgePosition.x +=offset;
        yEdgePosition.y +=offset;
    }
}
