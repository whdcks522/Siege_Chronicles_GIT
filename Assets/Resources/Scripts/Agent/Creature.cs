using Google.Protobuf.WellKnownTypes;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Unity.MLAgents;
using Unity.MLAgents.Policies;
using Unity.VisualScripting;
using UnityEditor.Presets;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class Creature : MonoBehaviour
{
    [Header("쉐이더에 쓰일 텍스쳐")]
    public Texture baseTexture;
    [Header("쉐이더에 쓰일 스킨 렌더러")]
    public SkinnedMeshRenderer skinnedMeshRenderer;
    [Header("더미 쉐이더에 쓰일 스킨 렌더러")]
    public MeshRenderer meshRenderer;

    [Header("생명체의 최대 체력")]
    public float maxHealth;
    [Header("생명체의 현재 체력")]
    public float curHealth;

    [Header("현재 공격 중인지")]
    public bool isAttack = false;

    [Header("우리 타워")]
    public Transform ourTower;
    public TowerManager ourTowerManager;
    [Header("상대 타워")]
    public Transform enemyTower;
    public TowerManager enemyTowerManager;
    [Header("우리 타워에서 시작할 장소")]
    public Transform startPoint;


    [Header("총알이 시작되는 곳")]
    public Transform bulletStartPoint;

    [Header("캐릭터 위의 미니 UI")]
    public GameObject miniCanvas;
    public Image miniHealth;
    public Text curReward;

    [Header("달리는 속도")]
    public int runSpd;
    int rotSpd = 120;//회전 속도

    Vector3 moveVec;//이동용 벡터

    public enum CreatureMoveEnum { Idle, Run }//머신러닝으로 취할수 있는 행동
    public CreatureMoveEnum curCreatureMoveEnum;

    public enum CreatureSpinEnum { LeftSpin, None, RightSpin }//머신러닝으로 취할수 있는 회전
    public CreatureSpinEnum curCreatureSpinEnum;

    
    public enum TeamEnum { Gray, Blue, Red }//속하는 팀
    [Header("속하는 팀")]
    public TeamEnum curTeamEnum;

    public enum CreatureTypeEnum { Dummy, Infantry_A, Shooter_A }//머신러닝으로 취할수 있는 행동
    public CreatureTypeEnum curCreatureTypeEnum;

    public Rigidbody rigid;
    public Animator anim;
    public Agent agent;

    [Header("상대팀이 들어있는 폴더")]
    public Transform enemyCreatureFolder;
    public BehaviorParameters behaviorParameters;

    public GameManager gameManager;
    public ObjectManager objectManager;

    UIManager UIManager;
    Transform cameraGround;
    Transform mainCamera;
    //--------




    //public enum CreatureTypeEnum { Melee, Range }//속하는 팀
    //public TeamEnum curTeamEnum;
    /*
    이동하면 점수

    공격 맞추면 점수    
    공격 못맞추면 실점

    타워 맞추면 득점
    타워 이기는쪽은 득점, 나머지는 실점
    */

    private void Awake()
    {
        UIManager = gameManager.uiManager;
        objectManager = gameManager.objectManager;
        mainCamera = UIManager.cameraObj;
        cameraGround = UIManager.cameraGround;


        //텍스쳐 매터리얼 설정
        if (curCreatureTypeEnum != CreatureTypeEnum.Dummy)
        {
            skinnedMeshRenderer.material.SetTexture("_BaseTexture", baseTexture);

            if (curTeamEnum == TeamEnum.Blue)//파랑 팀
            {
                //팀 번호 설정
                //behaviorParameters.TeamId = 0;
                //아군 타워 설정
                ourTower = gameManager.blueTower;
                //적 타워 설정
                enemyTower = gameManager.redTower;

            }
            else if (curTeamEnum == TeamEnum.Red)//빨강 팀
            {
                //팀 번호 설정
                //behaviorParameters.TeamId = 1;
                //아군 타워 설정
                ourTower = gameManager.redTower;
                //적 타워 설정
                enemyTower = gameManager.blueTower;
            }

            ourTowerManager = ourTower.GetComponent<TowerManager>();
            enemyTowerManager = enemyTower.GetComponent<TowerManager>();
            //시작지점 설정
            startPoint = ourTowerManager.creatureStartPoint;

            if (curTeamEnum == TeamEnum.Blue)//파랑 팀
            {
                //적 폴더 설정
                enemyCreatureFolder = objectManager.redCreatureFolder;
            }
            else if (curTeamEnum == TeamEnum.Red)//빨강 팀
            {
                //적 폴더 설정
                enemyCreatureFolder = objectManager.blueCreatureFolder;
            }
        }
        else if (curCreatureTypeEnum == CreatureTypeEnum.Dummy)
        {
            Revive();
        }
    }


    #region 생명체 활성화
    public void Revive()
    {
        
        if (curCreatureTypeEnum != CreatureTypeEnum.Dummy)
        {
            //위치 초기화
            transform.position = startPoint.position;
            //회전 초기화
            transform.LookAt(enemyTower.position);
        }
            

        //공격 대기 시간 초기화
        isAttack = false;
        //가속 초기화
        rigid.velocity = Vector3.zero;
        //체력 회복
        curHealth = maxHealth;


        //체력 UI 관리
        miniCanvas.SetActive(true);
        miniHealth.fillAmount = 1;
        if (curTeamEnum == TeamEnum.Blue)
            miniHealth.color = Color.blue;
        else if (curTeamEnum == TeamEnum.Red)
            miniHealth.color = Color.red;

        //오브젝트 활성화
        gameObject.SetActive(true);
        //기상 애니메이션
        curCreatureSpinEnum = CreatureSpinEnum.None;
        curCreatureMoveEnum = CreatureMoveEnum.Idle;

        if (curCreatureTypeEnum != CreatureTypeEnum.Dummy)
            anim.SetTrigger("isRage");

        VisibleWarp();
    }
    #endregion


    #region 물리 동작
    private void FixedUpdate()
    {
        if (gameObject.layer == LayerMask.NameToLayer("Creature") && curCreatureTypeEnum != CreatureTypeEnum.Dummy)
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
                case CreatureSpinEnum.LeftSpin:
                    moveVec = transform.rotation.eulerAngles;
                    // 왼쪽으로 조금 회전합니다 (여기서는 y축 값만 조정합니다)
                    moveVec.y -= rotSpd * Time.deltaTime;
                    // 새로운 회전값을 설정합니다
                    transform.rotation = Quaternion.Euler(moveVec);

                    break;
                case CreatureSpinEnum.None:
                    //회전 가속도 초기화
                    moveVec = transform.rotation.eulerAngles;
                    moveVec.x = 0;
                    moveVec.z = 0;
                    transform.localEulerAngles = moveVec;

                    rigid.angularVelocity = Vector3.zero;
                    break;

                case CreatureSpinEnum.RightSpin:
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
        // 물체 A에서 B를 바라보는 회전 구하기
        cameraVec = mainCamera.transform.position - cameraGround.transform.position;
        lookRotation = Quaternion.LookRotation(cameraVec);

        // 물체 C에 회전 적용
        miniCanvas.transform.rotation = lookRotation;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Bullet"))//폭탄과 충돌했을 때
        {
            Bullet bullet = other.GetComponent<Bullet>();
            if (bullet.curTeamEnum != curTeamEnum)//팀이 다를 경우
            {
                //피해량 확인
                Agent bulletAgent = bullet.bulletHost.agent;
                float damage = bullet.bulletDamage;

                //공격자 점수 증가
                bulletAgent.AddReward(damage / 10f);
                //피해 관리
                damageControl(damage);

                //피격한 총알 후처리
                if (bullet.curBulletMoveEnum != Bullet.BulletMoveEnum.Slash)
                    bullet.BulletOff();
            }
        }
    }

    #region 피격 처리
    public void damageControl(float _dmg)
    {
        //피해량 계산
        curHealth -= _dmg;
        if (curHealth < 0) curHealth = 0;
        else if (curHealth > maxHealth) curHealth = maxHealth;
        //UI관리
        miniHealth.fillAmount = curHealth / maxHealth;

        //충격 초기화
        if (curHealth > 0 && !isAttack && curCreatureTypeEnum != CreatureTypeEnum.Dummy)//피격하고 살아 있으면서, 공격중이 아니라면
        {
            anim.SetTrigger("isHit");
        }
        else if (curHealth <= 0) AlmostDead();

    }
    #endregion

    #region 사망처리
    void AlmostDead()
    {
        //피격당하지 않도록, 레이어 변경
        gameObject.layer = LayerMask.NameToLayer("WarpCreature");

        //애니메이션 실행
        if (curCreatureTypeEnum != CreatureTypeEnum.Dummy)
            anim.SetTrigger("isDeath");

        //미니 UI 닫기
        miniCanvas.SetActive(false);

        //먼지 종료
        //왜곡장
        InvisibleWarp();
    }

    //완전히 죽음
    public void CompletelyDead() => gameObject.SetActive(false);


    #endregion

    #region 맞는 방향으로 가고 있는지

    //목표 방향 벡터
    Vector3 goalVec;
    //이동하는 벡터
    Vector3 curVec;

    public void GetMatchingVelocityReward()
    {
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
            float tmpReward = (cosineSimilarity + 1f) / 2f;  //0f ~ 1f
            tmpReward -= 0.5f;                         //-0.5f ~ 0.5f

            //Debug.Log(tmpReward);

            agent.AddReward(tmpReward / 1000f);
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

    public void EnemyFirstRangeCalc()//모든 적을 잡고 나서 공성
    {
        bool isLive = false;
        curRange = 9999;

        foreach (Transform obj in enemyCreatureFolder) 
        {

            if (obj.gameObject.layer == LayerMask.NameToLayer("Creature")) 
            {
                isLive = true;

                //적과의 거리
                float tmpRange = (obj.position - transform.position).magnitude;
                if (tmpRange < curRange)
                {
                    curRange = tmpRange;
                    curTarget = obj;
                }
            } 
        }

        if (!isLive)//남은 적이 없다면
        {
            curTarget = enemyTower;
            curRange = (curTarget.position - transform.position).magnitude - 2;//타워의 두께 계산
        }
    }

    public void TowerFirstRangeCalc()//가까운 적부터 사냥
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

    public Transform[] DummyPoints;
    public void resetEnv() 
    {
        //본인 부활
        Revive();

        //더미 2개 위치 조정
        int saveR = Random.Range(0, DummyPoints.Length);

        int newR = Random.Range(0, DummyPoints.Length);
        while (saveR == newR) 
        {
            newR = Random.Range(0, DummyPoints.Length);
        }

        for (int i = 0; i < enemyCreatureFolder.childCount; i++) 
        {
            enemyCreatureFolder.GetChild(i).GetComponent<Creature>().Revive();
            if(i == 0)
                enemyCreatureFolder.GetChild(i).transform.position = DummyPoints[saveR].position;
            else if (i == 1)
                enemyCreatureFolder.GetChild(i).transform.position = DummyPoints[newR].position;
        }
        //포탑 체력 갱신
        enemyTowerManager.TowerOn();
    }

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
        

        float duration = 1.05f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            if (curHealth <= 0 && !InVisible) break;


            float progress = elapsedTime / duration;
            float value = Mathf.Lerp(firstValue, targetValue, progress);
            elapsedTime += Time.deltaTime;

            if (curCreatureTypeEnum != CreatureTypeEnum.Dummy)
                skinnedMeshRenderer.material.SetFloat("_AlphaFloat", value);
            else if (curCreatureTypeEnum == CreatureTypeEnum.Dummy)
                meshRenderer.material.SetFloat("_AlphaFloat", value);

            yield return null;
        }
        if (curCreatureTypeEnum != CreatureTypeEnum.Dummy)
            skinnedMeshRenderer.material.SetFloat("_AlphaFloat", targetValue);
        else if (curCreatureTypeEnum == CreatureTypeEnum.Dummy)
            meshRenderer.material.SetFloat("_AlphaFloat", targetValue);

        if (InVisible)//안보이도록
        {
            //피격당하지 않도록, 레이어 변경
            gameObject.layer = LayerMask.NameToLayer("WarpCreature");

            if (curCreatureTypeEnum == CreatureTypeEnum.Dummy)
                gameObject.SetActive(false);
        }
        else if(!InVisible)//보이도록 
        {
            //피격당하도록, 레이어 변경
            gameObject.layer = LayerMask.NameToLayer("Creature");
        }
         
    }
    #endregion
}
