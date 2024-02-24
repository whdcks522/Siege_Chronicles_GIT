using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;
using static Creature;

public class ParentAgent : Agent
{
    public Creature creature;

    //������Ʈ���� ���� ����(���)
    virtual public void AgentAttack() 
    {

    }

    public float GetMatchingVelocityReward(Vector3 velocityGoal, Vector3 actualVelocity)
    {
        // �� ���� ������ ���� ��� (���� ����)
        float angle = Vector3.Angle(velocityGoal, actualVelocity);
        // �ڻ��� ���絵 ��� (-1���� 1������ ��)
        float cosineSimilarity = Mathf.Cos(angle * Mathf.Deg2Rad);

        //���ִٸ� 0�� ��ȯ
        if (creature.curCreatureMoveEnum == CreatureMoveEnum.Idle)
            return 0f;

        // 0���� 1������ ���� ��ȯ
        return (cosineSimilarity + 1f) / 2f;
    }
}
