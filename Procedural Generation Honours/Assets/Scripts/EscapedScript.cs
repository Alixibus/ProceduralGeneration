using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapedScript : MonoBehaviour {

    private void OnTrigger(Collider other)
    {
        if (other.tag == "GetawayVehicle")
        {
            print("vehicle found");
        }
    }
}
