using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class shooter_A_Agent : ParentAgent
{
    [Header("�������")]
    public Transform point;

    public override void OnEpisodeBegin()//EndEpisode�� ȣ����� �� ����(���� ȣ���� ���� ��°�� ����)
    {
        creature.Revive();
        transform.position = point.position;
    }
}
