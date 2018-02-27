using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_Road_Builder : MonoBehaviour {

    [SerializeField]
    bool haveRoad = false;
    bool northHasRoad, southHasRoad, eastHasRoad, westHasRoad;
    

    [SerializeField]
    string roadName;

    private void Start()
    {
        
        foreach (Transform child in gameObject.transform)
        {
            if (child.tag == "Road Piece")
            {
                haveRoad = true;
                roadName = child.name;
            }
        }        
    }

    void CheckNeighbour()
    {

    }
}
