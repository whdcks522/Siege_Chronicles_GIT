using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Creature;

public class TowerManager : MonoBehaviour
{
    [Header("타워의 최대 체력")]
    public float maxHealth;
    [Header("타워체의 현재 체력")]
    public float curHealth;

    [Header("가까운 적")]
    public Transform target;
    [Header("공격 가능한 최대 거리")]
    public float maxRange;
    [Header("현재 대상과의 거리")]
    public float curRange;

    public TeamEnum curTeamEnum;
    [Header("우리 성")]
    public Transform ourTower;
    [Header("상대 성")]
    public Transform enemyTower;

    [Header("총알이 시작되는 곳")]
    public Transform bulletStartPoint;

    [Header("캐릭터 위의 미니 UI")]
    public GameObject miniUI;
    public Image miniHealth;

    Transform mainCamera;
    GameManager gameManager;
    private void Awake()
    {
        mainCamera = gameManager.uiManager.cameraObj;
    }

    private void LateUpdate()
    {
        miniUI.transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward,
            mainCamera.transform.rotation * Vector3.up);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Bullet"))//폭탄과 충돌했을 때
        {
            Bullet bullet = other.GetComponent<Bullet>();
            if (bullet.curTeamEnum != curTeamEnum)//팀이 다를 경우
            {
                //피해량 확인
                ParentAgent bulletParentAgent = bullet.bulletHost;
                float damage = bullet.bulletDamage;

                //점수 증가
                bulletParentAgent.AddReward(damage / 10f);
                //피해 관리
                damageControl(damage);
            }
        }
    }

    void damageControl(float _dmg)
    {
        //피해량 계산
        curHealth -= _dmg;
        if (curHealth < 0) curHealth = 0;
        else if (curHealth > maxHealth) curHealth = maxHealth;
        //UI관리
        //miniHealth.fillAmount = curHealth / maxHealth;

        //충격 초기화
        if (curHealth > 0)//피격하고 살아 있음
        {
            //anim.SetTrigger("isHit");
        }
        else if (curHealth <= 0) Dead();
    }

    void Dead() 
    {
        
    }
}
