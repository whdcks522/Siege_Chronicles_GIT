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

    public enum BulletMoveEnum
    {
       Tracer, Canon
    }
    [Header("총알의 이동")]
    public BulletMoveEnum curBulletMoveEnum;

    [Header("히트 이펙트")]
    public string hitStr;

    public GameManager gameManager;
    ObjectManager objectManager;
    Rigidbody rigid;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        if (curBulletMoveEnum == BulletMoveEnum.Canon) 
            rigid.useGravity = true;
    }

    private void Update()
    {
        curTime += Time.deltaTime;
        if (curTime > maxTime)
        {
            bulletOff();
        }
    }

    #region 총알 활성 동기화
    public void bulletOnRPC()
    {
        //게임오브젝트 활성화
        gameObject.SetActive(true);
        //회전 초기화
        //transform.rotation = Quaternion.identity;
        //시간 동기화
        curTime = 0f;
    }
    #endregion

    #region 총알 비활성 동기화
    public void bulletOff()//칼에 맞았거나, 자연소멸했거나
    {

        //게임오브젝트 비활성화
        gameObject.SetActive(false);
        //-1: 플레이어가 총알에 맞은 경우
        // 0: 자연 소멸한 경우
        //+1: 플레이어가 칼로 총알을 파괴한 경우
        /*
        if (isHit)//종료 이펙트 출력
        {
            GameObject hit = gameManager.CreateObj(hitStr, GameManager.PoolTypes.BulletType);
            Effect hitEffect = hit.GetComponent<Effect>();

            if (PhotonNetwork.InRoom)
            {
                if (PhotonNetwork.IsMasterClient)
                    hitEffect.photonView.RPC("effectOnRPC", RpcTarget.AllBuffered, transform.position);
            }
            else if (!PhotonNetwork.InRoom)
            {
                hitEffect.effectOnRPC(transform.position);
            }
        }
        */

    }
    #endregion

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.CompareTag("Outline")) //맵 밖으로 나가지면 종료
        {
            bulletOff();
        }
    }
}