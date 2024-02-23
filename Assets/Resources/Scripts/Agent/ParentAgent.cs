using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;

public class ParentAgent : Agent
{
    public Creature creature;

    #region 공격한 생명체의 점수 증가
    public void AddAward(float _score)
    {
        AddReward(_score);
    }
    #endregion
}
