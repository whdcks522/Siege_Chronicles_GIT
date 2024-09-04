using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TipManager : MonoBehaviour
{
    [Header("�Ŵ���")]
    public GameManager gameManager;
    AudioManager audioManager;

    [Serializable]//�ʿ��ϴ���
    public class PanelPageInfo
    {
        public Sprite pageSprite;
        [TextArea]
        public string pageStr;
    }
    [Header("�г� ���÷����� ���� �迭")]
    public PanelPageInfo[] panelPageInfoArray;

    [Header("�� �г� ������Ʈ")]
    public GameObject tipPanel;

    [Header("�� �г��� ���÷���")]
    public Image tipPanelDisplay;
    [Header("�� �г��� ����")]
    public Text tipPanelDesc;

    [Header("�� �г��� ������ �ؽ�Ʈ")]
    public Text tipPanelPageText;

    [Header("�� �г��� ������ ���� ��ư")]
    public Button tipPanelPageLeftBtn;

    [Header("�� �г��� ������ ������ ��ư")]
    public Button tipPanelPageRightBtn;

    [Header("�� �г� �ִϸ��̼�")]
    public Animator tipAnim;

    [Header("�� �г��� �ؽ�Ʈ �ִϸ��̼�")]
    public Animator pageAnim;

    private void Start()
    {
        audioManager = gameManager.audioManager;
    }

    public void PanelOn(int startPage) //�г� Ȱ��, ��Ȱ�� ����
    {
        //�ø��� �ִϸ��̼� ���
        tipAnim.SetBool("isPanel", true);

        //�г� ���¸� �ʱ�ȭ
        curPageindex = startPage;

        //�״�� �����ֱ�
        PanelPageControl(0);
    }

    public void PanelOff() //�г� Ȱ��, ��Ȱ�� ����
    {
        //������ �ִϸ��̼� ���
        tipAnim.SetBool("isPanel", false);

        //���� �ѱ�� ȿ����(�ø��� ���� PanelPageControl���� �˾Ƽ� ����)
        audioManager.PlaySfx(AudioManager.Sfx.PaperSfx);
    }

    //���� ��� ����������
    int curPageindex = 0;

    public void PanelPageControl(int pageIndex) //�г� ������ ��Ʈ��
    {
        //���� �ѱ�� ȿ����
        audioManager.PlaySfx(AudioManager.Sfx.PaperSfx);


        //������ �� ����
        curPageindex += pageIndex;

        //���� ��ȣ�ۿ� Ȱ��ȭ
        tipPanelPageLeftBtn.interactable = true;
        tipPanelPageRightBtn.interactable = true;

        //�ؽ�Ʈ �� ��ȭ
        tipPanelPageText.color = gameManager.uiManager.textYellow;

        if (curPageindex <= 0)//ù�� ° �������� ���
        {
            curPageindex = 0;
            tipPanelPageLeftBtn.interactable = false;//���� ���� ��Ȱ��ȭ
        }
        else if (curPageindex >= panelPageInfoArray.Length - 1)//������ �������� ���
        {
            curPageindex = panelPageInfoArray.Length - 1;
            tipPanelPageRightBtn.interactable = false;//���� ���� ��Ȱ��ȭ

            tipPanelPageText.color = gameManager.uiManager.textGreen;
        }

        //�ش� ������ ���� �޾ƿ���
        tipPanelDisplay.sprite = panelPageInfoArray[curPageindex].pageSprite;
        tipPanelDesc.text = panelPageInfoArray[curPageindex].pageStr;

        //���� ������ �˷��ֱ�
        tipPanelPageText.text = (curPageindex + 1) + "/" + panelPageInfoArray.Length;

        //������ �ؽ�Ʈ �ִϸ��̼�
        pageAnim.SetBool("isFlash", true);
    }
}
