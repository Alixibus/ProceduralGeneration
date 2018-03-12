using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingSpawnScript : MonoBehaviour {
    public GameObject[] buildingPrefabTest;

    private void Start()
    {
        BuildBuilding();
    }
    void BuildBuilding()
    {
        GameObject temptBuilding = Instantiate(buildingPrefabTest[0], new Vector3(transform.position.x, 0.5f, transform.position.z), Quaternion.identity);
        temptBuilding.transform.SetParent(transform);
    }
}
