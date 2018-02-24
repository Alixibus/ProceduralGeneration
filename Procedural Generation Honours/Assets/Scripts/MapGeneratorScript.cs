using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapGeneratorScript : MonoBehaviour {

    public GameObject startPointPreFab, exitPointPreFab, getawayVehiclePrefab;
    public GameObject[] escapePathPreFab;
    public InputField seedInputField;
    public Text seedHolder;
    private GameObject startPoint, currentTile, exitPoint, getAwayVehicle;
    private Vector3 preExitTile;
    private List<GameObject> gridPath;
    private GameObject[] instantiatedMap;
    private List<GameObject> escapeRoute;
    public int gridWidth, gridHeight = 0;
    [SerializeField]
    int seed;
    bool exitFound;
    int findCornerHeight = 0;
    int findCornerWidth = 0;

    // Use this for initialization
    void Start () {        
        BuildGrid();
        BuildMap();

        seed = 12345;
        seedHolder.text = "Current Seed = " + seed;
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

        findCornerHeight -= gridHeight / 2;
        findCornerWidth -= gridWidth / 2;
        int totalGridPoints = gridWidth * gridHeight;
        gridPath = new List<GameObject>();

        int gridCount = 0;
        for (int makeWidth = 0; makeWidth < gridWidth; makeWidth++)
        {
            for (int makeHeight = 0; makeHeight < gridHeight; makeHeight++)
            {
                gridPath.Add(Instantiate(escapePathPreFab[0], new Vector3(findCornerWidth + makeWidth, 0, findCornerHeight + makeHeight), Quaternion.identity));
                gridCount++;
            }
        }
    }

    void BuildMap()
    {
        exitFound = false;
        //Instantiate(prefabObjects[0], new Vector3(0, 0, 0), Quaternion.identity);
        instantiatedMap = new GameObject[gridPath.Count];
        escapeRoute = new List<GameObject>();
        //Generate start point near inner circle, for testing only use 0,0
        int randomStart = Mathf.RoundToInt(Random.Range(-1.0f, 1.0f));
        //startPoint = Instantiate(startPointPreFab, new Vector3(randomStart, 0, randomStart), Quaternion.identity);
       
        startPoint = Instantiate(startPointPreFab, new Vector3(0, 0, 0), Quaternion.identity);
        getAwayVehicle = Instantiate(getawayVehiclePrefab, new Vector3(startPoint.transform.position.x + 0.077f, startPoint.transform.position.y + 0.078f, startPoint.transform.position.z + 0.346f), Quaternion.identity);

        //Generate exit Route on Outer
        ChooseExitPoint();
        currentTile = startPoint;

        int tileCount = 0;
        while (exitFound == false)
        {
            escapeRoute.Add(Instantiate(escapePathPreFab[0], ChooseNextTile(currentTile.transform.position, preExitTile), Quaternion.identity));
            currentTile = escapeRoute[tileCount];
            tileCount++;
        }

        for (int x = 0; x < instantiatedMap.Length; x++)
        {
            if (gridPath[x].transform.position == startPoint.transform.position || gridPath[x].transform.position == exitPoint.transform.position)
            {
                continue;
            }
            else if (gridPath[x].transform.position.z == gridHeight / 2 || gridPath[x].transform.position.z == findCornerHeight || gridPath[x].transform.position.x == gridWidth / 2 || gridPath[x].transform.position.x == findCornerWidth)
            {
                instantiatedMap[x] = Instantiate(escapePathPreFab[2], gridPath[x].transform.position, Quaternion.identity);
            }
            else
            {
                instantiatedMap[x] = Instantiate(escapePathPreFab[1], gridPath[x].transform.position, Quaternion.identity);
                
            }
            foreach (GameObject escapeVector in escapeRoute)
            {
                if (gridPath[x].transform.position == escapeVector.transform.position)
                {
                    Destroy(instantiatedMap[x], 0.0f);
                    break;
                }
            }
        }        

        for (int eachGridPoint = 0; eachGridPoint < gridPath.Count; eachGridPoint++)
        {
            Destroy(gridPath[eachGridPoint], 0.0f);
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
                exitPoint = Instantiate(exitPointPreFab, new Vector3(findCornerWidth + Random.Range(1, gridWidth - 1), 0, gridHeight / 2), Quaternion.Euler(0.0f,90.0f,0.0f));
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
                exitPoint = Instantiate(exitPointPreFab, new Vector3(gridWidth / 2, 0, findCornerHeight + Random.Range(1, gridHeight - 1)), Quaternion.Euler(0.0f, 180.0f, 0.0f));
                preExitTile = new Vector3(exitPoint.transform.position.x - 1, exitPoint.transform.position.y, exitPoint.transform.position.z);
                break;
            case 5:
                //For Testing purposes
                exitPoint = Instantiate(exitPointPreFab, new Vector3(gridWidth / 2, 0, findCornerHeight + 1), Quaternion.Euler(0.0f, 180.0f, 0.0f));
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
            for (int eachGridPoint = 0; eachGridPoint < instantiatedMap.Length; eachGridPoint++)
            {
                Destroy(instantiatedMap[eachGridPoint], 0.0f);
                Destroy(gridPath[eachGridPoint], 0.0f);
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

            seed = Mathf.RoundToInt(Random.seed);
            print(seed);
            seedHolder.text = "Current Seed = " + seed;
        }
        else
        {            
            seed = System.Convert.ToInt32(seedInputField.text);
            Random.InitState(seed);

            for (int eachGridPoint = 0; eachGridPoint < instantiatedMap.Length; eachGridPoint++)
            {
                Destroy(instantiatedMap[eachGridPoint], 0.0f);
                Destroy(gridPath[eachGridPoint], 0.0f);
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
        }
    }
}



