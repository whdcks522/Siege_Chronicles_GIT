using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;

public class AnimatorSupport : MonoBehaviour
{
    Creature creature;
    SuperAgent superAgent;

    private void Awake()
    {
        creature = transform.parent.GetComponent<Creature>();
        superAgent = transform.parent.GetComponent<SuperAgent>();
    }

    //공격 대기 초기화
    public void AttackClear() => creature.isAttack = false;

    //생명체 완전히 사망 처리
    public void CompletelyDeadAnimation()
    {
        creature.CompletelyDead();
    }

    //에이전트별 액션 1(각자 다형성 적용해서 커스텀), 
    public void AgentAction_1() => superAgent.AgentAction_1();
}
