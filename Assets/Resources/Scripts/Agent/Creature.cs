using Google.Protobuf.WellKnownTypes;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Unity.MLAgents;
using Unity.MLAgents.Policies;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class Creature : MonoBehaviour
{
    [Header("크리쳐 별 기본 능력")]
    public float maxHealth;//생명체의 최대 체력
    public float curHealth;//생명체의 현재 체력
    public Transform bulletStartPoint;//총알이 시작되는 곳
    public int runSpd;//달리는 속도
    int rotSpd = 120;//회전 속도

    [Header("크리쳐 별 특수 능력 여부")]
    //보호막을 갖고 있는지(크리쳐에 의해서만 피해를 받음, 방패병만 소유)
    public bool isShield;
    //존재 자체로 조금씩 자원이 증가하는 양(회계병만 소유, 나머지는 0)
    public float isCoinSteal;

    Vector3 moveVec;//이동용 벡터(몰라도 됨)

    [Header("현재 공격 중인지")]//연속 공격 방식
    public bool isAttack = false;

    [Header("텍스쳐")]//천천히 보이는 용
    public Texture baseTexture;
    public SkinnedMeshRenderer skinnedMeshRenderer;//쉐이더에 쓰일 스킨 렌더러

    [Header("우리 타워")]
    public Transform ourTower;
    public TowerManager ourTowerManager;
    public Transform ourCreatureFolder;//상대팀이 들어있는 폴더
    [Header("중립 크리쳐 폴더")]
    public Transform grayCreatureFolder;//중립 팀이 들어있는 폴더
    [Header("상대 타워")]
    public Transform enemyTower;
    public TowerManager enemyTowerManager;
    public Transform enemyCreatureFolder;//상대팀이 들어있는 폴더
    [Header("우리 타워에서 시작할 장소")]
    public Transform startPoint;

    

    [Header("UI 관련")]
    public GameObject miniCanvas;//캐릭터 위의 미니 UI
    public Image miniHealth;//크리쳐의 체력 게이지
    public GameObject CorpseExplosionObj;//시체폭발이 활성화돼 있는지 나타내는 아이콘

    
    public enum TeamEnum {Blue, Red, Gray}//속하는 팀
    [Header("속하는 팀")]
    public TeamEnum curTeamEnum;

    public int teamIndex;//팀 인덱스, 강화학습용

    public enum CreatureMoveEnum { Idle, Run }//머신러닝으로 취할수 있는 행동
    public CreatureMoveEnum curCreatureMoveEnum;

    public enum CreatureSpinEnum { LeftSpin, None, RightSpin }//머신러닝으로 취할수 있는 회전
    public CreatureSpinEnum curCreatureSpinEnum;

    public Rigidbody rigid;//물리법칙
    public Animator anim;//애니메이션
    public Agent agent;//강화학습 에이전트

    public BehaviorParameters behaviorParameters;//머신러닝 디폴트에서도 조작이 되므로 방지하기 위함

    Transform cameraGround;//카메라가 관찰하는 땅의 지점

    Transform mainCamera;//메인 카메라 객체(체력바가 그 곳을 바라 보도록)

    

    [Header("매니저")]
    public GameManager gameManager;
    public ObjectManager objectManager;
    UIManager UIManager;
    

    /*
    이동 방향에 따라 점수
    가만히 서있으면 실점

    공격 맞추면 점수    
    타워 맞추면 득점
    */

    private void Awake()
    {
        if (gameManager == null) 
            Debug.LogError("게임매니저 없음");
        UIManager = gameManager.uiManager;

        objectManager = gameManager.objectManager;
        mainCamera = UIManager.cameraObj;
        cameraGround = UIManager.cameraGround;

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
        teamIndex = (int)(curTeamEnum);

        ourTowerManager = ourTower.GetComponent<TowerManager>();
        enemyTowerManager = enemyTower.GetComponent<TowerManager>();
        //시작지점 설정
        startPoint = ourTowerManager.creatureStartPoint;

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

    public void BeforeRevive(TeamEnum tmpTeamEnum, GameManager tmpGameManager) 
    {
        //팀 설정
        curTeamEnum = tmpTeamEnum;
        //매니저 전달
        gameManager = tmpGameManager;

        Awake();
        Revive();
    }

    public void Revive()//크리쳐 부활시 설정 초기화
    {
        //위치 초기화
        transform.position = startPoint.position;
        //회전 초기화
        transform.LookAt(enemyTower.position);

        //공격 대기 시간 초기화
        isAttack = false;
        //가속 초기화
        rigid.velocity = Vector3.zero;
        //체력 회복
        curHealth = maxHealth;

        //체력 UI 관리
        miniCanvas.SetActive(true);
        miniHealth.fillAmount = 1;
        CorpseExplosionObj.SetActive(false);

        if (curTeamEnum == TeamEnum.Blue)
            miniHealth.color = Color.blue;
        else if (curTeamEnum == TeamEnum.Red)
            miniHealth.color = Color.red;

        //오브젝트 활성화
        gameObject.SetActive(true);
        //기상 애니메이션
        curCreatureSpinEnum = CreatureSpinEnum.None;
        curCreatureMoveEnum = CreatureMoveEnum.Idle;
        anim.SetTrigger("isRage");

        //점차 보이도록
        VisibleWarp();
    }
    #endregion


    #region 물리 동작
    private void FixedUpdate()//Update: 매 프레임
    {
        if (gameObject.layer == LayerMask.NameToLayer("Creature"))
        {
            //행동 관리
            switch (curCreatureMoveEnum)
            {
                case CreatureMoveEnum.Idle://멈추기
                    moveVec = new Vector3(0, rigid.velocity.y, 0);
                    if (moveVec.y >= 0)
                        moveVec.y = 0;
                    rigid.velocity = moveVec;

                    anim.SetBool("isRun", false);
                    break;
                case CreatureMoveEnum.Run://달리기
                    moveVec = new Vector3(0, rigid.velocity.y, 0) + transform.forward * runSpd;
                    if (moveVec.y >= 0)
                        moveVec.y = 0;
                    rigid.velocity = moveVec.normalized * runSpd;
                    rigid.angularVelocity = Vector3.zero;

                    anim.SetBool("isRun", true);
                    break;
            }
            switch (curCreatureSpinEnum)
            {
                case CreatureSpinEnum.LeftSpin://좌회전
                    moveVec = transform.rotation.eulerAngles;
                    // 왼쪽으로 조금 회전합니다 (여기서는 y축 값만 조정합니다)
                    moveVec.y -= rotSpd * Time.deltaTime;
                    // 새로운 회전값을 설정합니다
                    transform.rotation = Quaternion.Euler(moveVec);

                    break;
                case CreatureSpinEnum.None://앞쪽 바라보기
                    //회전 가속도 초기화
                    moveVec = transform.rotation.eulerAngles;
                    moveVec.x = 0;
                    moveVec.z = 0;
                    transform.localEulerAngles = moveVec;

                    rigid.angularVelocity = Vector3.zero;
                    break;

                case CreatureSpinEnum.RightSpin://우회전
                    moveVec = transform.rotation.eulerAngles;
                    // 왼쪽으로 조금 회전합니다 (여기서는 y축 값만 조정합니다)
                    moveVec.y += rotSpd * Time.deltaTime;
                    // 새로운 회전값을 설정합니다
                    transform.rotation = Quaternion.Euler(moveVec);
                    break;
            }
            
        }
    }
    #endregion

    //카메라 회전값
    Vector3 cameraVec;
    Quaternion lookRotation;
    private void LateUpdate()
    {
        if (!gameManager.isML)
        {
            // 물체 A에서 B를 바라보는 회전 구하기
            cameraVec = mainCamera.transform.position - cameraGround.transform.position;
            lookRotation = Quaternion.LookRotation(cameraVec);

            // 물체 C에 회전 적용
            miniCanvas.transform.rotation = lookRotation;

            if (isCoinSteal != 0)//머신러닝 중이라면 할 필요 없으므로
            {
                //존재 자체로도 자원이 증가하는 유닛이 존재
                ourTowerManager.curTowerResource += isCoinSteal * Time.deltaTime;
            }
        }
    }

    

    private void OnTriggerEnter(Collider other)//무언가와 충돌했을 시
    {
        if (other.gameObject.CompareTag("Bullet"))//폭탄과 충돌했을 때
        {
            Bullet bullet = other.GetComponent<Bullet>();
            if (bullet.curBulletEffectEnum == Bullet.BulleEffectEnum.Damage)//피해를 주는 것과 충돌
            {
                if (bullet.curTeamEnum != curTeamEnum)//팀이 다를 경우
                {
                    //피해량 확인
                    float damage = bullet.bulletDamage;

                    if (bullet.isCreature)//크리쳐에 의한 공격이면
                    {
                        Agent bulletAgent = bullet.bulletHost.agent;
                        //공격자 점수 증가
                        bulletAgent.AddReward(damage / 10f);

                        damageControl(damage);
                    }
                    else if (!bullet.isCreature)//타워에 의한 공격이면
                    {
                        if (!isShield)//보호막은 주술에 의한 공격을 무효화
                            damageControl(damage);
                    }          

                    //피격한 총알 후처리
                    if (bullet.curBulletMoveEnum != Bullet.BulletMoveEnum.Slash)
                    {
                        //총알 비활성화
                        bullet.BulletOff();
                    }
                }
            }
            else if (bullet.curBulletEffectEnum == Bullet.BulleEffectEnum.Cure) //회복하는 것과 충돌
            {
                if (bullet.curTeamEnum == curTeamEnum)//팀이 같을 경우
                {
                    //피해 관리
                    damageControl(bullet.bulletDamage);
                }
            }
        }
    }

    #region 피격 처리
    public void damageControl(float _dmg)
    {
        if (_dmg != 0) //폭발은 실제 데미지가 0이므로
        {
            //피해량 계산
            curHealth -= _dmg;

            if (curHealth < 0) curHealth = 0;
            else if (curHealth > maxHealth) curHealth = maxHealth;

            //UI관리
            miniHealth.fillAmount = curHealth / maxHealth;

            //효과음 관리
            /*
            if (_dmg >= 0) 
            {

            }
            else if (_dmg < 0)
            {

            }
            */

            //충격 초기화
            if (curHealth <= 0)
                AlmostDead();
        }
    }
    #endregion

    #region 사망처리
    void AlmostDead()
    {
        if (gameManager.isML)//머신러닝 중이라면
        {
            agent.EndEpisode();
        }
        else if (!gameManager.isML)//일반 상황이라면
        {
            //피격당하지 않도록, 레이어 변경
            gameObject.layer = LayerMask.NameToLayer("WarpCreature");

            //애니메이션 실행
            anim.SetTrigger("isDeath");

            //미니 UI 닫기
            miniCanvas.SetActive(false);

            //시체 폭발 여부
            //if (CorpseExplosionObj.activeSelf)
            {
                
            }

            //왜곡장
            InvisibleWarp();
        }
    }

    //투명해진 후에, 완전히 죽음
    public void CompletelyDead() 
    {
        //자기 타워에 등록된 크리쳐 수 감소
        if(gameObject.activeSelf)
            ourTowerManager.curCreatureCount--;

        //생명체 비활성화
        gameObject.SetActive(false);

        //중립 폴더로 옮기기
        transform.parent = objectManager.grayCreatureFolder;
    }
    #endregion

    #region 맞는 방향으로 가고 있는지

    //목표 방향 벡터(원거리 공격용으로도 사용)
    public Vector3 goalVec;
    //실제 이동하는 벡터
    Vector3 curVec;

    public float GetMatchingVelocityReward()//가장 가까운 적에게 다가가면 득점, 멀어지면 실점
    {
        float tmpReward = 0;
        //목표 방향 벡터
        goalVec = (curTarget.transform.position - transform.position).normalized;
        //현재값 서있는 벡터
        curVec = rigid.velocity.normalized;

        // 두 벡터 사이의 각도 계산 (라디안 단위)
        float angle = Vector3.Angle(goalVec, curVec);
        // 코사인 유사도 계산 (-1부터 1까지의 값)
        float cosineSimilarity = Mathf.Cos(angle * Mathf.Deg2Rad);

        if (curCreatureMoveEnum != CreatureMoveEnum.Idle)//서있다면 0을 반환
        {
            tmpReward = (cosineSimilarity + 1f) / 2f;  //0f ~ 1f
            tmpReward -= 0.5f;                         //-0.5f ~ 0.5f

            //Debug.Log(tmpReward);
        }
        return tmpReward;
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
            if (obj.gameObject.layer == LayerMask.NameToLayer("Creature"))
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
        }
        else if(!InVisible)//보이도록 
        {
            //피격당하도록, 레이어 변경
            gameObject.layer = LayerMask.NameToLayer("Creature");
        }
    }
    #endregion
}
