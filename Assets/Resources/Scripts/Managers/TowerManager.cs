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

    public TeamEnum curTeamEnum;

    [Header("타워의 아군 생성 위치")]
    public Transform creatureStartPoint;
    [Header("타워의 캐논 위치")]
    public Transform bulletStartPoint;

    [Header("캐릭터 위의 미니 UI")]
    public GameObject miniCanvas;
    public Image miniHealth;

    [Header("매니저")]
    public GameManager gameManager;
    UIManager UIManager;
    Transform cameraGround;
    Transform mainCamera;
    AiManager aiManager;
    private void Awake()
    {
        UIManager = gameManager.uiManager;
        mainCamera = UIManager.cameraObj;
        cameraGround = UIManager.cameraGround;
        aiManager = gameManager.aiManager;


        if (curTeamEnum == TeamEnum.Blue)
            miniHealth.color = Color.blue;
        else if (curTeamEnum == TeamEnum.Red)
            miniHealth.color = Color.red;
    }



    //카메라 회전값
    Vector3 cameraVec;
    Quaternion lookRotation;
    private void LateUpdate()
    {
        // 물체 A에서 B를 바라보는 회전 구하기
        cameraVec = cameraGround.transform.position - mainCamera.transform.position;
        lookRotation = Quaternion.LookRotation(cameraVec);

        // 물체 C에 회전 적용
        miniCanvas.transform.rotation = lookRotation;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Bullet"))//총알과 충돌
        {
            
            Bullet bullet = other.gameObject.GetComponent<Bullet>();

            if (bullet.curTeamEnum != curTeamEnum)//팀이 다를 경우만 피해 처리
            {
                //피해량 확인
                ParentAgent bulletParentAgent = bullet.bulletHost;
                float damage = bullet.bulletDamage;

                //점수 증가
                bulletParentAgent.AddReward(damage / 20f);
                //피해 관리
                damageControl(damage);

                //피격한 총알 후처리
                if (bullet.curBulletMoveEnum != Bullet.BulletMoveEnum.Slash)
                    bullet.BulletOff();
            }
        }
    }

    void damageControl(float _dmg)
    {
        //피해량 계산
        //Debug.Log(_dmg);
        curHealth -= _dmg;
        if (curHealth < 0) curHealth = 0;
        else if (curHealth > maxHealth) curHealth = maxHealth;

        //UI관리
        miniHealth.fillAmount = curHealth / maxHealth;

        //충격 초기화
        if (curHealth > 0)//피격하고 살아 있음
        {
            //anim.SetTrigger("isHit");
        }
        else if (curHealth <= 0) Dead();
    }

    void Dead() 
    {
        if (aiManager.isML)
        {
            //모두 초기화
            if(curTeamEnum == TeamEnum.Blue)
                aiManager.AiEnd(1);
            if (curTeamEnum == TeamEnum.Red)
                aiManager.AiEnd(-1);
        }
    }

    public void TowerOn() //타워 활성화
    {
        curHealth = maxHealth;
        miniHealth.fillAmount = 1;
    }
}
