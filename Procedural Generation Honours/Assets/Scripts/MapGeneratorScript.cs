using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGeneratorScript : MonoBehaviour {

    public GameObject[] prefabObjects;
    private GameObject startPoint, exitPoint;
    private GameObject[] gridPath;
    private GameObject[] instantiatedMap;
    public int gridWidth, gridHeight = 0;
    int findCornerHeight = 0;
    int findCornerWidth = 0;

    // Use this for initialization
    void Start () {        
        BuildGrid();
        BuildMap();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(Input.GetMouseButtonDown(0) && gridPath != null)
        {
            for (int eachGridPoint = 0; eachGridPoint < gridPath.Length; eachGridPoint++)
            {
                Destroy(instantiatedMap[eachGridPoint], 0.0f);
            }
            Destroy(startPoint, 0.0f);
            Destroy(exitPoint, 0.0f);
            gridWidth = 0;
            gridHeight = 0;
            findCornerHeight = 0;
            findCornerWidth = 0;
            BuildGrid();
            BuildMap();
        }
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
        gridPath = new GameObject[totalGridPoints];

        int gridCount = 0;
        for (int makeWidth = 0; makeWidth < gridWidth; makeWidth++)
        {
            for (int makeHeight = 0; makeHeight < gridHeight; makeHeight++)
            {
                gridPath[gridCount] = Instantiate(prefabObjects[0], new Vector3(findCornerWidth + makeWidth, 0, findCornerHeight + makeHeight), Quaternion.identity);
                gridCount++;
            }
        }
    }

    void BuildMap()
    {
        //Instantiate(prefabObjects[0], new Vector3(0, 0, 0), Quaternion.identity);       
        instantiatedMap = new GameObject[gridPath.Length];
        //Generate start point near inner circle, for testing only use 0,0
        //int randomStart = Mathf.RoundToInt(Random.Range(-1.0f, 1.0f));
        //startPoint = Instantiate(prefabObjects[1], new Vector3(randomStart, 0, randomStart), Quaternion.identity);
        startPoint = Instantiate(prefabObjects[1], new Vector3(0, 0, 0), Quaternion.identity);

        //Generate exit Route on Outer
        ChooseExitPoint();

        for(int x = 0; x < gridPath.Length; x++)
        {
            if (gridPath[x].transform.position == startPoint.transform.position || gridPath[x].transform.position == exitPoint.transform.position)
            {
                continue;
            }
            else if(gridPath[x].transform.position.z == gridHeight / 2 || gridPath[x].transform.position.z == findCornerHeight || gridPath[x].transform.position.x == gridWidth / 2 || gridPath[x].transform.position.x == findCornerWidth)
            {
                instantiatedMap[x] = Instantiate(prefabObjects[5], gridPath[x].transform.position, Quaternion.identity);
            }
            else
            {
                instantiatedMap[x] = Instantiate(prefabObjects[4], gridPath[x].transform.position, Quaternion.identity);
            }
        }

        for (int eachGridPoint = 0; eachGridPoint < gridPath.Length; eachGridPoint++)
        {
            Destroy(gridPath[eachGridPoint], 0.0f);
        }
    }

    void ChooseExitPoint()
    {
        //Choose top, bottom, left or right for exit location by randomly generating a number between 1 and 4,
        // let 1 represent Top of Grid, 2 Bottom, 3 Left and 4 Right
        int randomNumber = Mathf.RoundToInt(Random.Range(1.0f, 4.0f));

        switch(randomNumber)
        {
            case 1:
                exitPoint = Instantiate(prefabObjects[2], new Vector3(findCornerWidth + Random.Range(1, gridWidth - 1), 0, gridHeight / 2), Quaternion.identity);
                break;
            case 2:
                exitPoint = Instantiate(prefabObjects[2], new Vector3(findCornerWidth + Random.Range(1, gridWidth - 1), 0, findCornerHeight), Quaternion.identity);
                break;
            case 3:
                exitPoint = Instantiate(prefabObjects[2], new Vector3(findCornerWidth, 0, findCornerHeight + Random.Range(1, gridHeight - 1)), Quaternion.identity);                
                break;
            case 4:
                exitPoint = Instantiate(prefabObjects[2], new Vector3(gridWidth / 2, 0, findCornerHeight + Random.Range(1, gridHeight - 1)), Quaternion.identity);
                break;
            default:
                print("No chosen location");
                break;
        }
    }

    void FindExcapePath(Vector3 startPos, Vector3 exitPos)
    {

    }
}



