using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartManager : MonoBehaviour
{
    public GameManager gameManager;

    UIManager UiManager;
    AudioManager audioManager;

    void Awake() 
    {
        UiManager = gameManager.uiManager;
        audioManager = gameManager.audioManager;

        //해상도 조정(x길이, y길이, 전체화면 여부), 남은 부분은 자동으로 검은색 처리
        //Screen.SetResolution(1920, 1080, false);
    }
    

    public void StartGame() 
    {
        gameObject.SetActive(false);

        //선택창 열기
        UiManager.SelectObj.SetActive(true);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
