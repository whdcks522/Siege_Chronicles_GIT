using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartManager : MonoBehaviour
{
    public GameObject StartUi;

    public void StartGame() 
    {
        StartUi.SetActive(false);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

}
