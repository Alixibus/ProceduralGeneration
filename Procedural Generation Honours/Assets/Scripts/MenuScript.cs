using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;

public class MenuScript : MonoBehaviour
{

    public GameObject startPointPreFab, exitPointPreFab;
    public GameObject[] buildingBlocks;
    public GameObject seedHolder; 
    private GameObject startPoint, currentTile, exitPoint;
    private Vector3 preExitTile;
    private List<GameObject> gridPath;
    private GameObject[,] instantiatedMap;
    public GameObject[] roadPieces;
    public int gridWidth, gridHeight = 0;
    public AnimationCurve probabilityCurve;
    [SerializeField]
    int seed;
    int tileCount = 0;
    bool exitFound;
    int findCornerHeight = 0;
    int findCornerWidth = 0;

    // Use this for initialization
    void Start()
    {
        BuildGrid();
    }

    private void Update()
    {
    }

    void BuildGrid()
    {
        if (gridWidth == 0)
        {
            gridWidth = Mathf.RoundToInt(UnityEngine.Random.Range(9.0f, 25.0f));
            while (gridWidth % 2 == 0)
            {
                gridWidth = Mathf.RoundToInt(UnityEngine.Random.Range(9.0f, 25.0f));
            }
        }
        if (gridHeight == 0)
        {
            gridHeight = Mathf.RoundToInt(UnityEngine.Random.Range(5.0f, 25.0f));
            while (gridHeight % 2 == 0)
            {
                gridHeight = Mathf.RoundToInt(UnityEngine.Random.Range(5.0f, 25.0f));
            }
        }
        gridHeight = 10;
        gridWidth = 20;
        findCornerHeight = 0;
        findCornerWidth = 0;
        int totalGridPoints = gridWidth * gridHeight;
        gridPath = new List<GameObject>();

        int gridCount = 0;
        for (int makeWidth = 0; makeWidth <= gridWidth; makeWidth++)
        {
            for (int makeHeight = 0; makeHeight <= gridHeight; makeHeight++)
            {
                gridPath.Add(Instantiate(buildingBlocks[0], new Vector3(findCornerWidth + makeWidth, 0, findCornerHeight + makeHeight), Quaternion.identity));
                gridCount++;
            }
        }
        //Vector3 testPos = new Vector3();
        //List<GameObject> goAtPos = gridPath.Where(x => x.transform.position == testPos).ToList();

        BuildMap();
    }

    void BuildMap()
    {
        exitFound = false;
        //Instantiate(prefabObjects[0], new Vector3(0, 0, 0), Quaternion.identity);
        instantiatedMap = new GameObject[gridWidth + 1, gridHeight + 1];
        //Generate start point near inner circle, for testing only use 0,0
        //int randomStart = Mathf.RoundToInt(Random.Range(-1.0f, 1.0f));
        startPoint = Instantiate(startPointPreFab, new Vector3(Mathf.RoundToInt(gridWidth / 2), 0, Mathf.RoundToInt(gridHeight / 2)), Quaternion.identity);
        Road_Type_Script roadScript = startPoint.GetComponent<Road_Type_Script>();
        roadScript.exitPointNorth = true;
        roadScript.exitPointSouth = true;
        roadScript.exitPointEast = true;
        roadScript.exitPointWest = true;
        roadScript.thisHasRoad = true;

        instantiatedMap[Mathf.RoundToInt(startPoint.transform.position.x), Mathf.RoundToInt(startPoint.transform.position.z)] = startPoint;

        //Generate exit Route on Outer
        ChooseExitPoint();
        instantiatedMap[Mathf.RoundToInt(exitPoint.transform.position.x), Mathf.RoundToInt(exitPoint.transform.position.z)] = exitPoint;
        currentTile = startPoint;


        while (exitFound == false)
        {
            //escapeRoute.Add(Instantiate(buildingBlocks[1], ChooseNextTile(currentTile.transform.position, preExitTile), Quaternion.identity));
            GameObject tempObjectHolder = Instantiate(buildingBlocks[1], ChooseNextTile(currentTile.transform.position, preExitTile), Quaternion.identity);
            instantiatedMap[Mathf.RoundToInt(tempObjectHolder.transform.position.x), Mathf.RoundToInt(tempObjectHolder.transform.position.z)] = tempObjectHolder;
            currentTile = tempObjectHolder;
        }

        //for (int x = 0; x < instantiatedMap.Length; x++)
        //{

        //    if (gridPath[x].transform.position == startPoint.transform.position || gridPath[x].transform.position == exitPoint.transform.position)
        //    {
        //        continue;
        //    }
        //    else if (gridPath[x].transform.position.z == gridHeight || gridPath[x].transform.position.z == findCornerHeight || gridPath[x].transform.position.x == gridWidth || gridPath[x].transform.position.x == findCornerWidth)
        //    {
        //        instantiatedMap[Mathf.RoundToInt(gridPath[x].transform.position.x), Mathf.RoundToInt(gridPath[x].transform.position.z)] = Instantiate(buildingBlocks[4], gridPath[x].transform.position, Quaternion.identity);
        //    }
        //    else
        //    {
        //        if (instantiatedMap[Mathf.RoundToInt(gridPath[x].transform.position.x), Mathf.RoundToInt(gridPath[x].transform.position.z)] == null)
        //        {
        //            tileDecision(gridPath[x].transform.position);
        //            tileCount++;
        //        }
        //    }
        //}
        StartCoroutine(Waiting());


        //StartCoroutine(Waiting(instantiatedMap[7, 3]));
        //decideRoad(instantiatedMap[7, 4]);
    }

