using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;
using static Creature;

public class ParentAgent : Agent
{
    public Creature creature;
    public Rigidbody rigid;

    private void Awake()
    {
        //Debug.Log("ASDFSA");
        //rigid = GetComponent<Rigidbody>();
    }

    //에이전트에서 각자 공격(상속)
    virtual public void AgentAttack() 
    {

    }


    //목표 방향 벡터
    Vector3 goalVec;
    //현재값 서있는 벡터
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

        
        if (creature.curCreatureMoveEnum == CreatureMoveEnum.Idle)//서있다면 0을 반환
            reward = 0f;
        else // 0부터 1까지의 값을 반환
            reward = (cosineSimilarity + 1f) / 2f;

        //Debug.Log(reward); //0f ~ 1f
        AddReward(reward / 100f);
    }
}
