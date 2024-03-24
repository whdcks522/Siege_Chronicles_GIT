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

        //해상도 조정(x길이, y길이, 전체화면 여부), 남은 부분은 자동으로 검은색 처리
        //Screen.SetResolution(1920, 1080, false);
    }
    
    public void StartGame() 
    {
        //시작 창 닫기
        gameObject.SetActive(false);

        //선택 창 열기
        UiManager.selectManager.gameObject.SetActive(true);
    }

    public void ExitGame()=> Application.Quit();


    [Serializable]//필요하더라
    public class PanelPageInfo
    {
        public Sprite pageSprite;
        [TextArea]
        public string pageStr;
    }
    [Header("패널 디스플레이의 정보 배열")]
    public PanelPageInfo[] panelPageInfoArray;

    [Header("팁 패널")]
    public GameObject tipPanel;

    [Header("팁 패널의 디스플레이")]
    public Image tipPanelDisplay;
    [Header("팁 패널의 텍스트")]
    public Text tipPanelText;

    [Header("팁 패널의 페이지 텍스트")]
    public Text tipPanelPageText;

    [Header("팁 패널의 페이지 왼쪽 버튼")]
    public Button tipPanelPageLeftBtn;

    [Header("팁 패널의 페이지 오른쪽 버튼")]
    public Button tipPanelPageRightBtn;

    public void PanelActivateControl(bool isActivate) //패널 활성, 비활성 관리
    {
        //패널 활성 비활성
        tipPanel.SetActive(isActivate);

        if (isActivate) //패널을 여는 경우
        {
            //패널 상태를 초기화
            PanelPageControl(0);
        }
    }

    int curPageindex = 0;
    public void PanelPageControl(int pageIndex) //패널 페이지 컨트롤
    {
        //0이면 초기 화면, -1이면 왼쪽, +1이면 오른쪽
        if (pageIndex == 0) //0으로 초기화
            curPageindex = 0;
        else if (pageIndex == -1)//왼쪽으로 넘기기
            curPageindex -= 1;
        else if (pageIndex == 1)//오른쪽으로 넘기기
            curPageindex += 1;

        //오류 조정
        tipPanelPageLeftBtn.interactable = true;
        tipPanelPageRightBtn.interactable = true;
        if (curPageindex <= 0)
        {
            curPageindex = 0;
            tipPanelPageLeftBtn.interactable = false;
        }
        else if (curPageindex >= panelPageInfoArray.Length - 1)
        {
            curPageindex = panelPageInfoArray.Length - 1;
            tipPanelPageRightBtn.interactable = false;
        }

        //해당 페이지 정보 받아오기
        tipPanelDisplay.sprite = panelPageInfoArray[curPageindex].pageSprite;
        tipPanelText.text = panelPageInfoArray[curPageindex].pageStr;

        tipPanelPageText.text = (curPageindex + 1) + "/" + panelPageInfoArray.Length;
    }
}
