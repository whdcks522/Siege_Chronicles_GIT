using Google.Protobuf.WellKnownTypes;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Presets;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class Creature : MonoBehaviour
{
    [Header("생명체의 최대 체력")]
    public float maxHealth;
    [Header("생명체의 현재 체력")]
    public float curHealth;

    [Header("생명체의 최대 공격 대기 시간")]
    public float maxTime;
    [Header("생명체의 현재 공격 대기 시간")]
    public float curTime;

    [Header("우리 성")]
    public Transform ourTower;
    [Header("상대 성")]
    public Transform enemyTower;

    [Header("캐릭터 위의 미니 UI")]
    public GameObject miniUI;
    public Image miniHealth;

    [Header("달리는 속도")]
    public int runSpd;
    int rotSpd = 30;//회전 속도

    Vector3 moveVec;//이동용 벡터

    Rigidbody rigid;
    Animator anim;

    public enum CreatureMove {Idle, Run, LeftSpin, RightSpin }//머신러닝으로 취할수 있는 행동
    public CreatureMove curCreatureMove;

    public enum TeamEnum { Blue, Red }//머신러닝으로 취할수 있는 행동
    public TeamEnum curTeamEnum;

    //생명체의 상태 목록
    bool isDead = false;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();

        //int index = System.Array.IndexOf(System.Enum.GetValues(typeof(CreatureMove)), curCreatureMove);
        //Debug.Log(index);
    }

    #region 생명체 활성화
    public void Revive()
    {
        //공격 대기 시간 초기화
        curTime = 0;
        //가속 초기화
        rigid.velocity = Vector3.zero;
        //체력 회복
        curHealth = maxHealth;
        miniHealth.fillAmount = 1;

        isDead = false;

        //오브젝트 활성화
        gameObject.SetActive(true);
    }
    #endregion

    #region 애니메이션 조작
    void AnimationControl() 
    {
    
    }
    #endregion


    #region 물리 동작
    private void FixedUpdate()
    {
        switch (curCreatureMove) 
        {
            case CreatureMove.Idle://멈추기
                moveVec = new Vector3(0, rigid.velocity.y, 0);
                rigid.velocity = moveVec;

                anim.SetBool("isIdle", true);
                break;
            case CreatureMove.Run://달리기
                moveVec = new Vector3(0, rigid.velocity.y, 0) + Vector3.forward * runSpd;
                rigid.velocity = moveVec.normalized * runSpd;
                rigid.angularVelocity = Vector3.zero;

                anim.SetBool("isRun", true);
                break;
            case CreatureMove.LeftSpin:
                moveVec = transform.rotation.eulerAngles;
                // 왼쪽으로 조금 회전합니다 (여기서는 y축 값만 조정합니다)
                moveVec.y -= rotSpd * Time.deltaTime;
                // 새로운 회전값을 설정합니다
                transform.rotation = Quaternion.Euler(moveVec);

                anim.SetBool("isIdle", true);
                break;
            case CreatureMove.RightSpin:
                moveVec = transform.rotation.eulerAngles;
                // 왼쪽으로 조금 회전합니다 (여기서는 y축 값만 조정합니다)
                moveVec.y += rotSpd * Time.deltaTime;
                // 새로운 회전값을 설정합니다
                transform.rotation = Quaternion.Euler(moveVec);

                anim.SetBool("isIdle", true);
                break;
        }
    }
    #endregion

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Bomb"))//폭탄과 충돌했을 때
        {
            //int damage = other.gameObject.GetComponent<Bomb>().bombDmg;
            //피격 처리
            //damageControlRPC(damage * 3);
        }
    }

    #region 피격 처리
    public void damageControlRPC(float _dmg)
    {
        if (isDead)
            return;

        //피해량 계산
        curHealth -= _dmg;
        if (curHealth <= 0) curHealth = 0;
        else if (curHealth > maxHealth) curHealth = maxHealth;
        //UI관리
        miniHealth.fillAmount = curHealth / maxHealth;

        //충격 초기화
        if (curHealth > 0)//피격
        {
               
        }
        else if (curHealth <= 0) Dead();
    }
    #endregion

    #region 사망처리
    void Dead()
    {
        //if (!isML)
        {
            //사망 처리
            isDead = true;
            curCreatureMove = CreatureMove.Idle;
            
            //미니 UI 닫기
            miniHealth.fillAmount = 0;
            //먼지 종료
            
            //곧 죽음
            Invoke("SoonDie", 1.5f);
        }
    }
    #endregion

    #region 사망 뒤, 소멸
    void SoonDie()//죽었고 조금 뒤, 죽음에 대한 처리
    {
        //게임오브젝트 활성화
        gameObject.SetActive(false);
    }
    #endregion


}
