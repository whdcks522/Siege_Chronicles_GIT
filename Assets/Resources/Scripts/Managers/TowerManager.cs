using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using static Creature;

public class TowerManager : MonoBehaviour
{
    [Header("Ÿ���� �ִ� ü��")]
    public float maxHealth;
    [Header("Ÿ���� ���� ü��")]
    public float curHealth;

    public TeamEnum curTeamEnum;

    [Header("Ÿ���� �Ʊ� ���� ��ġ")]
    public Transform creatureStartPoint;
    [Header("Ÿ���� ĳ�� ��ġ")]
    public Transform bulletStartPoint;

    [Header("��� Ÿ���� ��ġ")]
    public Transform enemyTower;

    [Header("Ÿ�� ���� �̴� UI")]
    public GameObject miniCanvas;
    public Image miniHealth;


    [Header("�Ŵ���")]
    public GameManager gameManager;
    ObjectManager objectManager;
    UIManager UiManager;
    AudioManager audioManager;

    private void Awake()
    {
        objectManager = gameManager.objectManager;
        UiManager = gameManager.uiManager;
        audioManager = gameManager.audioManager;

        if (curTeamEnum == TeamEnum.Blue)
        {
            miniHealth.color = Color.blue;
            enemyTower = gameManager.redTower;

            ourCreatureFolder = gameManager.objectManager.blueCreatureFolder;
            enemyCreatureFolder = gameManager.objectManager.redCreatureFolder;
        }
        else if (curTeamEnum == TeamEnum.Red)
        {
            miniHealth.color = Color.red;
            enemyTower = gameManager.blueTower;

            ourCreatureFolder = gameManager.objectManager.redCreatureFolder;
            enemyCreatureFolder = gameManager.objectManager.blueCreatureFolder;
        }

        StartCoroutine(UpdateCoroutine());
    }

    [Header("������� ����ִ� ����(����_��ü���߿�)")]
    public Transform ourCreatureFolder;
    [Header("������� ����ִ� ����(����_��ݿ�)")]
    public Transform enemyCreatureFolder;

    [Header("Ÿ���� �ڿ� ����")]
    public float curTowerResource = 0f;     //���� �ڿ�
    public float maxTowerResource = 10f;    //�ִ� �ڿ�

    [Header("�� Ÿ���� ������ ����� ���� ������")]
    public SpellData futureSpellData;

    WaitForSeconds waitSec = new WaitForSeconds(0.05f);
    IEnumerator UpdateCoroutine()
    {
        while (true)
        {
            if (UiManager.selectManager.index_Battle == 1)
            {
                if (maxTowerResource > curTowerResource)
                {
                    //���� ����� ���� �ڿ� ����
                    curTowerResource += BankSpeedArr[curBankIndex] * 0.1f;
                }
                else if (maxTowerResource <= curTowerResource)
                {
                    //���� �ڿ����� �ִ�ġ�� ���� �ʵ���
                    curTowerResource = maxTowerResource;
                }

                if (curTeamEnum == TeamEnum.Red) //���� �� Ÿ������
                {
                    if (futureSpellData != null)//��ȯ�� ���� �������ٸ�
                    {
                        if (curTowerResource >= futureSpellData.spellValue && CreatureCountCheck())//�ڿ��� ����ϸ鼭 �ڽ��� ũ���� ��ȯ ���ΰ� ����� ��
                        {
                            //ũ���� ��ȯ
                            if (gameManager.isEnemySpawn)
                            {
                                SpawnCreature(futureSpellData.spellPrefab.name);

                                //���� ȿ����
                                audioManager.PlaySfx(AudioManager.Sfx.SpellSuccessSfx);
                            }
                            //��� ó��
                            curTowerResource -= futureSpellData.spellValue;

                            //�ٸ� ���� ��ȯ�ϱ� ���� �ʱ�ȭ
                            futureSpellData = null;

                        }
                    }
                    else if (futureSpellData == null)//��ȯ�� ���� �������� �ʾҴٸ�
                    {
                        //� ũ���ĸ� ��ȯ�� �� �������� ����
                        int r = 0;
                        //Random.Range(0, gameManager.creatureSpellDataArr.Length);
                        futureSpellData = gameManager.creatureSpellDataArr[r];
                    }
                }
            }

            yield return waitSec;
        }
    }


