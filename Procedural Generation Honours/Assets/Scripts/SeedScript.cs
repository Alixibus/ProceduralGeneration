using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;

public class SeedScript : MonoBehaviour {

    //Declaration of variables public
    public Sprite[] vehicleOutlines;
    public Sprite chosenVehicleOutline;
    public Button vehicleSelectorButton;
    public Text vehicleDescription;
    public string chosenVehicleDescription;
    public GameObject[] theGetawayVehicles;
    public int vehicleCount = 1;

    //Getter and Setter for GetawayVehicle, used for vehicle selection
    public GameObject GetawayVehicle
    {
        get { return getawayVehicleChosen; }
        set { getawayVehicleChosen = value; }
    }
    public GameObject getawayVehicleChosen;

    //Getter and Setter for seed number used for procedural generation
    public int SeedNumber
    {
        get { return seedNumberChosen; }
        set { seedNumberChosen = value; }
    }
    public int seedNumberChosen;

    //Getter and Setter for boolean definition of seeded play
    public bool SeededPlay
    {
        get { return seededPlayChosen; }
        set { seededPlayChosen = value; }
    }    public bool seededPlayChosen;
    
    //Awake called to declare initial variables
    public void Awake()
    {
        //ensuring the object is available accross the scenes
        DontDestroyOnLoad(gameObject);

        //initial setting of the getaway vehicle for play
        GetawayVehicle = theGetawayVehicles[0];
        vehicleSelectorButton.image.sprite = vehicleOutlines[0];
        vehicleDescription.text = "-1967 Chevrolet Corvette Stingray";
    }

    //Called to set the seed of the main game manager
    public void SetSeedNumber(Text seedNo)
    {
        SeedNumber = int.Parse(seedNo.text);
        SeededPlay = true;
        SceneManager.LoadScene("GameScene");
    }

    //Used for selection of the vehicle on the main screen
    public void VehicleSelection()
    {
        vehicleCount++;
        //Here ensures the vehicle count reset after the third car has been looped through
        if (vehicleCount == 4)
        {
            vehicleCount = 1;
        }
        if (vehicleCount == 1)
        {
            GetawayVehicle = theGetawayVehicles[0];
            chosenVehicleOutline = vehicleOutlines[0];
            chosenVehicleDescription = "-1967 Chevrolet Corvette Stingray";
        }
        if (vehicleCount == 2)
        {
            GetawayVehicle = theGetawayVehicles[1];
            chosenVehicleOutline = vehicleOutlines[1];
            chosenVehicleDescription = "-1967 Shelby Mustang GT500";
        }
        if (vehicleCount == 3)
        {
            GetawayVehicle = theGetawayVehicles[2];
            chosenVehicleOutline = vehicleOutlines[2];
            chosenVehicleDescription = "-1970 Dodge Charger";
        }
        vehicleSelectorButton.image.sprite = chosenVehicleOutline;
        vehicleDescription.text = chosenVehicleDescription;
    }
}
