using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("�ӽŷ���������")]
    public bool isML;

    //[Header("�Ҹ� ��� ����")]
    //public bool isSound;

    [Header("����� ��")]
    public Transform blueTower;
    public TowerManager blueTowerManager;
    [Header("������ ��")]
    public Transform redTower;
    public TowerManager redTowerManager;

    [Header("���� ���̵�")]//1, 2 ,3(�⺻: 2)
    public int gameLevel;

    //���ӸŴ������� ��������� ũ���� ��ȯ�ϴ� �뵵
    [Header("Infantry ������")]
    public GameObject Infantry;
    [Header("Shooter ������")]
    public GameObject Shooter;

    //Ÿ�� �ų������� ���⸦ �з��ϴ� �뵵
    [Header("Gun ������")]
    public GameObject Gun;
    [Header("Flame ������")]
    public GameObject Flame;
    [Header("GrandCure ������")]
    public GameObject GrandCure;



    [Header("�Ŵ��� ���")]
    public ObjectManager objectManager;
    public UIManager uiManager;
    public AudioManager audioManager;

    #region ���� ���� �״�� ������ '��õ�'
    public void RetryGame()
    {
        //���� �ѱ�� ȿ����
        audioManager.PlaySfx(AudioManager.Sfx.PaperSfx);

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
        blueTowerManager.ResetTower();
        redTowerManager.ResetTower();

        //ũ���� ��ȯ(�׽�Ʈ��)
        //redTowerManager.SpawnCreature(Shooter.name);
        //redTowerManager.SpawnCreature(Shooter.name);

        //UI �ʱ�ȭ
        uiManager.resetUI();

    }
    #endregion


    public void ResetGame()     //������ 'ó������'
    {
        //���� �ѱ�� ȿ����
        audioManager.PlaySfx(AudioManager.Sfx.PaperSfx);

        //ȭ���� ó�� ȭ������
        SceneManager.LoadScene(0);
    }


    public void QuitGame()     //������ '�����ϱ�'
    {
        //���� �ѱ�� ȿ����
        audioManager.PlaySfx(AudioManager.Sfx.PaperSfx);

        //���� ����
        Application.Quit();
    }
}