    #region Ÿ�� ��� �ʱ�ȭ;

    [Header("��ũ ���� ��ҵ�")]
    public int[] BankValueArr = { 5, 6, 7, 8 };//��ũ ��ư�� ������ ���� �ʿ��� ��� �迭
    float[] BankSpeedArr = { 0.6f, 0.7f, 0.8f, 0.9f, 1f };//��ũ ��ư�� ������ �ڿ��� �����ϰ� �Ǵ� �ӵ� �迭
    public int curBankIndex = 0;//���� ��ũ ������ ������

    public void ResetTower()//���� ������� ���� Ÿ�� ���� �ʱ�ȭ
    {
        //Ÿ�� ü�� �ʱ�ȭ
        curHealth = maxHealth;
        miniHealth.fillAmount = 1;

        //Ÿ�� �ڿ� �ʱ�ȭ
        curTowerResource = 0;

        //Ÿ�� ���� ��ġ �ʱ�ȭ
        curBankIndex = 0;

        //Ÿ�� ũ���� �� �ʱ�ȭ
        curCreatureCount = 0;
    }
    #endregion

    #region ũ���� �� ����;

    [Header("�ڱ� �� ũ������ ���� ��")]
    public int curCreatureCount;  

    public bool CreatureCountCheck()//��ȯ �μ� ���� ��, ��ȯ ���� ���� ��ȯ
    {
        bool canSpawn = false;

        if (curCreatureCount < gameManager.maxCreatureCount)//��ȯ ������ ���
        {
            //ũ���� �� ���� ��ȭ
            curCreatureCount++;
            canSpawn = true;
        }
        else if (curCreatureCount >= gameManager.maxCreatureCount)//��ȯ �Ұ����� ���
        {
            canSpawn = false;
        }

        if (curTeamEnum == Creature.TeamEnum.Blue)//�Ķ����̸� 
        {
            CreatureCountText();
        }

        return canSpawn;
    }

    public void CreatureCountText()//ũ���� ���� �ؽ�Ʈ ����
    {
        //��ȯ ���� �ؽ�Ʈ ����
        UiManager.creatureCountText.text = curCreatureCount.ToString() + "/" + gameManager.maxCreatureCount.ToString();
        //��ȯ ���� �ؽ�Ʈ ���� �ִϸ��̼�
        UiManager.creatureCountAnim.SetBool("isFlash", true);
    }
    #endregion

    private void OnTriggerEnter(Collider other)//�ε���
    {
        if (other.gameObject.CompareTag("Bullet"))//�Ѿ˰� �浹
        {
            Bullet bullet = other.gameObject.GetComponent<Bullet>();

            if (bullet.curTeamEnum != curTeamEnum && bullet.byCreature)//���� �ٸ� ���鼭 ũ���Ŀ� ���� ���ݸ� ���� ó��
            {

                //���� ����
                DamageControl(bullet);

                //�ǰ��� �Ѿ� ��ó��
                if (bullet.curBulletMoveEnum != Bullet.BulletMoveEnum.Slash)
                {
                    bullet.BulletOff();
                }
            }
        }
    }

