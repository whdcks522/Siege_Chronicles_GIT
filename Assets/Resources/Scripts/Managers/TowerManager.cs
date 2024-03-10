using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;
using UnityEngine.UI;
using static Creature;
using static Unity.Barracuda.BurstCPUOps;

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
    ObjectManager objectManager;
    UIManager UIManager;
    Transform cameraGround;
    Transform mainCamera;
    private void Awake()
    {
        objectManager = gameManager.objectManager;
        UIManager = gameManager.uiManager;
        mainCamera = UIManager.cameraObj;
        cameraGround = UIManager.cameraGround;

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

        curTime += Time.deltaTime;
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


    #region 데미지 계산
    void damageControl(Bullet bullet)
    {
        //피해량 확인
        Agent bulletAgent = bullet.bulletHost.agent;
        float damage = bullet.bulletDamage;

        float damagePoint = damage / 8f;

        if (!gameManager.isML)
        {
            curHealth -= damage;

            if (curHealth < 0) curHealth = 0;
            else if (curHealth > maxHealth) curHealth = maxHealth;

            //UI관리
            miniHealth.fillAmount = curHealth / maxHealth;
        }


        //충격 초기화
        if (curHealth > 0)//타워가 피격하고 살아 있음
        {
            //공격자 점수 증가
            bulletAgent.AddReward(damagePoint);
        }
        else if (curHealth <= 0) //게임 종료
        {

        }
    }
    #endregion

    #region 대포 조절

    public float maxTime = 0.3f;
    public float curTime = 0;

    public void RadarControl(Transform targetPos, bool isDirect)
    {
        //반사체 회전
        if (isDirect) 
        {
            
            bulletStartPoint.transform.LookAt(targetPos);
        }
        else if (!isDirect)
        {
            //반사체 회전
            bulletStartPoint.transform.LookAt(targetPos);
            bulletStartPoint.transform.rotation = Quaternion.Euler(-61.503f, bulletStartPoint.transform.rotation.eulerAngles.y, 180);
        }

        if(curTime >= maxTime) 
        {
            curTime = 0;

            string bulletName = "Tower_Gun";

            GameObject bullet = objectManager.CreateObj(bulletName, ObjectManager.PoolTypes.BulletPool);
            Bullet bullet_bullet = bullet.GetComponent<Bullet>();
            Rigidbody bullet_rigid = bullet.GetComponent<Rigidbody>();

            bullet_bullet.gameManager = gameManager;
            bullet_bullet.Init();


            //이동
            bullet.transform.position = bulletStartPoint.position + bulletStartPoint.forward * 5;
            //가속
            bullet_rigid.velocity = (targetPos.position - bulletStartPoint.position).normalized * bullet_bullet.bulletSpeed;
            //회전
            Quaternion targetRotation = Quaternion.LookRotation(bullet_rigid.velocity);
            bullet.transform.rotation = targetRotation;

            //활성화
            bullet_bullet.BulletOnByTower(curTeamEnum);

        }
    }
    #endregion
}
