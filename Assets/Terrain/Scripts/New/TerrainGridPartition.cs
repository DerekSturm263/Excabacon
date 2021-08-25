using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGridPartition : MonoBehaviour
{
    public int HorizontalSize = 8;
    public int VerticalSize =8;
    TerrainPoint[] Points = new TerrainPoint[0];
    
    private void Start() 
    {
        SetupPoints();
    }
    // sets up points positions
    
    
    //basic setting up of points could be replaced later
    void SetupPoints()
    {
         
                Points = new TerrainPoint[HorizontalSize *VerticalSize];
                
                for(int x = 0, i =0;x < HorizontalSize;x++)
                {
                    for(int y = 0; y <VerticalSize;y++,i++)
                    {
                        float randomNum = RandomNoise.randomNoise();
                        bool no = true;
                        
                        //basic test randomisation function, to be replaced later
                        if(randomNum <= 0.7f){
                            no = false;
                            Points[i] = new TerrainPoint(no,new Vector2(x,y));

                        }else{
                            no = true;
                            Points[i] = new TerrainPoint(no,new Vector2(x,y));
                        }
                        //end of random function that is to be replaced later :)
                    }
                }
    }

    private void OnDrawGizmos() 
    {
        // Horizontal and verticle may be replaced later once the scaling and resolution functionality is figured out
        
            if(Points.Length > 1)
                {
                    for(int i = 0,x = 0;x < HorizontalSize;x++)
                        {
                        for(int y = 0; y <VerticalSize;y++,i++)
                            {
                                Vector3 Position = new Vector3(x,-y,0);
                                
                                Gizmos.DrawCube(Position,new Vector3(0.2f,0.2f,0.2f));
                                if(Points[i].On == true)
                                {
                                    Gizmos.color = Color.white;

                                }else
                                { 
                                    Gizmos.color = Color.black;
                                }
                    }
                }
            }
            

        
    }
}
