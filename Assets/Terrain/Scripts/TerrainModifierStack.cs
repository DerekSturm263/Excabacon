using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[ExecuteAlways]
public class TerrainModifierStack : MonoBehaviour
{
    [HideInInspector]
    public List<TerrainModifier> modifiers;
    
    public float CalculateAll(Vector2 position)
    {
        float fullyCalculated = 0;
        float AccumulatedOpacity =0;

        for(int i = 0; i < modifiers.Count; i ++)
        {
            fullyCalculated = fullyCalculated + (modifiers[i].calculate(position) * modifiers[i].opacity); 
            AccumulatedOpacity += modifiers[i].opacity;
            
        }
        //needs to have intensity calculated using combined opacities and modifier count factored in
        fullyCalculated/= AccumulatedOpacity;
        return fullyCalculated;
    }
    


[CustomEditor(typeof(TerrainModifierStack))]
public class ModifierStackEditor : Editor
{
    bool show = true;
    
    
    public override void OnInspectorGUI()
    {
        
        Rect position = new Rect(0,0,30,50);
        DrawDefaultInspector();

        TerrainModifierStack Modscript = (TerrainModifierStack)target;
        
        show = EditorGUILayout.BeginFoldoutHeaderGroup(show,"Modifiers");
            // dropdown for modifiers ui
            if(show)
            {
                
                        bool candelete = false;
                for(int i = 0; i < Modscript.modifiers.Count; ++i)
                {
                    

                        EditorGUILayout.BeginHorizontal();
                            
                        Modscript.modifiers[i].ShowSettingsDropdown = EditorGUILayout.Foldout(Modscript.modifiers[i].ShowSettingsDropdown,"Modifier " + ( i + 1));   
                        EditorGUILayout.Space();
                        EditorGUILayout.Separator();
                        Modscript.modifiers[i].NodeName = EditorGUILayout.TextField(Modscript.modifiers[i].NodeName);
                        /*
                        EditorGUILayout.Space();
                        */
                        
                        if(GUILayout.Button("Up"))
                            {
                                int index_checked = Mathf.Clamp(i,0,Modscript.modifiers.Count);
                                
                                TerrainModifier targetmodifier = Modscript.modifiers[i -1];
                                TerrainModifier swaptargetmodifier =Modscript.modifiers[i];
                                Modscript.modifiers[i -1] = swaptargetmodifier;
                                Modscript.modifiers[i] = targetmodifier;

                            }
                            if(GUILayout.Button("Down"))
                            {
                                int index_checked = Mathf.Clamp(i,0,Modscript.modifiers.Count);
                                
                                TerrainModifier targetmodifier = Modscript.modifiers[i + 1];
                                TerrainModifier swaptargetmodifier =Modscript.modifiers[i];
                                Modscript.modifiers[i + 1] = swaptargetmodifier;
                                Modscript.modifiers[i] = targetmodifier;
                            }
                            if(GUILayout.Button("Remove"))
                            {
                                candelete = true;
                            }
                            
                        EditorGUILayout.EndHorizontal();
                           
                            

                        
                        if(Modscript.modifiers[i].ShowSettingsDropdown)
                        {
                            
                           
                            EditorGUILayout.Space();
                            EditorGUILayout.Space();
                            EditorGUILayout.Space();
                            EditorGUILayout.Space();
                            
                            EditorGUILayout.Separator();
                            
                            
                            EditorGUILayout.BeginHorizontal();
                            
                            EditorGUILayout.LabelField("Type");
                            Modscript.modifiers[i].modifier = (modtype)EditorGUILayout.EnumPopup(Modscript.modifiers[i].modifier);  
                            EditorGUILayout.EndHorizontal();
                            
                            
                            
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField("Opacity");
                            Modscript.modifiers[i].opacity = EditorGUILayout.Slider(Modscript.modifiers[i].opacity,0,1);
                            
                            
                            EditorGUILayout.EndHorizontal();
                            
                            Modscript.modifiers[i].NodeDisplayUpdate();
                            
                            EditorGUILayout.Separator();
                            
                            
                            
                        }

                          if (candelete){
                            Modscript.modifiers.Remove(Modscript.modifiers[i]);
                            candelete = false;
                          }

                    EditorGUILayout.EndFoldoutHeaderGroup();
                    EditorGUILayout.Separator();  

                   
                }
                    EditorGUILayout.Separator();    
                 

            }
        EditorGUILayout.EndFoldoutHeaderGroup();
        EditorGUILayout.Separator();
        EditorGUILayout.Separator();
        EditorGUILayout.Separator();
        if(GUILayout.Button("Add Modifier"))
        {
            Modscript.modifiers.Add(new TerrainModifier());
        }

    
    }

     private void OnInspectorUpdate()
    {
        Repaint();    
    }
    
}
}
