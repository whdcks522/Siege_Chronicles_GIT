using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;

public class ParentAgent : Agent
{
    public Creature creature;

    public void AddAward(float _score)//������ ����ü�� ���� ����
    {
        AddReward(_score);
    }

    //������Ʈ���� ���� ����(���)
    virtual public void AgentAttack() 
    {

    }
}
