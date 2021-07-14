using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.InputSystem;

public class VoxelGrid : MonoBehaviour
{
    public TerrainModifierStack tm_stack;
    

    Dictionary<int,List<triangle>> trianlgleDictionary = new Dictionary<int,List<triangle>>();
    
    List<List<int>> Outlines= new List<List<int>>();
    HashSet<int> checkedVertices = new HashSet<int>();
    public int resolution;
    
    private voxel[] voxels;
    private Mesh mesh;

    public float voxelSize,gridsize;

    private List<Vector3> vertices;

    private List<int> triangles; 

    public Vector2 GenerationOffsetPosition;

    public VoxelGrid xNeighbor,yNeighbor,xyNeighbor;

    private voxel dummyX,dummyY,dummyT;


    private int[] rowCacheMax,rowCacheMin;
    
    private int edgeCacheMin,edgeCacheMax;

    public GameObject spawnPrefabTest;
   /*public void initialize()
   {
       
       refresh();
   }*/

   //private void Awake() {
       //initalize(resolution,1);
  // }

    public List<int> allIndices = new List<int>();
    public List<int> newIndices = new List<int>();
    public List<int> restIndices = new List<int>();

    struct triangle 
    {
        public int VertexIndexA;
        public int VertexIndexB;
        public int VertexIndexC;
        int[] vertices;
        public triangle(int a,int b,int c)
        {
            VertexIndexA = a;
            VertexIndexB = b;
            VertexIndexC = c;

            vertices = new int[3];
            vertices[0] = a;
            vertices[1] = b;
            vertices[2] = c;
        }
        public int this [int i]{
            get{
                return vertices[i];
            }
        }
        public bool contains(int vertexIndex){
            return vertexIndex == VertexIndexA ||vertexIndex == VertexIndexB ||vertexIndex == VertexIndexC;
        }
    }
    public void InitializeFromOther()
    {
        initalize(resolution,1);
    }
    
    public void initalize(int vresolution, float size)
    {
        
        Outlines.Clear();
        checkedVertices.Clear();
        
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
        
       
       
        rowCacheMax = new int[resolution *2 +1];
        rowCacheMin = new int[resolution *2 +1];
       //Refresh();
        StartCoroutine(DelayedRefresh());
        
    }
    void GenerateCollisions()
        {
            EdgeCollider2D[] currentCollider = gameObject.GetComponents<EdgeCollider2D>();
            for(int i = 0; i < currentCollider.Length; i++){
                Destroy(currentCollider[i]);
            }
            CalculateMeshOutlines();
            foreach(List<int> outline in Outlines){
                EdgeCollider2D edgecollider = gameObject.AddComponent<EdgeCollider2D>();
                Vector2[] edgepoints = new Vector2[outline.Count];
                for(int i = 0; i <outline.Count;i++){
                    edgepoints[i] =new Vector2(vertices[outline[i]].x,vertices[outline[i]].y) ;
                }
                edgecollider.points = edgepoints;
            }
        }
    
