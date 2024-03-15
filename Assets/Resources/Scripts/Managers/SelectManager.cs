using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using static SpellData;

public class SelectManager : MonoBehaviour
{
    [Header("선택된 스펠 버튼")]
    public SpellButton selectedSpellBtn;

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

    [Header("스펠버튼 배열")]
    public SpellButton[] spellBtnArr = new SpellButton[4];

    [Header("매니저")]
    public GameManager gameManager;
    UIManager uiManager;
    AudioManager audioManager;
    

    

    void Awake()
    {
        uiManager = gameManager.uiManager;
        audioManager = gameManager.audioManager;
    }

    public void StartGame()//게임 시작
    {
        gameObject.SetActive(false);
        Time.timeScale = 1;//0은 설정 안함

        //battleUI로 스펠 전달
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

        gameManager.resetEnv();
    }

    public void RestartGame()//게임 초기화
    {
        SceneManager.LoadScene(0);
    }


}
