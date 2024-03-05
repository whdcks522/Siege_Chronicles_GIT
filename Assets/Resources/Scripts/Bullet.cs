using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("총알의 최대 수명")]
    public float maxTime;
    [Header("총알의 충돌 수명")]
    public float colTime;
    float curTime = 0f;
    [Header("총알의 피해량")]
    public int bulletDamage;
    [Header("총알의 속도")]
    public float bulletSpeed;
    [Header("총알의 충돌 영역")]
    public Collider bulletCollider;

    [Header("총알의 주인")]
    public Creature bulletHost;
    [Header("총알 주인이 속한 팀")]
    public Creature.TeamEnum curTeamEnum;

    public enum BulletMoveEnum
    {
       Slash, Tracer, Canon
    }
    [Header("총알의 이동 방식")]
    public BulletMoveEnum curBulletMoveEnum;

    [Header("사라질 때, 생성되는 총알")]
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

    public void Init()//최초 설정
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

    #region 총알 활성
    public void BulletOn(Creature _creature)
    {
        //주인 설정
        bulletHost = _creature;
        curTeamEnum = _creature.curTeamEnum;
        //시간 동기화
        curTime = 0f;
        //이미 충돌했는지 확인 값 초기화
        isAlreadyHit = false;
        //충돌 영역 활성화
        bulletCollider.enabled = true;
        //게임오브젝트 활성화
        gameObject.SetActive(true);
    }
    #endregion

    #region 총알 비활성
    public void BulletOff()
    {
        //총알 비활성화
        gameObject.SetActive(false);
        
        if (curBulletMoveEnum != BulletMoveEnum.Slash) 
        {
            //파괴 총알 활성
            string bulletName = endBullet.name;

            GameObject bomb = objectManager.CreateObj(bulletName, ObjectManager.PoolTypes.BulletPool);
            Bullet bomb_bullet = bomb.GetComponent<Bullet>();
            //이동
            bomb.transform.position = transform.position;
            //활성화
            bomb_bullet.BulletOn(bulletHost);
            

        }
    }
    #endregion

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Untagged") && !isAlreadyHit) //맵과 충돌하면 감점
        {
            //여러번 충돌 방지
            isAlreadyHit = true;

            //감점
            bulletHost.agent.AddReward(- bulletDamage / 50f);
            if (curBulletMoveEnum != BulletMoveEnum.Slash)
            {
                //총알 비활성화
                BulletOff();
            }
        }
    }
}