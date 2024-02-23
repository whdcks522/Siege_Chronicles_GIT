using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;

public class ParentAgent : Agent
{
    public Creature creature;

    public void AddAward(float _score)//공격한 생명체의 점수 증가
    {
        AddReward(_score);
    }

    //에이전트에서 각자 공격(상속)
    virtual public void AgentAttack() 
    {

    }
}
