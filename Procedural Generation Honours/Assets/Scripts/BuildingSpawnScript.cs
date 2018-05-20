using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingSpawnScript : MonoBehaviour {
    public GameObject[] buildingPrefabTest;
    Collider thisCollider;

    //Script for generating buildings
    private void Start()
    {

        thisCollider = gameObject.GetComponent<Collider>();
        if(thisCollider.enabled == false)
        {
            thisCollider.enabled = true;
        }
        BuildBuilding();
    }
    void BuildBuilding()
    {
        //Randomly decide which building type to generate
        float randomValue = Random.value;        
        if (randomValue > 0 && randomValue < 0.3)
        {
            GameObject temptBuilding = Instantiate(buildingPrefabTest[0], new Vector3(transform.position.x, 0.05f, transform.position.z), Quaternion.identity);
            temptBuilding.transform.SetParent(transform);
            thisCollider.enabled = false;
        }
        if (randomValue > 0.31 && randomValue < 0.6)
        {
            GameObject temptBuilding = Instantiate(buildingPrefabTest[1], new Vector3(transform.position.x, 0.05f, transform.position.z), Quaternion.identity);
            temptBuilding.transform.SetParent(transform);
            thisCollider.enabled = false;
        }
        if (randomValue > 0.61)
        {
            GameObject temptBuilding = Instantiate(buildingPrefabTest[2], new Vector3(transform.position.x, 0.05f, transform.position.z), Quaternion.identity);
            temptBuilding.transform.SetParent(transform);
            thisCollider.enabled = false;
        }
    }
}
