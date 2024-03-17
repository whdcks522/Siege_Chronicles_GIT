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
            enemyCreatureFolder = gameManager.objectManager.redCreatureFolder;
        }
        else if (curTeamEnum == TeamEnum.Red)
        {
            miniHealth.color = Color.red;
            enemyTower = gameManager.blueTower;
            enemyCreatureFolder = gameManager.objectManager.blueCreatureFolder;
        }
    }



    //ī�޶� ȸ����
    Vector3 cameraVec;
    Quaternion lookRotation;

    [Header("������� ����ִ� ����")]
    public Transform enemyCreatureFolder;

    //�÷��̾��� ���� �ڿ�
    public float curTowerResource = 0f;
    //�÷��̾��� �ִ� �ڿ�
    public float maxTowerResource = 10f;

    private void Update()
    {
        if (maxTowerResource > curTowerResource)
        {
            //���� ����� ���� �ڿ� ����
            curTowerResource += Time.deltaTime;
        }
        else if (maxTowerResource <= curTowerResource)
        {
            //���� �ڿ����� �ִ�ġ�� ���� �ʵ���
            maxTowerResource = curTowerResource;
        }
        

        // ��ü A���� B�� �ٶ󺸴� ȸ�� ���ϱ�
        cameraVec = cameraGround.transform.position - mainCamera.transform.position;
        lookRotation = Quaternion.LookRotation(cameraVec);

        // ��ü C�� ȸ�� ����
        miniCanvas.transform.rotation = lookRotation;
    }



    private void OnTriggerEnter(Collider other)
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
                    bullet.BulletOff();
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

        if (!gameManager.isML)
        {
            curHealth -= damage;

            if (curHealth < 0) curHealth = 0;
            else if (curHealth > maxHealth) curHealth = maxHealth;

            //UI����
            miniHealth.fillAmount = curHealth / maxHealth;

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


    #region ���� ��� �з�

    Vector3 enemyVec;

    public void WeaponSort(string tmpWeaponName) 
    {
        if (tmpWeaponName == gameManager.Gun.name) Tower_Gun();
        else if (tmpWeaponName == gameManager.Flame.name) Tower_Flame();
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
                bullet_bullet.BulletOnByTower(Creature.TeamEnum.Blue);
            }
        }
        if(isShot)
            RadarControl(enemyVec);
        else if (!isShot)
            RadarControl(enemyTower.transform.position);
    }
    #endregion

    #region ���̾
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
        bullet_bullet.BulletOnByTower(Creature.TeamEnum.Blue);
    }
    #endregion
}
