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
        AgentAttackRed();
    }

    [Header("����ϴ� �Ѿ�")]
    public Transform useBullet;

    #region ��Ȳ�� ���� ����
    public void AgentAttackRed()
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




}
