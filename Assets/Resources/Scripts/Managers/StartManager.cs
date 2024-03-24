using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        //���� â �ݱ�
        gameObject.SetActive(false);

        //���� â ����
        UiManager.selectManager.gameObject.SetActive(true);
    }

    public void ExitGame()=> Application.Quit();


    [Serializable]//�ʿ��ϴ���
    public class PanelPageInfo
    {
        public Sprite pageSprite;
        [TextArea]
        public string pageStr;
    }
    [Serializable]
    public class PanelPageInfoArray
    {
        public PanelPageInfo[] panelPageInfo;
    }
    [Header("�г� ���÷����� ���� �迭")]
    public PanelPageInfoArray[] panelPageInfoArray;

    [Header("�� �г�")]
    public GameObject tipPanel;

    [Header("�� �г��� ���÷���")]
    public Image tipPanelDisplay;
    [Header("�� �г��� �ؽ�Ʈ")]
    public Text tipPanelText;

    [Header("�� �г��� ������ �ؽ�Ʈ")]
    public Text tipPanelPageText;


    public void PanelActivateControl(bool isActivate) //�г� Ȱ��, ��Ȱ�� ����
    {
        //�г� Ȱ�� ��Ȱ��
        tipPanel.SetActive(isActivate);

        if (isActivate) //�г��� ���� ���
        {
            //�г� ���¸� �ʱ�ȭ
        }
    }

    public void PanelPageControl(int pageIndex) //�г� ������ ��Ʈ��
    {
        //0�̸� �ʱ� ȭ��, -1�̸� ����, +1�̸� ������

        //�ش� ������ ��ġ��

    }
}
