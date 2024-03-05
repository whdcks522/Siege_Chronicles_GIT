using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
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


    float breakPoint = 20f;
    void damageControl(Bullet bullet)
    {
        //���ط� Ȯ��
        Agent bulletAgent = bullet.bulletHost.agent;
        float damage = bullet.bulletDamage;

        float attackPoint = damage / 10f;
        /*
        //curHealth -= damage;
        if (curHealth < 0) curHealth = 0;
        else if (curHealth > maxHealth) curHealth = maxHealth;

        //UI����
        miniHealth.fillAmount = curHealth / maxHealth;
        */

        //��� �ʱ�ȭ
        if (curHealth > 0)//�ǰ��ϰ� ��� ����
        {
            //������ ���� ����
            bulletAgent.AddReward(attackPoint);
            //Debug.Log("�� ����");
        }
        else if (curHealth <= 0) //�ı� �Ϸ��
        {
            //bulletAgent.AddReward(breakPoint);

            //�ó����� ����
            //bulletAgent.EndEpisode();


            aiManager.MlCreature.resetEnv();
        }

    }
    /*
    void Dead() 
    {
        
        if (aiManager.isML)
        {
            //��� �ʱ�ȭ
            if (curTeamEnum == TeamEnum.Blue)//�Ķ� Ÿ���� ����
            {
                //aiManager.AiEnd(-1);//���� ����
            }
            if (curTeamEnum == TeamEnum.Red)//���� Ÿ���� ����
            {
                //aiManager.AiEnd(1);//�Ķ� ����
            }
        }
        
    }
*/
    public void TowerOn() //Ÿ�� Ȱ��ȭ
    {
        curHealth = maxHealth;
        miniHealth.fillAmount = 1;
    }
}
