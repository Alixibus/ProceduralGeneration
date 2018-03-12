using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_Road_Builder : MonoBehaviour
{    
    
    public bool exitNorth
    {
        get { return exitPointNorth;  }
        set { exitPointNorth = value; }
    }
    public bool exitPointNorth;
    
    public bool exitSouth
    {
        get { return exitPointSouth; }
        set { exitPointSouth = value; }
    }
    public bool exitPointSouth;
    
    public bool exitEast
    {
        get { return exitPointEast; }
        set { exitPointEast = value; }
    }
    public bool exitPointEast;
    
    public bool exitWest
    {
        get { return exitPointWest; }
        set { exitPointWest = value; }
    }
    public bool exitPointWest;
    
    public bool haveRoad
    {
        get { return thisHasRoad; }
        set { thisHasRoad = value; }
    }
    public bool thisHasRoad;
}
