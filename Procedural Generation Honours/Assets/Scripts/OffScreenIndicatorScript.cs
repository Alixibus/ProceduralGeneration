using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffScreenIndicatorScript : MonoBehaviour {
    Transform endPoint = null;
    Transform getawayVehicle = null;

    private void Update()
    {
        if(getawayVehicle == null)
        {
            if(GameObject.FindWithTag("GetawayVehicle") != null)
            {
                getawayVehicle = GameObject.FindWithTag("GetawayVehicle").transform;
            }
        }
        if (endPoint == null)
        {
            if (GameObject.FindWithTag("EndPoint") != null)
            {
                endPoint = GameObject.FindWithTag("EndPoint").transform;
            }
        }

        if (getawayVehicle != null && endPoint != null)
        {
            Debug.DrawLine(getawayVehicle.position, endPoint.position, Color.red, 0.1f, false);
            Vector3 dir = getawayVehicle.InverseTransformPoint(endPoint.position);
            float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;

            //angle += 180;

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
