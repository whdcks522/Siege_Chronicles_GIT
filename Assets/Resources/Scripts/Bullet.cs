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
    float curTime = 0f;//�Ѿ��� ���� ����
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


    public enum BulletMoveEnum//Slash: ��������(������� ��������, �浹�ص� �Ȼ����), Tracer(�������� ��� �Ѿ�),Canon
    {
        Slash, Tracer, Canon
    }
    [Header("�Ѿ��� �̵� ���")]
    public BulletMoveEnum curBulletMoveEnum;

    public enum BulleEffectEnum
    {
        Damage, Cure
    }
    [Header("�Ѿ��� Ư��ȿ�� ���")]
    public BulleEffectEnum curBulletEffectEnum;

    [Header("����� ��, �����Ǵ� �Ѿ�")]
    public Transform endBullet;

    [Header("�Ŵ�����")]
    public GameManager gameManager;
    public ObjectManager objectManager;

    [Header("ũ������ �Ѿ�����")]
    public bool isCreature;

    private void Awake()//���� ����
    {
        bulletCollider = GetComponent<Collider>();
    }

    public void Init()//���� ����
    {
        objectManager = gameManager.objectManager;
    }

    private void FixedUpdate()//�� �����Ӹ��� �ݺ�
    {
        curTime += Time.deltaTime;
        if (curTime >= maxTime)
        {
            BulletOff();
        }
        else if (curTime >= colTime && curBulletMoveEnum == BulletMoveEnum.Slash)
        {
            bulletCollider.enabled = false;
        }
    }

    #region �Ѿ� Ȱ��
    public void BulletOnByCreature(Creature _creature)//ũ���Ŀ� ���ؼ� �Ѿ� Ȱ��
    {
        //���ΰ� �� ����
        bulletHost = _creature;
        curTeamEnum = _creature.curTeamEnum;

        BulletOn();
    }

    public void BulletOnByTower(Creature.TeamEnum teamEnum)//Ÿ���� ���ؼ� �Ѿ� Ȱ��
    {
        //�� ����
        curTeamEnum = teamEnum;

        BulletOn();
    }

    public void BulletOn()
    {
        //�ð� ����ȭ
        curTime = 0f;
        //�浹 ���� Ȱ��ȭ
        bulletCollider.enabled = true;
        //���ӿ�����Ʈ Ȱ��ȭ
        gameObject.SetActive(true);
    }
    #endregion

    #region �Ѿ� ��Ȱ��
    public void BulletOff()
    {
        //�ڽ� �Ѿ� ����
        EndBulletOn();

        //�Ѿ� ��Ȱ��ȭ
        gameObject.SetActive(false);
    }
    #endregion

    #region �ڽ� �Ѿ� ����
    public void EndBulletOn()
    {
        if (endBullet != null && gameObject.activeSelf)
        {
            //�ı� �Ѿ� Ȱ��
            string bulletName = endBullet.name;

            GameObject bomb = objectManager.CreateObj(bulletName, ObjectManager.PoolTypes.BulletPool);
            Bullet bomb_bullet = bomb.GetComponent<Bullet>();
            //�̵�
            bomb.transform.position = transform.position;
            //Ȱ��ȭ
            if (isCreature)
                bomb_bullet.BulletOnByCreature(bulletHost);
            else if (!isCreature)
                bomb_bullet.BulletOnByTower(curTeamEnum);
        }
    }
    #endregion

    private void OnTriggerEnter(Collider other)//�ҷ��� ���𰡿� �浹��
    {
        if (other.transform.CompareTag("Untagged") && other.gameObject.name != "InvisibleWall") //�ʰ� �浹
        {
            if (curBulletMoveEnum == BulletMoveEnum.Canon)//ĳ��(=ȭ����)
            {
                BulletOff();
            }
            else if (curBulletMoveEnum == BulletMoveEnum.Tracer)//�Ѿ�(=���, ���)
            {
                EndBulletOn();
            }
        }
    }
}