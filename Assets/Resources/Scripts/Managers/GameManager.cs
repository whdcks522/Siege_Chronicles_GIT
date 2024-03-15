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

    public void resetEnv()
    {
        

        //총알 초기화
        foreach (GameObject tmpBullet in objectManager.bulletFolder) 
        {
            tmpBullet.SetActive(false);
        }

        //타워 체력 초기화
        blueTowerManager.curHealth = blueTowerManager.maxHealth;
        redTowerManager.curHealth = redTowerManager.maxHealth;

        //크리쳐

        //마나 초기화
        blueTowerManager.curTowerResource = 0;
        redTowerManager.curTowerResource = 0;


    }
}