    void ChooseExitPoint()
    {
        //Choose top, bottom, left or right for exit location by randomly generating a number between 1 and 4,
        // let 1 represent Top of Grid, 2 Bottom, 3 Left and 4 Right
        int randomNumber = Mathf.RoundToInt(UnityEngine.Random.Range(1.0f, 4.0f));
        Road_Type_Script roadScript = null;
        // int randomNumber = 5; //For Testing

        switch (randomNumber)
        {
            case 1:
                exitPoint = Instantiate(exitPointPreFab, new Vector3(findCornerWidth + UnityEngine.Random.Range(1, gridWidth - 1), 0, gridHeight), Quaternion.Euler(0.0f, 90.0f, 0.0f));
                preExitTile = new Vector3(exitPoint.transform.position.x, exitPoint.transform.position.y, exitPoint.transform.position.z - 1);
                roadScript = exitPoint.GetComponent<Road_Type_Script>();
                roadScript.exitPointNorth = false;
                roadScript.exitPointSouth = true;
                roadScript.exitPointEast = false;
                roadScript.exitPointWest = false;
                roadScript.thisHasRoad = true;
                break;
            case 2:
                exitPoint = Instantiate(exitPointPreFab, new Vector3(findCornerWidth + UnityEngine.Random.Range(1, gridWidth - 1), 0, findCornerHeight), Quaternion.Euler(0.0f, 270.0f, 0.0f));
                preExitTile = new Vector3(exitPoint.transform.position.x, exitPoint.transform.position.y, exitPoint.transform.position.z + 1);
                roadScript = exitPoint.GetComponent<Road_Type_Script>();
                roadScript.exitPointNorth = true;
                roadScript.exitPointSouth = false;
                roadScript.exitPointEast = false;
                roadScript.exitPointWest = false;
                roadScript.thisHasRoad = true;
                break;
            case 3:
                exitPoint = Instantiate(exitPointPreFab, new Vector3(findCornerWidth, 0, findCornerHeight + UnityEngine.Random.Range(1, gridHeight - 1)), Quaternion.identity);
                preExitTile = new Vector3(exitPoint.transform.position.x + 1, exitPoint.transform.position.y, exitPoint.transform.position.z);
                roadScript = exitPoint.GetComponent<Road_Type_Script>();
                roadScript.exitPointNorth = false;
                roadScript.exitPointSouth = false;
                roadScript.exitPointEast = true;
                roadScript.exitPointWest = false;
                roadScript.thisHasRoad = true;
                break;
            case 4:
                exitPoint = Instantiate(exitPointPreFab, new Vector3(gridWidth, 0, findCornerHeight + UnityEngine.Random.Range(1, gridHeight - 1)), Quaternion.Euler(0.0f, 180.0f, 0.0f));
                preExitTile = new Vector3(exitPoint.transform.position.x - 1, exitPoint.transform.position.y, exitPoint.transform.position.z);
                roadScript = exitPoint.GetComponent<Road_Type_Script>();
                roadScript.exitPointNorth = false;
                roadScript.exitPointSouth = false;
                roadScript.exitPointEast = false;
                roadScript.exitPointWest = true;
                roadScript.thisHasRoad = true;
                break;
            case 5:
                //For Testing purposes
                exitPoint = Instantiate(exitPointPreFab, new Vector3(gridWidth, 0, findCornerHeight + 1), Quaternion.Euler(0.0f, 180.0f, 0.0f));
                preExitTile = new Vector3(exitPoint.transform.position.x - 1, exitPoint.transform.position.y, exitPoint.transform.position.z);
                break;
            default:
                print("No chosen location");
                break;
        }
    }

