using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TileTerrainTestScript))]
public class TileTerrainTestEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TileTerrainTestScript terrainScript = (TileTerrainTestScript)target;

        if(GUILayout.Button("Generate")){
            terrainScript.BuildTerrain();
        }
    }
}
