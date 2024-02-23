using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("총알의 최대 수명")]
    public float maxTime;
    float curTime = 0f;

    [Header("총알의 피해량")]
    public int bulletDamage;

    [Header("총알의 속도")]
    public float bulletSpeed;

    [Header("총알의 주인")]
    public ParentAgent bulletHost;

    public enum BulletEnum
    {
        Normal, PowerUp, UnBreakable, Chase
    }
    [Header("총알의 특수효과")]
    public BulletEnum bulletEnum;

    [Header("시작 시, 플래시 이펙트 사용 여부")]
    public bool isFlash;
    public string flashStr;
    [Header("종료 시, 히트 이펙트 사용 여부")]
    public bool isHit;
    public string hitStr;

    Rigidbody rigid;
}
