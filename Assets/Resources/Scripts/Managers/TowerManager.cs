using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;
using UnityEngine.UI;
using static Creature;
using static Unity.Barracuda.BurstCPUOps;

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
    
    Transform cameraGround;
    Transform mainCamera;
    private void Awake()
    {
        objectManager = gameManager.objectManager;
        UiManager = gameManager.uiManager;
        audioManager = gameManager.audioManager;
        mainCamera = UiManager.cameraObj;
        cameraGround = UiManager.cameraGround;

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

        if (gameManager.isML) 
        {
            bulletStartPoint.gameObject.SetActive(false);
        }
    }

    //ī�޶� ȸ����
    Vector3 cameraVec;
    Quaternion lookRotation;

    [Header("������� ����ִ� ����(����_��ü���߿�)")]
    public Transform ourCreatureFolder;
    [Header("������� ����ִ� ����(����_��ݿ�)")]
    public Transform enemyCreatureFolder;

    [Header("Ÿ���� �ڿ� ����")]
    public float curTowerResource = 0f;     //�÷��̾��� ���� �ڿ�
    public float maxTowerResource = 10f;    //�÷��̾��� �ִ� �ڿ�

    [Header("�� Ÿ���� ������ ����� ���� ������")]
    public SpellData futureSpellData;

    private void Update()//Update�� ���ʸ��� ����
    {
        if (gameManager.isBattle && !gameManager.isML) 
        {
            if (maxTowerResource > curTowerResource)
            {
                //���� ����� ���� �ڿ� ����
                curTowerResource += Time.deltaTime * BankSpeedArr[curBankIndex];
            }
            else if (maxTowerResource <= curTowerResource)
            {
                //���� �ڿ����� �ִ�ġ�� ���� �ʵ���
                curTowerResource = maxTowerResource;
            }

            // ��ü A���� B�� �ٶ󺸴� ȸ�� ���ϱ�
            cameraVec = cameraGround.transform.position - mainCamera.transform.position;
            lookRotation = Quaternion.LookRotation(cameraVec);

            // ĵ�۽��� ȸ�� ����
            miniCanvas.transform.rotation = lookRotation;

            if (curTeamEnum == TeamEnum.Red) //���� �� Ÿ������
            {
                if (futureSpellData != null)//��ȯ�� ���� �������ٸ�
                {
                    Debug.Log("���: " + futureSpellData.spellValue);
                    if (curTowerResource >= futureSpellData.spellValue && CreatureCountCheck())//�ڿ��� ����ϸ鼭 �ڽ��� ũ���� ��ȯ ���ΰ� ����� ��
                    {
                        Debug.Log("��ȯ: " + futureSpellData.spellPrefab.name);
                        //ũ���� ��ȯ
                        //SpawnCreature(futureSpellData.spellPrefab.name);
                        //�ٸ� ���� ��ȯ�ϱ� ���� �ʱ�ȭ
                        futureSpellData = null;
                    }
                }
                else if (futureSpellData == null)//��ȯ�� ���� �������� �ʾҴٸ�
                {
                    //� ũ���ĸ� ��ȯ�� �� �������� ����
                    int r = Random.Range(0, gameManager.creatureSpellDataArr.Length);
                    futureSpellData = gameManager.creatureSpellDataArr[r];
                }
            }
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

    public bool CreatureCountCheck()//��ȯ �μ� ���� ��, ���� ���� ��ȯ
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
            //��ȯ ���� �ؽ�Ʈ ����
            UiManager.creatureCountText.text = curCreatureCount.ToString() + "/" + gameManager.maxCreatureCount.ToString();
            //��ȯ ���� �ؽ�Ʈ ���� �ִϸ��̼�

        }

        return canSpawn;
    }

    public void CreatureCount() 
    {
    
    }
    #endregion

    private void OnTriggerEnter(Collider other)//�ε���
    {
        if (other.gameObject.CompareTag("Bullet"))//�Ѿ˰� �浹
        {
            Bullet bullet = other.gameObject.GetComponent<Bullet>();

            if (bullet.curTeamEnum != curTeamEnum)//���� �ٸ� ��츸 ���� ó��
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
    void DamageControl(Bullet bullet)
    {
        //���ط� Ȯ��
        float damage = bullet.bulletDamage;

        if (bullet.isCreature)
        {
            Agent bulletAgent = bullet.bulletHost.agent;
            //������ ���� ����
            bulletAgent.AddReward(damage / 10f);
        }

        if (!gameManager.isML)//�ӽŷ����� �ƴѰ��
        {
            curHealth -= damage;

            if (curHealth < 0) curHealth = 0;
            else if (curHealth > maxHealth) curHealth = maxHealth;

            //Ÿ�� ü�¹� ����
            miniHealth.fillAmount = curHealth / maxHealth;
            //Ÿ�� �ǰ� ȿ����
            if(damage != 0)
                audioManager.PlaySfx(AudioManager.Sfx.TowerCrashSfx);

            if (curHealth <= 0) //���� ����
            {
                
            }
        }
    }
    #endregion

    //���� ���� ���� ����
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
        //������ ��� �ߴ��� ����
        bool isShot = false;

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
                bullet_bullet.BulletOnByTower(curTeamEnum);
            }
        }
        if(isShot)//��� ����� �ִ� ���, Ÿ���� ���̴��� ������ ����� �ٶ�
            RadarControl(enemyVec);
        else if (!isShot)//����� ���� ���, Ÿ���� ���̴��� �� Ÿ���� �ٶ�
            RadarControl(enemyTower.transform.position);
    }
    #endregion

    #region ȭ����
    void Tower_Flame()//���� ����� ��� �����ʿ� clickPoint �ڵ� ���� �ʿ�
    {
        GameObject bullet = objectManager.CreateObj("Tower_Flame", ObjectManager.PoolTypes.BulletPool);
        Bullet bullet_bullet = bullet.GetComponent<Bullet>();
        Rigidbody bullet_rigid = bullet.GetComponent<Rigidbody>();

        bullet_bullet.gameManager = gameManager;
        bullet_bullet.Init();

        //�̵�
        bullet.transform.position = UiManager.cameraCloud.position;
        //����
        bullet_rigid.velocity = (UiManager.clickPoint.position - bullet.transform.position).normalized * bullet_bullet.bulletSpeed;

        //Ȱ��ȭ
        bullet_bullet.BulletOnByTower(curTeamEnum);
    }
    #endregion

    #region ��ȸ��
    void Tower_GrandCure()
    {
        GameObject bullet = objectManager.CreateObj("Tower_GrandCure", ObjectManager.PoolTypes.BulletPool);
        Bullet bullet_bullet = bullet.GetComponent<Bullet>();
        //Rigidbody bullet_rigid = bullet.GetComponent<Rigidbody>();

        bullet_bullet.gameManager = gameManager;
        bullet_bullet.Init();

        //�̵�
        bullet.transform.position = UiManager.clickPoint.position;
        //Ȱ��ȭ
        bullet_bullet.BulletOnByTower(curTeamEnum);
    }
    #endregion

    #region ��ü����
    void Tower_CorpseExplosion()
    {
        foreach (Transform obj in ourCreatureFolder)
        {
            if (obj.gameObject.layer == LayerMask.NameToLayer("Creature"))
            {
                //��ü ���� ������ Ȱ��ȭ
                Creature creature = obj.gameObject.GetComponent<Creature>();
                creature.CorpseExplosionObj.SetActive(true);
            }
        }

        //Ÿ���� ���̴��� �� Ÿ���� �ٶ�
        RadarControl(enemyTower.transform.position);
    }
    #endregion
}
