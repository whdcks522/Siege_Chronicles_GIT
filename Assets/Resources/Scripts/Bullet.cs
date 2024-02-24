using System.Collections;
using System.Collections.Generic;
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
    public ParentAgent bulletHost;
    [Header("총알 주인이 속한 팀")]
    public Creature.TeamEnum curTeamEnum;

    public enum BulletMoveEnum
    {
       Slash, Tracer, Canon
    }
    [Header("총알의 이동 방식")]
    public BulletMoveEnum curBulletMoveEnum;

    [Header("사라질 때, 사용할 이펙트")]
    public string endStr;

    public GameManager gameManager;
    ObjectManager objectManager;
    Rigidbody rigid;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        
        bulletCollider = GetComponent<Collider>();
    }

    private void Update()
    {
        curTime += Time.deltaTime;
        if (curTime > maxTime)
        {
            gameObject.SetActive(false);
        }
        else if(curTime > colTime && curBulletMoveEnum == BulletMoveEnum.Slash)
        {
            bulletCollider.enabled = false;
        }
    }

    #region 총알 활성 동기화
    public void BulletOn(ParentAgent a)
    {
        //부모 설정
        bulletHost = a;
        curTeamEnum = a.creature.curTeamEnum;
        //회전 초기화
        //transform.rotation = Quaternion.identity;
        //시간 동기화
        curTime = 0f;
        //충돌 영역 활성화
        bulletCollider.enabled = true;
        //게임오브젝트 활성화
        gameObject.SetActive(true);
    }
    #endregion


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.CompareTag("Untagged")) //맵 밖으로 나가지면 종료
        {
            Debug.Log("벽과 닿음");
            if (curBulletMoveEnum != BulletMoveEnum.Slash) 
            {
                gameObject.SetActive(false);
            }
        }
    }
}