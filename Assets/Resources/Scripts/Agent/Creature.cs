using Google.Protobuf.WellKnownTypes;
using System.Collections;
using System.Collections.Generic;
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
    float curHealth;

    [Header("가까운 적")]
    public Transform target;
    [Header("공격 가능한 최대 거리")]
    public float maxRange;
    [Header("현재 대상과의 거리")]
    public float curRange;

    [Header("우리 성")]
    public Transform ourTower;
    [Header("상대 성")]
    public Transform enemyTower;

    [Header("총알이 시작되는 곳")]
    public Transform bulletStart;

    [Header("캐릭터 위의 미니 UI")]
    public GameObject miniUI;
    public Image miniHealth;

    [Header("달리는 속도")]
    public int runSpd;
    int rotSpd = 120;//회전 속도

    Vector3 moveVec;//이동용 벡터

    

    public GameManager gameManager;
    Rigidbody rigid;
    Animator anim;
    ParentAgent parentAgent;

    public enum CreatureMoveEnum {Idle, Run}//머신러닝으로 취할수 있는 행동
    public CreatureMoveEnum curCreatureMoveEnum;
    
    public enum CreatureSpinEnum { LeftSpin, None,  RightSpin }//머신러닝으로 취할수 있는 회전
    public CreatureSpinEnum curCreatureSpinEnum;

    public enum TeamEnum { None, Blue, Red }//속하는 팀
    public TeamEnum curTeamEnum;

    //public enum CreatureTypeEnum { Melee, Range }//속하는 팀
    //public TeamEnum curTeamEnum;

    //생명체의 상태 목록
    bool isDead = false;
    [Header("현재 공격 중인지")]
    public bool isAttack = false;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        parentAgent = GetComponent<ParentAgent>();

        //텍스쳐 매터리얼 설정
        skinnedMeshRenderer.material.SetTexture("_BaseTexture", baseTexture);

        //int index = System.Array.IndexOf(System.Enum.GetValues(typeof(CreatureMove)), curCreatureMove);
        //Debug.Log(index);
    }

    #region 생명체 활성화
    public void Revive()
    {
        //공격 대기 시간 초기화
        isAttack = false;
        //대상과의 거리 초기화
        curRange = 0;
        //가속 초기화
        rigid.velocity = Vector3.zero;
        //체력 회복
        curHealth = maxHealth;
        isDead = false;

        //miniHealth.fillAmount = 1;

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
            //if (target != null)
               // creature.curRange = (creature.target.transform.position - transform.position).magnitude;

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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Bullet"))//폭탄과 충돌했을 때
        {
            Bullet bullet = other.GetComponent<Bullet>();
            if (bullet.curTeamEnum != curTeamEnum)//팀이 다를 경우
            {
                ParentAgent bulletHost = bullet.GetComponent<ParentAgent>();
                float damage = bullet.bulletDamage;

                bulletHost.AddReward(damage / 100f);//20이면 0.2f
            }
        }
    }

    #region 피격 처리
    public void damageControlRPC(float _dmg)
    {
        if (isDead)
            return;

        //피해량 계산
        curHealth -= _dmg;
        if (curHealth < 0) curHealth = 0;
        else if (curHealth > maxHealth) curHealth = maxHealth;
        //UI관리
        //miniHealth.fillAmount = curHealth / maxHealth;

        //충격 초기화
        if (curHealth > 0)//피격하고 살아 있음
        {
            anim.SetTrigger("isHit");
        }
        else if (curHealth <= 0) Dead();
    }
    #endregion

    #region 사망처리
    void Dead()
    {
        //사망 처리
        isDead = true;
        //애니메이션 실행
        anim.SetTrigger("isDeath");
        //미니 UI 닫기
        //miniHealth.fillAmount = 0;
        //먼지 종료

        //곧 죽음
        Invoke("SoonDie", 1.5f);
    }
    #endregion

    #region 사망 뒤, 소멸
    void SoonDie()//죽었고 조금 뒤, 죽음에 대한 처리
    {
        //게임오브젝트 활성화
        gameObject.SetActive(false);
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
    IEnumerator Dissolve(bool  InVisible)//왜곡장 1.5초간
    {
        

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
