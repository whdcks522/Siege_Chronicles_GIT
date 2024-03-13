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

        //�ػ� ����(x����, y����, ��üȭ�� ����), ���� �κ��� �ڵ����� ������ ó��
        //Screen.SetResolution(1920, 1080, false);
    }
    

    public void StartGame() 
    {
        gameObject.SetActive(false);

        //����â ����
        UiManager.selectManager.gameObject.SetActive(true);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
