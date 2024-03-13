using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static SpellData;

public class SelectManager : MonoBehaviour
{
    [Header("선택된 스펠 아이콘")]
    public Image selectedSpellIcon;

    [Header("선택된 스펠 이름")]
    public Text selectedSpellName;

    [Header("선택된 스펠 타입")]
    public Text selectedSpellType;

    [Header("선택된 스펠 비용")]
    public Text selectedSpellValue;

    [Header("선택된 스펠 설명")]
    public Text selectedSpellDesc;


    [Header("매니저")]
    public GameManager gameManager;
    UIManager UiManager;
    AudioManager audioManager;

    void Awake()
    {
        UiManager = gameManager.uiManager;
        audioManager = gameManager.audioManager;
    }


    public void StartGame()//게임 시작
    {
        gameObject.SetActive(false);
    }

    public void aa() 
    {
    
    }
}
