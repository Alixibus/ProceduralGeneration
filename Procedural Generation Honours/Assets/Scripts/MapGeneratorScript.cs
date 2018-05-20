using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class MapGeneratorScript : MonoBehaviour {

    //variable decleration of all public variables required for algorithm to function
    public GameObject startPointPreFab, exitPointPreFab, getawayVehiclePrefab;
    public GameObject[] buildingBlocks;
    public InputField seedInputField;
    public Canvas carCanvas, endCanvas;
    public Text seedNameHolder;
    public int gridWidth, gridHeight = 0;
    public AnimationCurve probabilityCurve;
    public GameObject exitIndicator;
    public GameObject[] obstacles;
    public GameObject[] roadPieces;
    public Sprite[] vehicleOutlines;
    public Button vehicleSelectorButton;
    public Text vehicleDescription;
    public GameObject[] theGetawayVehicles;

    //Variable decleration for all private variables required for algorithm
    private GameObject startPoint, currentTile, exitPoint, getawayVehicle, seedHolder;
    private Vector3 preExitTile;
    private List<GameObject> gridPath;
    private GameObject[,] instantiatedMap;
    private GameObject obstacleSpawner;
    private Camera gameCamera, carCamera, minimapCamera;
    private List<GameObject> escapeRoute;


    [SerializeField]
    int seed;
    int counter = 0;
    float escapeDistance;
    bool exitFound;
    bool escaped;
    bool escapeSceneSet = false;
    bool cameraFollowCar = false;
    int findCornerHeight = 0;
    int findCornerWidth = 0;
    int vehicleCount = 1;

    // Use this for initialization
    void Start()
    {     
        //fin the seed gameobject and store here
        seedHolder = GameObject.Find("SeedHolder");

        //start obstacles co routine, this could be improved in future iterations
        StartCoroutine(Obstacles());

        //check if the seedHolder has an object inside, if so and seededPlay returns true
        //take this seeded number stored and assign it as the seed of the level
        // if not randomly generate a seed between -10000 and 10000, this is the best method
        //for procedural generation to function
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
            getawayVehiclePrefab = seedHolder.GetComponent<SeedScript>().GetawayVehicle;
            vehicleSelectorButton.image.sprite = seedHolder.GetComponent<SeedScript>().chosenVehicleOutline;
            vehicleDescription.text = seedHolder.GetComponent<SeedScript>().chosenVehicleDescription;
        }
        else
        {
            seed = Random.Range(-10000, 10000);
        }

        //set the car camera
        carCamera = Camera.main;
        
        //IMPORTANT: InitState is the method of setting the Random starting number, due to
        //psuedo randomisation this garuntee's the procedural generation
        Random.InitState(seed);
        
        //Kickoff the BuildGrid method and BuildMap method
        BuildGrid();
        BuildMap();

        //seed = Mathf.RoundToInt(Random.seed);
        seedNameHolder.text = "" + seed;
    }

    // Update is called once per frame
    void Update()
    {
        //assign the distance of vehicle from exit point
        escapeDistance = Vector3.Distance(exitPoint.transform.position, getawayVehicle.transform.position);
        
        //if vehicle is close to exit, the exit has been reached, so begin end screen
        // could be improved in future iterations
        if (escapeDistance < 0.2f)
        {
            escaped = true;
        }
        if (escaped == true && escapeSceneSet == false)
        {
            carCamera.enabled = false;
            minimapCamera.enabled = false;
            gameCamera.enabled = true;
            carCanvas.enabled = false;
            endCanvas.enabled = true;
            escapeSceneSet = true;
        }

        //activate the "Pause" menu
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if( endCanvas != null)
            {
                if (endCanvas.enabled != true)
                {
                    endCanvas.enabled = true;
                }
                else if(endCanvas.enabled == true)
                {
                    endCanvas.enabled = false;
                }
            }
        }
    }

    void BuildGrid()
    {
        // ensure all obstacles are destroyed
        Block[] destroyBlocks = FindObjectsOfType<Block>();
        foreach(Block block in destroyBlocks)
        {
            Destroy(block.gameObject);
        }

        //build the grid width, ensuring an odd width
        if (gridWidth == 0)
        {
            gridWidth = Mathf.RoundToInt(Random.Range(25.0f, 50.0f));
            while (gridWidth % 2 == 0)
            {
                gridWidth = Mathf.RoundToInt(Random.Range(25.0f, 40.0f));
            }
        }
        //generate grid height ensuring odd height
        if (gridHeight == 0)
        {
            gridHeight = Mathf.RoundToInt(Random.Range(25.0f, 40.0f));
            while (gridHeight % 2 == 0)
            {
                gridHeight = Mathf.RoundToInt(Random.Range(25.0f, 50.0f));
            }
        }

        //set the corner of the grid
        findCornerHeight = 0;
        findCornerWidth = 0;

        //find total grid points
        int totalGridPoints = gridWidth * gridHeight;
        gridPath = new List<GameObject>();
        
        //create the grid
        int gridCount = 0;
        for (int makeWidth = 0; makeWidth <= gridWidth; makeWidth++)
        {
            for (int makeHeight = 0; makeHeight <= gridHeight; makeHeight++)
            {
                gridPath.Add(Instantiate(buildingBlocks[0], new Vector3(findCornerWidth + makeWidth, 0, findCornerHeight + makeHeight), Quaternion.identity));
                gridCount++;
            }
        }
    }

    void BuildMap()
    {
        exitFound = false;
        instantiatedMap = new GameObject[gridWidth + 1, gridHeight + 1];
        escapeRoute = new List<GameObject>();
        
        //Instantiate the start point in the centre of the grid
        startPoint = Instantiate(startPointPreFab, new Vector3(Mathf.RoundToInt(gridWidth / 2), 0, Mathf.RoundToInt(gridHeight / 2)), Quaternion.identity);

        //set the start point road exits
        Road_Type_Script roadScript = startPoint.GetComponent<Road_Type_Script>();
        roadScript.exitPointNorth = true;
        roadScript.exitPointSouth = true;
        roadScript.exitPointEast = true;
        roadScript.exitPointWest = true;
        roadScript.thisHasRoad = true;

        //add to the map array
        instantiatedMap[Mathf.RoundToInt(startPoint.transform.position.x), Mathf.RoundToInt(startPoint.transform.position.z)] = startPoint;
        
        //Instantiate chosen getaway vehicle
        getawayVehicle = Instantiate(getawayVehiclePrefab, new Vector3(startPoint.transform.position.x + 0.077f, startPoint.transform.position.y + 0.078f, startPoint.transform.position.z + 0.346f), Quaternion.identity);
               

        //Generate exit Route on Outer
        ChooseExitPoint();
        instantiatedMap[Mathf.RoundToInt(exitPoint.transform.position.x), Mathf.RoundToInt(exitPoint.transform.position.z)] = exitPoint;
        currentTile = startPoint;

        //Loop through tiles beginning with start, locate closest tile to exit point, assign this as a step towards exit
        //continue till exit found
        while (exitFound == false)
        {
            GameObject tempObjectHolder = Instantiate(buildingBlocks[1], ChooseNextTile(currentTile.transform.position, preExitTile), Quaternion.identity);
            instantiatedMap[Mathf.RoundToInt(tempObjectHolder.transform.position.x), Mathf.RoundToInt(tempObjectHolder.transform.position.z)] = tempObjectHolder;
            currentTile = tempObjectHolder;
        }

        //for the rest of the grid points not already assigned, decide what type of tile
        //to generate, taking into consideration whether tile is on the edge or deciding
        // between park and road tile
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
                }
            }
        }

        //Loop through all tiles generated, waiting for a set timeframe, then deciding road type
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

        //destroy grid after generation
        for (int eachGridPoint = 0; eachGridPoint < gridPath.Count; eachGridPoint++)
        {
            Destroy(gridPath[eachGridPoint], 0.0f);
        }

        //enabled the minimap camera of instantiated getaway vehicle
        foreach (Transform search in getawayVehicle.transform)
        {
            if (search.tag == "MinimapCamera")
            {
                minimapCamera = search.GetComponent<Camera>();
                minimapCamera.enabled = true;
            }
        }

        //Set exit camera
        foreach (Transform holder in exitPoint.transform)
        {
            if (holder.tag == "ExitCameraHolder")
            {
                foreach (Transform camSearch in holder.transform)
                {

                    if (camSearch.tag == "ExitCamera")
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

        StartCoroutine(DoubleCheck());
    }

    void ChooseExitPoint()
    {
        //Choose top, bottom, left or right for exit location by randomly generating a number between 1 and 4,
        // let 1 represent Top of Grid, 2 Bottom, 3 Left and 4 Right
        int randomNumber = Mathf.RoundToInt(Random.Range(1.0f, 4.0f));
        Road_Type_Script roadScript = null;
        // int randomNumber = 5; //For Testing

        switch (randomNumber)
        {
            case 1:
                exitPoint = Instantiate(exitPointPreFab, new Vector3(findCornerWidth + Random.Range(1, gridWidth - 1), 0, gridHeight), Quaternion.Euler(0.0f, 90.0f, 0.0f));
                preExitTile = new Vector3(exitPoint.transform.position.x, exitPoint.transform.position.y, exitPoint.transform.position.z - 1);
                roadScript = exitPoint.GetComponent<Road_Type_Script>();
                roadScript.exitPointNorth = false;
                roadScript.exitPointSouth = true;
                roadScript.exitPointEast = false;
                roadScript.exitPointWest = false;
                roadScript.thisHasRoad = true;
                break;
            case 2:
                exitPoint = Instantiate(exitPointPreFab, new Vector3(findCornerWidth + Random.Range(1, gridWidth - 1), 0, findCornerHeight), Quaternion.Euler(0.0f, 270.0f, 0.0f));
                preExitTile = new Vector3(exitPoint.transform.position.x, exitPoint.transform.position.y, exitPoint.transform.position.z + 1);
                roadScript = exitPoint.GetComponent<Road_Type_Script>();
                roadScript.exitPointNorth = true;
                roadScript.exitPointSouth = false;
                roadScript.exitPointEast = false;
                roadScript.exitPointWest = false;
                roadScript.thisHasRoad = true;
                break;
            case 3:
                exitPoint = Instantiate(exitPointPreFab, new Vector3(findCornerWidth, 0, findCornerHeight + Random.Range(1, gridHeight - 1)), Quaternion.identity);
                preExitTile = new Vector3(exitPoint.transform.position.x + 1, exitPoint.transform.position.y, exitPoint.transform.position.z);
                roadScript = exitPoint.GetComponent<Road_Type_Script>();
                roadScript.exitPointNorth = false;
                roadScript.exitPointSouth = false;
                roadScript.exitPointEast = true;
                roadScript.exitPointWest = false;
                roadScript.thisHasRoad = true;
                break;
            case 4:
                exitPoint = Instantiate(exitPointPreFab, new Vector3(gridWidth, 0, findCornerHeight + Random.Range(1, gridHeight - 1)), Quaternion.Euler(0.0f, 180.0f, 0.0f));
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
        //here a vector is passed in, the north, south, east and west vecotors surrounding this are
        //checked and which ever is closest to the exit point, becomes the chosen tile for escape path instantiation
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
        //Generate is used, to destroy and redo all aspects of the algorithm, this can be used for restarting the same seed, starting a new seed
        // or choosing a known seed
        if (chosenSeed == 0)//Start a new level, seed number to be randomised
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
                getawayVehiclePrefab = seedHolder.GetComponent<SeedScript>().GetawayVehicle;
            }
            else
            {
                seed = Random.Range(-10000, 10000);
            }
            Random.InitState(seed);
            StopAllCoroutines();
            seedNameHolder.text = "" + seed;

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
            Destroy(getawayVehicle, 0.0f);
            gridWidth = 0;
            gridHeight = 0;
            findCornerHeight = 0;
            findCornerWidth = 0;
            BuildGrid();
            BuildMap();
        }
        else if(chosenSeed == 1)//Choose specific see to play
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
            print("Seed = " + seed);
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
            Destroy(getawayVehicle, 0.0f);
            gridWidth = 0;
            gridHeight = 0;
            findCornerHeight = 0;
            findCornerWidth = 0;
            BuildGrid();
            BuildMap();

            print(seed);
            seedNameHolder.text = "" + seed;
            seedInputField.text = "";
        }
        else if (chosenSeed == 2)//Restart using current seed
        {
            if (seedHolder != null)
            {
                seedHolder.GetComponent<SeedScript>().SeedNumber = seed;
                
                seedHolder.GetComponent<SeedScript>().SeededPlay = false;
                getawayVehiclePrefab = seedHolder.GetComponent<SeedScript>().GetawayVehicle;
            }
            Random.InitState((int)seed);
            print("Seed = " + seed);
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
            Destroy(getawayVehicle, 0.0f);
            gridWidth = 0;
            gridHeight = 0;
            findCornerHeight = 0;
            findCornerWidth = 0;
            BuildGrid();
            BuildMap();

            print(seed);
            seedNameHolder.text = "" + seed;
            seedInputField.text = "";
        }
        carCamera.enabled = true;
        minimapCamera.enabled = true;
        gameCamera.enabled = false;
        carCanvas.enabled = true;
        endCanvas.enabled = false;
        escapeSceneSet = false;
    }

    void TileDecision(Vector3 tileLocation)
    {
        //Before deciding whether the tile can be Park or Road
        //we need to check the neighbours to ensure the escape 
        //tiles and the start tile can have road
        bool canBePark = true;
        if (instantiatedMap[Mathf.RoundToInt(tileLocation.x), Mathf.RoundToInt(tileLocation.z + 1)] != null)
        {
            if (instantiatedMap[Mathf.RoundToInt(tileLocation.x), Mathf.RoundToInt(tileLocation.z + 1)].tag == "EscapePath" || instantiatedMap[Mathf.RoundToInt(tileLocation.x), Mathf.RoundToInt(tileLocation.z + 1)].tag == "StartPoint")
            {
                canBePark = false;
            }
        }
        if (instantiatedMap[Mathf.RoundToInt(tileLocation.x), Mathf.RoundToInt(tileLocation.z - 1)] != null)
        {
            if (instantiatedMap[Mathf.RoundToInt(tileLocation.x), Mathf.RoundToInt(tileLocation.z - 1)].tag == "EscapePath" || instantiatedMap[Mathf.RoundToInt(tileLocation.x), Mathf.RoundToInt(tileLocation.z - 1)].tag == "StartPoint")
            {
                canBePark = false;
            }
        }
        if (instantiatedMap[Mathf.RoundToInt(tileLocation.x + 1), Mathf.RoundToInt(tileLocation.z)] != null)
        {
            if (instantiatedMap[Mathf.RoundToInt(tileLocation.x + 1), Mathf.RoundToInt(tileLocation.z)].tag == "EscapePath" || instantiatedMap[Mathf.RoundToInt(tileLocation.x + 1), Mathf.RoundToInt(tileLocation.z)].tag == "StartPoint")
            {
                canBePark = false;
            }
        }
        if (instantiatedMap[Mathf.RoundToInt(tileLocation.x - 1), Mathf.RoundToInt(tileLocation.z)] != null)
        {
            if (instantiatedMap[Mathf.RoundToInt(tileLocation.x - 1), Mathf.RoundToInt(tileLocation.z)].tag == "EscapePath" || instantiatedMap[Mathf.RoundToInt(tileLocation.x - 1), Mathf.RoundToInt(tileLocation.z)].tag == "StartPoint")
            {
                canBePark = false;
            }
        }

        //check a randomised value against the animation curve and decide outcome
        float animationCurveTest;
        animationCurveTest = probabilityCurve.Evaluate(Random.value);

        if (animationCurveTest < 0.3 && canBePark == true)
        {
            instantiatedMap[Mathf.RoundToInt(tileLocation.x), Mathf.RoundToInt(tileLocation.z)] = Instantiate(buildingBlocks[3], tileLocation + new Vector3(0, 0.01524f, 0), Quaternion.identity);
        }
        else
        {
            GameObject roadTileHolder = null;
            roadTileHolder = instantiatedMap[Mathf.RoundToInt(tileLocation.x), Mathf.RoundToInt(tileLocation.z)] = Instantiate(buildingBlocks[2], tileLocation, Quaternion.identity);
        }
    }

    IEnumerator DoubleCheck()
    {
        //call to check to ensure there are no doubleings of tiles on top of each other
        
        yield return new WaitForSeconds(0.5f);
        for (int x = 1; x < gridWidth; x++)
        {
            for (int z = 1; z < gridHeight; z++)
            {
                if (instantiatedMap[x, z] != null)
                {
                    int counter = 0;
                    foreach (Transform child in instantiatedMap[x, z].transform)
                    {
                        counter++;
                        if (counter > 1 && child.tag != "ParkPiece")
                        {                            
                            Destroy(child.gameObject);
                        }
                    }
                }
            }
        }
    }

    IEnumerator Waiting(GameObject passThrough)
    {
        yield return new WaitForSeconds(0.2f);
        RoadBuilder(passThrough);
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
            //if (instantiatedMap[Mathf.RoundToInt(passedTile.transform.position.x + 1), Mathf.RoundToInt(passedTile.transform.position.z)].tag == "ParkPiece" || instantiatedMap[Mathf.RoundToInt(passedTile.transform.position.x + 1), Mathf.RoundToInt(passedTile.transform.position.z)].tag == "Boundary")
            //{
            //    possibleEast = false;
            //}
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
            //if (instantiatedMap[Mathf.RoundToInt(passedTile.transform.position.x - 1), Mathf.RoundToInt(passedTile.transform.position.z)].tag == "ParkPiece" || instantiatedMap[Mathf.RoundToInt(passedTile.transform.position.x - 1), Mathf.RoundToInt(passedTile.transform.position.z)].tag == "Boundary")
            //{
            //    possibleWest = false;
            //}
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
    public void VehicleSelection()
    {
        vehicleCount++;
        if (vehicleCount == 4)
        {
            vehicleCount = 1;
        }
        if (vehicleCount == 1)
        {
            //getawayVehiclePrefab = theGetawayVehicles[0];
            if (seedHolder != null)
            {
                seedHolder.GetComponent<SeedScript>().GetawayVehicle = theGetawayVehicles[0];
            }
            vehicleSelectorButton.image.sprite = vehicleOutlines[0];
            vehicleDescription.text = "-1967 Chevrolet Corvette Stingray";
            print("Corvette Selected");
        }
        if (vehicleCount == 2)
        {
            //getawayVehiclePrefab = theGetawayVehicles[1];
            if (seedHolder != null)
            {
                seedHolder.GetComponent<SeedScript>().GetawayVehicle = theGetawayVehicles[1];
            }
            vehicleSelectorButton.image.sprite = vehicleOutlines[1];
            vehicleDescription.text = "-1967 Shelby Mustang GT500";
            print("Mustang Selected");
        }
        if (vehicleCount == 3)
        {
            //getawayVehiclePrefab = theGetawayVehicles[2];
            if (seedHolder != null)
            {
                seedHolder.GetComponent<SeedScript>().GetawayVehicle = theGetawayVehicles[2];
            }
            vehicleSelectorButton.image.sprite = vehicleOutlines[2];
            vehicleDescription.text = "-1970 Dodge Charger";
            print("Charger Selected");
        }
    }

    IEnumerator Obstacles()
    {        
        int firstRun = 2;
        if(firstRun == 1)
        {
            yield return new WaitForSeconds(5);
            firstRun++;
        }
        else
        {
            yield return new WaitForSeconds(5);
            float randomValue = Random.Range(0.0f, 100.0f);
            if (randomValue > 0.0f)
            {
                Vector3 vehiclePosition = getawayVehicle.transform.position;
                Vector3 vehicleDirection = -getawayVehicle.transform.forward;
                Quaternion vehicleRotation = getawayVehicle.transform.rotation;
                float offset = 2;

                Vector3 spawnLocation = vehiclePosition + vehicleDirection * offset;
                GameObject spawnedObstacle = Instantiate(obstacles[0], spawnLocation + new Vector3 (0, 0.1f, 0), Quaternion.Euler(-90, 0, 0));
                spawnedObstacle.transform.LookAt(getawayVehicle.transform, -getawayVehicle.transform.right);
                //spawn the obstacle
                print("Spawned an Obstacle");
            }
            else
            {
                print("Not this time Sonny");
            }
        }
        StartCoroutine(Obstacles());
    }
}

