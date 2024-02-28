using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Policies;
using UnityEngine;
using static Creature;

public class ParentAgent : Agent
{
    [Header("보상 값")]
    public float rewardValue;

    [Header("상대팀이 들어있는 폴더")]
    public Transform enemyCreatureFolder;

    [Header("사용하는 총알")]
    public Transform useBullet;

    public BehaviorParameters behaviorParameters;
    public Creature creature;
    public Rigidbody rigid;//상속 때문
    public Animator anim;

    public GameManager gameManager;
    public ObjectManager objectManager;
    
    public AudioManager audioManager;
    

    //에이전트에서 각자 공격(상속)
    virtual public void AgentAttack() 
    {

    }


    //목표 방향 벡터
    Vector3 goalVec;
    //이동하는 벡터
    Vector3 curVec;

    public void GetMatchingVelocityReward() 
    {
        //목표 방향 벡터
        goalVec = (creature.enemyTower.transform.position - transform.position).normalized;
        //현재값 서있는 벡터
        curVec = rigid.velocity.normalized;


        // 두 벡터 사이의 각도 계산 (라디안 단위)
        float angle = Vector3.Angle(goalVec, curVec);
        // 코사인 유사도 계산 (-1부터 1까지의 값)
        float cosineSimilarity = Mathf.Cos(angle * Mathf.Deg2Rad);

        float reward = 0f;


        if (creature.curCreatureMoveEnum != CreatureMoveEnum.Idle)//서있다면 0을 반환
        {
            reward = (cosineSimilarity + 1f) / 2f;  //0f ~ 1f
            reward -= 0.5f;                         //-0.5f ~ 0.5f
          
            //Debug.Log(reward);
            AddReward(reward / 1000f);
        }
    }

    public override void OnEpisodeBegin()//EndEpisode가 호출됐을 때 사용됨(씬을 호출할 때는 통째로 삭제)
    {
        //Debug.Log("초기화");//계속 호출됨(팀 단위 초기화만 해도)

        StateReturn();

        
    }

    public void StateReturn() 
    {
        //Debug.Log("초기화");
        //SetReward(0);

        creature.Revive();
    }

    [Header("공격 가능한 최대 거리")]
    public float maxRange;
    [Header("현재 대상과의 거리")]
    public float curRange;
    [Header("가장 가까운 대상")]
    public Transform curTarget;


    #region 적들과의 거리 계산
    protected void RangeCalculate() 
    {
        curRange = (creature.enemyTower.position - transform.position).magnitude - 2;//타워의 두께 계산
        curTarget = creature.enemyTower;

        for (int i = 0; i < enemyCreatureFolder.childCount; i++)
        {
            if (enemyCreatureFolder.GetChild(i).gameObject.layer == LayerMask.NameToLayer("Creature"))//활성화돼있다면
            {
                //적과의 거리
                float tmpRange = (enemyCreatureFolder.GetChild(i).position - transform.position).magnitude;
                if (curRange > tmpRange) 
                {
                    curRange = tmpRange;
                    curTarget = enemyCreatureFolder.GetChild(i);
                }
                
            }
        }

    }
    #endregion 

    /*
    이동하면 점수

    공격 맞추면 점수    
    공격 못맞추면 실점

    타워 맞추면 득점
    타워 이기는쪽은 득점, 나머지는 실점
    */
}
