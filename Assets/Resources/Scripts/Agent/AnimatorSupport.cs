using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorSupport : MonoBehaviour
{
    public Creature creature;

    #region 공격 대기 초기화
    public void AttackClear() => creature.isAttack = false;
    #endregion


}
