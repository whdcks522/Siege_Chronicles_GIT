using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;

public class ParentAgent : Agent
{
    public Creature creature;

    //에이전트에서 각자 공격(상속)
    virtual public void AgentAttack() 
    {

    }
}