    #region ������ ���
    GameObject damageFont = null;
    void DamageControl(Bullet bullet)
    {
        //���ط� Ȯ��
        float damage = bullet.bulletDamage;
        if (curTeamEnum == TeamEnum.Blue)
        {
            damage /= 2;
        }
        else if (curTeamEnum == TeamEnum.Red)//gameLevel�� Ŭ���� �Ⱦ���
        {
            damage /= gameManager.gameLevel;
        }

        if (curTeamEnum == TeamEnum.Blue)//�Ķ� Ÿ���� ������ �Ķ���
        {
            damageFont = objectManager.CreateObj("BlueDamageFont", ObjectManager.PoolTypes.DamageFontPool);
        }
        else //���� Ÿ���� ������ ������
        {
            damageFont = objectManager.CreateObj("RedDamageFont", ObjectManager.PoolTypes.DamageFontPool);
        }
        //��Ʈ ��ġ�� ���� ����
        damageFont.transform.position = transform.position;
        damageFont.GetComponent<DamageFont>().ReName(damage.ToString());


        curHealth -= damage;

        //Ÿ�� ü�¹� ����
        miniHealth.fillAmount = curHealth / maxHealth;

        if (curHealth > 0)
        {
            if (damage != 0)
                audioManager.PlaySfx(AudioManager.Sfx.TowerCrashSfx);
        }
        else if (curHealth <= 0) //���� ����
        {
            curHealth = 0;

            //���� ȭ���� ���� ��ư ��Ȱ��ȭ
            UiManager.startBtn.SetActive(false);

            if (curTeamEnum == Creature.TeamEnum.Red)//���� ���� �� ���
            {
                //���� ȭ���� �ؽ�Ʈ ����
                UiManager.victoryTitle.SetActive(true);

                //�¸� ȿ����
                audioManager.PlaySfx(AudioManager.Sfx.WinSfx);
            }
            else if (curTeamEnum == Creature.TeamEnum.Blue) //�Ķ� ���� �� ���
            {
                //���� ȭ���� �ؽ�Ʈ ����
                UiManager.defeatTitle.SetActive(true);

                //�й� ȿ����
                audioManager.PlaySfx(AudioManager.Sfx.LoseSfx);
            }
            //���� ���Ḧ ���� ���� ��Ű��
            UiManager.SettingControl(true);
        }
    }
    #endregion

    //Ÿ�� �ּ���(��ų��) ���� ���� ����
    public void RadarControl(Vector3 targetVec) => bulletStartPoint.transform.LookAt(targetVec);

    #region ũ���� ��ȯ
    public void SpawnCreature(string tmpCreatureName)
    {
        //����ü ����
        GameObject obj = objectManager.CreateObj(tmpCreatureName, ObjectManager.PoolTypes.CreaturePool);
        Creature creature = obj.GetComponent<Creature>();
        //Ȱ�� ���� ����
        creature.BeforeRevive(curTeamEnum, gameManager);
    }
    #endregion

    #region ����(����) ��� �з�
    Vector3 enemyVec;
    public void WeaponSort(string tmpWeaponName) 
    {
        if (tmpWeaponName == gameManager.Gun.name) Tower_Gun();
        else if (tmpWeaponName == gameManager.Flame.name) Tower_Flame();
        else if (tmpWeaponName == gameManager.GrandCure.name) Tower_GrandCure();
        else if (tmpWeaponName == gameManager.CorpseExplosion.name) Tower_CorpseExplosion();
    }
    #endregion

