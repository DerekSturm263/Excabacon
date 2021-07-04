using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[ExecuteInEditMode]

public class MeshGenerator : MonoBehaviour
{
    
    public TerrainModifierStack tm_stack;
    public Vector2Int DebugTextureSize = new Vector2Int(256,256);
    public Renderer texturerender;
    
    private void OnValidate() {
        CalculateDebugTexture();
    }
    void Start()
    {
        CalculateDebugTexture();
    }

    public void CalculateDebugTexture()
    {
        Texture2D texture = new Texture2D(DebugTextureSize.x,DebugTextureSize.y);
        
        Color[] colorMap = new Color[DebugTextureSize.x * DebugTextureSize.y];
        
        for(int y =0; y <DebugTextureSize.y; y++){
            for(int x =0; x<DebugTextureSize.x;x++){
                colorMap[y * DebugTextureSize.x + x ] = Color.Lerp(Color.black,Color.white,tm_stack.CalculateAll(new Vector2(x,y)));
            }
        }

    texture.SetPixels (colorMap);
    texture.Apply();
    texturerender.sharedMaterial.mainTexture = texture;
    //texturerender.transform.localScale = new Vector3(DebugTextureSize.x,1,DebugTextureSize.y);
    }
}


[CustomEditor(typeof(MeshGenerator))]
public class MeshGeneratorUI : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        MeshGenerator mesh_Gen = (MeshGenerator)target;

        if(GUILayout.Button("Generate")){
            mesh_Gen.CalculateDebugTexture();
        }
    }
}

