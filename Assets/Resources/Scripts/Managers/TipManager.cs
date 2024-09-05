using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TipManager : MonoBehaviour
{
    [Header("매니저")]
    public GameManager gameManager;
    AudioManager audioManager;

    [Serializable]//필요하더라
    public class PanelPageInfo
    {
        public Sprite pageSprite;
        [TextArea(4, 10)]
        public string pageStr;
    }


    [Header("패널 디스플레이의 정보 배열")]
    public PanelPageInfo[] panelPageInfoArray;

    [Header("팁 패널 오브젝트")]
    public GameObject tipPanel;

    [Header("팁 패널의 디스플레이")]
    public Image tipPanelDisplay;
    [Header("팁 패널의 설명")]
    public Text tipPanelDesc;

    [Header("팁 패널의 페이지 텍스트")]
    public Text tipPanelPageText;

    [Header("팁 패널의 페이지 왼쪽 버튼")]
    public Button tipPanelPageLeftBtn;

    [Header("팁 패널의 페이지 오른쪽 버튼")]
    public Button tipPanelPageRightBtn;

    [Header("팁 패널 애니메이션")]
    public Animator tipAnim;

    [Header("팁 패널의 텍스트 애니메이션")]
    public Animator pageAnim;

    private void Start()
    {
        audioManager = gameManager.audioManager;
    }

    public void PanelOn(int startPage) //패널 활성, 비활성 관리
    {
        //올리는 애니메이션 재생
        tipAnim.SetBool("isPanel", true);

        //패널 상태를 초기화
        curPageindex = startPage;

        //그대로 보여주기
        PanelPageControl(0);
    }

    public void PanelOff() //패널 활성, 비활성 관리
    {
        //내리는 애니메이션 재생
        tipAnim.SetBool("isPanel", false);

        //종이 넘기는 효과음(올리는 경우는 PanelPageControl에서 알아서 수행)
        audioManager.PlaySfx(AudioManager.Sfx.PaperSfx);
    }

    //현재 어느 페이지인지
    int curPageindex = 0;

    public void PanelPageControl(int pageIndex) //패널 페이지 컨트롤
    {
        //종이 넘기는 효과음
        audioManager.PlaySfx(AudioManager.Sfx.PaperSfx);


        //페이지 수 조작
        curPageindex += pageIndex;

        //방향 상호작용 활성화
        tipPanelPageLeftBtn.interactable = true;
        tipPanelPageRightBtn.interactable = true;

        //텍스트 색 변화
        tipPanelPageText.color = gameManager.uiManager.textYellow;

        if (curPageindex <= 0)//첫번 째 페이지인 경우
        {
            curPageindex = 0;
            tipPanelPageLeftBtn.interactable = false;//좌측 방향 비활성화
        }
        else if (curPageindex >= panelPageInfoArray.Length - 1)//마지막 페이지인 경우
        {
            curPageindex = panelPageInfoArray.Length - 1;
            tipPanelPageRightBtn.interactable = false;//우측 방향 비활성화

            tipPanelPageText.color = gameManager.uiManager.textGreen;
        }

        //해당 페이지 정보 받아오기
        tipPanelDisplay.sprite = panelPageInfoArray[curPageindex].pageSprite;
        tipPanelDesc.text = panelPageInfoArray[curPageindex].pageStr;

        //현재 페이지 알려주기
        tipPanelPageText.text = (curPageindex + 1) + "/" + panelPageInfoArray.Length;

        //페이지 텍스트 애니메이션
        pageAnim.SetBool("isFlash", true);
    }
}
