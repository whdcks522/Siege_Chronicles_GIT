using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static SpellData;

public class SelectManager : MonoBehaviour
{
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


    [Header("�Ŵ���")]
    public GameManager gameManager;
    UIManager UiManager;
    AudioManager audioManager;

    void Awake()
    {
        UiManager = gameManager.uiManager;
        audioManager = gameManager.audioManager;
    }


    public void StartGame()//���� ����
    {
        gameObject.SetActive(false);
    }

    public void aa() 
    {
    
    }
}
