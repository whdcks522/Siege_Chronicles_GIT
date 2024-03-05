using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;
using UnityEngine.UI;
using static Creature;

public class TowerManager : MonoBehaviour
{
    [Header("타워의 최대 체력")]
    public float maxHealth;
    [Header("타워의 현재 체력")]
    public float curHealth;

    public TeamEnum curTeamEnum;

    [Header("타워의 아군 생성 위치")]
    public Transform creatureStartPoint;
    [Header("타워의 캐논 위치")]
    public Transform bulletStartPoint;

    [Header("상대 타워의 위치")]
    public Transform enemyTower;

    [Header("타워 위의 미니 UI")]
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
        {
            miniHealth.color = Color.blue;
            enemyTower = gameManager.redTower;
            enemyCreatureFolder = gameManager.objectManager.redCreatureFolder;
        }
        else if (curTeamEnum == TeamEnum.Red)
        {
            miniHealth.color = Color.red;
            enemyTower = gameManager.blueTower;
            enemyCreatureFolder = gameManager.objectManager.blueCreatureFolder;
        }
    }



    //카메라 회전값
    Vector3 cameraVec;
    Quaternion lookRotation;

    [Header("상대팀이 들어있는 폴더")]
    public Transform enemyCreatureFolder;
    
    private void Update()
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
                //피해 관리
                damageControl(bullet);

                //피격한 총알 후처리
                if (bullet.curBulletMoveEnum != Bullet.BulletMoveEnum.Slash)
                    bullet.BulletOff();
            }
        }
    }


    float breakPoint = 20f;
    void damageControl(Bullet bullet)
    {
        //피해량 확인
        Agent bulletAgent = bullet.bulletHost.agent;
        float damage = bullet.bulletDamage;

        float attackPoint = damage / 10f;
        /*
        //curHealth -= damage;
        if (curHealth < 0) curHealth = 0;
        else if (curHealth > maxHealth) curHealth = maxHealth;

        //UI관리
        miniHealth.fillAmount = curHealth / maxHealth;
        */

        //충격 초기화
        if (curHealth > 0)//피격하고 살아 있음
        {
            //공격자 점수 증가
            bulletAgent.AddReward(attackPoint);
            //Debug.Log("성 공격");
        }
        else if (curHealth <= 0) //파괴 완료됨
        {
            //bulletAgent.AddReward(breakPoint);

            //시나리오 종료
            //bulletAgent.EndEpisode();


            aiManager.MlCreature.resetEnv();
        }

    }
    /*
    void Dead() 
    {
        
        if (aiManager.isML)
        {
            //모두 초기화
            if (curTeamEnum == TeamEnum.Blue)//파랑 타워가 죽음
            {
                //aiManager.AiEnd(-1);//빨강 득점
            }
            if (curTeamEnum == TeamEnum.Red)//빨강 타워가 죽음
            {
                //aiManager.AiEnd(1);//파랑 득점
            }
        }
        
    }
*/
    public void TowerOn() //타워 활성화
    {
        curHealth = maxHealth;
        miniHealth.fillAmount = 1;
    }
}
