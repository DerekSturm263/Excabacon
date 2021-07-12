using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelStencil 
{
    protected bool filltype = false;

    protected int centerX,centerY,radius;
    
    public int Xstart{
        get
        {
            return centerX-radius;
        }
    }
    public int Ystart{
        get
        {
            return centerY - radius;
        }
    }
    public int Xend{
        get
        {
            return centerX + radius;
        }
    }
    public int Yend{
        get
        {
            return centerY +radius;
        }
    }
    
    public virtual void Initialize(bool fillType,int radius)
    {
        this.filltype = fillType;
        this.radius = radius;
    }
    public virtual void setCenter(int x, int y)
    {
        centerX = x;
        centerY = y;
    }
    
    public virtual bool Apply(int x, int y,bool voxel)
    {
        return filltype;
    }
    
}

public class VoxelStencilCircle:VoxelStencil
{
    private int sqrRadius;

    public override void Initialize(bool fillType, int radius)
    {
        base.Initialize(fillType, radius);
        sqrRadius = radius * radius;
    }

    public override bool Apply(int x, int y, bool voxel)
    {
        x -= centerX;
        y -= centerY;
        if( x* x + y *y <=sqrRadius){
            return filltype;
        }
        return voxel;
    }
}
