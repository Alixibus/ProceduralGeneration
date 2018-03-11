using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Escape_Road_Script : MonoBehaviour {
    [SerializeField]
    bool northHasRoad, southHasRoad, eastHasRoad, westHasRoad;
    public GameObject[] roadPrefabs;

    [SerializeField]
    public bool exitNorth
    {
        get { return exitPointNorth; }
        set { exitPointNorth = value; }
    }
    public bool exitPointNorth;

    [SerializeField]
    public bool exitSouth
    {
        get { return exitPointSouth; }
        set { exitPointSouth = value; }
    }
    public bool exitPointSouth;

    [SerializeField]
    public bool exitEast
    {
        get { return exitPointEast; }
        set { exitPointEast = value; }
    }
    public bool exitPointEast;

    [SerializeField]
    public bool exitWest
    {
        get { return exitPointWest; }
        set { exitPointWest = value; }
    }
    public bool exitPointWest;

    [SerializeField]
    public bool haveRoad
    {
        get { return thisHasRoad; }
        set { thisHasRoad = value; }
    }
    public bool thisHasRoad;


    [SerializeField]
    string roadName;

    private void Start()
    {
        CheckNeighbour();
        //StartCoroutine(Waiting());
    }

    IEnumerator Waiting()
    {
        yield return new WaitForSeconds(0.1f);
        CheckNeighbour();
    }

    void CheckNeighbour()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.forward, out hit))
        {
            if (hit.collider.tag == "EscapePath" || hit.collider.tag == "StartPoint" || hit.collider.tag == "EndPoint")
            {
                northHasRoad = true;
                //print("The North has Road");
                //Instantiate(roadPrefabs[0], transform.position, Quaternion.identity);
            }
        }
        if (Physics.Raycast(transform.position, -Vector3.forward, out hit))
        {
            if (hit.collider.tag == "EscapePath" || hit.collider.tag == "StartPoint" || hit.collider.tag == "EndPoint")
            {
                southHasRoad = true;
                //print("The North has Road");
                //Instantiate(roadPrefabs[0], transform.position, Quaternion.identity);
            }
        }
        if (Physics.Raycast(transform.position, Vector3.right, out hit))
        {
            if (hit.collider.tag == "EscapePath" || hit.collider.tag == "StartPoint" || hit.collider.tag == "EndPoint")
            {
                eastHasRoad = true;
                //print("The North has Road");
                //Instantiate(roadPrefabs[0], transform.position, Quaternion.identity);
            }
        }
        if (Physics.Raycast(transform.position, -Vector3.right, out hit))
        {
            if (hit.collider.tag == "EscapePath" || hit.collider.tag == "StartPoint" || hit.collider.tag == "EndPoint")
            {
                westHasRoad = true;
                //print("The North has Road");
                //Instantiate(roadPrefabs[0], transform.position, Quaternion.identity);
            }
        }
        chooseRoad();
    }

    void chooseRoad()
    {
        if (northHasRoad == true && southHasRoad == true)
        {
            float randomise = Random.value;
            if (randomise > 0 && randomise < 0.2f)
            {
                //TJunction has been selected and instantiated, road exit locations have been confirmed
                GameObject chosenRoad = Instantiate(roadPrefabs[1], transform.position, Quaternion.identity);
                chosenRoad.transform.SetParent(transform);
                exitPointNorth = true;
                exitPointSouth = true;
                exitPointEast = true;
                haveRoad = true;
            }
            else
            {
                //Straight has been selected and instantiated, road exit locations have been confirmed
                GameObject chosenRoad = Instantiate(roadPrefabs[0], transform.position, Quaternion.identity);
                chosenRoad.transform.SetParent(transform);
                exitPointNorth = true;
                exitPointSouth = true;
                haveRoad = true;
            }
        }
        if (northHasRoad == true && eastHasRoad == true)
        {
            float randomise = Random.value;
            if (randomise > 0 && randomise < 0.5f)
            {
                //TJunction has been selected and instantiated, road exit locations have been confirmed
                GameObject chosenRoad = Instantiate(roadPrefabs[1], transform.position, Quaternion.Euler(0, 270, 0));
                chosenRoad.transform.SetParent(transform);
                exitPointNorth = true;
                exitPointEast = true;
                exitPointWest = true;
                haveRoad = true;
            }
            else
            {
                //Corner has been selected and instantiated, road exit locations have been confirmed
                GameObject chosenRoad = Instantiate(roadPrefabs[2], transform.position, Quaternion.identity);
                chosenRoad.transform.SetParent(transform);
                exitPointNorth = true;
                exitPointEast = true;
                haveRoad = true;
            }
        }
        if (northHasRoad == true && westHasRoad == true)
        {
            float randomise = Random.value;
            if (randomise > 0 && randomise < 0.5f)
            {
                //TJunction has been selected and instantiated, road exit locations have been confirmed
                GameObject chosenRoad = Instantiate(roadPrefabs[1], transform.position, Quaternion.Euler(0, 270, 0));
                chosenRoad.transform.SetParent(transform);
                exitPointNorth = true;
                exitPointEast = true;
                exitPointWest = true;
                haveRoad = true;
            }
            else
            {
                //Corner has been selected and instantiated, road exit locations have been confirmed
                GameObject chosenRoad = Instantiate(roadPrefabs[2], transform.position, Quaternion.Euler(0, 270, 0));
                chosenRoad.transform.SetParent(transform);
                exitPointNorth = true;
                exitPointWest = true;
                haveRoad = true;
            }
        }
        if (southHasRoad == true && eastHasRoad == true)
        {
            float randomise = Random.value;
            if (randomise > 0 && randomise < 0.5f)
            {
                //TJunction has been selected and instantiated, road exit locations have been confirmed
                GameObject chosenRoad = Instantiate(roadPrefabs[1], transform.position, Quaternion.Euler(0, 90, 0));
                chosenRoad.transform.SetParent(transform);
                exitPointSouth = true;
                exitPointEast = true;
                exitPointWest = true;
                haveRoad = true;
            }
            else
            {
                //Corner has been selected and instantiated, road exit locations have been confirmed
                GameObject chosenRoad = Instantiate(roadPrefabs[2], transform.position, Quaternion.Euler(0, 90, 0));
                chosenRoad.transform.SetParent(transform);
                exitPointSouth = true;
                exitPointEast = true;
                haveRoad = true;
            }
        }
        if (southHasRoad == true && westHasRoad == true)
        {
            float randomise = Random.value;
            if (randomise > 0 && randomise < 0.5f)
            {
                //TJunction has been selected and instantiated, road exit locations have been confirmed
                GameObject chosenRoad = Instantiate(roadPrefabs[1], transform.position, Quaternion.Euler(0, 90, 0));
                chosenRoad.transform.SetParent(transform);
                exitPointSouth = true;
                exitPointEast = true;
                exitPointWest = true;
                haveRoad = true;
            }
            else
            {
                //Corner has been selected and instantiated, road exit locations have been confirmed
                GameObject chosenRoad = Instantiate(roadPrefabs[2], transform.position, Quaternion.Euler(0, 180, 0));
                chosenRoad.transform.SetParent(transform);
                exitPointSouth = true;
                exitPointWest = true;
                haveRoad = true;
            }
        }
        if (eastHasRoad == true && westHasRoad == true)
        {
            float randomise = Random.value;
            if (randomise > 0 && randomise < 0.2f)
            {
                //TJunction has been selected and instantiated, road exit locations have been confirmed
                GameObject chosenRoad = Instantiate(roadPrefabs[1], transform.position, Quaternion.Euler(0, 90, 0));
                chosenRoad.transform.SetParent(transform);
                exitPointSouth = true;
                exitPointEast = true;
                exitPointWest = true;
                haveRoad = true;
            }
            else
            {
                //Straight has been selected and instantiated, road exit locations have been confirmed
                GameObject chosenRoad = Instantiate(roadPrefabs[0], transform.position, Quaternion.Euler(0, 90, 0));
                chosenRoad.transform.SetParent(transform);
                exitPointEast = true;
                exitPointWest = true;
                haveRoad = true;
            }
        }
    }
}