    // probably see if there is a way of doing this that does not require this treacherous peice of code lol
    public IEnumerator DelayedRefresh()
    {
        yield return new WaitForSeconds(0.01f);
        Refresh();
        yield return new WaitForSeconds(0.1f);
        GenerateCollisions();
        //RandomSplitMeshTest();
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
    void CalculateMeshOutlines()
    {
        for(int vi =0; vi < vertices.Count;vi ++){
            if(!checkedVertices.Contains(vi)){
                int newOutlineVertex = GetConnectedOutlineVertex(vi);
                if (newOutlineVertex != -1){
                    checkedVertices.Add(vi);

                    List<int> NewOutline = new List<int>();
                    NewOutline.Add(vi);
                    Outlines.Add(NewOutline);
                    FollowOutline(newOutlineVertex,Outlines.Count -1);
                    Outlines[Outlines.Count -1].Add(vi);
                }
            }
        }
    }

    void FollowOutline(int vertexIndex,int OutlineIndex){
        Outlines[OutlineIndex].Add(vertexIndex);
        checkedVertices.Add(vertexIndex);
        int nextVertexIndex = GetConnectedOutlineVertex(vertexIndex);

        if(nextVertexIndex != -1){
            FollowOutline(nextVertexIndex,OutlineIndex);
        }
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
        /*
        if(xNeighbor != null)
        {
            dummyX.BecomeXdummyOf(xNeighbor.voxels[0],gridsize);
        }
        */
        FillFIrstRowCache();
        TriangulateCellRows();
        if(yNeighbor != null){
                TriangulateGapRow();
            }
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
    }

    private void FillFIrstRowCache()
    {
        CacheFirstCorner(voxels[0]);
        
        int i;
        for( i = 0; i <resolution -1; i++)
        {
            CacheNextEdgeAndCOrner(i *2,voxels[i],voxels[i +1]);
        }
        if(xNeighbor != null)
        {
            dummyX.BecomeXdummyOf(xNeighbor.voxels[0],gridsize);
            CacheNextEdgeAndCOrner(i * 2,voxels[i],dummyX);
        }
    }

    private void CacheNextEdgeAndCOrner(int i,voxel xmin,voxel xmax)
    {
        if(xmin.state != xmax.state)
        {
            rowCacheMax[i +1] = vertices.Count;
            Vector3 p;
            p.x = xmin.xEdge;
            p.y = xmin.position.y;
            p.z = 0f;
            vertices.Add(p);
        }

        if(xmax.state)
        {
            rowCacheMax[i + 2] = vertices.Count;
            vertices.Add(xmax.position);
            
        }
    }

    private void CacheFirstCorner(voxel voxel)
    {
        if(voxel.state)
        {
            rowCacheMax[0] = vertices.Count;
            vertices.Add(voxel.position);
        }
    }
    private void TriangulateCellRows()
    {
        int cells_x = resolution -1;
        int cells_y = resolution -1;
        for(int i = 0,y = 0; y <cells_y ; y++, i++)
        {
            SwapRowCaches();
            CacheFirstCorner(voxels[i + resolution]);
            CacheNextMiddleEdge(voxels[i],voxels[i + resolution]);
            for(int x = 0; x < cells_x; x++, i++)
            {
                voxel 
                    a = voxels[i],
                    b = voxels[i +1],
                    c = voxels[i + resolution],
                    d = voxels[i + resolution +1];
                
                int cacheindex = x*2;
                
                CacheNextEdgeAndCOrner(cacheindex,c,d);      
                CacheNextMiddleEdge(b,d);
                TriangulateCell(cacheindex, a,b,c,d);
            }
            if(xNeighbor !=null)
            {
                TriangulateGapCell(i);
            }
            
        }

    }

    private void CacheNextMiddleEdge(voxel ymin,voxel ymax)
    {
        edgeCacheMin = edgeCacheMax;
        if(ymin.state != ymax.state)
        {
            edgeCacheMax = vertices.Count;
            Vector3 p;
            p.x = ymin.position.x;
            p.y = ymin.yEdge;
            p.z = 0f;
            vertices.Add(p);
        }
    }
    private void TriangulateGapCell(int i)
    {
        voxel dummyswap = dummyT;
        dummyswap.BecomeXdummyOf(xNeighbor.voxels[i +1],gridsize);
        dummyT = dummyX;
        dummyX = dummyswap;
        

        int cacheindex = (resolution -1) *2;
        CacheNextEdgeAndCOrner(cacheindex,voxels[i + resolution], dummyX);
        CacheNextMiddleEdge(dummyT,dummyX);
        
        
        TriangulateCell(cacheindex,voxels[i],dummyT,voxels[i +resolution],dummyX);
    }

    private void SwapRowCaches()
    {
        int[] rowSwap = rowCacheMin;
        rowCacheMin = rowCacheMax;
        rowCacheMax = rowSwap;
    }

    private void TriangulateGapRow()
    {
        dummyY.BecomeYdummyOf(yNeighbor.voxels[0],gridsize);
        int cells = resolution -1;
        int offset = cells * resolution;

        SwapRowCaches();
        CacheFirstCorner(dummyY);
        CacheNextMiddleEdge(voxels[cells * resolution],dummyY);
    
        for(int x = 0; x <cells; x++)
        {
            voxel dummySwap = dummyT;
            dummySwap.BecomeYdummyOf(yNeighbor.voxels[x+1],gridsize);
            dummyT = dummyY;
            dummyY = dummySwap;
            
            int cacheIndex = x*2;
            
            CacheNextEdgeAndCOrner( cacheIndex,dummyT,dummyY);
            
            CacheNextMiddleEdge(voxels[x + offset +1],dummyY);
            
            TriangulateCell(cacheIndex,voxels[x + offset],voxels[x + offset +1],dummyT,dummyY);
        }
        
        if(xNeighbor !=null){
            dummyT.BecomeXYDummyOF(xyNeighbor.voxels[0],gridsize);
            
            int cacheIndex = cells * 2;
            
            CacheNextEdgeAndCOrner(cacheIndex,dummyY,dummyT);
           
            CacheNextMiddleEdge(dummyX,dummyT);
            
            TriangulateCell(cacheIndex,voxels[voxels.Length - 1],dummyX ,dummyY ,dummyT);
        }
    }
    private void TriangulateCell(int i ,voxel a, voxel b, voxel c, voxel d)
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
                AddTriangle(rowCacheMin[i], edgeCacheMin, rowCacheMin[i + 1]);
                break;
            case 2:
                AddTriangle(rowCacheMin[i + 2], rowCacheMin[i + 1], edgeCacheMax);
                break;
            case 3:
                AddQuad(rowCacheMin[i], edgeCacheMin, edgeCacheMax, rowCacheMin[i + 2]);
                break;
            case 4:
                AddTriangle(rowCacheMax[i], rowCacheMax[i + 1], edgeCacheMin);
                break;
            case 5:
                AddQuad(rowCacheMin[i], rowCacheMax[i], rowCacheMax[i + 1], rowCacheMin[i + 1]);
                break;
            case 6:
                AddTriangle(rowCacheMin[i + 2], rowCacheMin[i + 1], edgeCacheMax);
                AddTriangle(rowCacheMax[i], rowCacheMax[i + 1], edgeCacheMin);
                break; 
            case 7:
                AddPentagon(rowCacheMin[i], rowCacheMax[i], rowCacheMax[i + 1], edgeCacheMax, rowCacheMin[i + 2]);
                break;
            case 8:
                AddTriangle(rowCacheMax[i + 2], edgeCacheMax, rowCacheMax[i + 1]);
                break;
            case 9:
                AddTriangle(rowCacheMin[i], edgeCacheMin, rowCacheMin[i + 1]);
                AddTriangle(rowCacheMax[i + 2], edgeCacheMax, rowCacheMax[i + 1]);
                break;

            case 10:
                AddQuad(rowCacheMin[i + 1], rowCacheMax[i + 1], rowCacheMax[i + 2], rowCacheMin[i + 2]);
                break;
            case 11:
                AddPentagon(rowCacheMin[i + 2], rowCacheMin[i], edgeCacheMin, rowCacheMax[i + 1], rowCacheMax[i + 2]);
                break;
            case 12:
                AddQuad(edgeCacheMin, rowCacheMax[i], rowCacheMax[i + 2], edgeCacheMax);
                break;
            case 13:
                AddPentagon(rowCacheMax[i], rowCacheMax[i + 2], edgeCacheMax, rowCacheMin[i + 1], rowCacheMin[i]);
                break;
            case 14:
                AddPentagon(rowCacheMax[i + 2],rowCacheMin[i + 2], rowCacheMin[i + 1], edgeCacheMin, rowCacheMax[i]);
                break;
            case 15:
                AddQuad(rowCacheMin[i], rowCacheMax[i], rowCacheMax[i + 2], rowCacheMin[i + 2]);
                checkedVertices.Add(rowCacheMin[i]);
                checkedVertices.Add(rowCacheMax[i]);
                checkedVertices.Add(rowCacheMax[i + 2]);
                checkedVertices.Add(rowCacheMin[i + 2]);
                break;

        }
    }


    
    private void AddTriangle(int a, int b, int c)
    {
        
        triangles.Add(a);
        triangles.Add(b);
        triangles.Add(c);

        triangle tri = new triangle (a,b,c);
        AddTriangleDictionary(tri.VertexIndexA,tri);
        AddTriangleDictionary(tri.VertexIndexB,tri);
        AddTriangleDictionary(tri.VertexIndexC,tri);
    }
    int GetConnectedOutlineVertex(int VertexIndex)
    {
        List<triangle> TrisContainingVertex = trianlgleDictionary[VertexIndex];
        for(int i = 0; i < TrisContainingVertex.Count; i++){
            triangle Tri = TrisContainingVertex[i];
            
            for(int j = 0; j <3;j++)
            {
                int vertexB = Tri[j];
                if(vertexB != VertexIndex && !checkedVertices.Contains(vertexB))
                {
                    if(IsOutlineEdge(VertexIndex,vertexB)){
                        return vertexB;
                    }

                }
            }
        }
            return -1;
    }
    
    bool IsOutlineEdge(int vertexA, int VertexB)
    {
        List<triangle> trianglesContainingVertA = trianlgleDictionary[vertexA];
        int sharedTriCount = 0;
        for(int i = 0; i <trianglesContainingVertA.Count; i++)
        {
            if(trianglesContainingVertA[i].contains(VertexB)){
                sharedTriCount ++;
                if(sharedTriCount >1){
                    break;
                }
            }
        }
        return sharedTriCount == 1;
    }

    void AddTriangleDictionary(int VertexIndexKey,triangle tri)
    {
        if(trianlgleDictionary.ContainsKey(VertexIndexKey)){
            trianlgleDictionary[VertexIndexKey].Add(tri);
        }else{
            List<triangle> tirangleList = new List<triangle>();
            tirangleList.Add(tri);
            trianlgleDictionary.Add(VertexIndexKey,tirangleList);
        }
    }
    private void AddQuad(int a,int b,int c,int d)
    {


        triangles.Add(a);
        triangles.Add(b);
        triangles.Add(c);
        triangle tri = new triangle (a,b,c);
        AddTriangleDictionary(tri.VertexIndexA,tri);
        AddTriangleDictionary(tri.VertexIndexB,tri);
        AddTriangleDictionary(tri.VertexIndexC,tri);
       
        triangles.Add(a);
        triangles.Add(c);
        triangles.Add(d);
        triangle tri2 = new triangle (a,c,d);
        AddTriangleDictionary(tri.VertexIndexA,tri2);
        AddTriangleDictionary(tri.VertexIndexB,tri2);
        AddTriangleDictionary(tri.VertexIndexC,tri2);
    }

    private void AddPentagon(int a,int b, int c, int d, int e)
    {

        
        triangles.Add(a);
        triangles.Add(b);
        triangles.Add(c);
        
        triangle tri = new triangle (a,b,c);
        AddTriangleDictionary(tri.VertexIndexA,tri);
        AddTriangleDictionary(tri.VertexIndexB,tri);
        AddTriangleDictionary(tri.VertexIndexC,tri);

        triangles.Add(a);
        triangles.Add(c);
        triangles.Add(d);
        
        triangle tri2 = new triangle (a,c,d);
        AddTriangleDictionary(tri.VertexIndexA,tri2);
        AddTriangleDictionary(tri.VertexIndexB,tri2);
        AddTriangleDictionary(tri.VertexIndexC,tri2);
        
        triangles.Add(a);
        triangles.Add(d);
        triangles.Add(e);

        triangle tri3 = new triangle (a,d,e);
        AddTriangleDictionary(tri.VertexIndexA,tri3);
        AddTriangleDictionary(tri.VertexIndexB,tri3);
        AddTriangleDictionary(tri.VertexIndexC,tri3);
    }
    
    // random code i found on a unity forum that can get split based off of separate peices, potentially will be used to check where certain colliders should be
    
    private void RandomSplitMeshTest()
    {
         MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        Mesh mesh = GetComponent<MeshFilter>().mesh;
 
        int[] indices = mesh.triangles;
        Vector3[] verts = mesh.vertices;
 
        //list all indices
        for (int i = 0; i < indices.Length; i++){
            allIndices.Add(indices[i]);
            restIndices.Add(indices[i]);
        }
 
        while (restIndices.Count > 0){
            newIndices.Clear();
            //Get first triangle
            for (int i = 0; i < 3; i++){
                newIndices.Add(restIndices[i]);
            }
            for (int i = 1; i < restIndices.Count/3; i++){
                if( newIndices.Contains(restIndices[(i*3)+0]) || newIndices.Contains(restIndices[(i*3)+1]) || newIndices.Contains(restIndices[(i*3)+2]) ){
                    for (int q = 0; q < 3; q++){
                        newIndices.Add(restIndices[(i*3)+q]);
                    }
                }
            }
            restIndices.Clear();
            for (int n = 0; n < allIndices.Count; n++){
                if (!newIndices.Contains(allIndices[n]) ){
                    restIndices.Add(allIndices[n]);
                }
            }
            allIndices.Clear();
            for (int i = 0; i < restIndices.Count; i++){
                allIndices.Add(restIndices[i]);
            }
 
            print("island?");
            //mesh.triangles = restIndices.ToArray();
            /*
            Mesh newMesh = new Mesh();
            newMesh.vertices = verts;
            newMesh.triangles = newIndices.ToArray();
 
            newMesh.RecalculateNormals();

            GameObject newGameObject = new GameObject("newGameObject");
            newGameObject.AddComponent<MeshRenderer>().material = meshRenderer.material;
            newGameObject.AddComponent<MeshFilter>().mesh = newMesh;
            */
        }
        //Destroy(this.gameObject);
    }


[System.Serializable]
public class voxel
{
    public bool state = true;
    public Vector2 position,xEdgePosition,yEdgePosition;
    
    public float xEdge,yEdge;
    
    
    public voxel(int x, int y, float size)
    {
        position.x = (x + 0.5f) * size;
        position.y = (y + 0.5f) * size;

        xEdge = position.x + size * 0.5f;
        yEdge = position.y +size * 0.5f;

    }

    public voxel (){}
    public void BecomeXdummyOf(voxel voxel, float offset)
    {
        state = voxel.state;
        position = voxel.position;

        position.x +=offset;
        xEdge = voxel.xEdge + offset;
        yEdge = voxel.yEdge;
    }

        public void BecomeYdummyOf(voxel voxel, float offset)
    {
        state = voxel.state;
        position = voxel.position;

        position.y +=offset;
        xEdge = voxel.xEdge;
        yEdge = voxel.yEdge +offset;
    }

    public void BecomeXYDummyOF(voxel voxel,float offset)
    {
        state = voxel.state;
        position = voxel.position;

        position.x += offset;
        position.y += offset;

        xEdge = voxel.xEdge + offset;
        yEdge = voxel.yEdge + offset;
    }
}
}

