using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorSupport : MonoBehaviour
{
    Creature creature;
    ParentAgent parentAgent;

    private void Awake()
    {
        creature = transform.parent.GetComponent<Creature>();
        parentAgent = transform.parent.GetComponent<ParentAgent>();
    }

    //공격 대기 초기화
    public void AttackClear() => creature.isAttack = false;

    public void AgentAttack()//에이전트 공격(상속)
    {
        parentAgent.AgentAttack();
    }
}
