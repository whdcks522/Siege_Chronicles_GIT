using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;
using static Creature;

public class ParentAgent : Agent
{
    public Creature creature;

    //에이전트에서 각자 공격(상속)
    virtual public void AgentAttack() 
    {

    }

    public float GetMatchingVelocityReward(Vector3 velocityGoal, Vector3 actualVelocity)
    {
        // 두 벡터 사이의 각도 계산 (라디안 단위)
        float angle = Vector3.Angle(velocityGoal, actualVelocity);
        // 코사인 유사도 계산 (-1부터 1까지의 값)
        float cosineSimilarity = Mathf.Cos(angle * Mathf.Deg2Rad);

        //서있다면 0을 반환
        if (creature.curCreatureMoveEnum == CreatureMoveEnum.Idle)
            return 0f;

        // 0부터 1까지의 값을 반환
        return (cosineSimilarity + 1f) / 2f;
    }
}
