﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;

public class SeedScript : MonoBehaviour {

    public Sprite[] vehicleOutlines;
    public Button vehicleSelectorButton;
    public Text vehicleDescription;
    public GameObject[] theGetawayVehicles;
    int vehicleCount = 1;

    public GameObject GetawayVehicle
    {
        get { return getawayVehicleChosen; }
        set { getawayVehicleChosen = value; }
    }
    public GameObject getawayVehicleChosen;

    public int SeedNumber
    {
        get { return seedNumberChosen; }
        set { seedNumberChosen = value; }
    }
    public int seedNumberChosen;

    public bool SeededPlay
    {
        get { return seededPlayChosen; }
        set { seededPlayChosen = value; }
    }    public bool seededPlayChosen;

    public void Awake()
    {
        DontDestroyOnLoad(gameObject);
        print(SeedNumber);
        GetawayVehicle = theGetawayVehicles[0];
        vehicleSelectorButton.image.sprite = vehicleOutlines[0];
        vehicleDescription.text = "-1967 Chevrolet Corvette Stingray";
        print("Corvette Selected");
    }

    public void SetSeedNumber(Text seedNo)
    {
        SeedNumber = int.Parse(seedNo.text);
        SeededPlay = true;
        print(SeedNumber);
        SceneManager.LoadScene("GameScene");
    }

    public void VehicleSelection()
    {
        vehicleCount++;
        if (vehicleCount == 4)
        {
            vehicleCount = 1;
        }
        if (vehicleCount == 1)
        {
            GetawayVehicle = theGetawayVehicles[0];
            vehicleSelectorButton.image.sprite = vehicleOutlines[0];
            vehicleDescription.text = "-1967 Chevrolet Corvette Stingray";
            print("Corvette Selected");
        }
        if (vehicleCount == 2)
        {
            GetawayVehicle = theGetawayVehicles[1];
            vehicleSelectorButton.image.sprite = vehicleOutlines[1];
            vehicleDescription.text = "-1967 Shelby Mustang GT500";
            print("Mustang Selected");
        }
        if (vehicleCount == 3)
        {
            GetawayVehicle = theGetawayVehicles[2];
            vehicleSelectorButton.image.sprite = vehicleOutlines[2];
            vehicleDescription.text = "-1970 Dodge Charger";
            print("Charger Selected");
        }   
    }
}
