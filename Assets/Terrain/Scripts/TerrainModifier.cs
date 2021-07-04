using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
//[CustomEditor(typeof(TerrainModifier))]
[ExecuteAlways]
public enum modtype
    {
        None,Perlin_Noise,Random_Noise
    }



[System.Serializable]
public class TerrainModifier
{
    //class instance/refferences
    public bool ShowSettingsDropdown = true;        
    public modtype modifier;
    
    //purely visual parameters
    public string NodeName = "New Modifier";

    
    //global parameters
    public float opacity = 1;
    public Vector2 Position; 
    //perlin noise paramters
    public float perlinscale_Mod = 50; 

    public float perlin_Seed = 256;
    public Vector2 perlin_Scale = new Vector2(256,256);
     
    
    public void NodeDisplayUpdate()
    {
        switch(modifier)
        {
            case modtype.None:

            break;

            case modtype.Perlin_Noise:
                PerlinNoiseModifier();
            break;

            case modtype.Random_Noise:
                RandomNoiseModifier();
            break;
        }
        
    }
    
    //calculation 
    
    
    public float calculate(Vector2 position)
    {
         float calc = 0;
         switch(modifier)
        {
            case modtype.None:
                calc = 0;
            break;

            case modtype.Perlin_Noise:
                calc = PerlinNoise.Perlin_Noise(position,perlin_Scale,perlinscale_Mod,perlin_Seed);
            break;

            case modtype.Random_Noise:
                calc =RandomNoise.randomNoise();
            break;
        }
        
        return calc;
    }
    
    public void NullModifier()
    {

    }
     
     
    public void PerlinNoiseModifier()
    {

        DisplayFloatProperty(ref perlin_Seed,"Perlin Seed");
        DisplayVector2Property(ref perlin_Scale,"Perlin Noise Scale");
        
        DisplayFloatProperty(ref perlinscale_Mod,"Perlin Noise Scale Modifier");
    }

    public void RandomNoiseModifier()
    {

    }
     
   
       public float randomNoise(){
        float randomnoise = Random.value;
        return randomnoise;
    }
     
     
     public void DisplayStringProperty(ref string property,string name)
    {
       EditorGUILayout.BeginHorizontal();
       EditorGUILayout.LabelField(name);
       
       property = EditorGUILayout.TextField(property); 

       EditorGUILayout.EndHorizontal();
    }

    public void DisplayFloatProperty(ref float property,string name)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(name);
        
        property = EditorGUILayout.FloatField(property);

        EditorGUILayout.EndHorizontal();
    }

    public void DisplayVector2Property( ref Vector2 property,string name)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(name);
        
        property = EditorGUILayout.Vector2Field("",property);

        EditorGUILayout.EndHorizontal();
    }
}
