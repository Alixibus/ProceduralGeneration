using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class MapGeneratorScript : MonoBehaviour {

    public GameObject startPointPreFab, exitPointPreFab, getawayVehiclePrefab;
    public GameObject[] buildingBlocks;
    public GameObject[] fillerPreFab;
    public InputField seedInputField;
    public Text seedHolder;
    private GameObject startPoint, currentTile, exitPoint, getAwayVehicle;
    private Vector3 preExitTile;
    private List<GameObject> gridPath;
    private GameObject[,] instantiatedMap;
    private List<GameObject> escapeRoute;
    public int gridWidth, gridHeight = 0;
    [SerializeField]
    int seed;
    public float percentageChancePark;
    int numberOfParkTiles;
    bool exitFound;
    int findCornerHeight = 0;
    int findCornerWidth = 0;

    // Use this for initialization
    void Start ()
    {
        //seed = Random.Range(-10000, 10000);
        seed = 2074; //for Testing purposes 
        Random.InitState(seed);

        BuildGrid();
        BuildMap();

        //seed = Mathf.RoundToInt(Random.seed);
        seedHolder.text = "Current Seed = " + seed;
        percentageChancePark = 0.20f;
        numberOfParkTiles = Mathf.RoundToInt(((gridWidth * gridHeight) - (((gridWidth * 2) + (gridHeight * 2)) - 2)) * percentageChancePark);
        print("Number of park Tiles is = " + numberOfParkTiles);
        
    }
	
	// Update is called once per frame
	void Update ()
    {
    }

    void BuildGrid()
    {
        if (gridWidth == 0)
        {
            gridWidth = Mathf.RoundToInt(Random.Range(9.0f, 25.0f));
            while (gridWidth % 2 == 0)
            {
                gridWidth = Mathf.RoundToInt(Random.Range(9.0f, 25.0f));
            }
        }
        if (gridHeight == 0)
        {
            gridHeight = Mathf.RoundToInt(Random.Range(5.0f, 25.0f));
            while (gridHeight % 2 == 0)
            {
                gridHeight = Mathf.RoundToInt(Random.Range(5.0f, 25.0f));
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
        instantiatedMap[Mathf.RoundToInt(startPoint.transform.position.x), Mathf.RoundToInt(startPoint.transform.position.z)] = startPoint;

        //startPoint = Instantiate(startPointPreFab, new Vector3(0, 0, 0), Quaternion.identity);
        getAwayVehicle = Instantiate(getawayVehiclePrefab, new Vector3(startPoint.transform.position.x + 0.077f, startPoint.transform.position.y + 0.078f, startPoint.transform.position.z + 0.346f), Quaternion.identity);

        //Generate exit Route on Outer
        ChooseExitPoint();
        instantiatedMap[Mathf.RoundToInt(exitPoint.transform.position.x), Mathf.RoundToInt(exitPoint.transform.position.z)] = exitPoint;
        currentTile = startPoint;

        int tileCount = 0;
        while (exitFound == false)
        {
            //escapeRoute.Add(Instantiate(buildingBlocks[1], ChooseNextTile(currentTile.transform.position, preExitTile), Quaternion.identity));
            GameObject tempObjectHolder = Instantiate(buildingBlocks[1], ChooseNextTile(currentTile.transform.position, preExitTile), Quaternion.identity);
            instantiatedMap[Mathf.RoundToInt(tempObjectHolder.transform.position.x), Mathf.RoundToInt(tempObjectHolder.transform.position.z)] = tempObjectHolder;
            currentTile = tempObjectHolder;
            tileCount++;
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
                    float randomize = Random.value;
                    if (randomize < 0.1)
                    {
                        instantiatedMap[Mathf.RoundToInt(gridPath[x].transform.position.x), Mathf.RoundToInt(gridPath[x].transform.position.z)] = Instantiate(buildingBlocks[3], gridPath[x].transform.position, Quaternion.identity);
                    }
                    else
                    {
                        instantiatedMap[Mathf.RoundToInt(gridPath[x].transform.position.x), Mathf.RoundToInt(gridPath[x].transform.position.z)] = Instantiate(buildingBlocks[2], gridPath[x].transform.position, Quaternion.identity);
                    }
                    
                }

            }
            /*foreach (GameObject escapeVector in escapeRoute)
            {
                if (gridPath[x].transform.position == escapeVector.transform.position)
                {
                    Destroy(instantiatedMap[Mathf.RoundToInt(gridPath[x].transform.position.x), Mathf.RoundToInt(gridPath[x].transform.position.z)], 0.0f);
                    break;
                }
            }*/
        }        

        for (int eachGridPoint = 0; eachGridPoint < gridPath.Count; eachGridPoint++)
        {
            Destroy(gridPath[eachGridPoint], 0.0f);
        }

        addNeighbours();
        foreach (GameObject chosenTiles in instantiatedMap)
        {
            if (chosenTiles.GetComponent<Test_Road_Builder>() != null)
            {
                //chosenTiles.GetComponent<Test_Road_Builder>().beginAllProcesses();
            }
        }
            
    }

    void ChooseExitPoint()
    {
        //Choose top, bottom, left or right for exit location by randomly generating a number between 1 and 4,
        // let 1 represent Top of Grid, 2 Bottom, 3 Left and 4 Right
        int randomNumber = Mathf.RoundToInt(Random.Range(1.0f, 4.0f));

       // int randomNumber = 5; //For Testing

        switch(randomNumber)
        {
            case 1:
                exitPoint = Instantiate(exitPointPreFab, new Vector3(findCornerWidth + Random.Range(1, gridWidth - 1), 0, gridHeight), Quaternion.Euler(0.0f,90.0f,0.0f));
                preExitTile = new Vector3(exitPoint.transform.position.x, exitPoint.transform.position.y, exitPoint.transform.position.z - 1);
                break;
            case 2:
                exitPoint = Instantiate(exitPointPreFab, new Vector3(findCornerWidth + Random.Range(1, gridWidth - 1), 0, findCornerHeight), Quaternion.Euler(0.0f, 270.0f, 0.0f));
                preExitTile = new Vector3(exitPoint.transform.position.x, exitPoint.transform.position.y, exitPoint.transform.position.z + 1);
                break;
            case 3:
                exitPoint = Instantiate(exitPointPreFab, new Vector3(findCornerWidth, 0, findCornerHeight + Random.Range(1, gridHeight - 1)), Quaternion.identity);
                preExitTile = new Vector3(exitPoint.transform.position.x + 1, exitPoint.transform.position.y, exitPoint.transform.position.z);
                break;
            case 4:
                exitPoint = Instantiate(exitPointPreFab, new Vector3(gridWidth, 0, findCornerHeight + Random.Range(1, gridHeight - 1)), Quaternion.Euler(0.0f, 180.0f, 0.0f));
                preExitTile = new Vector3(exitPoint.transform.position.x - 1, exitPoint.transform.position.y, exitPoint.transform.position.z);
                break;
            case 5:
                //For Testing purposes
                exitPoint = Instantiate(exitPointPreFab, new Vector3(gridWidth , 0, findCornerHeight + 1), Quaternion.Euler(0.0f, 180.0f, 0.0f));
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
        if(chosenTile.x == exitPoint.x)
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
            seed = Random.Range(-10000, 10000);
            Random.InitState(seed);
            seedHolder.text = "Current Seed = " + seed;
            
            for (int x = 0; x <= gridWidth; x++)
            {
                for(int y = 0; y <= gridHeight; y++)
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

            numberOfParkTiles = Mathf.RoundToInt(((gridWidth * gridHeight) - (((gridWidth * 2) + (gridHeight * 2)) - 2)) * percentageChancePark);
            print("Number of park Tiles is = " + numberOfParkTiles);
        }
        else
        {            
            long seed = System.Convert.ToInt64(seedInputField.text);
            Random.InitState((int)seed);

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
            seedHolder.text = "Current Seed = " + seed;
            seedInputField.text = "";

            numberOfParkTiles = Mathf.RoundToInt(((gridWidth * gridHeight) - (((gridWidth * 2) + (gridHeight * 2)) - 2)) * percentageChancePark);
            print("Number of park Tiles is = " + numberOfParkTiles);
        }
        
    }

    void addNeighbours()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int z = 0; z < gridHeight; z++)
            {
                if (instantiatedMap[x, z].transform.position.x != findCornerWidth && instantiatedMap[x, z].transform.position.z != findCornerHeight)
                { 
                    Test_Road_Builder roadScriptHolder = null;
                    if (instantiatedMap[x, z].GetComponent<Test_Road_Builder>() != null)
                    {
                        roadScriptHolder = instantiatedMap[x, z].GetComponent<Test_Road_Builder>();
                    }
                    if (roadScriptHolder != null)
                    {
                        if ((instantiatedMap[x, z].transform.position.z + 1) < gridHeight)
                        {
                            //roadScriptHolder.setNorthNeighbour(instantiatedMap[x, z + 1]);
                        }
                        if ((instantiatedMap[x, z].transform.position.z - 1) > findCornerHeight)
                        {
                            //roadScriptHolder.setSouthNeighbour(instantiatedMap[x, z - 1]);
                        }
                        if ((instantiatedMap[x, z].transform.position.x + 1) < gridWidth)
                        {
                            //roadScriptHolder.setEastNeighbour(instantiatedMap[x + 1, z]);
                        }
                        if ((instantiatedMap[x, z].transform.position.x - 1) > findCornerWidth)
                        {
                            //roadScriptHolder.setWestNeighbour(instantiatedMap[x - 1, z]);
                        }
                    }
                }
            }
        }
    }
}



