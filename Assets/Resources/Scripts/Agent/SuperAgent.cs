using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;

public class SuperAgent : Agent
{
    [Header("크리쳐")]
    public Creature creature;

    [Header("사용하는 총알")]
    public Transform useBullet;

    virtual public void AgentAction_1()//상속해서 각자 공격하는 용 함수
    {

    }
}
