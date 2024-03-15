using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using static SpellData;

public class SelectManager : MonoBehaviour
{
    [Header("���õ� ���� ��ư")]
    public SpellButton selectedSpellBtn;

    [Header("���õ� ���� ������")]
    public Image selectedSpellIcon;

    [Header("���õ� ���� �̸�")]
    public Text selectedSpellName;

    [Header("���õ� ���� Ÿ��")]
    public Text selectedSpellType;

    [Header("���õ� ���� ���")]
    public Text selectedSpellValue;

    [Header("���õ� ���� ����")]
    public Text selectedSpellDesc;

    [Header("�����ư �迭")]
    public SpellButton[] spellBtnArr = new SpellButton[4];

    [Header("������ �г�")]
    public GameObject rightPanel;

    [Header("�Ŵ���")]
    public GameManager gameManager;
    UIManager uiManager;
    AudioManager audioManager;
    
    void Awake()
    {
        uiManager = gameManager.uiManager;
        audioManager = gameManager.audioManager;

        //������ �г� ��Ȱ��ȭ
        rightPanel.SetActive(false);
    }

    public void StartGame()//���� ����
    {
        gameObject.SetActive(false);
        Time.timeScale = 1;//0�� ���� ����

        //battleUI�� ���� ����
        for(int i = 0; i < spellBtnArr.Length; i++)
        {
            if (spellBtnArr[i].spellData != null)
            {
                uiManager.spellBtnArr[i].spellData = spellBtnArr[i].spellData;
                spellBtnArr[i].IconChange(uiManager.spellBtnArr[i]);
            }
            else if (spellBtnArr[i].spellData == null) 
            {
                uiManager.spellBtnArr[i].ButtonOff();
            }
        }

        //���� ȯ�� �ʱ�ȭ
        gameManager.resetEnv();
    }

    public void RestartGame()//���� �ʱ�ȭ
    {
        SceneManager.LoadScene(0);
    }
}
