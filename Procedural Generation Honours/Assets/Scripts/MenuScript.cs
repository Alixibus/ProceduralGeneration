using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;

public class MenuScript : MonoBehaviour
{

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
