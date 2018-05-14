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
    public GameObject leftBrake, rightBrake;
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
        float steering = maxSteeringAngle * Input.GetAxis("Horizontal");
        float brakeTorque = Mathf.Abs(Input.GetAxis("Jump"));

        print(currentSpeed);
        if (currentSpeed < topSpeed)
        {
            if (Input.GetAxis("Vertical") > 0 && gearDisplay.text == "D")
            {
                motor += 0.1f * -Input.GetAxis("Vertical");
                currentSpeed += 0.1f * Input.GetAxis("Vertical");
                accelerating = true;
            }
            if (Input.GetAxis("Vertical") <= 0 && currentSpeed > 0 && brakeTorque < 0.001)
            {
                motor -= 0.05f;
                currentSpeed -= 0.05f;
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

        if (Input.GetKey(KeyCode.Keypad1))
        {
            carState = gears[0];
        }
        if (Input.GetKey(KeyCode.Keypad2))
        {
            carState = gears[2];
        }
        if (Input.GetKey(KeyCode.Keypad0))
        {
            carState = gears[1];
        }
        if (carState == "drive")
        {
            if(gearDisplay.text != "D")
            {
                gearDisplay.text = "D";
            }
            if (currentSpeed > 25.0f && currentSpeed < 26.0f)
            {
                motor += 0.2f;
                currentSpeed -= 0.2f;
            }
            if (currentSpeed < 0 && currentSpeed <= 0.2f)
            {
                currentSpeed = 0.0f;
                motor = 0;
            }
            
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
}