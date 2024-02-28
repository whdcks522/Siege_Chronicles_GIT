using System.Collections;
using System.Collections.Generic;
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
    UIManager UIManager;
    Transform cameraGround;
    Transform mainCamera;
    AiManager aiManager;
    private void Awake()
    {
        UIManager = gameManager.uiManager;
        mainCamera = UIManager.cameraObj;
        cameraGround = UIManager.cameraGround;
        aiManager = gameManager.aiManager;


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
    [Header("���� ����� ���� ��ġ")]
    public Transform curTarget;
    [Header("���� ����� ������ �Ÿ�")]
    public float curRange;

    private void Update()
    {
        //Ÿ������ ���� ����� �� �ľ�
        curRange = (enemyTower.position - transform.position).magnitude - 2;//Ÿ���� �β� ���
        curTarget = enemyTower;

        
        for (int i = 0; i < enemyCreatureFolder.childCount; i++)
        {
            if (enemyCreatureFolder.GetChild(i).gameObject.layer == LayerMask.NameToLayer("Creature"))//Ȱ��ȭ���ִٸ�
            {
                //������ �Ÿ�
                float tmpRange = (enemyCreatureFolder.GetChild(i).position - transform.position).magnitude;
                if (curRange > tmpRange)
                {
                    curRange = tmpRange;
                    curTarget = enemyCreatureFolder.GetChild(i);
                }

            }
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
                //���ط� Ȯ��
                ParentAgent bulletParentAgent = bullet.bulletHost;
                float damage = bullet.bulletDamage;

                

                float winPoint = damage / 10f;
                float loosePoint = -damage / 20f;
                //Debug.Log("winPoint: " + winPoint + " / loosePoint: " + loosePoint);

                //������ ���� ����
                bulletParentAgent.AddReward(winPoint * 2f);

                //�� �� ���� ����
                if (curTeamEnum == TeamEnum.Blue)//�Ķ� Ÿ���� �ǰݴ���
                {
                    aiManager.blueAgentGroup.AddGroupReward(loosePoint);//�Ķ� ����
                    aiManager.redAgentGroup.AddGroupReward(winPoint);//���� ����
                }
                else if (curTeamEnum == TeamEnum.Red)//���� Ÿ���� �ǰݴ���
                {
                    aiManager.blueAgentGroup.AddGroupReward(winPoint);//�Ķ� ����
                    aiManager.redAgentGroup.AddGroupReward(loosePoint);//���� ����
                }
                //���� ����
                damageControl(damage);

                //�ǰ��� �Ѿ� ��ó��
                if (bullet.curBulletMoveEnum != Bullet.BulletMoveEnum.Slash)
                    bullet.BulletOff();
            }
        }
    }

    void damageControl(float _dmg)
    {

        curHealth -= _dmg;
        if (curHealth < 0) curHealth = 0;
        else if (curHealth > maxHealth) curHealth = maxHealth;

        //UI����
        miniHealth.fillAmount = curHealth / maxHealth;

        //��� �ʱ�ȭ
        if (curHealth > 0)//�ǰ��ϰ� ��� ����
        {
            
        }
        else if (curHealth <= 0) Dead();
    }

    void Dead() 
    {
        if (aiManager.isML)
        {
            //��� �ʱ�ȭ
            if(curTeamEnum == TeamEnum.Blue)//�Ķ� Ÿ���� ����
                aiManager.AiEnd(-1);//���� ����
            if (curTeamEnum == TeamEnum.Red)//���� Ÿ���� ����
                aiManager.AiEnd(1);//�Ķ� ����
        }
    }

    public void TowerOn() //Ÿ�� Ȱ��ȭ
    {
        curHealth = maxHealth;
        miniHealth.fillAmount = 1;
    }
}
