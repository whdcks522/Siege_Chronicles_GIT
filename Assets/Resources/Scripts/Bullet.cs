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
       Melee, Tracer, Canon
    }
    [Header("�Ѿ��� �̵� ���")]
    public BulletMoveEnum curBulletMoveEnum;

    [Header("����� ��, ����� ����Ʈ")]
    public string endStr;

    public GameManager gameManager;
    ObjectManager objectManager;
    Rigidbody rigid;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        if (curBulletMoveEnum == BulletMoveEnum.Canon) 
            rigid.useGravity = true;
        bulletCollider = GetComponent<Collider>();
    }

    private void Update()
    {
        curTime += Time.deltaTime;
        if (curTime > maxTime)
        {
            gameObject.SetActive(false);
        }
        else if(curTime > colTime)
        {
            bulletCollider.enabled = false;
        }
    }

    #region �Ѿ� Ȱ�� ����ȭ
    public void BulletOn(ParentAgent _parentAgent)
    {
        //�θ� ����
        bulletHost = _parentAgent;
        curTeamEnum = bulletHost.creature.curTeamEnum;
        //ȸ�� �ʱ�ȭ
        //transform.rotation = Quaternion.identity;
        //�ð� ����ȭ
        curTime = 0f;
        //�浹 ���� Ȱ��ȭ
        bulletCollider.enabled = true;
        //���ӿ�����Ʈ Ȱ��ȭ
        gameObject.SetActive(true);
    }
    #endregion


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.CompareTag("Outline")) //�� ������ �������� ����
        {
            if (curBulletMoveEnum != BulletMoveEnum.Melee) 
            {
                gameObject.SetActive(true);
            }
        }
    }
}