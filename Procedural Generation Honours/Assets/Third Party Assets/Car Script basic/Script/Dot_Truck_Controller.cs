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
	public bool motor;    
	public bool steering;
	public bool reverseTurn; 
}

public class Dot_Truck_Controller : MonoBehaviour {

	public float maxMotorTorque;
	public float maxSteeringAngle;
    float motor;
    public float topSpeed = 5;
    public float currentSpeed = 0;
    public List<Dot_Truck> truck_Infos;
    public AudioClip[] vehicleAudio;
    public AudioSource engineAudio;
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
        print(motor);
        if(currentSpeed < topSpeed)
        {
            if (Input.GetAxis("Vertical") > 0)
            {
                motor += 0.02f * -Input.GetAxis("Vertical");
                currentSpeed += 0.02f * Input.GetAxis("Vertical");
                accelerating = true;
            }
            if(Input.GetAxis("Vertical") < 0 && currentSpeed > 0)
            {
                motor -= 0.05f;
                currentSpeed -= 0.05f;
                accelerating = false;
            }
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
        float steering = maxSteeringAngle * Input.GetAxis("Horizontal");
        float brakeTorque = Mathf.Abs(Input.GetAxis("Jump"));
        if (brakeTorque > 0.001)
        {
            if(currentSpeed > 0)
            {
                motor += 0.05f * Input.GetAxis("Jump");
                currentSpeed -= 0.05f * Input.GetAxis("Jump");
            }
            if(engineAudio.isPlaying && vehicleInMotion == true)
            {
                engineAudio.Stop();
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
        if(Input.GetAxis("Vertical") > 0 && accelerating == true)
        {
            if (engineAudio.clip != vehicleAudio[1])
            {
                engineAudio.clip = vehicleAudio[1];
                engineAudio.Play();
            }
        }
        if (Input.GetAxis("Vertical") > 0 && speedTopped == true)
        {
            if (engineAudio.clip != vehicleAudio[2])
            {
                engineAudio.clip = vehicleAudio[2];
                engineAudio.Play();
            }
        }
        if(currentSpeed <= 0 || vehicleInMotion == false)
        {
            if (engineAudio.clip != vehicleAudio[0])
            {
                engineAudio.clip = vehicleAudio[0];
                engineAudio.Play();
            }
        }
        if (!engineAudio.isPlaying)
        {
            engineAudio.clip = vehicleAudio[0];
            engineAudio.Play();
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