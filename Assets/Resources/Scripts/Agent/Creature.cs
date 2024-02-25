using Google.Protobuf.WellKnownTypes;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Presets;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class Creature : MonoBehaviour
{
    [Header("쉐이더에 쓰일 텍스쳐")]
    public Texture baseTexture;
    [Header("쉐이더에 쓰일 스킨 렌더러")]
    public SkinnedMeshRenderer skinnedMeshRenderer;

    [Header("생명체의 최대 체력")]
    public float maxHealth;
    [Header("생명체의 현재 체력")]
    public float curHealth;

    [Header("현재 공격 중인지")]
    public bool isAttack = false;

    [Header("우리 성")]
    public Transform ourTower;
    [Header("시작할 장소")]
    public Transform createPoint;
    [Header("상대 성")]
    public Transform enemyTower;

    [Header("총알이 시작되는 곳")]
    public Transform bulletStartPoint;

    [Header("캐릭터 위의 미니 UI")]
    public GameObject miniCanvas;
    public Image miniHealth;

    [Header("달리는 속도")]
    public int runSpd;
    int rotSpd = 120;//회전 속도

    Vector3 moveVec;//이동용 벡터

    public enum CreatureMoveEnum { Idle, Run }//머신러닝으로 취할수 있는 행동
    public CreatureMoveEnum curCreatureMoveEnum;

    public enum CreatureSpinEnum { LeftSpin, None, RightSpin }//머신러닝으로 취할수 있는 회전
    public CreatureSpinEnum curCreatureSpinEnum;

    public enum TeamEnum { None, Blue, Red }//속하는 팀
    public TeamEnum curTeamEnum;

    //public enum CreatureTypeEnum { Melee, Range }//속하는 팀
    //public TeamEnum curTeamEnum;

    Rigidbody rigid;
    Animator anim;
    ParentAgent parentAgent;
    public GameManager gameManager;
    
    UIManager UIManager;
    Transform cameraGround;
    Transform mainCamera;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        parentAgent = GetComponent<ParentAgent>();

        UIManager = gameManager.uiManager;
        mainCamera = UIManager.cameraObj;
        cameraGround = UIManager.cameraGround;


        //텍스쳐 매터리얼 설정
        skinnedMeshRenderer.material.SetTexture("_BaseTexture", baseTexture);
    }

    #region 생명체 활성화
    public void Revive()
    {
        //공격 대기 시간 초기화
        isAttack = false;
        //가속 초기화
        rigid.velocity = Vector3.zero;
        //체력 회복
        curHealth = maxHealth;

        //체력 UI 관리
        miniCanvas.SetActive(true);
        miniHealth.fillAmount = 1;
        if(curTeamEnum == TeamEnum.Blue)
            miniHealth.color = Color.blue;
        else if (curTeamEnum == TeamEnum.Red)
            miniHealth.color = Color.red;
        //오브젝트 활성화
        gameObject.SetActive(true);
        //기상 애니메이션
        curCreatureSpinEnum = CreatureSpinEnum.None;
        curCreatureMoveEnum = CreatureMoveEnum.Idle;
        anim.SetTrigger("isRage");

        VisibleWarp();
    }
    #endregion
   

    #region 물리 동작
    private void FixedUpdate()
    {
        if (gameObject.layer == LayerMask.NameToLayer("Creature")) 
        {

            //행동 관리
            switch (curCreatureMoveEnum)
            {
                case CreatureMoveEnum.Idle://멈추기
                    moveVec = new Vector3(0, rigid.velocity.y, 0);
                    rigid.velocity = moveVec;

                    anim.SetBool("isRun", false);
                    break;
                case CreatureMoveEnum.Run://달리기
                    moveVec = new Vector3(0, rigid.velocity.y, 0) + transform.forward * runSpd;
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

                    //anim.SetBool("isRun", false);
                    break;
                case CreatureSpinEnum.None:
                    break;
                case CreatureSpinEnum.RightSpin:
                    moveVec = transform.rotation.eulerAngles;
                    // 왼쪽으로 조금 회전합니다 (여기서는 y축 값만 조정합니다)
                    moveVec.y += rotSpd * Time.deltaTime;
                    // 새로운 회전값을 설정합니다
                    transform.rotation = Quaternion.Euler(moveVec);

                    //anim.SetBool("isRun", false);
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
                ParentAgent bulletParentAgent = bullet.bulletHost;
                float damage = bullet.bulletDamage;

                //공격자 점수 증가
                bulletParentAgent.AddReward(damage / 10f);
                //피해 관리
                damageControl(damage);

                //피격한 총알 후처리
                if(bullet.curBulletMoveEnum != Bullet.BulletMoveEnum.Slash)
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
        if (curHealth > 0)//피격하고 살아 있음
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
        anim.SetTrigger("isDeath");

        //미니 UI 닫기
        miniCanvas.SetActive(false);

        //먼지 종료
        //왜곡장
        InvisibleWarp();
    }

    //완전히 죽음
    public void CompletelyDead()=> gameObject.SetActive(false);
    

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
        

        float duration = 1.05f;
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
