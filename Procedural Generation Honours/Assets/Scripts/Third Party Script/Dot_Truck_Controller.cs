using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

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
    Text gearDisplay;
    float motor;
    float topSpeed = 25.0f;
    float reverseTopSpeed = -10.0f;
    float brakeTorque;
    float steering;
    [SerializeField]
    GameObject brakeHolder;
    public float currentSpeed = 0;
    public List<Dot_Truck> truck_Infos;
    bool accelerating, speedTopped, vehicleInMotion;
    string[] gears = new string[] { "drive", "neutral", "reverse" };
    string carState;

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

    void Start()
    {
        carState = gears[1];
        if (gearDisplay == null)
        {
            gearDisplay = GameObject.FindGameObjectWithTag("GearText").GetComponent<Text>();
        }
    }

    public void Update()
    {     

        print(maxSteeringAngle);

        if (currentSpeed > 0.3f && currentSpeed < topSpeed)
        {
            if (Input.GetAxis("Vertical") <= 0 && brakeTorque < 0.001)
            {
                motor += 0.05f;
                currentSpeed -= 0.05f;
                accelerating = false;
            }
        }

        if (currentSpeed < -0.3f && currentSpeed < reverseTopSpeed)
        {
            if (Input.GetAxis("Vertical") <= 0 && brakeTorque < 0.001)
            {
                motor -= 0.05f;
                currentSpeed += 0.05f;
                accelerating = false;
            }
        }

        if (currentSpeed > 20.01f && currentSpeed < 25.01f)
        {
            maxSteeringAngle = 10.0f;
        }
        else if (currentSpeed > 10.0f && currentSpeed < 20.0f)
        {
            maxSteeringAngle = 20.0f;
        }
        else
        {
            maxSteeringAngle = 30.0f;
        }

        steering = maxSteeringAngle * Input.GetAxis("Horizontal");
        brakeTorque = Mathf.Abs(Input.GetAxis("Jump"));

        if (currentSpeed > 0.2 && currentSpeed < -0.2 && Input.GetAxis("Vertical") <= 0)
        {
            currentSpeed = 0.0f;
            motor = 0;
        }

        if (Input.GetKey(KeyCode.KeypadPlus))
        {
            carState = gears[0];
        }
        if (Input.GetKey(KeyCode.KeypadMinus))
        {
            carState = gears[2];
        }
        if (Input.GetKey(KeyCode.Keypad0))
        {
            carState = gears[1];
        }
        if (carState == "drive")
        {
            if (gearDisplay.text != "D")
            {
                gearDisplay.text = "D";
            }

            Accelerating();
        }

        if(carState == "neutral")
        {
            if (gearDisplay.text != "N")
            {
                gearDisplay.text = "N";
            }
        }
        if(carState == "reverse")
        {
            if (gearDisplay.text != "R")
            {
                gearDisplay.text = "R";
            }
            Reversing();
        }

        if (brakeTorque > 0.001)
        {
            if (brakeHolder.activeSelf != true)
            {
                brakeHolder.SetActive(true);
            }
        }
        else
        {
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

    void Accelerating()
    {
        if (currentSpeed < topSpeed)
        {
            if (Input.GetAxis("Vertical") > 0 && carState == "drive")
            {
                motor += 0.1f * -Input.GetAxis("Vertical");
                currentSpeed += 0.1f * Input.GetAxis("Vertical");
                accelerating = true;
            }
        }
        if (currentSpeed > 25.0f && currentSpeed < 26.0f)
        {
            motor += 0.2f;
            currentSpeed -= 0.2f;
        }

        if (brakeTorque > 0.001)
        {
            if (currentSpeed > 0)
            {
                motor -= 0.3f * -Input.GetAxis("Jump");
                currentSpeed -= 0.3f * Input.GetAxis("Jump");
            }
            else
            {
                currentSpeed = 0;
            }
        }
    }

    void Reversing()
    {
        if (currentSpeed > reverseTopSpeed)
        {
            if (Input.GetAxis("Vertical") > 0 && carState == "reverse")
            {
                motor += 0.1f * Input.GetAxis("Vertical");
                currentSpeed -= 0.1f * Input.GetAxis("Vertical");
                accelerating = true;
            }
            if (Input.GetAxis("Vertical") >= 0 && currentSpeed < 0 && brakeTorque < 0.001)
            {
                motor -= 0.05f;
                currentSpeed += 0.05f;
                accelerating = false;
            }
        }
        if (currentSpeed > -11.0f && currentSpeed < -10.0f)
        {
            motor -= 0.2f;
            currentSpeed += 0.2f;
        }

        if (brakeTorque > 0.001)
        {
            if (currentSpeed < 0)
            {
                motor -= 0.2f * Input.GetAxis("Jump");
                currentSpeed += 0.2f * Input.GetAxis("Jump");
            }
            else
            {
                currentSpeed = 0;
            }
        }
    }
}