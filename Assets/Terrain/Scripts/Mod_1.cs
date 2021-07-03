using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[ExecuteInEditMode]
[System.Serializable]
public  class PerlinNoise : ModifierNodeBase
{
    
    public static float Perlin_Noise(Vector2 position, Vector2 PerlinScale,float Scalemod,float Seed){
        float perlin = 0;

        float noise_x = position.x / PerlinScale.x *Scalemod +Seed ;
        float noise_y = position.y /PerlinScale.y *Scalemod +Seed;
        
        
        perlin = Mathf.PerlinNoise(noise_x,noise_y); 
        
        return perlin;
    }

}
public class RandomNoise : ModifierNodeBase
{

    
    public static float randomNoise(){
        float randomnoise = 1;
        randomnoise = Random.value;
        return randomnoise;
    }
}
