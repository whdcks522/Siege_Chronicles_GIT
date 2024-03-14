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

    private void Update()
    {
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
                damageControl(bullet);

                //�ǰ��� �Ѿ� ��ó��
                if (bullet.curBulletMoveEnum != Bullet.BulletMoveEnum.Slash)
                    bullet.BulletOff();
            }
        }
    }


    #region ������ ���
    void damageControl(Bullet bullet)
    {
        //���ط� Ȯ��
        Agent bulletAgent = bullet.bulletHost.agent;
        float damage = bullet.bulletDamage;

        float damagePoint = damage / 8f;

        if (!gameManager.isML)
        {
            curHealth -= damage;

            if (curHealth < 0) curHealth = 0;
            else if (curHealth > maxHealth) curHealth = maxHealth;

            //UI����
            miniHealth.fillAmount = curHealth / maxHealth;
        }


        //��� �ʱ�ȭ
        if (curHealth > 0)//Ÿ���� �ǰ��ϰ� ��� ����
        {
            //������ ���� ����
            bulletAgent.AddReward(damagePoint);
        }
        else if (curHealth <= 0) //���� ����
        {

        }
    }
    #endregion

    #region ���� ����
    public void RadarControl(Transform targetPos)
    {
        //�ݻ�ü ȸ��
        bulletStartPoint.transform.LookAt(targetPos);
        /*
        if (!isDirect)
        {
            //�ݻ�ü ȸ��
            bulletStartPoint.transform.LookAt(targetPos);
            bulletStartPoint.transform.rotation = Quaternion.Euler(-61.503f, bulletStartPoint.transform.rotation.eulerAngles.y, 180);
        }
        */
        /*
        if(curTime >= maxTime) 
        {
            curTime = 0;

            //string bulletName = "Tower_Gun";
            string bulletName = "Tower_Flame";

            GameObject bullet = objectManager.CreateObj(bulletName, ObjectManager.PoolTypes.BulletPool);
            Bullet bullet_bullet = bullet.GetComponent<Bullet>();
            Rigidbody bullet_rigid = bullet.GetComponent<Rigidbody>();

            bullet_bullet.gameManager = gameManager;
            bullet_bullet.Init();


            //�̵�
            bullet.transform.position = bulletStartPoint.position + bulletStartPoint.forward * 5;
            //����
            bullet_rigid.velocity = (targetPos.position - bulletStartPoint.position).normalized * bullet_bullet.bulletSpeed;
            //ȸ��
            Quaternion targetRotation = Quaternion.LookRotation(bullet_rigid.velocity);
            bullet.transform.rotation = targetRotation;

            //Ȱ��ȭ
            bullet_bullet.BulletOnByTower(curTeamEnum);

        }
        */
    }
    #endregion
}