    Vector3 ChooseNextTile(Vector3 currentTile, Vector3 exitPoint)
    {
        Vector3 chosenTile = currentTile;
        Vector3 northPoint = new Vector3(currentTile.x, currentTile.y, currentTile.z + 1);
        Vector3 southPoint = new Vector3(currentTile.x, currentTile.y, currentTile.z - 1);
        Vector3 eastPoint = new Vector3(currentTile.x + 1, currentTile.y, currentTile.z);
        Vector3 westPoint = new Vector3(currentTile.x - 1, currentTile.y, currentTile.z);

        if (Vector3.Distance(northPoint, exitPoint) < Vector3.Distance(chosenTile, exitPoint) && northPoint.z != gridHeight / 2)
        {
            chosenTile = northPoint;
        }
        if (Vector3.Distance(southPoint, exitPoint) < Vector3.Distance(chosenTile, exitPoint) && southPoint.z != findCornerHeight)
        {
            chosenTile = southPoint;
        }
        if (Vector3.Distance(eastPoint, exitPoint) < Vector3.Distance(chosenTile, exitPoint) && eastPoint.x != gridWidth / 2)
        {
            chosenTile = eastPoint;
        }
        if (Vector3.Distance(westPoint, exitPoint) < Vector3.Distance(chosenTile, exitPoint) && westPoint.x != findCornerWidth)
        {
            chosenTile = westPoint;
        }
        if (chosenTile.x == exitPoint.x)
        {
            chosenTile = exitPoint;
            exitFound = true;
        }
        return chosenTile;
    }

    public void Generate()
    {
        seed = UnityEngine.Random.Range(-10000, 10000);
        UnityEngine.Random.InitState(seed);
        tileCount = 0;
        StopAllCoroutines();

        DestroyImmediate(startPoint.gameObject);
        startPoint = null;
        DestroyImmediate(exitPoint.gameObject);
        exitPoint = null;
        for (int x = 0; x <= gridWidth; x++)
        {
            for (int y = 0; y <= gridHeight; y++)
            {
                Destroy(instantiatedMap[x, y], 0.0f);
            }
        }
        //gridPath.Clear();
        //escapeRoute.Clear();
        //foreach(GameObject instantiated in instantiatedMap)
        //{
        //    DestroyImmediate(instantiated.gameObject);
        //}
        //Array.Clear(instantiatedMap, 0, instantiatedMap.Length);

        //foreach (GameObject grid in gridPath)
        //{
        //    DestroyImmediate(grid.gameObject);
        //}
        //gridPath.Clear();

        //foreach (GameObject path in escapeRoute)
        //{
        //    DestroyImmediate(path.gameObject);
        //}
        //escapeRoute.Clear();
        //for (int x = 0; x < gridPath.Count; x++)
        //{
        //    Destroy(gridPath[x].gameObject, 0.0f);
        //}
        gridWidth = 0;
        gridHeight = 0;
        findCornerHeight = 0;
        findCornerWidth = 0;
        BuildGrid();
        BuildMap();
    }

