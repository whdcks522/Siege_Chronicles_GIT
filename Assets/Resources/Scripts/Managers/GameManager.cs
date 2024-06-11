using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("적이 크리쳐를 소환할 것인지")]
    public bool isEnemySpawn;

    [Header("Bmg 여부")]
    public bool isBgm;

    [Header("블루팀 성")]
    public Transform blueTower;
    public TowerManager blueTowerManager;
    [Header("레드팀 성")]
    public Transform redTower;
    public TowerManager redTowerManager;

    [Header("게임 난이도")]//1, 2 ,3(기본: 2)
    public int gameLevel;

    [Header("각 팀별로 소환가능한 크리쳐의 최대 수")]
    public int maxCreatureCount;

    //적 타워가 소환하는 용도
    [Header("크리쳐 스펠 데이터 프리펩 배열")]
    public SpellData[] creatureSpellDataArr;//Infantry, Shooter, Shielder, Accountant 프리펩

    //타워 매너저에서 무기를 분류하는 용도
    [Header("무기 프리펩")]
    public GameObject Gun;//Gun 프리펩
    public GameObject Flame;//Flame 프리펩
    public GameObject GrandCure;//GrandCure 프리펩
    public GameObject CorpseExplosion;//CorpseExplosion 프리펩

    [Header("매니저 목록")]
    public ObjectManager objectManager;
    public UIManager uiManager;
    public AudioManager audioManager;

    #region 현재 스펠 그대로 게임을 '재시도'
    public void RetryGame()
    {
        //종이 넘기는 효과음
        audioManager.PlaySfx(AudioManager.Sfx.PaperSfx);

        //총알 초기화
        for (int i = 0; i < objectManager.bulletFolder.childCount; i++)
        {
            objectManager.bulletFolder.GetChild(i).gameObject.SetActive(false);
        }
        //데미지 폰트 초기화
        for (int i = 0; i < objectManager.damageFontFolder.childCount; i++)
        {
            objectManager.damageFontFolder.GetChild(i).gameObject.SetActive(false);
        }
        //파랑 크리쳐 초기화
        for (int i = 0; i < objectManager.blueCreatureFolder.childCount; i++)
        {
            objectManager.blueCreatureFolder.GetChild(i).gameObject.SetActive(false);
        }
        //빨강 크리쳐 초기화
        for (int i = 0; i < objectManager.redCreatureFolder.childCount; i++)
        {
            objectManager.redCreatureFolder.GetChild(i).gameObject.SetActive(false);
        }

        //타워 체력 초기화
        blueTowerManager.ResetTower();
        redTowerManager.ResetTower();

        //UI 초기화
        uiManager.resetUI();

        //설정 화면의 글자 초기화
        uiManager.startBtn.SetActive(true);
        uiManager.victoryTitle.SetActive(false);
        uiManager.defeatTitle.SetActive(false);
    }
    #endregion

    public void ResetGame()//게임을 '처음으로'
    {
        //시간 배속을 1로
        Time.timeScale = 1;

        //화면을 처음 화면으로
        SceneManager.LoadScene(0);
    }

    public void QuitGame()//게임을 '종료하기'
    {
        Application.Quit();
    }
}
