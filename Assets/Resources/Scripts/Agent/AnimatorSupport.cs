using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorSupport : MonoBehaviour
{
    public Creature creature;

    #region ���� ��� �ʱ�ȭ
    public void AttackClear() => creature.isAttack = false;
    #endregion


}
