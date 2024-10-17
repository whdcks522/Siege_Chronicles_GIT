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
    public int creatureSpawnIndex = 0;
    IEnumerator UpdateCoroutine()
    {
        while (true)
        {
            if (UiManager.selectManager.index_Battle == 1)
            {
                if (maxTowerResource > curTowerResource)
                {
                    //���� ����� ���� �ڿ� ����
                    curTowerResource += gameManager.BankSpeedArr[curBankIndex] * 0.1f;
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
                            else 
                            {
                                curCreatureCount -= 1;
                            }

                            //��� ó��
                            curTowerResource -= futureSpellData.spellValue;

                            //�ٸ� ���� ��ȯ�ϱ� ���� �ʱ�ȭ
                            futureSpellData = null;

                        }
                    }
                    else if (futureSpellData == null)//��ȯ�� ���� �������� �ʾҴٸ�, � ũ���ĸ� ��ȯ�� �� �������� ����
                    {
                        /*
                        int createIndex = UnityEngine.Random.Range(0, gameManager.creatureSpellDataArr.Length - 1);

                        //0, 1, 2�߿��� �¾Ұ� 2�� ���� ��츸 ��ȯ

                        if (!recentHit && createIndex == 2) //������ ����� ���� ��ȯ�ؾ� �ϹǷ�
                        {
                            createIndex = 0;
                        }

                        futureSpellData = gameManager.creatureSpellDataArr[createIndex];
                        recentHit = false;
                        */

                        if (recentHit)//�ֱٿ� �¾����� 
                        {
                            futureSpellData = gameManager.creatureSpellDataArr[2];
                            recentHit = false;
                        }
                        else //�ȸ¾�����, �������� - ����� ������ ��ȯ
                        {
                            creatureSpawnIndex = (creatureSpawnIndex + 1) % 2;
                            futureSpellData = gameManager.creatureSpellDataArr[creatureSpawnIndex];
                        }
                    }
                }
            }

            yield return waitSec;
        }
    }
    public bool recentHit = false;//public �׽�Ʈ��

    #region Ÿ�� ��� �ʱ�ȭ;

    [Header("��ũ ���� ��ҵ�")]
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

        //��ȯ ��� �ʱ�ȭ
        recentHit = false;
        creatureSpawnIndex = 0;
        futureSpellData = null;
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

        if (curCreatureCount == gameManager.maxCreatureCount)
        {
            UiManager.creatureCountText.color = UiManager.textRed;
        }
        else 
        {
            UiManager.creatureCountText.color = UiManager.textYellow;
        }
    }
    #endregion

    GameObject damageFont = null;//������ ��Ʈ�� ���� ������Ʈ
    private void OnTriggerEnter(Collider other)//�ε���
    {
        if (other.gameObject.CompareTag("Bullet"))//�Ѿ˰� �浹
        {
            Bullet bullet = other.gameObject.GetComponent<Bullet>();
            //���ط� Ȯ��
            float damage = bullet.bulletDamage;

            if (bullet.curTeamEnum != curTeamEnum && bullet.byCreature && damage != 0)//���� �ٸ� ���鼭 ũ���Ŀ� ���� ���ݸ� ���� ó��
            {
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
                damageFont.transform.position = other.transform.position;
                damageFont.GetComponent<DamageFont>().ReName(damage.ToString());

                //���� ����
                DamageControl(damage);

                //�ǰ��� �Ѿ� ��ó��
                if (bullet.curBulletMoveEnum != Bullet.BulletMoveEnum.Slash)
                {
                    bullet.BulletOff();
                }
            }
        }
    }

    #region ������ ���
    public void DamageControl(float damage)
    {
        //�ֱٿ� ���� ���� Ȯ��(���� ��ȯ�� ����)
        recentHit = true;

        //ü�� ����
        curHealth -= damage;

        //Ÿ�� ü�¹� ����
        miniHealth.fillAmount = curHealth / maxHealth;

        //Debug.LogWarning("�����?");

        if (curHealth > 0)//Ÿ���� ���ݹ޴� ��
        {
            if (damage != 0)
                audioManager.PlaySfx(AudioManager.Sfx.TowerCrashSfx);
        }
        else if (curHealth <= 0 && !UiManager.settingBackground.activeSelf) //Ÿ���� ü���� 0�̸�, ���� ȭ���� ���� �ִ� ���
        {
            //Debug.LogWarning("������ ȣ��ǳ�?");

            curHealth = 0;

            if (curTeamEnum == Creature.TeamEnum.Red)//���� ���� �� ���
            {
                //���� ȭ���� �ؽ�Ʈ ����
                UiManager.victoryTitle.SetActive(true);

                //�¸� ȿ����
                audioManager.PlayBgm(AudioManager.Bgm.WinBgm);

                //�ҷ��ö� ���� ��ٸ�
                gameManager.fireManager.StartCor();
            }
            else if (curTeamEnum == Creature.TeamEnum.Blue) //�Ķ� ���� �� ���
            {
                //�̹� ������̶��, ����
                if (Tower_Gun_Cor != null)
                    StopCoroutine(Tower_Gun_Cor);

                //���� ȭ���� �ؽ�Ʈ ����
                UiManager.defeatTitle.SetActive(true);

                //�й� ȿ����
                audioManager.PlayBgm(AudioManager.Bgm.LoseBgm);
            }
            //���� ���Ḧ ���� ���� ��Ű��
            UiManager.SettingControl(true);

            //���� ȭ���� ���� ��ư ��Ȱ��ȭ
            UiManager.setBtn.interactable = false;
            UiManager.setBtn.transform.GetChild(0).GetComponent<Text>().color = UiManager.textRed;
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
        if (tmpWeaponName == gameManager.Gun.name) 
        {
            //�̹� ������̶��, ����
            if (Tower_Gun_Cor != null)
                StopCoroutine(Tower_Gun_Cor);

            //���
            Tower_Gun_Cor = StartCoroutine(Tower_Gun());
        } 
        else if (tmpWeaponName == gameManager.Flame.name) Tower_Flame();
        else if (tmpWeaponName == gameManager.GrandCure.name) Tower_GrandCure();
        else if (tmpWeaponName == gameManager.CorpseExplosion.name) Tower_CorpseExplosion();
    }
    #endregion


    #region �̴ϰ� ����

    //[Header("����� ���� ����ִ� �� ũ���� ����Ʈ")]
    List<GameObject> gunList = new List<GameObject>();
    Coroutine Tower_Gun_Cor;
    readonly int maxBulletCount = 6;//�̴ϰ��� �߻� ��

    IEnumerator Tower_Gun() 
    {
        //���̴��� �� Ÿ���� �Ĵٺ�
        RadarControl(enemyTower.transform.position);

        //������ �Ѿ��� ������ n��
        int bulletCount = 0;

        gunList.Clear();
        //�� ũ���� ��ġ �ľ�
        for (int i = 0; i < enemyCreatureFolder.childCount; i++)
        {
            if (enemyCreatureFolder.GetChild(i).gameObject.activeSelf && //����ִ� ���
                enemyCreatureFolder.GetChild(i).gameObject.layer == LayerMask.NameToLayer("Creature")) //ũ���� ���̾��� ��츸(�ȱ׷� �ܻ� ��)
            {
                //����Ʈ�� ���ϱ�
                gunList.Add(enemyCreatureFolder.GetChild(i).gameObject);
            }
        }

        if (gunList.Count > 0) 
        {
            //Debug.LogWarning("����Ʈ�� ũ��: "+ gunList.Count);

            gunList.Sort((a, b) => Vector3.Distance(transform.position, a.transform.position)
                               .CompareTo(Vector3.Distance(transform.position, b.transform.position)));

            while (bulletCount < maxBulletCount)
            {
                //��� ȿ����
                audioManager.PlaySfx(AudioManager.Sfx.GunSfx);

                //Ÿ���� ���� �Ĵٺ�����
                enemyVec = gunList[bulletCount % gunList.Count].transform.position;
                
                //����� �ٶ� ������ ����
                RadarControl(enemyVec);

                GameObject bullet = objectManager.CreateObj("Tower_Gun", ObjectManager.PoolTypes.BulletPool);
                Bullet bullet_bullet = bullet.GetComponent<Bullet>();
                Rigidbody bullet_rigid = bullet.GetComponent<Rigidbody>();

                bullet_bullet.gameManager = gameManager;
                bullet_bullet.Init();


                //�Ѿ� ������� ����(���⼭�� ����)
                bullet_bullet.bulletTarget = gunList[bulletCount % gunList.Count].gameObject;
                //�Ѿ� ��ġ ����
                bullet.transform.position = bulletStartPoint.position + bulletStartPoint.transform.forward * 10;
                //�Ѿ� ���� ����
                bullet_rigid.velocity = (enemyVec - bullet.transform.position).normalized * bullet_bullet.bulletSpeed;
                //�Ѿ� ȸ�� ����
                Quaternion targetRotation = Quaternion.LookRotation(bullet_rigid.velocity);
                bullet.transform.rotation = targetRotation;


                //�Ѿ� Ȱ��ȭ
                bullet_bullet.BulletOn(curTeamEnum);

                bulletCount++;

                //Debug.LogWarning(bulletCount);

                //�� 0.1�� ���
                yield return waitSec;
                yield return waitSec;
            }
        } 
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
