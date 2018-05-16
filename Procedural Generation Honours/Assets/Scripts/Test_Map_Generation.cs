using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Test_Map_Generation : MonoBehaviour
{

    public GameObject startPointPreFab, exitPointPreFab, getawayVehiclePrefab;
    public GameObject[] buildingBlocks;
    public InputField seedInputField;
    public Canvas carCanvas, endCanvas;
    public Text seedNameHolder;
    private GameObject startPoint, currentTile, exitPoint, getAwayVehicle, seedHolder;
    private Vector3 preExitTile;
    private List<GameObject> gridPath;
    private GameObject[,] instantiatedMap;
    private GameObject obstacleSpawner;
    public GameObject[] obstacles;
    public GameObject[] roadPieces;
    private Camera gameCamera, carCamera, minimapCamera;
    public GameObject exitIndicator;
    private List<GameObject> escapeRoute;
    public int gridWidth, gridHeight = 0;
    public AnimationCurve probabilityCurve;
    [SerializeField]
    int seed;
    float topSpeed, currentSpeed, turnSpeed, reverseSpeed;
    int tileCount = 0;
    float escapeDistance;
    bool exitFound;
    bool escaped;
    bool escapeSceneSet = false;
    bool cameraFollowCar = false;
    int findCornerHeight = 0;
    int findCornerWidth = 0;

    // Use this for initialization
    void Start()
    {
        seedHolder = GameObject.Find("SeedHolder");
        
        if(seedHolder != null)
        {
            if(seedHolder.GetComponent<SeedScript>().SeededPlay)
            {
                seed = seedHolder.GetComponent<SeedScript>().SeedNumber;
            }
            else
            {
                seed = Random.Range(-10000, 10000);
                seedHolder.GetComponent<SeedScript>().SeedNumber = seed;
            }
            seedHolder.GetComponent<SeedScript>().SeededPlay = false;
        }

        //seed = 2074; //for Testing purposes 
        Random.InitState(seed);
        topSpeed = 0.02f;
        reverseSpeed = -0.01f;
        turnSpeed = 2;

        BuildGrid();
        BuildMap();

        //seed = Mathf.RoundToInt(Random.seed);
        seedNameHolder.text = "Thank you for playing to replay this map use Seed number:" + seed;


    }

    // Update is called once per frame
    void Update()
    { 
        escapeDistance = Vector3.Distance( exitPoint.transform.position, getAwayVehicle.transform.position);
        //print(escapeDistance);
        if(escapeDistance < 0.2f)
        {
            escaped = true;
            print("escaped");
        }
        if (escaped == true && escapeSceneSet == false)
        {
            print("Camera Switch");

            carCamera.enabled = false;
            minimapCamera.enabled = false;
            gameCamera.enabled = true;
            carCanvas.enabled = false;
            endCanvas.enabled = true;
            escapeSceneSet = true;
        }

        if(Input.GetKeyDown(KeyCode.G))
        {
            Obstacle();
        }

    }

    void BuildGrid()
    {
        if (gridWidth == 0)
        {
            gridWidth = Mathf.RoundToInt(Random.Range(25.0f, 50.0f));
            while (gridWidth % 2 == 0)
            {
                gridWidth = Mathf.RoundToInt(Random.Range(25.0f, 50.0f));
            }
        }
        if (gridHeight == 0)
        {
            gridHeight = Mathf.RoundToInt(Random.Range(25.0f, 50.0f));
            while (gridHeight % 2 == 0)
            {
                gridHeight = Mathf.RoundToInt(Random.Range(25.0f, 50.0f));
            }
        }

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
        Vector3 testPos = new Vector3();
        List<GameObject> goAtPos = gridPath.Where(x => x.transform.position == testPos).ToList();
    }

    void BuildMap()
    {
        exitFound = false;
        //Instantiate(prefabObjects[0], new Vector3(0, 0, 0), Quaternion.identity);
        instantiatedMap = new GameObject[gridWidth + 1, gridHeight + 1];
        escapeRoute = new List<GameObject>();
        //Generate start point near inner circle, for testing only use 0,0
        //int randomStart = Mathf.RoundToInt(Random.Range(-1.0f, 1.0f));

        startPoint = Instantiate(startPointPreFab, new Vector3(Mathf.RoundToInt(gridWidth / 2), 0, Mathf.RoundToInt(gridHeight / 2)), Quaternion.identity);

        Test_Road_Builder testRoadScript = startPoint.GetComponent<Test_Road_Builder>();
        testRoadScript.exitPointNorth = true;
        testRoadScript.exitPointSouth = true;
        testRoadScript.exitPointEast = true;
        testRoadScript.exitPointWest = true;
        testRoadScript.thisHasRoad = true;

        instantiatedMap[Mathf.RoundToInt(startPoint.transform.position.x), Mathf.RoundToInt(startPoint.transform.position.z)] = startPoint;

        //startPoint = Instantiate(startPointPreFab, new Vector3(0, 0, 0), Quaternion.identity);
        getAwayVehicle = Instantiate(getawayVehiclePrefab, new Vector3(startPoint.transform.position.x + 0.077f, startPoint.transform.position.y + 0.078f, startPoint.transform.position.z + 0.346f), Quaternion.identity);
        
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

        for (int x = 0; x < instantiatedMap.Length; x++)
        {

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
                    TileDecision(gridPath[x].transform.position);
                    tileCount++;
                }
            }
        }

        for (int x = 1; x < gridWidth; x++)
        {
            for (int z = 1; z < gridHeight; z++)
            {
                if (instantiatedMap[x, z] != null)
                {
                        StartCoroutine(Waiting(instantiatedMap[x, z]));
                }
            }
        }

        //StartCoroutine(Waiting(instantiatedMap[7, 3]));
        //decideRoad(instantiatedMap[7, 4]);

        for (int eachGridPoint = 0; eachGridPoint < gridPath.Count; eachGridPoint++)
        {
            Destroy(gridPath[eachGridPoint], 0.0f);
        }

        foreach (Transform search in getAwayVehicle.transform)
        {
            if(search.tag == "MinimapCamera")
            {
                minimapCamera = search.GetComponent<Camera>();
                minimapCamera.enabled = true;
            }
        }
        foreach (Transform holder in exitPoint.transform)
        {
            if (holder.tag == "ExitCameraHolder")
            {
                foreach (Transform camSearch in holder.transform)
                {

                    if(camSearch.tag == "ExitCamera")
                    {
                        gameCamera = camSearch.GetComponent<Camera>();
                        print("camera found");
                        gameCamera.enabled = false;
                    }
                }
            }
        }
                  
        carCanvas.enabled = true;
        endCanvas.enabled = false;

        escaped = false;
        escapeSceneSet = false;
    }

    void ChooseExitPoint()
    {
        //Choose top, bottom, left or right for exit location by randomly generating a number between 1 and 4,
        // let 1 represent Top of Grid, 2 Bottom, 3 Left and 4 Right
        int randomNumber = Mathf.RoundToInt(Random.Range(1.0f, 4.0f));
        Test_Road_Builder testRoadScript = null;
        // int randomNumber = 5; //For Testing

        switch (randomNumber)
        {
            case 1:
                exitPoint = Instantiate(exitPointPreFab, new Vector3(findCornerWidth + Random.Range(1, gridWidth - 1), 0, gridHeight), Quaternion.Euler(0.0f, 90.0f, 0.0f));
                preExitTile = new Vector3(exitPoint.transform.position.x, exitPoint.transform.position.y, exitPoint.transform.position.z - 1);
                testRoadScript = exitPoint.GetComponent<Test_Road_Builder>();
                testRoadScript.exitPointNorth = false;
                testRoadScript.exitPointSouth = true;
                testRoadScript.exitPointEast = false;
                testRoadScript.exitPointWest = false;
                testRoadScript.thisHasRoad = true;
                break;
            case 2:
                exitPoint = Instantiate(exitPointPreFab, new Vector3(findCornerWidth + Random.Range(1, gridWidth - 1), 0, findCornerHeight), Quaternion.Euler(0.0f, 270.0f, 0.0f));
                preExitTile = new Vector3(exitPoint.transform.position.x, exitPoint.transform.position.y, exitPoint.transform.position.z + 1);
                testRoadScript = exitPoint.GetComponent<Test_Road_Builder>();
                testRoadScript.exitPointNorth = true;
                testRoadScript.exitPointSouth = false;
                testRoadScript.exitPointEast = false;
                testRoadScript.exitPointWest = false;
                testRoadScript.thisHasRoad = true;
                break;
            case 3:
                exitPoint = Instantiate(exitPointPreFab, new Vector3(findCornerWidth, 0, findCornerHeight + Random.Range(1, gridHeight - 1)), Quaternion.identity);
                preExitTile = new Vector3(exitPoint.transform.position.x + 1, exitPoint.transform.position.y, exitPoint.transform.position.z);
                testRoadScript = exitPoint.GetComponent<Test_Road_Builder>();
                testRoadScript.exitPointNorth = false;
                testRoadScript.exitPointSouth = false;
                testRoadScript.exitPointEast = true;
                testRoadScript.exitPointWest = false;
                testRoadScript.thisHasRoad = true;
                break;
            case 4:
                exitPoint = Instantiate(exitPointPreFab, new Vector3(gridWidth, 0, findCornerHeight + Random.Range(1, gridHeight - 1)), Quaternion.Euler(0.0f, 180.0f, 0.0f));
                preExitTile = new Vector3(exitPoint.transform.position.x - 1, exitPoint.transform.position.y, exitPoint.transform.position.z);
                testRoadScript = exitPoint.GetComponent<Test_Road_Builder>();
                testRoadScript.exitPointNorth = false;
                testRoadScript.exitPointSouth = false;
                testRoadScript.exitPointEast = false;
                testRoadScript.exitPointWest = true;
                testRoadScript.thisHasRoad = true;
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

    public void Generate(int chosenSeed)
    {
        if (chosenSeed == 0)
        {
            if (seedHolder != null)
            {
                if (seedHolder.GetComponent<SeedScript>().SeededPlay)
                {
                    seed = seedHolder.GetComponent<SeedScript>().SeedNumber;
                }
                else
                {
                    seed = Random.Range(-10000, 10000);
                    seedHolder.GetComponent<SeedScript>().SeedNumber = seed;
                }
                seedHolder.GetComponent<SeedScript>().SeededPlay = false;
            }
            Random.InitState(seed);
            tileCount = 0;
            StopAllCoroutines();
            seedNameHolder.text = "Thank you for playing to replay this map use Seed number: " + seed;

            for (int x = 0; x <= gridWidth; x++)
            {
                for (int y = 0; y <= gridHeight; y++)
                {
                    Destroy(instantiatedMap[x, y], 0.0f);
                }
            }
            for (int x = 0; x < gridPath.Count; x++)
            {
                Destroy(gridPath[x], 0.0f);
            }
            for (int x = 0; x < escapeRoute.Count; x++)
            {
                Destroy(escapeRoute[x], 0.0f);
            }
            Destroy(startPoint, 0.0f);
            Destroy(exitPoint, 0.0f);
            Destroy(getAwayVehicle, 0.0f);
            gridWidth = 0;
            gridHeight = 0;
            findCornerHeight = 0;
            findCornerWidth = 0;            
            BuildGrid();
            BuildMap();
        }
        else
        {
            long seed = System.Convert.ToInt64(seedInputField.text);

            if (seedHolder != null)
            {
                seedHolder.GetComponent<SeedScript>().SeededPlay = true;

                if (seedHolder.GetComponent<SeedScript>().SeededPlay)
                {                    
                    seedHolder.GetComponent<SeedScript>().SeedNumber = int.Parse(seedInputField.text);
                    seed = seedHolder.GetComponent<SeedScript>().SeedNumber;
                }

                seedHolder.GetComponent<SeedScript>().SeededPlay = false;
            }
            Random.InitState((int)seed);
            tileCount = 0;
            StopAllCoroutines();
            for (int x = 0; x <= gridWidth; x++)
            {
                for (int y = 0; y <= gridHeight; y++)
                {
                    Destroy(instantiatedMap[x, y], 0.0f);
                }
            }
            for (int x = 0; x < gridPath.Count; x++)
            {
                Destroy(gridPath[x], 0.0f);
            }
            for (int x = 0; x < escapeRoute.Count; x++)
            {
                Destroy(escapeRoute[x], 0.0f);
            }
            Destroy(startPoint, 0.0f);
            Destroy(exitPoint, 0.0f);
            Destroy(getAwayVehicle, 0.0f);
            gridWidth = 0;
            gridHeight = 0;
            findCornerHeight = 0;
            findCornerWidth = 0;
            BuildGrid();
            BuildMap();

            print(seed);
            seedNameHolder.text = "Thank you for playing to replay this map use Seed number: " + seed;
            seedInputField.text = "";
        }
    }

    void TileDecision(Vector3 tileLocation)
    {
        string chosenType = "";
        bool canBePark = true;
        Test_Road_Builder roadScriptHolder = null;
        if (instantiatedMap[Mathf.RoundToInt(tileLocation.x), Mathf.RoundToInt(tileLocation.z + 1)] != null)
        {
            if (instantiatedMap[Mathf.RoundToInt(tileLocation.x), Mathf.RoundToInt(tileLocation.z + 1)].tag == "EscapePath" || instantiatedMap[Mathf.RoundToInt(tileLocation.x), Mathf.RoundToInt(tileLocation.z + 1)].tag == "StartPoint")
            {
                canBePark = false;
            }
            //if (instantiatedMap[Mathf.RoundToInt(tileLocation.x), Mathf.RoundToInt(tileLocation.z + 1)].tag == "Road Tile")
            //{
            //    roadScriptHolder = instantiatedMap[Mathf.RoundToInt(tileLocation.x), Mathf.RoundToInt(tileLocation.z + 1)].GetComponent<Test_Road_Builder>();
            //    if (roadScriptHolder.haveRoad == true)
            //    {
            //        canBePark = false;
            //    }
            //}
        }
        if (instantiatedMap[Mathf.RoundToInt(tileLocation.x), Mathf.RoundToInt(tileLocation.z - 1)] != null)
        {
            if (instantiatedMap[Mathf.RoundToInt(tileLocation.x), Mathf.RoundToInt(tileLocation.z - 1)].tag == "EscapePath" || instantiatedMap[Mathf.RoundToInt(tileLocation.x), Mathf.RoundToInt(tileLocation.z - 1)].tag == "StartPoint")
            {
                canBePark = false;
            }
            //if (instantiatedMap[Mathf.RoundToInt(tileLocation.x), Mathf.RoundToInt(tileLocation.z - 1)].tag == "Road Tile")
            //{
            //    roadScriptHolder = instantiatedMap[Mathf.RoundToInt(tileLocation.x), Mathf.RoundToInt(tileLocation.z - 1)].GetComponent<Test_Road_Builder>();
            //    if (roadScriptHolder.haveRoad == true)
            //    {
            //        canBePark = false;
            //    }
            //}
        }
        if (instantiatedMap[Mathf.RoundToInt(tileLocation.x + 1), Mathf.RoundToInt(tileLocation.z)] != null)
        {
            if (instantiatedMap[Mathf.RoundToInt(tileLocation.x + 1), Mathf.RoundToInt(tileLocation.z)].tag == "EscapePath" || instantiatedMap[Mathf.RoundToInt(tileLocation.x + 1), Mathf.RoundToInt(tileLocation.z)].tag == "StartPoint")
            {
                canBePark = false;
            }
            //if (instantiatedMap[Mathf.RoundToInt(tileLocation.x + 1), Mathf.RoundToInt(tileLocation.z)].tag == "Road Tile")
            //{
            //    roadScriptHolder = instantiatedMap[Mathf.RoundToInt(tileLocation.x + 1), Mathf.RoundToInt(tileLocation.z)].GetComponent<Test_Road_Builder>();
            //    if (roadScriptHolder.haveRoad == true)
            //    {
            //        canBePark = false;
            //    }
            //}
        }
        if (instantiatedMap[Mathf.RoundToInt(tileLocation.x - 1), Mathf.RoundToInt(tileLocation.z)] != null)
        {
            if (instantiatedMap[Mathf.RoundToInt(tileLocation.x - 1), Mathf.RoundToInt(tileLocation.z)].tag == "EscapePath" || instantiatedMap[Mathf.RoundToInt(tileLocation.x - 1), Mathf.RoundToInt(tileLocation.z)].tag == "StartPoint")
            {
                canBePark = false;
            }
            //if (instantiatedMap[Mathf.RoundToInt(tileLocation.x - 1), Mathf.RoundToInt(tileLocation.z)].tag == "Road Tile")
            //{
            //    roadScriptHolder = instantiatedMap[Mathf.RoundToInt(tileLocation.x - 1), Mathf.RoundToInt(tileLocation.z)].GetComponent<Test_Road_Builder>();
            //    if (roadScriptHolder.haveRoad == true)
            //    {
            //        canBePark = false;
            //    }
            //}
        }

        float animationCurveTest;
        animationCurveTest = probabilityCurve.Evaluate(Random.value);

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

    IEnumerator Waiting(GameObject passThrough)
    {
        yield return new WaitForSeconds(0.2f);
        DecideRoad(passThrough);
    }

    void DecideRoad(GameObject passedTile)
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
        
        Test_Road_Builder testRoadScript = null;
        Escape_Road_Script escapeRoadScriptHolder = null;
        GameObject selectedRoadPiece;
        if (passedTile.GetComponent<Test_Road_Builder>() != null)
        {
            testRoadScript = passedTile.GetComponent<Test_Road_Builder>();
        }
        if (passedTile.GetComponent<Escape_Road_Script>() != null)
        {
            escapeRoadScriptHolder = passedTile.GetComponent<Escape_Road_Script>();
        }

        if (instantiatedMap[Mathf.RoundToInt(passedTile.transform.position.x), Mathf.RoundToInt(passedTile.transform.position.z + 1)] != null)
        {
            if (instantiatedMap[Mathf.RoundToInt(passedTile.transform.position.x), Mathf.RoundToInt(passedTile.transform.position.z + 1)].tag == "EscapePath")
            {
                Escape_Road_Script tempEscapeRoadScriptHolder = instantiatedMap[Mathf.RoundToInt(passedTile.transform.position.x), Mathf.RoundToInt(passedTile.transform.position.z + 1)].GetComponent<Escape_Road_Script>();
                if (tempEscapeRoadScriptHolder.thisHasRoad == true)
                {
                    possibleNorth = tempEscapeRoadScriptHolder.exitSouth;
                    northHasRoad = tempEscapeRoadScriptHolder.haveRoad;
                }
                else
                {
                    possibleNorth = true;
                }
            }
            if (instantiatedMap[Mathf.RoundToInt(passedTile.transform.position.x), Mathf.RoundToInt(passedTile.transform.position.z + 1)].tag == "StartPoint" || instantiatedMap[Mathf.RoundToInt(passedTile.transform.position.x), Mathf.RoundToInt(passedTile.transform.position.z + 1)].tag == "Road Tile" || instantiatedMap[Mathf.RoundToInt(passedTile.transform.position.x), Mathf.RoundToInt(passedTile.transform.position.z + 1)].tag == "EndPoint")
            {
                Test_Road_Builder tempTestRoadScriptHolder = instantiatedMap[Mathf.RoundToInt(passedTile.transform.position.x), Mathf.RoundToInt(passedTile.transform.position.z + 1)].GetComponent<Test_Road_Builder>();
                if (tempTestRoadScriptHolder.thisHasRoad == true)
                {
                    possibleNorth = tempTestRoadScriptHolder.exitSouth;
                    northHasRoad = tempTestRoadScriptHolder.haveRoad;
                }
                else
                {
                    possibleNorth = true;
                }
            }
        }
        if (instantiatedMap[Mathf.RoundToInt(passedTile.transform.position.x), Mathf.RoundToInt(passedTile.transform.position.z - 1)] != null)
        {

            if (instantiatedMap[Mathf.RoundToInt(passedTile.transform.position.x), Mathf.RoundToInt(passedTile.transform.position.z - 1)].tag == "EndPoint")
            {
                print("I Found The End");
            }
            if (instantiatedMap[Mathf.RoundToInt(passedTile.transform.position.x), Mathf.RoundToInt(passedTile.transform.position.z - 1)].tag == "EscapePath")
            {
                Escape_Road_Script tempEscapeRoadScriptHolder = instantiatedMap[Mathf.RoundToInt(passedTile.transform.position.x), Mathf.RoundToInt(passedTile.transform.position.z - 1)].GetComponent<Escape_Road_Script>();
                if (tempEscapeRoadScriptHolder.thisHasRoad == true)
                {
                    possibleSouth = tempEscapeRoadScriptHolder.exitNorth;
                    southHasRoad = tempEscapeRoadScriptHolder.haveRoad;
                }
                else
                {
                    possibleSouth = true;
                }
            }
            if (instantiatedMap[Mathf.RoundToInt(passedTile.transform.position.x), Mathf.RoundToInt(passedTile.transform.position.z - 1)].tag == "StartPoint" || instantiatedMap[Mathf.RoundToInt(passedTile.transform.position.x), Mathf.RoundToInt(passedTile.transform.position.z - 1)].tag == "Road Tile" || instantiatedMap[Mathf.RoundToInt(passedTile.transform.position.x), Mathf.RoundToInt(passedTile.transform.position.z - 1)].tag == "EndPoint")
            {
                Test_Road_Builder tempTestRoadScriptHolder = instantiatedMap[Mathf.RoundToInt(passedTile.transform.position.x), Mathf.RoundToInt(passedTile.transform.position.z - 1)].GetComponent<Test_Road_Builder>();
                if (tempTestRoadScriptHolder.thisHasRoad == true)
                {
                    possibleSouth = tempTestRoadScriptHolder.exitNorth;
                    southHasRoad = tempTestRoadScriptHolder.haveRoad;
                }
                else
                {
                    possibleSouth = true;
                }
            }
        }
        if (instantiatedMap[Mathf.RoundToInt(passedTile.transform.position.x + 1), Mathf.RoundToInt(passedTile.transform.position.z)] != null)
        {
            if (instantiatedMap[Mathf.RoundToInt(passedTile.transform.position.x + 1), Mathf.RoundToInt(passedTile.transform.position.z)].tag == "EscapePath")
            {
                Escape_Road_Script tempEscapeRoadScriptHolder = instantiatedMap[Mathf.RoundToInt(passedTile.transform.position.x + 1), Mathf.RoundToInt(passedTile.transform.position.z)].GetComponent<Escape_Road_Script>();
                if (tempEscapeRoadScriptHolder.thisHasRoad)
                {
                    possibleEast = tempEscapeRoadScriptHolder.exitWest;
                    eastHasRoad = tempEscapeRoadScriptHolder.haveRoad;
                }
                else
                {
                    possibleEast = true;
                }
            }
            if (instantiatedMap[Mathf.RoundToInt(passedTile.transform.position.x + 1), Mathf.RoundToInt(passedTile.transform.position.z)].tag == "StartPoint" || instantiatedMap[Mathf.RoundToInt(passedTile.transform.position.x + 1), Mathf.RoundToInt(passedTile.transform.position.z)].tag == "Road Tile" || instantiatedMap[Mathf.RoundToInt(passedTile.transform.position.x + 1), Mathf.RoundToInt(passedTile.transform.position.z)].tag == "EndPoint")
            {
                Test_Road_Builder tempTestRoadScriptHolder = instantiatedMap[Mathf.RoundToInt(passedTile.transform.position.x + 1), Mathf.RoundToInt(passedTile.transform.position.z)].GetComponent<Test_Road_Builder>();
                if (tempTestRoadScriptHolder.thisHasRoad == true)
                {
                    possibleEast = tempTestRoadScriptHolder.exitWest;
                    eastHasRoad = tempTestRoadScriptHolder.haveRoad;
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
            if (instantiatedMap[Mathf.RoundToInt(passedTile.transform.position.x - 1), Mathf.RoundToInt(passedTile.transform.position.z)].tag == "EscapePath")
            {
                Escape_Road_Script tempEscapeRoadScriptHolder = instantiatedMap[Mathf.RoundToInt(passedTile.transform.position.x - 1), Mathf.RoundToInt(passedTile.transform.position.z)].GetComponent<Escape_Road_Script>();
                if (tempEscapeRoadScriptHolder.thisHasRoad == true)
                {
                    possibleWest = tempEscapeRoadScriptHolder.exitEast;
                    westHasRoad = tempEscapeRoadScriptHolder.haveRoad;
                }
                else
                {
                    possibleWest = true;
                }
            }
            if (instantiatedMap[Mathf.RoundToInt(passedTile.transform.position.x - 1), Mathf.RoundToInt(passedTile.transform.position.z)].tag == "StartPoint" || instantiatedMap[Mathf.RoundToInt(passedTile.transform.position.x - 1), Mathf.RoundToInt(passedTile.transform.position.z)].tag == "Road Tile" || instantiatedMap[Mathf.RoundToInt(passedTile.transform.position.x - 1), Mathf.RoundToInt(passedTile.transform.position.z)].tag == "EndPoint")
            {
                Test_Road_Builder tempTestRoadScriptHolder = instantiatedMap[Mathf.RoundToInt(passedTile.transform.position.x - 1), Mathf.RoundToInt(passedTile.transform.position.z)].GetComponent<Test_Road_Builder>();
                if (tempTestRoadScriptHolder.thisHasRoad == true)
                {
                    possibleWest = tempTestRoadScriptHolder.exitEast;
                    westHasRoad = tempTestRoadScriptHolder.haveRoad;
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
        if (testRoadScript != null)
        {
            if (testRoadScript.haveRoad == false)
            {
                float animationCurveRandom;
                animationCurveRandom = probabilityCurve.Evaluate(Random.value);

                if (possibleNorth == true && possibleSouth == true && possibleEast == false && possibleWest == false)
                {
                    selectedRoadPiece = Instantiate(roadPieces[0], passedTile.transform.position, Quaternion.identity);
                    selectedRoadPiece.transform.SetParent(passedTile.transform);
                    testRoadScript.haveRoad = true;
                    roadChosen = true;
                    testRoadScript.exitSouth = true;
                    testRoadScript.exitEast = false;
                    testRoadScript.exitNorth = true;
                    testRoadScript.exitWest = false;
                }
                if (possibleNorth == true && possibleEast == true && possibleSouth == false && possibleWest == false)
                {
                    selectedRoadPiece = Instantiate(roadPieces[2], passedTile.transform.position, Quaternion.identity);
                    selectedRoadPiece.transform.SetParent(passedTile.transform);
                    testRoadScript.haveRoad = true;
                    roadChosen = true;
                    testRoadScript.exitSouth = false;
                    testRoadScript.exitEast = true;
                    testRoadScript.exitNorth = true;
                    testRoadScript.exitWest = false;
                }
                if (possibleNorth == true && possibleWest == true && possibleSouth == false && possibleEast == false)
                {

                    selectedRoadPiece = Instantiate(roadPieces[2], passedTile.transform.position, Quaternion.Euler(0, 270, 0));
                    selectedRoadPiece.transform.SetParent(passedTile.transform);
                    testRoadScript.haveRoad = true;
                    roadChosen = true;
                    testRoadScript.exitSouth = false;
                    testRoadScript.exitEast = false;
                    testRoadScript.exitNorth = true;
                    testRoadScript.exitWest = true;
                }
                if (possibleSouth == true && possibleEast == true && possibleNorth == false && possibleWest == false)
                {
                    selectedRoadPiece = Instantiate(roadPieces[2], passedTile.transform.position, Quaternion.Euler(0, 90, 0));
                    selectedRoadPiece.transform.SetParent(passedTile.transform);
                    testRoadScript.haveRoad = true;
                    roadChosen = true;
                    testRoadScript.exitSouth = true;
                    testRoadScript.exitEast = true;
                    testRoadScript.exitNorth = false;
                    testRoadScript.exitWest = false;
                }
                if (possibleSouth == true && possibleWest == true && possibleNorth == false && possibleEast == false)
                {
                    selectedRoadPiece = Instantiate(roadPieces[2], passedTile.transform.position, Quaternion.Euler(0, 180, 0));
                    selectedRoadPiece.transform.SetParent(passedTile.transform);
                    testRoadScript.haveRoad = true;
                    roadChosen = true;
                    testRoadScript.exitSouth = true;
                    testRoadScript.exitEast = false;
                    testRoadScript.exitNorth = false;
                    testRoadScript.exitWest = true;
                }
                if (possibleEast == true && possibleWest == true && possibleSouth == false && possibleNorth == false)
                {
                    selectedRoadPiece = Instantiate(roadPieces[0], passedTile.transform.position, Quaternion.Euler(0, 90, 0));
                    selectedRoadPiece.transform.SetParent(passedTile.transform);
                    testRoadScript.haveRoad = true;
                    roadChosen = true;
                    testRoadScript.exitSouth = false;
                    testRoadScript.exitEast = true;
                    testRoadScript.exitNorth = false;
                    testRoadScript.exitWest = true;
                }
                if (possibleEast == true && possibleWest == true && possibleSouth == true && possibleNorth == false)
                {
                    selectedRoadPiece = Instantiate(roadPieces[1], passedTile.transform.position, Quaternion.Euler(0, 90, 0));
                    selectedRoadPiece.transform.SetParent(passedTile.transform);
                    testRoadScript.haveRoad = true;
                    roadChosen = true;
                    testRoadScript.exitSouth = true;
                    testRoadScript.exitEast = true;
                    testRoadScript.exitNorth = false;
                    testRoadScript.exitWest = true;
                }
                if (possibleEast == true && possibleWest == true && possibleSouth == false && possibleNorth == true)
                {
                    selectedRoadPiece = Instantiate(roadPieces[1], passedTile.transform.position, Quaternion.Euler(0, 270, 0));
                    selectedRoadPiece.transform.SetParent(passedTile.transform);
                    testRoadScript.haveRoad = true;
                    roadChosen = true;
                    testRoadScript.exitSouth = false;
                    testRoadScript.exitEast = true;
                    testRoadScript.exitNorth = true;
                    testRoadScript.exitWest = true;
                }
                if (possibleEast == false && possibleWest == true && possibleSouth == true && possibleNorth == true)
                {
                    selectedRoadPiece = Instantiate(roadPieces[1], passedTile.transform.position, Quaternion.Euler(0, 180, 0));
                    selectedRoadPiece.transform.SetParent(passedTile.transform);
                    testRoadScript.haveRoad = true;
                    roadChosen = true;
                    testRoadScript.exitSouth = true;
                    testRoadScript.exitEast = false;
                    testRoadScript.exitNorth = true;
                    testRoadScript.exitWest = true;
                }
                if (possibleEast == true && possibleWest == false && possibleSouth == true && possibleNorth == true)
                {
                    selectedRoadPiece = Instantiate(roadPieces[1], passedTile.transform.position, Quaternion.identity);
                    selectedRoadPiece.transform.SetParent(passedTile.transform);
                    testRoadScript.haveRoad = true;
                    roadChosen = true;
                    testRoadScript.exitSouth = true;
                    testRoadScript.exitEast = true;
                    testRoadScript.exitNorth = true;
                    testRoadScript.exitWest = false;
                }
                if (possibleNorth == true && possibleSouth == true && possibleEast == true && possibleWest == true && roadChosen == false)
                {

                    if (animationCurveRandom < 0.1)
                    {
                        selectedRoadPiece = Instantiate(roadPieces[3], passedTile.transform.position, Quaternion.identity);
                        selectedRoadPiece.transform.SetParent(passedTile.transform);
                        testRoadScript.haveRoad = true;
                        roadChosen = true;
                        testRoadScript.exitSouth = true;
                        testRoadScript.exitEast = true;
                        testRoadScript.exitNorth = true;
                        testRoadScript.exitWest = true;
                    }
                    if (animationCurveRandom > 0.31 && animationCurveRandom < 0.6 && possibleEast == false && possibleWest == false)
                    {
                        selectedRoadPiece = Instantiate(roadPieces[0], passedTile.transform.position, Quaternion.identity);
                        selectedRoadPiece.transform.SetParent(passedTile.transform);
                        testRoadScript.haveRoad = true;
                        roadChosen = true;
                        testRoadScript.exitSouth = true;
                        testRoadScript.exitEast = false;
                        testRoadScript.exitNorth = true;
                        testRoadScript.exitWest = false;
                    }
                    if (roadChosen == false)
                    {
                        selectedRoadPiece = Instantiate(roadPieces[3], passedTile.transform.position, Quaternion.identity);
                        selectedRoadPiece.transform.SetParent(passedTile.transform);
                        testRoadScript.haveRoad = true;
                        roadChosen = true;
                        testRoadScript.exitSouth = true;
                        testRoadScript.exitEast = true;
                        testRoadScript.exitNorth = true;
                        testRoadScript.exitWest = true;
                    }
                }
                if (possibleNorth == true && possibleSouth == false && possibleEast == false && possibleWest == false && roadChosen == false)
                {
                    selectedRoadPiece = Instantiate(roadPieces[4], passedTile.transform.position, Quaternion.identity);
                    selectedRoadPiece.transform.SetParent(passedTile.transform);
                    testRoadScript.haveRoad = true;
                    roadChosen = true;
                    testRoadScript.exitSouth = false;
                    testRoadScript.exitEast = false;
                    testRoadScript.exitNorth = true;
                    testRoadScript.exitWest = false;
                }
                if (possibleNorth == false && possibleSouth == true && possibleEast == false && possibleWest == false && roadChosen == false)
                {
                    selectedRoadPiece = Instantiate(roadPieces[4], passedTile.transform.position, Quaternion.Euler(0, 180, 0));
                    selectedRoadPiece.transform.SetParent(passedTile.transform);
                    testRoadScript.haveRoad = true;
                    roadChosen = true;
                    testRoadScript.exitSouth = true;
                    testRoadScript.exitEast = false;
                    testRoadScript.exitNorth = false;
                    testRoadScript.exitWest = false;
                }
                if (possibleNorth == false && possibleSouth == false && possibleEast == true && possibleWest == false && roadChosen == false)
                {
                    selectedRoadPiece = Instantiate(roadPieces[4], passedTile.transform.position, Quaternion.Euler(0, 90, 0));
                    selectedRoadPiece.transform.SetParent(passedTile.transform);
                    testRoadScript.haveRoad = true;
                    roadChosen = true;
                    testRoadScript.exitSouth = false;
                    testRoadScript.exitEast = true;
                    testRoadScript.exitNorth = false;
                    testRoadScript.exitWest = false;
                }
                if (possibleNorth == false && possibleSouth == false && possibleEast == false && possibleWest == true && roadChosen == false)
                {
                    selectedRoadPiece = Instantiate(roadPieces[4], passedTile.transform.position, Quaternion.Euler(0, 270, 0));
                    selectedRoadPiece.transform.SetParent(passedTile.transform);
                    testRoadScript.haveRoad = true;
                    roadChosen = true;
                    testRoadScript.exitSouth = false;
                    testRoadScript.exitEast = false;
                    testRoadScript.exitNorth = false;
                    testRoadScript.exitWest = true;
                }
            }
        }
    }

    void Obstacle()
    {
       GameObject spawnedObstacle = Instantiate(obstacles[0], obstacleSpawner.transform.position, Quaternion.identity);        
    }
}

