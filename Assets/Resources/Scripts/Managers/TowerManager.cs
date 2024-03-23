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

    [Header("타워의 크리쳐 제한 정보")]
    public int curCreatureCount = 0;    //크리쳐의 현재 수
    public int maxCreatureCount = 8;    //크리쳐의 최대 수

    [Header("매니저")]
    public GameManager gameManager;
    ObjectManager objectManager;
    UIManager UiManager;
    AudioManager audioManager;
    
    Transform cameraGround;
    Transform mainCamera;
    private void Awake()
    {
        objectManager = gameManager.objectManager;
        UiManager = gameManager.uiManager;
        audioManager = gameManager.audioManager;
        mainCamera = UiManager.cameraObj;
        cameraGround = UiManager.cameraGround;

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

    [Header("타워의 자원 정보")]
    public float curTowerResource = 0f;     //플레이어의 현재 자원
    public float maxTowerResource = 10f;    //플레이어의 최대 자원

    private void Update()
    {
        if (maxTowerResource > curTowerResource)
        {
            //스펠 사용을 위한 자원 증가
            //Debug.LogWarning(BankSpeedArr[curBankIndex]);
            curTowerResource += Time.deltaTime * BankSpeedArr[curBankIndex];
        }
        else if (maxTowerResource <= curTowerResource)
        {
            //현재 자원량이 최대치를 넘지 않도록
             curTowerResource = maxTowerResource;
        }
        

        // 물체 A에서 B를 바라보는 회전 구하기
        cameraVec = cameraGround.transform.position - mainCamera.transform.position;
        lookRotation = Quaternion.LookRotation(cameraVec);

        // 물체 C에 회전 적용
        miniCanvas.transform.rotation = lookRotation;
    }

    #region 타워 요소 초기화;

    [Header("뱅크 관련 요소들")]
    public int[] BankValueArr = { 5, 6, 7, 8 };//뱅크 버튼을 누르기 위해 필요한 비용 배열
    float[] BankSpeedArr = { 0.6f, 0.7f, 0.8f, 0.9f, 1f };//뱅크 버튼을 눌러서 자원이 증가하게 되는 속도 배열
    public int curBankIndex = 0;//현재 뱅크 레벨이 몇인지

    public void ResetTower() 
    {
        //타워 체력 초기화
        curHealth = maxHealth;
        //타워 자원 초기화
        curTowerResource = 0;
        //타워 은행 초기화
        curBankIndex = 0;
    }
    #endregion



    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Bullet"))//총알과 충돌
        {

            Bullet bullet = other.gameObject.GetComponent<Bullet>();

            if (bullet.curTeamEnum != curTeamEnum)//팀이 다를 경우만 피해 처리
            {
                //피해 관리
                DamageControl(bullet);

                //피격한 총알 후처리
                if (bullet.curBulletMoveEnum != Bullet.BulletMoveEnum.Slash)
                    bullet.BulletOff();
            }
        }
    }


    #region 데미지 계산
    void DamageControl(Bullet bullet)
    {
        //피해량 확인
        float damage = bullet.bulletDamage;

        if (bullet.isCreature)
        {
            Agent bulletAgent = bullet.bulletHost.agent;
            //공격자 점수 증가
            bulletAgent.AddReward(damage / 10f);
        }

        if (!gameManager.isML)//머신러닝이 아닌경우
        {
            curHealth -= damage;

            if (curHealth < 0) curHealth = 0;
            else if (curHealth > maxHealth) curHealth = maxHealth;

            //UI관리
            miniHealth.fillAmount = curHealth / maxHealth;

            if (curHealth <= 0) //게임 종료
            {
                
            }
        }
    }
    #endregion

    //대포 포대 각도 조절
    public void RadarControl(Vector3 targetVec) => bulletStartPoint.transform.LookAt(targetVec);

    #region 크리쳐 소환
    public void SpawnCreature(string tmpCreatureName)
    {
        //생명체 생성
        GameObject obj = objectManager.CreateObj(tmpCreatureName, ObjectManager.PoolTypes.CreaturePool);
        Creature creature = obj.GetComponent<Creature>();
        //활동 전에 설정
        creature.BeforeRevive(curTeamEnum, gameManager);
    }
    #endregion


    #region 무기 사용 분류

    Vector3 enemyVec;

    public void WeaponSort(string tmpWeaponName) 
    {
        if (tmpWeaponName == gameManager.Gun.name) Tower_Gun();
        else if (tmpWeaponName == gameManager.Flame.name) Tower_Flame();
        else if (tmpWeaponName == gameManager.GrandCure.name) Tower_GrandCure();
    }
    #endregion

    #region 미니건 난사
    void Tower_Gun()
    {
        bool isShot = false;
        //적 크리쳐 위치 파악
        for (int i = 0; i < enemyCreatureFolder.childCount; i++)
        {
            if (enemyCreatureFolder.GetChild(i).gameObject.activeSelf && 
                enemyCreatureFolder.GetChild(i).gameObject.layer == LayerMask.NameToLayer("Creature")) 
            {
                isShot = true;

                enemyVec = enemyCreatureFolder.GetChild(i).transform.position;

                GameObject bullet = objectManager.CreateObj("Tower_Gun", ObjectManager.PoolTypes.BulletPool);
                Bullet bullet_bullet = bullet.GetComponent<Bullet>();
                Rigidbody bullet_rigid = bullet.GetComponent<Rigidbody>();

                bullet_bullet.gameManager = gameManager;
                bullet_bullet.Init();


                //이동
                bullet.transform.position = bulletStartPoint.position;
                //가속
                bullet_rigid.velocity = (enemyVec - bullet.transform.position).normalized * bullet_bullet.bulletSpeed;
                //회전
                Quaternion targetRotation = Quaternion.LookRotation(bullet_rigid.velocity);
                bullet.transform.rotation = targetRotation;

                //활성화
                bullet_bullet.BulletOnByTower(curTeamEnum);
            }
        }
        if(isShot)
            RadarControl(enemyVec);
        else if (!isShot)
            RadarControl(enemyTower.transform.position);
    }
    #endregion

    #region 파이어볼
    void Tower_Flame()//적이 사용할 경우 가속쪽에 clickPoint 코드 변경 필요
    {
        GameObject bullet = objectManager.CreateObj("Tower_Flame", ObjectManager.PoolTypes.BulletPool);
        Bullet bullet_bullet = bullet.GetComponent<Bullet>();
        Rigidbody bullet_rigid = bullet.GetComponent<Rigidbody>();

        bullet_bullet.gameManager = gameManager;
        bullet_bullet.Init();

        //이동
        bullet.transform.position = UiManager.cameraCloud.position;
        //가속
        bullet_rigid.velocity = (UiManager.clickPoint.position - bullet.transform.position).normalized * bullet_bullet.bulletSpeed;

        //활성화
        bullet_bullet.BulletOnByTower(curTeamEnum);
    }
    #endregion

    #region 대회복
    void Tower_GrandCure()
    {
        GameObject bullet = objectManager.CreateObj("Tower_GrandCure", ObjectManager.PoolTypes.BulletPool);
        Bullet bullet_bullet = bullet.GetComponent<Bullet>();
        //Rigidbody bullet_rigid = bullet.GetComponent<Rigidbody>();

        bullet_bullet.gameManager = gameManager;
        bullet_bullet.Init();

        //이동
        bullet.transform.position = UiManager.clickPoint.position;
        //활성화
        bullet_bullet.BulletOnByTower(curTeamEnum);
    }
    #endregion
}
