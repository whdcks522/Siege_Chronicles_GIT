using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartManager : MonoBehaviour
{
    [Header("�ִϸ��̼�")]
    public Animator startAnim;
    public Animator tipAnim;

    [Header("�Ŵ���")]
    public GameManager gameManager;
    AudioManager audioManager;

    private void Start()
    {
        audioManager = gameManager.audioManager;

        //���� ȭ�� ��� ���� ���
        audioManager.PlayBgm(AudioManager.Bgm.StartBgm);
    }

    public void StartGame()//���� ��ư ����
    {
        startAnim.SetBool("isStart", false);

        //���� �ѱ�� ȿ����
        audioManager.PlaySfx(AudioManager.Sfx.PaperSfx);
    }


    [Serializable]//�ʿ��ϴ���
    public class PanelPageInfo
    {
        public Sprite pageSprite;
        [TextArea]
        public string pageStr;
    }
    [Header("�г� ���÷����� ���� �迭")]
    public PanelPageInfo[] panelPageInfoArray;

    [Header("�� �г�")]
    public GameObject tipPanel;

    [Header("�� �г��� ���÷���")]
    public Image tipPanelDisplay;
    [Header("�� �г��� �ؽ�Ʈ")]
    public Text tipPanelText;

    [Header("�� �г��� ������ �ؽ�Ʈ")]
    public Text tipPanelPageText;

    [Header("�� �г��� ������ ���� ��ư")]
    public Button tipPanelPageLeftBtn;

    [Header("�� �г��� ������ ������ ��ư")]
    public Button tipPanelPageRightBtn;

    public void PanelActivateControl(bool isActivate) //�г� Ȱ��, ��Ȱ�� ����
    {
        tipAnim.SetBool("isPanel", isActivate);

        if (isActivate) //�г��� ���� ���
        {
            //�г� ���¸� �ʱ�ȭ
            PanelPageControl(0);
        }
        else if (!isActivate) 
        {
            //���� �ѱ�� ȿ����
            audioManager.PlaySfx(AudioManager.Sfx.PaperSfx);
        }
    }

    //���� ������ �ε���
    int curPageindex = 0;
    [Header("�� �г��� �ؽ�Ʈ �ִϸ��̼�")]
    public Animator pageAnim;
    public void PanelPageControl(int pageIndex) //�г� ������ ��Ʈ��
    {
        //���� �ѱ�� ȿ����
        audioManager.PlaySfx(AudioManager.Sfx.PaperSfx);

        //������ �ؽ�Ʈ �ִϸ��̼�
        pageAnim.SetBool("isFlash", true);


        //0�̸� �ʱ� ȭ��, -1�̸� ����, +1�̸� ������
        if (pageIndex == 0) //0���� �ʱ�ȭ
            curPageindex = 0;
        else if (pageIndex == -1)//�������� �ѱ��
            curPageindex -= 1;
        else if (pageIndex == 1)//���������� �ѱ��
            curPageindex += 1;

        //���� ����
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

        //�ش� ������ ���� �޾ƿ���
        tipPanelDisplay.sprite = panelPageInfoArray[curPageindex].pageSprite;
        tipPanelText.text = panelPageInfoArray[curPageindex].pageStr;

        tipPanelPageText.text = (curPageindex + 1) + "/" + panelPageInfoArray.Length;
    }
}
