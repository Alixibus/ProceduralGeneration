using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoNotDestroyScript : MonoBehaviour {

    // Use this for initialization
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void changeVisibility(bool changedValue)
    {
        gameObject.SetActive(changedValue);
    }
}
