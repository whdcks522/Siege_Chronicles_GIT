using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;

public class SuperAgent : Agent
{
    [Header("ũ����")]
    public Creature creature;

    [Header("����ϴ� �Ѿ�")]
    public Transform useBullet;

    virtual public void AgentAction_1()//����ؼ� ���� �����ϴ� �� �Լ�
    {

    }
}
