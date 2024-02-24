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

    //���� ��� �ʱ�ȭ
    public void AttackClear() => creature.isAttack = false;

    public void CompletelyDeadAnimation()//������ ����ü ��� ó��
    {
        creature.CompletelyDead();
    }

    public void AgentAttack()//������Ʈ ����(���)
    {
        parentAgent.AgentAttack();
    }

    


}
