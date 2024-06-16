using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartManager : MonoBehaviour
{
    [Header("애니메이션")]
    public Animator startAnim;
    public Animator tipAnim;

    [Header("매니저")]
    public GameManager gameManager;
    AudioManager audioManager;

    private void Start()
    {
        audioManager = gameManager.audioManager;

        //시작 화면 배경 음악 재생
        audioManager.PlayBgm(AudioManager.Bgm.StartBgm);
    }

    public void StartGame()//시작 버튼 누름
    {
        startAnim.SetBool("isStart", false);

        //종이 넘기는 효과음
        audioManager.PlaySfx(AudioManager.Sfx.PaperSfx);
    }


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
        tipAnim.SetBool("isPanel", isActivate);

        if (isActivate) //패널을 여는 경우
        {
            //패널 상태를 초기화
            PanelPageControl(0);
        }
        else if (!isActivate) 
        {
            //종이 넘기는 효과음
            audioManager.PlaySfx(AudioManager.Sfx.PaperSfx);
        }
    }

    //현재 페이지 인덱스
    int curPageindex = 0;
    [Header("팁 패널의 텍스트 애니메이션")]
    public Animator pageAnim;
    public void PanelPageControl(int pageIndex) //패널 페이지 컨트롤
    {
        //종이 넘기는 효과음
        audioManager.PlaySfx(AudioManager.Sfx.PaperSfx);

        //페이지 텍스트 애니메이션
        pageAnim.SetBool("isFlash", true);


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

        //텍스트 색 변화
        tipPanelPageText.color = gameManager.uiManager.textYellow;

        if (curPageindex <= 0)
        {
            curPageindex = 0;
            tipPanelPageLeftBtn.interactable = false;
        }
        else if (curPageindex >= panelPageInfoArray.Length - 1)//마지막 페이지인 경우
        {
            curPageindex = panelPageInfoArray.Length - 1;
            tipPanelPageRightBtn.interactable = false;

            tipPanelPageText.color = gameManager.uiManager.textGreen;
        }

        //해당 페이지 정보 받아오기
        tipPanelDisplay.sprite = panelPageInfoArray[curPageindex].pageSprite;
        tipPanelText.text = panelPageInfoArray[curPageindex].pageStr;

        tipPanelPageText.text = (curPageindex + 1) + "/" + panelPageInfoArray.Length;

        
    }
}
