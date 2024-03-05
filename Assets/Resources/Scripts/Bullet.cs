using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
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
    public Creature bulletHost;
    [Header("�Ѿ� ������ ���� ��")]
    public Creature.TeamEnum curTeamEnum;

    public enum BulletMoveEnum
    {
       Slash, Tracer, Canon
    }
    [Header("�Ѿ��� �̵� ���")]
    public BulletMoveEnum curBulletMoveEnum;

    [Header("����� ��, �����Ǵ� �Ѿ�")]
    public Transform endBullet;

    public GameManager gameManager;
    public ObjectManager objectManager;
    Rigidbody rigid;

    bool isAlreadyHit = false;

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
            bulletCollider.enabled = false;
        }
    }

    #region �Ѿ� Ȱ��
    public void BulletOn(Creature _creature)
    {
        //���� ����
        bulletHost = _creature;
        curTeamEnum = _creature.curTeamEnum;
        //�ð� ����ȭ
        curTime = 0f;
        //�̹� �浹�ߴ��� Ȯ�� �� �ʱ�ȭ
        isAlreadyHit = false;
        //�浹 ���� Ȱ��ȭ
        bulletCollider.enabled = true;
        //���ӿ�����Ʈ Ȱ��ȭ
        gameObject.SetActive(true);
    }
    #endregion

    #region �Ѿ� ��Ȱ��
    public void BulletOff()
    {
        //�Ѿ� ��Ȱ��ȭ
        gameObject.SetActive(false);
        
        if (curBulletMoveEnum != BulletMoveEnum.Slash) 
        {
            //�ı� �Ѿ� Ȱ��
            string bulletName = endBullet.name;

            GameObject bomb = objectManager.CreateObj(bulletName, ObjectManager.PoolTypes.BulletPool);
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
        if (other.transform.CompareTag("Untagged") && !isAlreadyHit) //�ʰ� �浹�ϸ� ����
        {
            //������ �浹 ����
            isAlreadyHit = true;

            //����
            bulletHost.agent.AddReward(- bulletDamage / 50f);
            if (curBulletMoveEnum != BulletMoveEnum.Slash)
            {
                //�Ѿ� ��Ȱ��ȭ
                BulletOff();
            }
        }
    }
}