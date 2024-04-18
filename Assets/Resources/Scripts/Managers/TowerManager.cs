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
    public int curCreatureCount;    //자기 팀 크리쳐의 현재 수


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

            ourCreatureFolder = gameManager.objectManager.blueCreatureFolder;
            enemyCreatureFolder = gameManager.objectManager.redCreatureFolder;
        }
        else if (curTeamEnum == TeamEnum.Red)
        {
            miniHealth.color = Color.red;
            enemyTower = gameManager.blueTower;

            ourCreatureFolder = gameManager.objectManager.redCreatureFolder;
            enemyCreatureFolder = gameManager.objectManager.blueCreatureFolder;
        }

        if (gameManager.isML) 
        {
            bulletStartPoint.gameObject.SetActive(false);
        }
    }

    //카메라 회전값
    Vector3 cameraVec;
    Quaternion lookRotation;

    [Header("상대팀이 들어있는 폴더(스펠_시체폭발용)")]
    public Transform ourCreatureFolder;
    [Header("상대팀이 들어있는 폴더(스펠_사격용)")]
    public Transform enemyCreatureFolder;

    [Header("타워의 자원 정보")]
    public float curTowerResource = 0f;     //플레이어의 현재 자원
    public float maxTowerResource = 10f;    //플레이어의 최대 자원

    [Header("적 타워가 앞으로 사용할 스펠 데이터")]
    public SpellData futureSpellData;

    private void Update()//Update는 매초마다 수행
    {
        if (gameManager.isBattle && !gameManager.isML) 
        {
            if (maxTowerResource > curTowerResource)
            {
                //스펠 사용을 위한 자원 증가
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

            // 캔퍼스에 회전 적용
            miniCanvas.transform.rotation = lookRotation;

            if (curTeamEnum == TeamEnum.Red) 
            {
                if (futureSpellData != null)
                {
                    Debug.Log(futureSpellData.spellValue);
                    if (curTowerResource < futureSpellData.spellValue)//자원이 부족함
                    {
                        Debug.Log("자원이 부족함");
                    }
                    if (curTowerResource >= futureSpellData.spellValue)//자원이 충분함
                    {
                        Debug.Log("자원이 충분함");
                        //SpawnCreature();
                    }
                }
                else if (futureSpellData == null)
                {
                    int r = Random.Range(0, gameManager.creatureSpellDataArr.Length);
                    futureSpellData = gameManager.creatureSpellDataArr[r];
                }
            }
        }
    }

    #region 타워 요소 초기화;

    [Header("뱅크 관련 요소들")]
    public int[] BankValueArr = { 5, 6, 7, 8 };//뱅크 버튼을 누르기 위해 필요한 비용 배열
    float[] BankSpeedArr = { 0.6f, 0.7f, 0.8f, 0.9f, 1f };//뱅크 버튼을 눌러서 자원이 증가하게 되는 속도 배열
    public int curBankIndex = 0;//현재 뱅크 레벨이 몇인지

    public void ResetTower()//게임 재시작을 위해 타워 정보 초기화
    {
        //타워 체력 초기화
        curHealth = maxHealth;
        miniHealth.fillAmount = 1;

        //타워 자원 초기화
        curTowerResource = 0;

        //타워 은행 수치 초기화
        curBankIndex = 0;
    }
    #endregion



    private void OnTriggerEnter(Collider other)//부딪힘
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
                {
                    bullet.BulletOff();
                }
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

            //타워 체력바 관리
            miniHealth.fillAmount = curHealth / maxHealth;
            //타워 피격 효과음
            if(damage != 0)
                audioManager.PlaySfx(AudioManager.Sfx.TowerCrashSfx);

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


    #region 스펠(무기) 사용 분류
    Vector3 enemyVec;
    public void WeaponSort(string tmpWeaponName) 
    {
        if (tmpWeaponName == gameManager.Gun.name) Tower_Gun();
        else if (tmpWeaponName == gameManager.Flame.name) Tower_Flame();
        else if (tmpWeaponName == gameManager.GrandCure.name) Tower_GrandCure();
        else if (tmpWeaponName == gameManager.CorpseExplosion.name) Tower_CorpseExplosion();
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
        if(isShot)//사격 대상이 있는 경우, 타워의 레이더가 마지막 대상을 바라봄
            RadarControl(enemyVec);
        else if (!isShot)//대상이 없는 경우, 적 타워를 바라봄
            RadarControl(enemyTower.transform.position);
    }
    #endregion

    #region 화염구
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

    #region 시체폭발
    void Tower_CorpseExplosion()
    {
        //int i = 0;

        foreach (Transform obj in ourCreatureFolder)
        {
            //Debug.Log(i);

            if (obj.gameObject.layer == LayerMask.NameToLayer("Creature"))
            {
                //Debug.Log(i+'/');

                //시체 폭발 아이콘 활성화
                Creature creature = obj.gameObject.GetComponent<Creature>();
                creature.CorpseExplosionObj.SetActive(true);
            }
        }
    }
    #endregion
}
