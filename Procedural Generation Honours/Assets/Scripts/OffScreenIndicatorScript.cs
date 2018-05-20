using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffScreenIndicatorScript : MonoBehaviour {

    Transform endPoint = null;
    Transform getawayVehicle = null;

    //Simple script for rotatin an empty game object
    //with an attached UI element that acts as an indicator on minimap
    private void Update()
    {
        //assign the getaway vehicle aslong as its not null
        if(getawayVehicle == null)
        {
            if(GameObject.FindWithTag("GetawayVehicle") != null)
            {
                getawayVehicle = GameObject.FindWithTag("GetawayVehicle").transform;
            }
        }

        //assign the endpoint as long as not null
        if (endPoint == null)
        {
            if (GameObject.FindWithTag("EndPoint") != null)
            {
                endPoint = GameObject.FindWithTag("EndPoint").transform;
            }
        }

        //if both getaway and end point are not null draw a line for visual debugging in editor
        // then work out the direction to rotate the anchor point
        //mathf works out the angle mathmatically then 180 is added due to the rotation of the vehicle in game
        //finally rotate the anchor empty gameobject to desired location
        if (getawayVehicle != null && endPoint != null)
        {
            Debug.DrawLine(getawayVehicle.position, endPoint.position, Color.red, 0.1f, false);
            Vector3 dir = getawayVehicle.InverseTransformPoint(endPoint.position);
            float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;

            angle += 180;

            transform.localEulerAngles = new Vector3(0, 180, angle);
        }
        else
        {
            if(getawayVehicle != null && endPoint == null)
            {
                print("Endpoint is null");
            }
            if(getawayVehicle == null && endPoint != null)
            {
                print("Getaway is null");
            }
            if(getawayVehicle == null && endPoint == null)
            {
                print("both are null");
            }
        }
    }
}
