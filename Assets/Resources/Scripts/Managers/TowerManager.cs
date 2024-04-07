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

    [Header("Ÿ���� ũ���� ���� ����")]
    public int curCreatureCount = 0;    //ũ������ ���� ��
    public int maxCreatureCount = 8;    //ũ������ �ִ� ��

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

    private void Update()//Update������ ���ʸ��� ����
    {
        if (maxTowerResource > curTowerResource)
        {
            //���� ����� ���� �ڿ� ����
            //Debug.LogWarning(BankSpeedArr[curBankIndex]);
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
    }

    #region Ÿ�� ��� �ʱ�ȭ;

    [Header("��ũ ���� ��ҵ�")]
    public int[] BankValueArr = { 5, 6, 7, 8 };//��ũ ��ư�� ������ ���� �ʿ��� ��� �迭
    float[] BankSpeedArr = { 0.6f, 0.7f, 0.8f, 0.9f, 1f };//��ũ ��ư�� ������ �ڿ��� �����ϰ� �Ǵ� �ӵ� �迭
    public int curBankIndex = 0;//���� ��ũ ������ ������

    public void ResetTower() 
    {
        //Ÿ�� ü�� �ʱ�ȭ
        curHealth = maxHealth;
        //Ÿ�� �ڿ� �ʱ�ȭ
        curTowerResource = 0;
        //Ÿ�� ���� �ʱ�ȭ
        curBankIndex = 0;
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
        if(isShot)
            RadarControl(enemyVec);
        else if (!isShot)
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

            }
        }
    }
    #endregion
}
