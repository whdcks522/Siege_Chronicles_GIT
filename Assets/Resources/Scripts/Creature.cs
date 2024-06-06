using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class Creature : MonoBehaviour
{
    [Header("크리쳐 별 기본 능력")]
    public float maxHealth;//생명체의 최대 체력
    public float curHealth;//생명체의 현재 체력

    [Header("크리쳐 별 특수 능력 여부")]
    //보호막을 갖고 있는지(피해를 받지 않음, 시간의 흐름에 따라 체력 감소, 방패병만 소유)
    public int isShield;
    //존재 자체로 조금씩 자원이 증가하는 양(회계병만 소유, 나머지는 0)
    public int isCoinRevive;


    

    [Header("우리 타워")]
    public Transform ourTower;
    public TowerManager ourTowerManager;
    public Transform ourCreatureFolder;//상대팀이 들어있는 폴더
    public Transform creatureStartPoint;
    [Header("중립 크리쳐 폴더")]
    public Transform grayCreatureFolder;//중립 팀이 들어있는 폴더
    [Header("상대 타워")]
    public Transform enemyTower;
    public TowerManager enemyTowerManager;
    public Transform enemyCreatureFolder;//상대팀이 들어있는 폴더


    [Header("UI 관련")]
    public GameObject miniCanvas;//캐릭터 위의 미니 UI
    public Image miniHealth;//크리쳐의 체력 게이지
    public GameObject CorpseExplosionObj;//시체폭발이 활성화돼 있는지 나타내는 아이콘

    [Header("매니저")]
    public GameManager gameManager;
    public ObjectManager objectManager;
    AudioManager audioManager;
    UIManager UIManager;
    public enum TeamEnum { Blue, Red, Gray }//속하는 팀
    [Header("그 외")]
    public TeamEnum curTeamEnum;
    public Rigidbody rigid;//물리법칙
    public Animator anim;//애니메이션
    public NavMeshAgent nav;//맵 이동하는 네비게이션

    Transform cameraGround;//카메라가 관찰하는 땅의 지점
    Transform mainCamera;//메인 카메라 객체(체력바가 그 곳을 바라 보도록)

    private void Awake()
    {
        if (gameManager == null) 
            Debug.LogError("게임매니저 없음");
        UIManager = gameManager.uiManager;

        objectManager = gameManager.objectManager;
        audioManager = gameManager.audioManager;
        mainCamera = UIManager.cameraObj;
        cameraGround = UIManager.cameraGround;
        rigid = GetComponent<Rigidbody>();

        nav = GetComponent<NavMeshAgent>();

        //매터리얼 변경
        skinnedMeshRenderer.material.SetTexture("_BaseTexture", baseTexture);

        if (curTeamEnum == TeamEnum.Blue)//파랑 팀
        {
            //아군 타워 설정
            ourTower = gameManager.blueTower;
            //적 타워 설정
            enemyTower = gameManager.redTower;
            //태그 변경
            gameObject.tag = "BlueCreature";

        }
        else if (curTeamEnum == TeamEnum.Red)//빨강 팀
        {
            //아군 타워 설정
            ourTower = gameManager.redTower;
            //적 타워 설정
            enemyTower = gameManager.blueTower;
            //태그 변경
            gameObject.tag = "RedCreature";
        }

        ourTowerManager = ourTower.GetComponent<TowerManager>();
        enemyTowerManager = enemyTower.GetComponent<TowerManager>();
        //시작지점 설정
        creatureStartPoint = ourTowerManager.creatureStartPoint;

        if (curTeamEnum == TeamEnum.Blue)//파랑 팀
        {
            //아군 폴더 설정
            ourCreatureFolder = objectManager.blueCreatureFolder;
            //적 폴더 설정
            enemyCreatureFolder = objectManager.redCreatureFolder;
        }
        else if (curTeamEnum == TeamEnum.Red)//빨강 팀
        {
            //아군 폴더 설정
            ourCreatureFolder = objectManager.redCreatureFolder;
            //적 폴더 설정
            enemyCreatureFolder = objectManager.blueCreatureFolder;
        }
        //부모 폴더 설정
        transform.parent = ourCreatureFolder;
        //사망 시 중립 폴더로 전이
        grayCreatureFolder = gameManager.objectManager.grayCreatureFolder;
    }

    #region 생명체 활성화
    [Header("게임 난이도")]//1, 2 ,3(기본: 2)
    public int gameLevel = 2;
    public void BeforeRevive(TeamEnum tmpTeamEnum, GameManager tmpGameManager) 
    {
        //팀 설정
        curTeamEnum = tmpTeamEnum;
        //매니저 전달
        gameManager = tmpGameManager;
        //난이도 설정
        if (curTeamEnum == TeamEnum.Blue) 
        {
            gameLevel = 2;
        }
        else if (curTeamEnum == TeamEnum.Red)
        {
            gameLevel = gameManager.gameLevel;
        }

        Awake();
        Revive();
    }

    public void Revive()//크리쳐 부활시 설정 초기화
    {
        //위치 초기화
        transform.position = creatureStartPoint.position;
        //회전 초기화
        transform.LookAt(enemyTower.position);
        //상대 초기화
        curTarget = null;

        //체력 회복
        curHealth = maxHealth;

        //체력 UI 관리
        miniCanvas.SetActive(true);
        miniHealth.fillAmount = 1;
        CanvasSpin();

        //시체 폭발 비활성화
        CorpseExplosionObj.SetActive(false);

        if (curTeamEnum == TeamEnum.Blue)
            miniHealth.color = Color.blue;
        else if (curTeamEnum == TeamEnum.Red)
            miniHealth.color = Color.red;

        //오브젝트 활성화
        gameObject.SetActive(true);

        nav.isStopped = true;

        //기상 애니메이션
        anim.SetTrigger("isRage");

        //점차 보이도록
        VisibleWarp();
    }
    #endregion

    #region 크리쳐별 액션
    [Header("크리쳐 별 투사체 정보")]
    public Transform bulletStartPoint;//총알이 시작되는 곳
    public int yUp;//투사체 y축 소환 위치
    public int zUp;//투사체 z축 소환 위치, 투사체가 쪼개지는 정도로도 사용됨
    float split = 11.25f;
    public GameObject useBullet;//사용하는 투사체
    public Vector3 targetVec;//목표 방향 벡터(원거리 공격용으로도 사용)

    public void AgentAction_1()
    {
        if (bulletStartPoint == null)//근접형인 경우
        {
            GameObject slash = objectManager.CreateObj(useBullet.name, ObjectManager.PoolTypes.BulletPool);
            Bullet slash_bullet = slash.GetComponent<Bullet>();

            //이동
            slash.transform.position = transform.position + transform.forward * zUp + Vector3.up * yUp;//위로 3, 앞으로 1

            //회전
            slash.transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x + 90,
                transform.rotation.eulerAngles.y - 180, transform.rotation.eulerAngles.z - 90);

            //활성화
            slash_bullet.BulletOn(curTeamEnum);
        }
        else //원거리 형인 경우
        {
            for (int x = yUp; x <= zUp; x++)//3발씩 발사하는 크리쳐를 위함
            {
                // 투사체 생성
                GameObject tracer = objectManager.CreateObj(useBullet.name, ObjectManager.PoolTypes.BulletPool);
                Bullet tracer_bullet = tracer.GetComponent<Bullet>();
                Rigidbody tracer_rigid = tracer.GetComponent<Rigidbody>();

                // 초기화
                tracer_bullet.gameManager = gameManager;
                tracer_bullet.Init();

                // 투사체 시작 위치 설정
                tracer.transform.position = bulletStartPoint.position;

                // 투사체 방향 설정
                targetVec = (curTarget.transform.position - transform.position).normalized;

                // 각도를 변환하여 새로운 방향 벡터 계산
                cameraRotation = Quaternion.Euler(0, x * split, 0);
                targetVec = cameraRotation * targetVec;

                // 투사체 속도 설정
                tracer_rigid.velocity = targetVec * tracer_bullet.bulletSpeed;

                // 투사체 활성화
                tracer_bullet.BulletOn(curTeamEnum);
            }
        }
    }
    #endregion

    #region 물리 동작
    int slashCount = 0;

    //공격 사거리 확인
    private void Update()//Update: 매 프레임
    {
        //가속 초기화
        rigid.velocity = Vector3.zero;
        rigid.angularVelocity = Vector3.zero;

        if (gameObject.layer == LayerMask.NameToLayer("Creature") && !nav.isStopped)//크리쳐 레이어면서 달리고 있는 경우
        {
            if (!curTarget.gameObject.activeSelf)//대상이 비활성화된 상태라면
            {
                //대상 탐색
                RangeFirstRangeCalc();

                //목표지로 설정
                nav.SetDestination(curTarget.transform.position);

                anim.SetBool("isRun", true);
            }

            //대상 탐색
            RangeFirstRangeCalc();

            if (curRange < maxRange)
            {
                nav.isStopped = true;
                nav.velocity = Vector3.zero;

                //애니메이션 중지
                anim.SetBool("isRun", false);

                //대상을 바라 보도록
                transform.LookAt(curTarget);

                if (bulletStartPoint == null)//근접형인 경우
                {
                    slashCount = (slashCount + 1) % 2;
                    if (slashCount == 0) anim.SetTrigger("isAttackLeft");
                    else if (slashCount == 1) anim.SetTrigger("isAttackRight");
                }
                else //원거리 형인 경우
                {
                    anim.SetTrigger("isGun");
                }
            }
        }

        if (isShield != 0 && curHealth > 0)//보호막이 있으면 체력이 점차 감소
        {
            //체력 감소
            curHealth -= maxHealth * 1f / isShield * Time.deltaTime;
            //최소 체력보다 낮지 않도록
            if (curHealth <= 0)
            {
                curHealth = 0;
                AlmostDead();
            }
            //UI관리
            miniHealth.fillAmount = curHealth / maxHealth;
        }

        //캔버스 회전
        CanvasSpin();
    }
    //카메라 회전값
    Vector3 cameraVec;
    Quaternion cameraRotation;

    void CanvasSpin()//현재 체력 캔버스 회전
    {
        // 물체 A에서 B를 바라보는 회전 구하기
        cameraVec = mainCamera.transform.position - cameraGround.transform.position;
        cameraRotation = Quaternion.LookRotation(cameraVec);
        // 물체 C에 회전 적용
        miniCanvas.transform.rotation = cameraRotation;
    }
    #endregion

    GameObject damageFont = null;//데미지 폰트용 게임 오브젝트
    private void OnTriggerEnter(Collider other)//무언가와 충돌했을 시
    {
        if (other.gameObject.CompareTag("Bullet"))//폭탄과 충돌했을 때
        {
            Bullet bullet = other.GetComponent<Bullet>();
            if (bullet.curBulletEffectEnum == Bullet.BulleEffectEnum.Damage && bullet.curTeamEnum != curTeamEnum)//다른 팀의 피해를 주는 것과 충돌
            {
                if (bullet.bulletDamage != 0)
                {
                    //피해량 확인(게임 레벨에 따라 안아프게 맞음, 높으면 안아픔)
                    float damage = bullet.bulletDamage / gameLevel;
                    if (isShield != 0)//보호막이 있으면, 공격 무효화
                        damage = 0;

                    //크리쳐 피격 효과음
                    audioManager.PlaySfx(AudioManager.Sfx.CreatureHitSfx);

                    //데미지 폰트 출력
                    if (curTeamEnum == TeamEnum.Blue)//파랑 타워가 맞으면 파랑색
                    {
                        damageFont = objectManager.CreateObj("BlueDamageFont", ObjectManager.PoolTypes.DamageFontPool);
                    }
                    else //빨강 타워가 맞으면 빨강색
                    {
                        damageFont = objectManager.CreateObj("RedDamageFont", ObjectManager.PoolTypes.DamageFontPool);
                    }
                    //폰트 위치와 글자 조정
                    damageFont.transform.position = transform.position;
                    damageFont.GetComponent<DamageFont>().ReName(damage.ToString());

                    //체력 감소
                    curHealth -= damage;
                    //최소 체력보다 낮지 않도록
                    if (curHealth <= 0) 
                    {
                        curHealth = 0;
                        AlmostDead();
                    }
                    //UI관리
                    miniHealth.fillAmount = curHealth / maxHealth;
                        


                    //피격한 총알 후처리
                    if (bullet.curBulletMoveEnum != Bullet.BulletMoveEnum.Slash)
                    {
                        //총알 비활성화
                        bullet.BulletOff();
                    }
                }
            }
            else if (bullet.curBulletEffectEnum == Bullet.BulleEffectEnum.Cure && bullet.curTeamEnum == curTeamEnum) //회복하는 것과 같은 팀이 충돌
            {
                //체력 회복(회복치는 난이도에 따른 조정 없음)
                curHealth += bullet.bulletDamage;

                damageFont = objectManager.CreateObj("PinkDamageFont", ObjectManager.PoolTypes.DamageFontPool);
            
                //폰트 위치와 글자 조정
                damageFont.transform.position = transform.position;
                damageFont.GetComponent<DamageFont>().ReName(bullet.bulletDamage.ToString());

                //최대 체력을 넘지 않도록
                if (curHealth > maxHealth) curHealth = maxHealth;

                //UI관리
                miniHealth.fillAmount = curHealth / maxHealth;

            }
        }
    }

    #region 사망처리
    void AlmostDead()
    {
        //피격당하지 않도록, 레이어 변경
        gameObject.layer = LayerMask.NameToLayer("WarpCreature");

        //애니메이션 실행
        anim.SetTrigger("isDeath");

        //미니 UI 닫기
        miniCanvas.SetActive(false);

        if (!CorpseExplosionObj.activeSelf)//시체 폭발이 아닌 경우
        {
            //크리쳐 피격 효과음
            audioManager.PlaySfx(AudioManager.Sfx.CreatureHitSfx);
        }
        else if (CorpseExplosionObj.activeSelf)//시체폭발인 경우
        {
            //시체폭발 폭발 효과음
            audioManager.PlaySfx(AudioManager.Sfx.CorpseExplosionAdaptSfx);

            GameObject bomb = objectManager.CreateObj("Tower_CorpseExplosion", ObjectManager.PoolTypes.BulletPool);
            Bullet bomb_bullet = bomb.GetComponent<Bullet>();
            //시체 폭발의 이동
            bomb.transform.position = transform.position;
            //시체 폭발의 팀 설정
            bomb_bullet.BulletOn(curTeamEnum);
        }

        //왜곡장
        InvisibleWarp();
    }

    //투명해진 후에, 완전히 죽음
    public void CompletelyDead() 
    {
        //자기 타워에 등록된 크리쳐 수 감소
        if (gameObject.activeSelf && curHealth <= 0)
        {
            if (isCoinRevive == 0)//재정 회계병이 아닌 경우
            {
                ourTowerManager.curCreatureCount--;

                if (curTeamEnum == TeamEnum.Blue)//블루팀 텍스트 갱신
                    ourTowerManager.CreatureCountText();

                //생명체 비활성화
                gameObject.SetActive(false);

                //중립 폴더로 옮기기
                transform.parent = objectManager.grayCreatureFolder;
            }
            else if (isCoinRevive != 0 && ourTowerManager.curTowerResource >= isCoinRevive)//재정회계병이면서 자원이 충분한 경우
            {
                //무료로 부활
                Revive();
            } 
        }  
    }
    #endregion

    #region 적들과의 거리 계산
    [Header("공격 가능한 최대 거리")]
    public float maxRange;
    [Header("현재 대상과의 거리")]
    public float curRange;
    [Header("가장 가까운 대상")]
    public Transform curTarget;
    public void RangeFirstRangeCalc()//가장 가까운 적이 누군지
    {
        curRange = (enemyTower.position - transform.position).magnitude - 2;//일단 적을 타워로 설정
        curTarget = enemyTower;

        foreach (Transform obj in enemyCreatureFolder)
        {
            if (obj.gameObject.activeSelf)//obj.gameObject.layer == LayerMask.NameToLayer("Creature")
            {
                //적과의 거리
                float tmpRange = (obj.position - transform.position).magnitude;
                if (tmpRange < curRange)
                {
                    curRange = tmpRange;
                    curTarget = obj;
                }
            }
        }
    }
    #endregion

    #region 왜곡장 

    [Header("텍스쳐")]//천천히 보이는 용
    public Texture baseTexture;
    public SkinnedMeshRenderer skinnedMeshRenderer;//쉐이더에 쓰일 스킨 렌더러
    public void InvisibleWarp() // 점차 안보이게 되는 것
    {
        StopCoroutine(Dissolve(false));
        StartCoroutine(Dissolve(true));
    }
    public void VisibleWarp() //점차 보이게 되는 것 
    {
        if (curHealth > 0)
        {
            StopCoroutine(Dissolve(true));
            StartCoroutine(Dissolve(false));
        }
    }
    IEnumerator Dissolve(bool InVisible)//왜곡장 1.5초간
    {
        //피격당하지 않도록, 레이어 변경
        gameObject.layer = LayerMask.NameToLayer("WarpCreature");

        float firstValue = InVisible ? 0f : 1f;      //true는 점차 안보이는 것
        float targetValue = InVisible ? 1f : 0f;     //false는 점차 보이는 것
        

        float duration = 1.5f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            if (curHealth <= 0 && !InVisible) break;


            float progress = elapsedTime / duration;
            float value = Mathf.Lerp(firstValue, targetValue, progress);
            elapsedTime += Time.deltaTime;

            skinnedMeshRenderer.material.SetFloat("_AlphaFloat", value);

            yield return null;
        }
        skinnedMeshRenderer.material.SetFloat("_AlphaFloat", targetValue);


        if (InVisible)//안보이도록
        {
            //피격당하지 않도록, 레이어 변경
            gameObject.layer = LayerMask.NameToLayer("WarpCreature");

            CompletelyDead();
        }
        else if(!InVisible)//보이도록 
        {
            //피격당하도록, 레이어 변경
            gameObject.layer = LayerMask.NameToLayer("Creature");

            nav.isStopped = false;
            //대상 탐색
            RangeFirstRangeCalc();
            //목표지로 설정
            nav.SetDestination(curTarget.transform.position);           

            //달리기 애니메이션
            anim.SetBool("isRun", true);
        }
    }
    #endregion
}