    void tileDecision(Vector3 tileLocation)
    {
        string chosenType = "";
        bool canBePark = true;
        Road_Type_Script roadScriptHolder = null;
        if (instantiatedMap[Mathf.RoundToInt(tileLocation.x), Mathf.RoundToInt(tileLocation.z + 1)] != null)
        {
            if (instantiatedMap[Mathf.RoundToInt(tileLocation.x), Mathf.RoundToInt(tileLocation.z + 1)].tag == "EscapePath" || instantiatedMap[Mathf.RoundToInt(tileLocation.x), Mathf.RoundToInt(tileLocation.z + 1)].tag == "StartPoint")
            {
                canBePark = false;
            }
            if (instantiatedMap[Mathf.RoundToInt(tileLocation.x), Mathf.RoundToInt(tileLocation.z + 1)].tag == "Road Tile")
            {
                roadScriptHolder = instantiatedMap[Mathf.RoundToInt(tileLocation.x), Mathf.RoundToInt(tileLocation.z + 1)].GetComponent<Road_Type_Script>();
                if (roadScriptHolder.haveRoad == true)
                {
                    canBePark = false;
                }
            }
        }
        if (instantiatedMap[Mathf.RoundToInt(tileLocation.x), Mathf.RoundToInt(tileLocation.z - 1)] != null)
        {
            if (instantiatedMap[Mathf.RoundToInt(tileLocation.x), Mathf.RoundToInt(tileLocation.z - 1)].tag == "EscapePath" || instantiatedMap[Mathf.RoundToInt(tileLocation.x), Mathf.RoundToInt(tileLocation.z - 1)].tag == "StartPoint")
            {
                canBePark = false;
            }
            if (instantiatedMap[Mathf.RoundToInt(tileLocation.x), Mathf.RoundToInt(tileLocation.z - 1)].tag == "Road Tile")
            {
                roadScriptHolder = instantiatedMap[Mathf.RoundToInt(tileLocation.x), Mathf.RoundToInt(tileLocation.z - 1)].GetComponent<Road_Type_Script>();
                if (roadScriptHolder.haveRoad == true)
                {
                    canBePark = false;
                }
            }
        }
        if (instantiatedMap[Mathf.RoundToInt(tileLocation.x + 1), Mathf.RoundToInt(tileLocation.z)] != null)
        {
            if (instantiatedMap[Mathf.RoundToInt(tileLocation.x + 1), Mathf.RoundToInt(tileLocation.z)].tag == "EscapePath" || instantiatedMap[Mathf.RoundToInt(tileLocation.x + 1), Mathf.RoundToInt(tileLocation.z)].tag == "StartPoint")
            {
                canBePark = false;
            }
            if (instantiatedMap[Mathf.RoundToInt(tileLocation.x + 1), Mathf.RoundToInt(tileLocation.z)].tag == "Road Tile")
            {
                roadScriptHolder = instantiatedMap[Mathf.RoundToInt(tileLocation.x + 1), Mathf.RoundToInt(tileLocation.z)].GetComponent<Road_Type_Script>();
                if (roadScriptHolder.haveRoad == true)
                {
                    canBePark = false;
                }
            }
        }
        if (instantiatedMap[Mathf.RoundToInt(tileLocation.x - 1), Mathf.RoundToInt(tileLocation.z)] != null)
        {
            if (instantiatedMap[Mathf.RoundToInt(tileLocation.x - 1), Mathf.RoundToInt(tileLocation.z)].tag == "EscapePath" || instantiatedMap[Mathf.RoundToInt(tileLocation.x - 1), Mathf.RoundToInt(tileLocation.z)].tag == "StartPoint")
            {
                canBePark = false;
            }
            if (instantiatedMap[Mathf.RoundToInt(tileLocation.x - 1), Mathf.RoundToInt(tileLocation.z)].tag == "Road Tile")
            {
                roadScriptHolder = instantiatedMap[Mathf.RoundToInt(tileLocation.x - 1), Mathf.RoundToInt(tileLocation.z)].GetComponent<Road_Type_Script>();
                if (roadScriptHolder.haveRoad == true)
                {
                    canBePark = false;
                }
            }
        }

        float animationCurveTest;
        animationCurveTest = probabilityCurve.Evaluate(UnityEngine.Random.value);

        if (animationCurveTest < 0.3 && canBePark == true)
        {
            chosenType = "Park";
            instantiatedMap[Mathf.RoundToInt(tileLocation.x), Mathf.RoundToInt(tileLocation.z)] = Instantiate(buildingBlocks[3], tileLocation, Quaternion.identity);
        }
        else
        {
            chosenType = "Road";
            GameObject roadTileHolder = null;
            roadTileHolder = instantiatedMap[Mathf.RoundToInt(tileLocation.x), Mathf.RoundToInt(tileLocation.z)] = Instantiate(buildingBlocks[2], tileLocation, Quaternion.identity);
        }

        //print(tileCount + " : " + animationCurveTest + " : " + chosenType + " : " + tileLocation);
    }

