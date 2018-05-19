using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;

public class MenuScript : MonoBehaviour
{
    public GameObject optionsHolder, imageHolder;
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

    public void OptionsMenu()
    {
        if (optionsHolder.activeInHierarchy)
        {
            optionsHolder.SetActive(false);
            imageHolder.SetActive(true);
        }
        else
        {
            optionsHolder.SetActive(true);
            imageHolder.SetActive(false);
        }
    }

    public void SwitchActive(GameObject objectSwitching)
    {
        if (objectSwitching.activeInHierarchy)
        {
            objectSwitching.SetActive(false);
        }
        else
        {
            objectSwitching.SetActive(true);
        }
    }
}
