using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("�ӽŷ���������")]
    public bool isML;

    [Header("����� ��")]
    public Transform blueTower;
    public TowerManager blueTowerManager;
    [Header("������ ��")]
    public Transform redTower;
    public TowerManager redTowerManager;

    [Header("Infantry ������")]
    public GameObject Infantry;
    [Header("Shooter ������")]
    public GameObject Shooter;
    [Header("Gun ������")]
    public GameObject Gun;
    [Header("Flame ������")]
    public GameObject Flame;



    [Header("�Ŵ��� ���")]
    public ObjectManager objectManager;
    public UIManager uiManager;
    public AudioManager audioManager;

    public void resetEnv()
    {
        

        //�Ѿ� �ʱ�ȭ
        foreach (GameObject tmpBullet in objectManager.bulletFolder) 
        {
            tmpBullet.SetActive(false);
        }

        //Ÿ�� ü�� �ʱ�ȭ
        blueTowerManager.curHealth = blueTowerManager.maxHealth;
        redTowerManager.curHealth = redTowerManager.maxHealth;

        //ũ����

        //���� �ʱ�ȭ
        blueTowerManager.curTowerResource = 0;
        redTowerManager.curTowerResource = 0;


    }
}
