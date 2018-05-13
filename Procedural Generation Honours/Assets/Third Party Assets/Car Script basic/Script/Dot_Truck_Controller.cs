using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Dot_Truck : System.Object
{
	public WheelCollider leftWheel;
	public GameObject leftWheelMesh;
	public WheelCollider rightWheel;
	public GameObject rightWheelMesh;
    public GameObject leftBrake, rightBrake;
	public bool motor;    
	public bool steering;
	public bool reverseTurn; 
}

public class Dot_Truck_Controller : MonoBehaviour {

    public float maxMotorTorque;
    public float maxSteeringAngle;
    float motor;
    float topSpeed = 20.0f;
    [SerializeField]
    Material brakeMat;
    [SerializeField]
    Texture brakeOffTexture;
    [SerializeField]
    Texture brakeOnTexture;
    [SerializeField]
    GameObject brakeHolder;
    public float currentSpeed = 0;
    public List<Dot_Truck> truck_Infos;
    bool accelerating, speedTopped, vehicleInMotion;

    public void VisualizeWheel(Dot_Truck wheelPair)
	{
		Quaternion rot;
		Vector3 pos;
		wheelPair.leftWheel.GetWorldPose ( out pos, out rot);
		wheelPair.leftWheelMesh.transform.position = pos;
		wheelPair.leftWheelMesh.transform.rotation = rot;
		wheelPair.rightWheel.GetWorldPose ( out pos, out rot);
		wheelPair.rightWheelMesh.transform.position = pos;
		wheelPair.rightWheelMesh.transform.rotation = rot;
	}

    public void Update()
    {
        print(currentSpeed);
        if(currentSpeed < topSpeed)
        {
            if (Input.GetAxis("Vertical") > 0)
            {
                motor += 0.2f * -Input.GetAxis("Vertical");
                currentSpeed  += 0.2f * Input.GetAxis("Vertical");
                accelerating = true;
            }
            //if(Input.GetAxis("Vertical") == 0 && motor < 0)
            //{
            //    motor += 0.2f;
            //    currentSpeed -= 0.2f;
            //    accelerating = false;
            //}
        }
        if(currentSpeed >= topSpeed)
        {
            accelerating = false;
            speedTopped = true;
        }
        if(currentSpeed > 0)
        {
            vehicleInMotion = true;
        }
        else
        {
            vehicleInMotion = false;
            accelerating = false;
            speedTopped = false;
        }
        if (currentSpeed > 15.0f && currentSpeed < 20.0f)
        {
            maxSteeringAngle = 10.0f;
        }
        if (currentSpeed > 8.0f && currentSpeed < 14.99f)
        {
            maxSteeringAngle = 30.0f;
        }
        else
        {
            maxSteeringAngle = 50.0f;
        }

        float steering = maxSteeringAngle * Input.GetAxis("Horizontal");
        float brakeTorque = Mathf.Abs(Input.GetAxis("Jump"));
        if (brakeTorque > 0.001)
        {
            Braking();
        }
        else
        {
            if (brakeMat.GetTexture("_EmissionMap") != brakeOffTexture)
            {
                brakeMat.SetTexture("_EmissionMap", brakeOffTexture);
            }
            if (brakeHolder.activeSelf != false)
            {
                brakeHolder.SetActive(false);
            }
        }

        foreach (Dot_Truck truck_Info in truck_Infos)
        {
            if (truck_Info.steering == true)
            {
                truck_Info.leftWheel.steerAngle = truck_Info.rightWheel.steerAngle = ((truck_Info.reverseTurn) ? -1 : 1) * steering;
            }

            if (truck_Info.motor == true)
            {
                truck_Info.leftWheel.motorTorque = motor;
                truck_Info.rightWheel.motorTorque = motor;
            }

            truck_Info.leftWheel.brakeTorque = brakeTorque;
            truck_Info.rightWheel.brakeTorque = brakeTorque;

            VisualizeWheel(truck_Info);
        }
    }

    void Braking()
    {
        if (currentSpeed > 0)
        {
            motor -= 0.3f * -Input.GetAxis("Jump");
            currentSpeed -= 0.3f * Input.GetAxis("Jump");
        }
        if (brakeMat.GetTexture("_EmissionMap") != brakeOnTexture)
        {
            brakeMat.SetTexture("_EmissionMap", brakeOnTexture);
        }
        if(brakeHolder.activeSelf != true)
        {
            brakeHolder.SetActive(true);
        }
    }

    void CarState(int currentState)
    {
        switch (currentState)
        {
            //Neutral
            case 1:
                break;
            case 2:
                break;
            default:
                break;
        }
    }
}