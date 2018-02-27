using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Road_Type_Script : MonoBehaviour {

    [SerializeField]
    bool haveRoad = false;

    [SerializeField]
    string roadName;

    private void Start()
    {
        foreach (Transform child in gameObject.transform)
        {
            if(child.tag == "Road Piece")
            {
                haveRoad = true;
                roadName = child.name;
            }
        }
    }
}
