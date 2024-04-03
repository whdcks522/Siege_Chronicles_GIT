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

    //���� ��� �ʱ�ȭ
    public void AttackClear() => creature.isAttack = false;

    //����ü ������ ��� ó��
    public void CompletelyDeadAnimation()
    {
        creature.CompletelyDead();
    }

    //������Ʈ�� �׼� 1(���� ������ �����ؼ� Ŀ����), 
    public void AgentAction_1() => superAgent.AgentAction_1();
}
