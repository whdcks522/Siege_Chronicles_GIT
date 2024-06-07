using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("총알의 최대 수명")]
    public float maxTime;
    [Header("총알의 충돌 수명")]
    public float colTime;
    //총알의 현재 수명
    float curTime = 0f;

    [Header("총알의 피해량")]
    public int bulletDamage;
    [Header("총알의 속도")]
    public float bulletSpeed;
    [Header("총알의 충돌 영역")]
    public Collider bulletCollider;

    [Header("총알의 목표 대상")]
    public GameObject bulletTarget;

    [Header("총알 주인이 속한 팀")]
    public Creature.TeamEnum curTeamEnum;

    public enum BulletMoveEnum//Slash: 근접공격(노랑애의 근접공격, 충돌해도 안사라짐), Tracer(개구리가 쏘는 총알),Canon
    {
        Slash, Tracer, Canon
    }
    [Header("총알의 이동 방식")]
    public BulletMoveEnum curBulletMoveEnum;

    public enum BulleEffectEnum
    {
        Damage, Cure
    }
    [Header("총알의 특수효과 방식")]
    public BulleEffectEnum curBulletEffectEnum;

    [Header("사라질 때, 생성되는 총알")]
    public Transform endBullet;

    [Header("크리쳐가 사용하는 총알인지")]
    public bool byCreature;

    [Header("매니저들")]
    public GameManager gameManager;
    public ObjectManager objectManager;

    private void Awake()//최초 설정
    {
        bulletCollider = GetComponent<Collider>();
    }

    public void Init()//최초 설정
    {
        objectManager = gameManager.objectManager;
    }

    private void FixedUpdate()//매 프레임마다 반복
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

    #region 총알 활성
    public void BulletOn(Creature.TeamEnum teamEnum)
    {
        //팀 설정
        curTeamEnum = teamEnum;

        //시간 동기화
        curTime = 0f;
        //충돌 영역 활성화
        bulletCollider.enabled = true;
        //게임오브젝트 활성화
        gameObject.SetActive(true);
    }
    #endregion

    #region 총알 비활성
    public void BulletOff()
    {
        //자식 총알 생성
        EndBulletOn();

        //총알 비활성화
        gameObject.SetActive(false);
    }
    #endregion

    #region 자식 총알 생성
    public void EndBulletOn()
    {
        if (endBullet != null && gameObject.activeSelf)
        {
            //파괴 총알 활성
            string bulletName = endBullet.name;

            GameObject bomb = objectManager.CreateObj(bulletName, ObjectManager.PoolTypes.BulletPool);
            Bullet bomb_bullet = bomb.GetComponent<Bullet>();
            //이동
            bomb.transform.position = transform.position;
            //활성화
            bomb_bullet.BulletOn(curTeamEnum);
        }
    }
    #endregion

    private void OnTriggerEnter(Collider other)//불렛이 무언가와 충돌함
    {
        if (other.transform.CompareTag("Untagged")) //맵과 충돌
        {
            if (curBulletMoveEnum == BulletMoveEnum.Canon)//캐논이면(=화염구)
            {
                BulletOff();//비활성화
            }
        }
    }
}