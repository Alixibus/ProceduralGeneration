using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Road_Type_Script : MonoBehaviour {

    
    public bool haveRoad = false;
    [SerializeField]
    bool northHasRoad, southHasRoad, eastHasRoad, westHasRoad;
    public bool exitPointNorth, exitPointSouth, exitPointEast, exitPointWest;
    public GameObject[] roadPrefabs;
    int counter = 0;


    [SerializeField]
    string roadName;

    private void Start()
    {
        northHasRoad = false;
        southHasRoad = false;
        eastHasRoad = false;
        westHasRoad = false;

        exitPointNorth = false;
        exitPointSouth = false;
        exitPointEast = false;
        exitPointWest = false;        
    }
    public void beginAllProcesses()
    {
        StartCoroutine(Waiting());
    }
    IEnumerator Waiting()
    {
        yield return new WaitForSeconds(0.1f);
        CheckNeighbour();
    }

    void CheckNeighbour()
    {
        Escape_Road_Script escapeScriptHolder = null;
        Road_Type_Script roadScriptHolder = null;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.forward, out hit))
        {
            if(hit.transform.gameObject.GetComponent<Escape_Road_Script>() != null)
            {
               escapeScriptHolder = hit.transform.gameObject.GetComponent<Escape_Road_Script>();
            }
            if(hit.transform.gameObject.GetComponent<Road_Type_Script>() != null)
            {
               roadScriptHolder = hit.transform.gameObject.GetComponent<Road_Type_Script>();
            }

            if (hit.transform.gameObject.GetComponent<Escape_Road_Script>() != null && escapeScriptHolder.exitPointSouth == true)
            {
                northHasRoad = true;
            }
            else if (hit.transform.gameObject.GetComponent<Road_Type_Script>() != null && roadScriptHolder.exitPointSouth == true)
            {
                northHasRoad = true;
            }
            else if (hit.transform.tag == "StartPoint")
            {
                northHasRoad = true;
                //print("The North has Road");
                //Instantiate(roadPrefabs[0], transform.position, Quaternion.identity);
            }
            else
            {
                northHasRoad = false;
            }
        }
        if (Physics.Raycast(transform.position, -Vector3.forward, out hit))
        {
            if (hit.transform.gameObject.GetComponent<Escape_Road_Script>() != null)
            {
                escapeScriptHolder = hit.transform.gameObject.GetComponent<Escape_Road_Script>();
            }
            if (hit.transform.gameObject.GetComponent<Road_Type_Script>() != null)
            {
                roadScriptHolder = hit.transform.gameObject.GetComponent<Road_Type_Script>();
            }
            if (hit.transform.gameObject.GetComponent<Escape_Road_Script>() != null && escapeScriptHolder.exitPointNorth == true)
            {
                southHasRoad = true;
            }
            else if (hit.transform.gameObject.GetComponent<Road_Type_Script>() != null && roadScriptHolder.exitPointNorth == true)
            {
                southHasRoad = true;
            }
            else if (hit.transform.tag == "StartPoint")
            {
                southHasRoad = true;
                //print("The North has Road");
                //Instantiate(roadPrefabs[0], transform.position, Quaternion.identity);
            }
            else
            {
                southHasRoad = false;
            }
        }
        if (Physics.Raycast(transform.position, Vector3.right, out hit))
        {
            if (hit.transform.gameObject.GetComponent<Escape_Road_Script>() != null)
            {
                escapeScriptHolder = hit.transform.gameObject.GetComponent<Escape_Road_Script>();
            }
            if (hit.transform.gameObject.GetComponent<Road_Type_Script>() != null)
            {
                roadScriptHolder = hit.transform.gameObject.GetComponent<Road_Type_Script>();
            }
            if (hit.transform.gameObject.GetComponent<Escape_Road_Script>() != null && escapeScriptHolder.exitPointWest == true)
            {
                eastHasRoad = true;
            }
            else if (hit.transform.gameObject.GetComponent<Road_Type_Script>() != null && roadScriptHolder.exitPointWest == true)
            {
                eastHasRoad = true;
            }
            else if (hit.transform.tag == "StartPoint")
            {
                eastHasRoad = true;
                //print("The North has Road");
                //Instantiate(roadPrefabs[0], transform.position, Quaternion.identity);
            }
            else
            {
                eastHasRoad = false;
            }
        }
        if (Physics.Raycast(transform.position, -Vector3.right, out hit))
        {
            if (hit.transform.gameObject.GetComponent<Escape_Road_Script>() != null)
            {
                escapeScriptHolder = hit.transform.gameObject.GetComponent<Escape_Road_Script>();
            }
            if (hit.transform.gameObject.GetComponent<Road_Type_Script>() != null)
            {
                roadScriptHolder = hit.transform.gameObject.GetComponent<Road_Type_Script>();
            }
            if (hit.transform.gameObject.GetComponent<Escape_Road_Script>() != null && escapeScriptHolder.exitPointEast == true)
            {
                westHasRoad = true;
            }
            else if (hit.transform.gameObject.GetComponent<Road_Type_Script>() != null && roadScriptHolder.exitPointEast == true)
            {
                westHasRoad = true;
            }
            else if (hit.transform.tag == "StartPoint")
            {
                westHasRoad = true;
                //print("The North has Road");
                //Instantiate(roadPrefabs[0], transform.position, Quaternion.identity);
            }
            else
            {
                westHasRoad = false;
            }
        }
        chooseRoad();
    }

    void chooseRoad()
    {
        if (northHasRoad == true && southHasRoad == true)
        {
            float randomise = Random.value;
            if (randomise > 0 && randomise < 0.2f && eastHasRoad == true)
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
            if (randomise > 0 && randomise < 0.5f && westHasRoad)
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
            if (randomise > 0 && randomise < 0.5f && eastHasRoad == true)
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
            if (randomise > 0 && randomise < 0.5f && westHasRoad)
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
            if (randomise > 0 && randomise < 0.5f && eastHasRoad == true)
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
            if (randomise > 0 && randomise < 0.2f && southHasRoad == true)
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
        if(northHasRoad == true && southHasRoad == false && eastHasRoad == false && westHasRoad == false)
        {
            float randomise = Random.value;
            if(randomise > 0 && randomise < 0.5f)
            {
                GameObject chosenRoad = Instantiate(roadPrefabs[4], transform.position, Quaternion.identity);
                chosenRoad.transform.SetParent(transform);
                exitPointNorth = true;
                exitPointSouth = false;
                exitPointEast = false;
                exitPointWest = false;
                haveRoad = true;
            }
            else
            {
                GameObject chosenRoad = Instantiate(roadPrefabs[3], transform.position, Quaternion.identity);
                chosenRoad.transform.SetParent(transform);
                exitPointNorth = true;
                exitPointSouth = true;
                exitPointEast = true;
                exitPointWest = true;
                haveRoad = true;
            }
        }
        if (northHasRoad == false && southHasRoad == true && eastHasRoad == false && westHasRoad == false)
        {
            float randomise = Random.value;
            if (randomise > 0 && randomise < 0.5f)
            {
                GameObject chosenRoad = Instantiate(roadPrefabs[4], transform.position, Quaternion.Euler(0, 180, 0));
                chosenRoad.transform.SetParent(transform);
                exitPointNorth = false;
                exitPointSouth = true;
                exitPointEast = false;
                exitPointWest = false;
                haveRoad = true;
            }
            else
            {
                GameObject chosenRoad = Instantiate(roadPrefabs[3], transform.position, Quaternion.identity);
                chosenRoad.transform.SetParent(transform);
                exitPointNorth = true;
                exitPointSouth = true;
                exitPointEast = true;
                exitPointWest = true;
                haveRoad = true;
            }
        }
        if (northHasRoad == false && southHasRoad == false && eastHasRoad == true && westHasRoad == false)
        {
            float randomise = Random.value;
            if (randomise > 0 && randomise < 0.5f)
            {
                GameObject chosenRoad = Instantiate(roadPrefabs[4], transform.position, Quaternion.Euler(0, 90, 0));
                chosenRoad.transform.SetParent(transform);
                exitPointNorth = false;
                exitPointSouth = false;
                exitPointEast = true;
                exitPointWest = false;
                haveRoad = true;
            }
            else
            {
                GameObject chosenRoad = Instantiate(roadPrefabs[3], transform.position, Quaternion.identity);
                chosenRoad.transform.SetParent(transform);
                exitPointNorth = true;
                exitPointSouth = true;
                exitPointEast = true;
                exitPointWest = true;
                haveRoad = true;
            }
        }
        if (northHasRoad == false && southHasRoad == false && eastHasRoad == false && westHasRoad == true)
        {
            float randomise = Random.value;
            if (randomise > 0 && randomise < 0.5f)
            {
                GameObject chosenRoad = Instantiate(roadPrefabs[4], transform.position, Quaternion.Euler(0, 270, 0));
                chosenRoad.transform.SetParent(transform);
                exitPointNorth = true;
                exitPointSouth = false;
                exitPointEast = false;
                exitPointWest = false;
                haveRoad = true;
            }
            else
            {
                GameObject chosenRoad = Instantiate(roadPrefabs[3], transform.position, Quaternion.identity);
                chosenRoad.transform.SetParent(transform);
                exitPointNorth = true;
                exitPointSouth = true;
                exitPointEast = true;
                exitPointWest = true;
                haveRoad = true;
            }
        }

        if (northHasRoad == false && southHasRoad == false && eastHasRoad == false && westHasRoad == false && counter < 5)
        {
            StartCoroutine(Waiting());
            counter++;
        }
        else
        {
            StopAllCoroutines();
        }
    }


    public bool hasRoadCheck()
    {
        return haveRoad;
    }
}
