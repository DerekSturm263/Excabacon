using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[ExecuteAlways]
public class TerrainModifierStack : MonoBehaviour
{
    public List<TerrainModifier> modifiers;
    void Start()
    {
    
    }

    // Update is called once per frame
    void Update()
    {
    }
    
    public float CalculateAll(Vector2 position)
    {
        float fullyCalculated = 0;

        for(int i = 0; i < modifiers.Count; i ++)
        {
            fullyCalculated = fullyCalculated + modifiers[i].calculate(position) * modifiers[i].opacity; 
            
        }
        //needs to have intensity calculated using combined opacities and modifier count factored in

        return fullyCalculated;
    }
    


[CustomEditor(typeof(TerrainModifierStack))]
public class ModifierStackEditor : Editor
{
    bool show = false;
    
    
    public override void OnInspectorGUI()
    {
        
        Rect position = new Rect(0,0,30,50);
        DrawDefaultInspector();

        TerrainModifierStack Modscript = (TerrainModifierStack)target;
        
        show = EditorGUILayout.BeginFoldoutHeaderGroup(show,"Modifiers");
            
            if(show)
            {
                
                int i = 0;
                foreach(TerrainModifier mod in  Modscript.modifiers)
                {
                    
                    i++;
                    
                    mod.ShowSettingsDropdown = EditorGUILayout.Foldout(mod.ShowSettingsDropdown,"Modifier " + ( i));
                        
                        if(mod.ShowSettingsDropdown)
                        {
                            
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField("Type");
                            mod.modifier = (modtype)EditorGUILayout.EnumPopup(mod.modifier);  
                            EditorGUILayout.EndHorizontal();
                            
                            
                            
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField("Opacity");
                            mod.opacity = EditorGUILayout.Slider(mod.opacity,0,1);
                            EditorGUILayout.EndHorizontal();
                            
                            mod.NodeDisplayUpdate();
                            
                            
                        }

                    EditorGUILayout.EndFoldoutHeaderGroup();
                }
                        
                 

            }
        EditorGUILayout.EndFoldoutHeaderGroup();

    
    }

     private void OnInspectorUpdate()
    {
        Repaint();    
    }

}
}