    IEnumerator Waiting()
    {
        for (int x = 0; x < instantiatedMap.Length; x++)
        {
            yield return new WaitForSeconds(0.02f);
            if (gridPath[x].transform.position == startPoint.transform.position || gridPath[x].transform.position == exitPoint.transform.position)
            {
                continue;
            }
            else if (gridPath[x].transform.position.z == gridHeight || gridPath[x].transform.position.z == findCornerHeight || gridPath[x].transform.position.x == gridWidth || gridPath[x].transform.position.x == findCornerWidth)
            {
                instantiatedMap[Mathf.RoundToInt(gridPath[x].transform.position.x), Mathf.RoundToInt(gridPath[x].transform.position.z)] = Instantiate(buildingBlocks[4], gridPath[x].transform.position, Quaternion.identity);
            }
            else
            {
                if (instantiatedMap[Mathf.RoundToInt(gridPath[x].transform.position.x), Mathf.RoundToInt(gridPath[x].transform.position.z)] == null)
                {
                    tileDecision(gridPath[x].transform.position);
                    tileCount++;
                }
            }
        }

        for (int eachGridPoint = 0; eachGridPoint < gridPath.Count; eachGridPoint++)
        {
            Destroy(gridPath[eachGridPoint], 0.0f);
        }

        yield return new WaitForSeconds(1);

        for (int x = 1; x < gridWidth; x++)
        {
            for (int z = 1; z < gridHeight; z++)
            {
                if (instantiatedMap[x, z] != null)
                {
                    yield return new WaitForSeconds(0.02f);
                    RoadBuilder(instantiatedMap[x, z]);
                }
            }
        }
    }
    void RoadBuilder(GameObject passedTile)
    {
        bool possibleNorth = false;
        bool possibleSouth = false;
        bool possibleEast = false;
        bool possibleWest = false;
        bool northHasRoad = false;
        bool southHasRoad = false;
        bool eastHasRoad = false;
        bool westHasRoad = false;
        bool roadChosen = false;

        Road_Type_Script roadScriptHolder = null;
        GameObject selectedRoadPiece;
        if (passedTile.GetComponent<Road_Type_Script>() != null)
        {
            roadScriptHolder = passedTile.GetComponent<Road_Type_Script>();
        }

        if (instantiatedMap[Mathf.RoundToInt(passedTile.transform.position.x), Mathf.RoundToInt(passedTile.transform.position.z + 1)] != null)
        {
            if (instantiatedMap[Mathf.RoundToInt(passedTile.transform.position.x), Mathf.RoundToInt(passedTile.transform.position.z + 1)].tag == "StartPoint" || instantiatedMap[Mathf.RoundToInt(passedTile.transform.position.x), Mathf.RoundToInt(passedTile.transform.position.z + 1)].tag == "Road Tile" || instantiatedMap[Mathf.RoundToInt(passedTile.transform.position.x), Mathf.RoundToInt(passedTile.transform.position.z + 1)].tag == "EndPoint")
            {
                Road_Type_Script tempRoadScriptHolder = instantiatedMap[Mathf.RoundToInt(passedTile.transform.position.x), Mathf.RoundToInt(passedTile.transform.position.z + 1)].GetComponent<Road_Type_Script>();
                if (tempRoadScriptHolder.thisHasRoad == true)
                {
                    possibleNorth = tempRoadScriptHolder.exitSouth;
                    northHasRoad = tempRoadScriptHolder.haveRoad;
                }
                else
                {
                    possibleNorth = true;
                }
            }
        }
        if (instantiatedMap[Mathf.RoundToInt(passedTile.transform.position.x), Mathf.RoundToInt(passedTile.transform.position.z - 1)] != null)
        {
            if (instantiatedMap[Mathf.RoundToInt(passedTile.transform.position.x), Mathf.RoundToInt(passedTile.transform.position.z - 1)].tag == "StartPoint" || instantiatedMap[Mathf.RoundToInt(passedTile.transform.position.x), Mathf.RoundToInt(passedTile.transform.position.z - 1)].tag == "Road Tile" || instantiatedMap[Mathf.RoundToInt(passedTile.transform.position.x), Mathf.RoundToInt(passedTile.transform.position.z - 1)].tag == "EndPoint")
            {
                Road_Type_Script tempRoadScriptHolder = instantiatedMap[Mathf.RoundToInt(passedTile.transform.position.x), Mathf.RoundToInt(passedTile.transform.position.z - 1)].GetComponent<Road_Type_Script>();
                if (tempRoadScriptHolder.thisHasRoad == true)
                {
                    possibleSouth = tempRoadScriptHolder.exitNorth;
                    southHasRoad = tempRoadScriptHolder.haveRoad;
                }
                else
                {
                    possibleSouth = true;
                }
            }
        }
        if (instantiatedMap[Mathf.RoundToInt(passedTile.transform.position.x + 1), Mathf.RoundToInt(passedTile.transform.position.z)] != null)
        {
            if (instantiatedMap[Mathf.RoundToInt(passedTile.transform.position.x + 1), Mathf.RoundToInt(passedTile.transform.position.z)].tag == "StartPoint" || instantiatedMap[Mathf.RoundToInt(passedTile.transform.position.x + 1), Mathf.RoundToInt(passedTile.transform.position.z)].tag == "Road Tile" || instantiatedMap[Mathf.RoundToInt(passedTile.transform.position.x + 1), Mathf.RoundToInt(passedTile.transform.position.z)].tag == "EndPoint")
            {
                Road_Type_Script tempRoadScriptHolder = instantiatedMap[Mathf.RoundToInt(passedTile.transform.position.x + 1), Mathf.RoundToInt(passedTile.transform.position.z)].GetComponent<Road_Type_Script>();
                if (tempRoadScriptHolder.thisHasRoad == true)
                {
                    possibleEast = tempRoadScriptHolder.exitWest;
                    eastHasRoad = tempRoadScriptHolder.haveRoad;
                }
                else
                {
                    possibleEast = true;
                }
            }
            if (instantiatedMap[Mathf.RoundToInt(passedTile.transform.position.x + 1), Mathf.RoundToInt(passedTile.transform.position.z)].tag == "ParkPiece" || instantiatedMap[Mathf.RoundToInt(passedTile.transform.position.x + 1), Mathf.RoundToInt(passedTile.transform.position.z)].tag == "Boundary")
            {
                possibleEast = false;
            }
        }

        if (instantiatedMap[Mathf.RoundToInt(passedTile.transform.position.x - 1), Mathf.RoundToInt(passedTile.transform.position.z)] != null)
        {
            if (instantiatedMap[Mathf.RoundToInt(passedTile.transform.position.x - 1), Mathf.RoundToInt(passedTile.transform.position.z)].tag == "StartPoint" || instantiatedMap[Mathf.RoundToInt(passedTile.transform.position.x - 1), Mathf.RoundToInt(passedTile.transform.position.z)].tag == "Road Tile" || instantiatedMap[Mathf.RoundToInt(passedTile.transform.position.x - 1), Mathf.RoundToInt(passedTile.transform.position.z)].tag == "EndPoint")
            {
                Road_Type_Script tempRoadScriptHolder = instantiatedMap[Mathf.RoundToInt(passedTile.transform.position.x - 1), Mathf.RoundToInt(passedTile.transform.position.z)].GetComponent<Road_Type_Script>();
                if (tempRoadScriptHolder.thisHasRoad == true)
                {
                    possibleWest = tempRoadScriptHolder.exitEast;
                    westHasRoad = tempRoadScriptHolder.haveRoad;
                }
                else
                {
                    possibleWest = true;
                }
            }
            if (instantiatedMap[Mathf.RoundToInt(passedTile.transform.position.x - 1), Mathf.RoundToInt(passedTile.transform.position.z)].tag == "ParkPiece" || instantiatedMap[Mathf.RoundToInt(passedTile.transform.position.x - 1), Mathf.RoundToInt(passedTile.transform.position.z)].tag == "Boundary")
            {
                possibleWest = false;
            }
        }
        if (roadScriptHolder != null)
        {
            if (roadScriptHolder.haveRoad == false)
            {
                float animationCurveRandom;
                animationCurveRandom = probabilityCurve.Evaluate(Random.value);

                if (possibleNorth == true && possibleSouth == true && possibleEast == false && possibleWest == false)
                {
                    selectedRoadPiece = Instantiate(roadPieces[0], passedTile.transform.position, Quaternion.identity);
                    selectedRoadPiece.transform.SetParent(passedTile.transform);
                    roadScriptHolder.haveRoad = true;
                    roadScriptHolder.exitSouth = true;
                    roadScriptHolder.exitEast = false;
                    roadScriptHolder.exitNorth = true;
                    roadScriptHolder.exitWest = false;
                }
                if (possibleNorth == true && possibleEast == true && possibleSouth == false && possibleWest == false)
                {
                    selectedRoadPiece = Instantiate(roadPieces[2], passedTile.transform.position, Quaternion.identity);
                    selectedRoadPiece.transform.SetParent(passedTile.transform);
                    roadScriptHolder.haveRoad = true;
                    roadScriptHolder.exitSouth = false;
                    roadScriptHolder.exitEast = true;
                    roadScriptHolder.exitNorth = true;
                    roadScriptHolder.exitWest = false;
                }
                if (possibleNorth == true && possibleWest == true && possibleSouth == false && possibleEast == false)
                {

                    selectedRoadPiece = Instantiate(roadPieces[2], passedTile.transform.position, Quaternion.Euler(0, 270, 0));
                    selectedRoadPiece.transform.SetParent(passedTile.transform);
                    roadScriptHolder.haveRoad = true;
                    roadScriptHolder.exitSouth = false;
                    roadScriptHolder.exitEast = false;
                    roadScriptHolder.exitNorth = true;
                    roadScriptHolder.exitWest = true;
                }
                if (possibleSouth == true && possibleEast == true && possibleNorth == false && possibleWest == false)
                {
                    selectedRoadPiece = Instantiate(roadPieces[2], passedTile.transform.position, Quaternion.Euler(0, 90, 0));
                    selectedRoadPiece.transform.SetParent(passedTile.transform);
                    roadScriptHolder.haveRoad = true;
                    roadScriptHolder.exitSouth = true;
                    roadScriptHolder.exitEast = true;
                    roadScriptHolder.exitNorth = false;
                    roadScriptHolder.exitWest = false;
                }
                if (possibleSouth == true && possibleWest == true && possibleNorth == false && possibleEast == false)
                {
                    selectedRoadPiece = Instantiate(roadPieces[2], passedTile.transform.position, Quaternion.Euler(0, 180, 0));
                    selectedRoadPiece.transform.SetParent(passedTile.transform);
                    roadScriptHolder.haveRoad = true;
                    roadScriptHolder.exitSouth = true;
                    roadScriptHolder.exitEast = false;
                    roadScriptHolder.exitNorth = false;
                    roadScriptHolder.exitWest = true;
                }
                if (possibleEast == true && possibleWest == true && possibleSouth == false && possibleNorth == false)
                {
                    selectedRoadPiece = Instantiate(roadPieces[0], passedTile.transform.position, Quaternion.Euler(0, 90, 0));
                    selectedRoadPiece.transform.SetParent(passedTile.transform);
                    roadScriptHolder.haveRoad = true;
                    roadScriptHolder.exitSouth = false;
                    roadScriptHolder.exitEast = true;
                    roadScriptHolder.exitNorth = false;
                    roadScriptHolder.exitWest = true;
                }
                if (possibleEast == true && possibleWest == true && possibleSouth == true && possibleNorth == false)
                {
                    selectedRoadPiece = Instantiate(roadPieces[1], passedTile.transform.position, Quaternion.Euler(0, 90, 0));
                    selectedRoadPiece.transform.SetParent(passedTile.transform);
                    roadScriptHolder.haveRoad = true;
                    roadScriptHolder.exitSouth = true;
                    roadScriptHolder.exitEast = true;
                    roadScriptHolder.exitNorth = false;
                    roadScriptHolder.exitWest = true;
                }
                if (possibleEast == true && possibleWest == true && possibleSouth == false && possibleNorth == true)
                {
                    selectedRoadPiece = Instantiate(roadPieces[1], passedTile.transform.position, Quaternion.Euler(0, 270, 0));
                    selectedRoadPiece.transform.SetParent(passedTile.transform);
                    roadScriptHolder.haveRoad = true;
                    roadScriptHolder.exitSouth = false;
                    roadScriptHolder.exitEast = true;
                    roadScriptHolder.exitNorth = true;
                    roadScriptHolder.exitWest = true;
                }
                if (possibleEast == false && possibleWest == true && possibleSouth == true && possibleNorth == true)
                {
                    selectedRoadPiece = Instantiate(roadPieces[1], passedTile.transform.position, Quaternion.Euler(0, 180, 0));
                    selectedRoadPiece.transform.SetParent(passedTile.transform);
                    roadScriptHolder.haveRoad = true;
                    roadScriptHolder.exitSouth = true;
                    roadScriptHolder.exitEast = false;
                    roadScriptHolder.exitNorth = true;
                    roadScriptHolder.exitWest = true;
                }
                if (possibleEast == true && possibleWest == false && possibleSouth == true && possibleNorth == true)
                {
                    selectedRoadPiece = Instantiate(roadPieces[1], passedTile.transform.position, Quaternion.identity);
                    selectedRoadPiece.transform.SetParent(passedTile.transform);
                    roadScriptHolder.haveRoad = true;
                    roadScriptHolder.exitSouth = true;
                    roadScriptHolder.exitEast = true;
                    roadScriptHolder.exitNorth = true;
                    roadScriptHolder.exitWest = false;
                }
                if (possibleNorth == true && possibleSouth == true && possibleEast == true && possibleWest == true && roadChosen == false)
                {

                    if (animationCurveRandom < 0.1)
                    {
                        selectedRoadPiece = Instantiate(roadPieces[3], passedTile.transform.position, Quaternion.identity);
                        selectedRoadPiece.transform.SetParent(passedTile.transform);
                        roadScriptHolder.haveRoad = true;
                        roadScriptHolder.exitSouth = true;
                        roadScriptHolder.exitEast = true;
                        roadScriptHolder.exitNorth = true;
                        roadScriptHolder.exitWest = true;
                    }
                    if (animationCurveRandom > 0.31 && animationCurveRandom < 0.6 && possibleEast == false && possibleWest == false)
                    {
                        selectedRoadPiece = Instantiate(roadPieces[0], passedTile.transform.position, Quaternion.identity);
                        selectedRoadPiece.transform.SetParent(passedTile.transform);
                        roadScriptHolder.haveRoad = true;
                        roadScriptHolder.exitSouth = true;
                        roadScriptHolder.exitEast = false;
                        roadScriptHolder.exitNorth = true;
                        roadScriptHolder.exitWest = false;
                    }
                    if (roadChosen == false)
                    {
                        selectedRoadPiece = Instantiate(roadPieces[3], passedTile.transform.position, Quaternion.identity);
                        selectedRoadPiece.transform.SetParent(passedTile.transform);
                        roadScriptHolder.haveRoad = true;
                        roadScriptHolder.exitSouth = true;
                        roadScriptHolder.exitEast = true;
                        roadScriptHolder.exitNorth = true;
                        roadScriptHolder.exitWest = true;
                    }
                }
                if (possibleNorth == true && possibleSouth == false && possibleEast == false && possibleWest == false && roadChosen == false)
                {
                    selectedRoadPiece = Instantiate(roadPieces[4], passedTile.transform.position, Quaternion.identity);
                    selectedRoadPiece.transform.SetParent(passedTile.transform);
                    roadScriptHolder.haveRoad = true;
                    roadScriptHolder.exitSouth = false;
                    roadScriptHolder.exitEast = false;
                    roadScriptHolder.exitNorth = true;
                    roadScriptHolder.exitWest = false;
                }
                if (possibleNorth == false && possibleSouth == true && possibleEast == false && possibleWest == false && roadChosen == false)
                {
                    selectedRoadPiece = Instantiate(roadPieces[4], passedTile.transform.position, Quaternion.Euler(0, 180, 0));
                    selectedRoadPiece.transform.SetParent(passedTile.transform);
                    roadScriptHolder.haveRoad = true;
                    roadScriptHolder.exitSouth = true;
                    roadScriptHolder.exitEast = false;
                    roadScriptHolder.exitNorth = false;
                    roadScriptHolder.exitWest = false;
                }
                if (possibleNorth == false && possibleSouth == false && possibleEast == true && possibleWest == false && roadChosen == false)
                {
                    selectedRoadPiece = Instantiate(roadPieces[4], passedTile.transform.position, Quaternion.Euler(0, 90, 0));
                    selectedRoadPiece.transform.SetParent(passedTile.transform);
                    roadScriptHolder.haveRoad = true;
                    roadScriptHolder.exitSouth = false;
                    roadScriptHolder.exitEast = true;
                    roadScriptHolder.exitNorth = false;
                    roadScriptHolder.exitWest = false;
                }
                if (possibleNorth == false && possibleSouth == false && possibleEast == false && possibleWest == true && roadChosen == false)
                {
                    selectedRoadPiece = Instantiate(roadPieces[4], passedTile.transform.position, Quaternion.Euler(0, 270, 0));
                    selectedRoadPiece.transform.SetParent(passedTile.transform);
                    roadScriptHolder.haveRoad = true;
                    roadScriptHolder.exitSouth = false;
                    roadScriptHolder.exitEast = false;
                    roadScriptHolder.exitNorth = false;
                    roadScriptHolder.exitWest = true;
                }
                roadChosen = true;
            }
        }
    }

    public void MenuSelect(int choice)
    {
        switch(choice)
        {
            case 1:
                SceneManager.LoadScene("GameScene");
                break;
            case 2:
                break;
            case 3:
                break;
            default:
                break;
        }
    }
}
