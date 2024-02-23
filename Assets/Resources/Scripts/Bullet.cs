using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("�Ѿ��� �ִ� ����")]
    public float maxTime;
    float curTime = 0f;

    [Header("�Ѿ��� ���ط�")]
    public int bulletDamage;

    [Header("�Ѿ��� �ӵ�")]
    public float bulletSpeed;

    [Header("�Ѿ��� ����")]
    public ParentAgent bulletHost;

    public enum BulletEnum
    {
        Normal, PowerUp, UnBreakable, Chase
    }
    [Header("�Ѿ��� Ư��ȿ��")]
    public BulletEnum bulletEnum;

    [Header("���� ��, �÷��� ����Ʈ ��� ����")]
    public bool isFlash;
    public string flashStr;
    [Header("���� ��, ��Ʈ ����Ʈ ��� ����")]
    public bool isHit;
    public string hitStr;

    Rigidbody rigid;
}
