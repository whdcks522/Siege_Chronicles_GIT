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

    //������Ʈ���� ���� ����(���)
    virtual public void AgentAttack() 
    {

    }


    //��ǥ ���� ����
    Vector3 goalVec;
    //���簪 ���ִ� ����
    Vector3 curVec;

    public void GetMatchingVelocityReward() 
    {
        //��ǥ ���� ����
        goalVec = (creature.enemyTower.transform.position - transform.position).normalized;
        //���簪 ���ִ� ����
        curVec = rigid.velocity.normalized;


        // �� ���� ������ ���� ��� (���� ����)
        float angle = Vector3.Angle(goalVec, curVec);
        // �ڻ��� ���絵 ��� (-1���� 1������ ��)
        float cosineSimilarity = Mathf.Cos(angle * Mathf.Deg2Rad);

        float reward = 0f;

        
        if (creature.curCreatureMoveEnum == CreatureMoveEnum.Idle)//���ִٸ� 0�� ��ȯ
            reward = 0f;
        else // 0���� 1������ ���� ��ȯ
            reward = (cosineSimilarity + 1f) / 2f;

        Debug.Log(reward);
        AddReward(reward / 100f);
    }
}
