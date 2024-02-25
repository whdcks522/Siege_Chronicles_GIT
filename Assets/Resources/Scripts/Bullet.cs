using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("�Ѿ��� �ִ� ����")]
    public float maxTime;
    [Header("�Ѿ��� �浹 ����")]
    public float colTime;
    float curTime = 0f;
    [Header("�Ѿ��� ���ط�")]
    public int bulletDamage;
    [Header("�Ѿ��� �ӵ�")]
    public float bulletSpeed;
    [Header("�Ѿ��� �浹 ����")]
    public Collider bulletCollider;

    [Header("�Ѿ��� ����")]
    public ParentAgent bulletHost;
    [Header("�Ѿ� ������ ���� ��")]
    public Creature.TeamEnum curTeamEnum;

    public enum BulletMoveEnum
    {
       Slash, Tracer, Canon
    }
    [Header("�Ѿ��� �̵� ���")]
    public BulletMoveEnum curBulletMoveEnum;

    [Header("����� ��, ����� ����Ʈ")]
    public string endStr;

    public GameManager gameManager;
    public ObjectManager objectManager;
    Rigidbody rigid;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        
        bulletCollider = GetComponent<Collider>();
        
    }

    public void Init()//���� ����
    {
        objectManager = gameManager.objectManager;
    }

    private void Update()
    {
        curTime += Time.deltaTime;
        if (curTime >= maxTime)
        {
            BulletOff();
        }
        else if(curTime >= colTime && curBulletMoveEnum == BulletMoveEnum.Slash)
        {
            //bulletCollider.enabled = false;
        }
    }

    #region �Ѿ� Ȱ��
    public void BulletOn(ParentAgent a)
    {
        //���� ����
        bulletHost = a;
        curTeamEnum = a.creature.curTeamEnum;
        //�ð� ����ȭ
        curTime = 0f;
        //�浹 ���� Ȱ��ȭ
        bulletCollider.enabled = true;
        //���ӿ�����Ʈ Ȱ��ȭ
        gameObject.SetActive(true);
    }
    #endregion

    #region �Ѿ� Ȱ��
    public void BulletOff()
    {
        //�Ѿ� ��Ȱ��ȭ
        gameObject.SetActive(false);
        
        if (curBulletMoveEnum != BulletMoveEnum.Slash) 
        {
            //�ı� �Ѿ� Ȱ��
            GameObject bomb = objectManager.CreateObj("shooter_A_Bomb", ObjectManager.PoolTypes.BulletPool);
            Bullet bomb_bullet = bomb.GetComponent<Bullet>();
            //�̵�
            bomb.transform.position = transform.position;
            //Ȱ��ȭ
            bomb_bullet.BulletOn(bulletHost);
            

        }
    }
    #endregion

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Untagged")) //�ʰ� �浹�ϸ� ����
        {
            if (curBulletMoveEnum != BulletMoveEnum.Slash)
            {
                Debug.LogError(gameObject.name + " �浹 ����: " + other.gameObject.name);
                //����
                bulletHost.AddReward(-0.1f);
                //�Ѿ� ��Ȱ��ȭ
                BulletOff();
            }
        }
    }
}