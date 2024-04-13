using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("머신러닝중인지")]
    public bool isML;

    //[Header("소리 재생 여부")]
    //public bool isSound;

    [Header("블루팀 성")]
    public Transform blueTower;
    public TowerManager blueTowerManager;
    [Header("레드팀 성")]
    public Transform redTower;
    public TowerManager redTowerManager;

    [Header("게임 난이도")]//1, 2 ,3(기본: 2)
    public int gameLevel;

    //게임매니저에서 실험용으로 크리쳐 소환하는 용도
    [Header("크리쳐 프리펩")]
    public GameObject Infantry;//Infantry 프리펩
    public GameObject Shooter;//Shooter 프리펩
    public GameObject Shielder;//Shielder 프리펩
    public GameObject Accountant;//Accountant 프리펩

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

        //크리쳐 소환(테스트용)
        //redTowerManager.SpawnCreature(Infantry.name);
        //redTowerManager.SpawnCreature(Shooter.name);

        //UI 초기화
        uiManager.resetUI();

    }
    #endregion


    public void ResetGame()     //게임을 '처음으로'
    {
        //시간 배속을 1로
        Time.timeScale = 1;

        //화면을 처음 화면으로
        SceneManager.LoadScene(0);
    }


    public void QuitGame()     //게임을 '종료하기'
    {
        //게임 종료
        Application.Quit();
    }
}
