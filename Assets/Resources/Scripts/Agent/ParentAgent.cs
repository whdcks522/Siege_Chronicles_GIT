using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;

public class ParentAgent : Agent
{
    public Creature creature;

    #region ������ ����ü�� ���� ����
    public void AddAward(float _score)
    {
        AddReward(_score);
    }
    #endregion
}
