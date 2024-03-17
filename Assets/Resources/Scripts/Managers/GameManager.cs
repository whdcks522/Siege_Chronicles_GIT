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

    #region ���� ȯ�� �ʱ�ȭ
    public void resetEnv()
    {
        //�Ѿ� �ʱ�ȭ
        for (int i = 0; i < objectManager.bulletFolder.childCount; i++)
        {
            objectManager.bulletFolder.GetChild(i).gameObject.SetActive(false);
        }
        //�Ķ� ũ���� �ʱ�ȭ
        for (int i = 0; i < objectManager.blueCreatureFolder.childCount; i++)
        {
            objectManager.blueCreatureFolder.GetChild(i).gameObject.SetActive(false);
        }
        //���� ũ���� �ʱ�ȭ
        for (int i = 0; i < objectManager.redCreatureFolder.childCount; i++)
        {
            objectManager.redCreatureFolder.GetChild(i).gameObject.SetActive(false);
        }


        //Ÿ�� ü�� �ʱ�ȭ
        blueTowerManager.curHealth = blueTowerManager.maxHealth;
        redTowerManager.curHealth = redTowerManager.maxHealth;
        //Ÿ�� �ڿ� �ʱ�ȭ
        blueTowerManager.curTowerResource = 0;
        redTowerManager.curTowerResource = 0;

        //ũ���� ��ȯ(�׽�Ʈ��)
        redTowerManager.SpawnCreature(Shooter.name);
        redTowerManager.SpawnCreature(Shooter.name);

        //UI �ʱ�ȭ
        uiManager.FocusControl(false, true);

    }
    #endregion
}
