using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class shooter_A_Agent : ParentAgent
{
    [Header("재시작점")]
    public Transform point;

    public override void OnEpisodeBegin()//EndEpisode가 호출됐을 때 사용됨(씬을 호출할 때는 통째로 삭제)
    {
        creature.Revive();
        transform.position = point.position;
    }
}
