using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Road_Type_Script : MonoBehaviour
{
    //Script designed to hold the information regarding implimentation 
    //of the road generation system, a list of bools used for checking
    public bool exitNorth
    {
        get { return exitPointNorth; }
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
