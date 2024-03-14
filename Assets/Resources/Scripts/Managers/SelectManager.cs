using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
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

    [Header("선택된 스펠버튼 배열")]
    public SpellButton[] spellBtnArr = new SpellButton[4];

    [Header("매니저")]
    public GameManager gameManager;
    AudioManager audioManager;

    

    void Awake()
    {
        audioManager = gameManager.audioManager;
    }

    public void StartGame()//게임 시작
    {
        gameObject.SetActive(false);
        Time.timeScale = 1;
    }

    public void RestartGame()//게임 초기화
    {
        SceneManager.LoadScene(0);
    }


}
