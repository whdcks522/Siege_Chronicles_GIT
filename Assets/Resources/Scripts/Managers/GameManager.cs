using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("���� ũ���ĸ� ��ȯ�� ������")]
    public bool isEnemySpawn;

    [Header("Bmg ����")]
    public bool isBgm;

    [Header("����� ��")]
    public Transform blueTower;
    public TowerManager blueTowerManager;
    [Header("������ ��")]
    public Transform redTower;
    public TowerManager redTowerManager;

    [Header("���� ���̵�")]//1, 2 ,3(�⺻: 2)
    public int gameLevel;

    [Header("�� ������ ��ȯ������ ũ������ �ִ� ��")]
    public int maxCreatureCount;


    //�� Ÿ���� ��ȯ�ϴ� �뵵
    [Header("ũ���� ���� ������ ������ �迭")]
    public SpellData[] creatureSpellDataArr;//Infantry, Shooter, Shielder, Accountant ������

    //Ÿ�� �ų������� ���⸦ �з��ϴ� �뵵
    [Header("���� ������")]
    public GameObject Gun;//Gun ������
    public GameObject Flame;//Flame ������
    public GameObject GrandCure;//GrandCure ������
    public GameObject CorpseExplosion;//CorpseExplosion ������

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
        //������ ��Ʈ �ʱ�ȭ
        for (int i = 0; i < objectManager.damageFontFolder.childCount; i++)
        {
            objectManager.damageFontFolder.GetChild(i).gameObject.SetActive(false);
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
        //redTowerManager.SpawnCreature(Infantry.name);
        //redTowerManager.SpawnCreature(Shooter.name);

        //UI �ʱ�ȭ
        uiManager.resetUI();

        //���� ȭ���� ���� �ʱ�ȭ
        uiManager.startBtn.SetActive(true);
        uiManager.victoryTitle.SetActive(false);
        uiManager.defeatTitle.SetActive(false);
    }
    #endregion


    public void ResetGame()//������ 'ó������'
    {
        //�ð� ����� 1��
        Time.timeScale = 1;

        //ȭ���� ó�� ȭ������
        SceneManager.LoadScene(0);
    }

    public void QuitGame()//������ '�����ϱ�'
    {
        Application.Quit();
    }
}