    #region �̴ϰ� ����
    void Tower_Gun()
    {
        //��� ȿ����
        audioManager.PlaySfx(AudioManager.Sfx.GunSfx);

        //������ ��� �ߴ��� ����
        bool isShot = false;

        //���̴��� �� Ÿ���� �Ĵٺ�
        RadarControl(enemyTower.transform.position);

        //�� ũ���� ��ġ �ľ�
        for (int i = 0; i < enemyCreatureFolder.childCount; i++)
        {
            if (enemyCreatureFolder.GetChild(i).gameObject.activeSelf && 
                enemyCreatureFolder.GetChild(i).gameObject.layer == LayerMask.NameToLayer("Creature")) 
            {
                isShot = true;

                enemyVec = enemyCreatureFolder.GetChild(i).transform.position;

                GameObject bullet = objectManager.CreateObj("Tower_Gun", ObjectManager.PoolTypes.BulletPool);
                Bullet bullet_bullet = bullet.GetComponent<Bullet>();
                Rigidbody bullet_rigid = bullet.GetComponent<Rigidbody>();

                bullet_bullet.gameManager = gameManager;
                bullet_bullet.Init();


                //�̵�
                bullet.transform.position = bulletStartPoint.position;
                //����
                bullet_rigid.velocity = (enemyVec - bullet.transform.position).normalized * bullet_bullet.bulletSpeed;
                //ȸ��
                Quaternion targetRotation = Quaternion.LookRotation(bullet_rigid.velocity);
                bullet.transform.rotation = targetRotation;

                //Ȱ��ȭ
                bullet_bullet.BulletOn(curTeamEnum);
            }
        }
        if(isShot)//��� ����� �ִ� ���, Ÿ���� ���̴��� ������ ����� �ٶ�
            RadarControl(enemyVec);
    }
    #endregion

    #region ȭ����
    void Tower_Flame()//���� ����� ��� �����ʿ� clickPoint �ڵ� ���� �ʿ�
    {
        //ȭ���� ȿ����
        audioManager.PlaySfx(AudioManager.Sfx.FlameSfx);

        GameObject bullet = objectManager.CreateObj("Tower_Flame", ObjectManager.PoolTypes.BulletPool);
        Bullet bullet_bullet = bullet.GetComponent<Bullet>();
        Rigidbody bullet_rigid = bullet.GetComponent<Rigidbody>();

        bullet_bullet.gameManager = gameManager;
        bullet_bullet.Init();

        //�̵�
        bullet.transform.position = UiManager.cameraCloud.position;
        //����
        bullet_rigid.velocity = (UiManager.clickSphere.position - bullet.transform.position).normalized * bullet_bullet.bulletSpeed;

        //Ȱ��ȭ
        bullet_bullet.BulletOn(curTeamEnum);
    }
    #endregion

    #region ��ȸ��
    void Tower_GrandCure()
    {
        //��ȸ�� ȿ����
        audioManager.PlaySfx(AudioManager.Sfx.GrandCureSfx);

        GameObject bullet = objectManager.CreateObj("Tower_GrandCure", ObjectManager.PoolTypes.BulletPool);
        Bullet bullet_bullet = bullet.GetComponent<Bullet>();

        bullet_bullet.gameManager = gameManager;
        bullet_bullet.Init();

        //�̵�
        bullet.transform.position = UiManager.clickSphere.position;
        //Ȱ��ȭ
        bullet_bullet.BulletOn(curTeamEnum);
    }
    #endregion

    #region ��ü����
    void Tower_CorpseExplosion()
    {
        //��ü���� ���� ȿ����
        audioManager.PlaySfx(AudioManager.Sfx.CorpseExplosionAdaptSfx);

        //Ÿ���� ���̴��� �� Ÿ���� �ٶ�
        RadarControl(enemyTower.transform.position);

        GameObject bomb = objectManager.CreateObj("Tower_CorpseExplosion", ObjectManager.PoolTypes.BulletPool);
        Bullet bomb_bullet = bomb.GetComponent<Bullet>();
        //��ü ������ �̵�
        bomb.transform.position = bulletStartPoint.position + bulletStartPoint.transform.forward * 3;
        //��ü ������ �� ����
        bomb_bullet.BulletOn(curTeamEnum);

        foreach (Transform obj in ourCreatureFolder)
        {
            if (obj.gameObject.layer == LayerMask.NameToLayer("Creature"))
            {
                //��ü ���� ������ Ȱ��ȭ
                Creature creature = obj.gameObject.GetComponent<Creature>();
                creature.CorpseExplosionObj.SetActive(true);
            }
        }      
    }
    #endregion
}
