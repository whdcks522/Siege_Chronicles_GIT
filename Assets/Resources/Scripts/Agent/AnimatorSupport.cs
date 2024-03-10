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

    //공격 대기 초기화
    public void AttackClear() => creature.isAttack = false;

    //생명체 완전히 사망 처리
    public void CompletelyDeadAnimation()
    {
        creature.CompletelyDead();
    }

    public void AgentAttack()//에이전트 공격(상속)
    {
        switch (creature.curCreatureTypeEnum) 
        {
            case Creature.CreatureTypeEnum.Infantry_A:
                Infantry_A_Attack();
                break;
            case Creature.CreatureTypeEnum.Shooter_A:
                Shooter_A_Attack();
                break;
        }
    }

    [Header("사용하는 총알")]
    public Transform useBullet;

    #region 주황색 참격 생성(shooter_A_Agent)
    public void Infantry_A_Attack()
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

    #region 초록 투사체 생성(shooter_A_Agent)
    public void Shooter_A_Attack()
    {
        string bulletName = useBullet.name;

        GameObject tracer = creature.objectManager.CreateObj(bulletName, ObjectManager.PoolTypes.BulletPool);
        Bullet tracer_bullet = tracer.GetComponent<Bullet>();
        Rigidbody tracer_rigid = tracer.GetComponent<Rigidbody>();

        tracer_bullet.gameManager = creature.gameManager;
        tracer_bullet.Init();


        //이동
        tracer.transform.position = creature.bulletStartPoint.position;
        //회전
        //tracer_rigid.velocity = tracer_bullet.bulletSpeed * transform.forward;
        tracer_rigid.velocity = creature.goalVec * tracer_bullet.bulletSpeed;
        //활성화
        tracer_bullet.BulletOn(creature);
    }
    #endregion




}
