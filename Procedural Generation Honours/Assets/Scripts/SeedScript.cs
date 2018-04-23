using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;

public class SeedScript : MonoBehaviour {

    public int seedNumber
    {
        get { return seedNumberChosen; }
        set { seedNumberChosen = value; }
    }
    public int seedNumberChosen;

    public bool seededPlay
    {
        get { return seededPlayChosen; }
        set { seededPlayChosen = value; }
    }    public bool seededPlayChosen;

    public void Awake()
    {
        DontDestroyOnLoad(gameObject);
        print(seedNumber);
    }

    public void SetSeedNumber(Text seedNo)
    {
        seedNumber = int.Parse(seedNo.text);
        seededPlay = true;
        print(seedNumber);
        SceneManager.LoadScene("GameScene");
    }
}
