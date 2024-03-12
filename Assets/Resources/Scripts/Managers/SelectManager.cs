using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectManager : MonoBehaviour
{
    public GameManager gameManager;

    UIManager UiManager;
    AudioManager audioManager;

    void Awake()
    {
        UiManager = gameManager.uiManager;
        audioManager = gameManager.audioManager;
    }


    public void StartGame()//게임 시작
    {
        gameObject.SetActive(false);
    }

    public void aa() 
    {
    
    }
}
