using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffScreenIndicatorScript : MonoBehaviour {
    public Transform endPoint;
    private void Update()
    {

        transform.LookAt(endPoint, Vector3.right);
    }
}
