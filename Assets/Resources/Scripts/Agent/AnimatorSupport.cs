using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;

public class AnimatorSupport : MonoBehaviour
{
    Creature creature;
    Agent agent;

    private void Awake()
    {
        creature = transform.parent.GetComponent<Creature>();
        agent = transform.parent.GetComponent<Agent>();
    }

    //공격 대기 초기화
    public void AttackClear() => creature.isAttack = false;

    public void CompletelyDeadAnimation()//생명체 완전히 사망 처리
    {
        creature.CompletelyDead();
    }

    public void AgentAttack()//에이전트 공격(상속)
    {
        AgentAttackRed();
    }

    [Header("사용하는 총알")]
    public Transform useBullet;

    #region 주황색 참격 생성
    public void AgentAttackRed()
    {
        string bulletName = useBullet.name;

        GameObject slash = creature.objectManager.CreateObj(bulletName, ObjectManager.PoolTypes.BulletPool);
        Bullet slash_bullet = slash.GetComponent<Bullet>();

        //이동
        slash.transform.position = transform.position + transform.forward + Vector3.up * 3;

        //회전
        slash.transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x + 90,
            transform.rotation.eulerAngles.y - 180, transform.rotation.eulerAngles.z - 90);
        //활성화
        slash_bullet.BulletOn(creature);
    }
    #endregion




}
