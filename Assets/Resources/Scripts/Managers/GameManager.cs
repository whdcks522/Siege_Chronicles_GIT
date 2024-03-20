using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("머신러닝중인지")]
    public bool isML;

    [Header("블루팀 성")]
    public Transform blueTower;
    public TowerManager blueTowerManager;
    [Header("레드팀 성")]
    public Transform redTower;
    public TowerManager redTowerManager;

    [Header("게임 난이도")]//1, 2 ,3(기본: 2)
    public int gameLevel;

    [Header("Infantry 프리펩")]
    public GameObject Infantry;
    [Header("Shooter 프리펩")]
    public GameObject Shooter;
    [Header("Gun 프리펩")]
    public GameObject Gun;
    [Header("Flame 프리펩")]
    public GameObject Flame;



    [Header("매니저 목록")]
    public ObjectManager objectManager;
    public UIManager uiManager;
    public AudioManager audioManager;



    #region 전투 환경 초기화
    public void resetEnv()
    {
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
        //redTowerManager.SpawnCreature(Shooter.name);
        //redTowerManager.SpawnCreature(Shooter.name);

        //포커스 초기화
        uiManager.FocusControl(false, true);

    }
    #endregion
}
