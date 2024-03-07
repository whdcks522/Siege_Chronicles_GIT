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

    //���� ��� �ʱ�ȭ
    public void AttackClear() => creature.isAttack = false;

    public void CompletelyDeadAnimation()//����ü ������ ��� ó��
    {
        creature.CompletelyDead();
    }

    public void AgentAttack()//������Ʈ ����(���)
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

    [Header("����ϴ� �Ѿ�")]
    public Transform useBullet;

    #region ��Ȳ�� ���� ����(shooter_A_Agent)
    public void Infantry_A_Attack()
    {
        string bulletName = useBullet.name;

        GameObject slash = creature.objectManager.CreateObj(bulletName, ObjectManager.PoolTypes.BulletPool);
        Bullet slash_bullet = slash.GetComponent<Bullet>();

        //�̵�
        slash.transform.position = transform.position + transform.forward + Vector3.up * 3;

        //ȸ��
        slash.transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x + 90,
            transform.rotation.eulerAngles.y - 180, transform.rotation.eulerAngles.z - 90);
        //Ȱ��ȭ
        slash_bullet.BulletOn(creature);
    }
    #endregion

    #region �ʷ� ����ü ����(shooter_A_Agent)
    public void Shooter_A_Attack()
    {
        string bulletName = useBullet.name;

        GameObject tracer = creature.objectManager.CreateObj(bulletName, ObjectManager.PoolTypes.BulletPool);
        Bullet tracer_bullet = tracer.GetComponent<Bullet>();
        Rigidbody tracer_rigid = tracer.GetComponent<Rigidbody>();

        tracer_bullet.gameManager = creature.gameManager;
        tracer_bullet.Init();


        //�̵�
        tracer.transform.position = creature.bulletStartPoint.position;
        //ȸ��
        //tracer_rigid.velocity = tracer_bullet.bulletSpeed * transform.forward;
        tracer_rigid.velocity = creature.goalVec * tracer_bullet.bulletSpeed;
        //Ȱ��ȭ
        tracer_bullet.BulletOn(creature);
    }
    #endregion




}
