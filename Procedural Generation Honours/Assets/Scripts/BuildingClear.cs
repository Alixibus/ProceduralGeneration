using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingClear : MonoBehaviour {

    public bool foundBuilding = false;
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Building")
        {
            Destroy(gameObject);
            print("Destroyed as building already here");
            foundBuilding = true;
        }
    }
}
