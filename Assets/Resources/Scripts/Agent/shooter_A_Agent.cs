using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using static Creature;

public class shooter_A_Agent : Agent
{
    public override void OnActionReceived(ActionBuffers actions)//�׼� ����(������ ����), �� �� ȣ�� 
    {
        if (!creature.isAttack && gameObject.layer == LayerMask.NameToLayer("Creature"))
        {
            creature.rewardValue = GetCumulativeReward();
            creature.curReward.text = creature.rewardValue.ToString("F1");

            //������� ���� ����
            creature.GetMatchingVelocityReward();

            //������ �Ÿ� ���
            creature.RangeCalculate();



            switch (actions.DiscreteActions[0])
            {
                case 0://�������� ȸ��
                    creature.curCreatureSpinEnum = CreatureSpinEnum.LeftSpin;
                    break;
                case 1://���ֱ�
                    creature.curCreatureSpinEnum = CreatureSpinEnum.None;
                    break;
                case 2://���������� ȸ��
                    creature.curCreatureSpinEnum = CreatureSpinEnum.RightSpin;
                    break;
            }

            switch (actions.DiscreteActions[1])
            {
                case 0://���ֱ�
                    creature.curCreatureMoveEnum = CreatureMoveEnum.Idle;
                    break;
                case 1://�޸���
                    creature.curCreatureMoveEnum = CreatureMoveEnum.Run;
                    break;
                case 2://����
                    if (creature.curRange <= creature.maxRange && gameObject.layer == LayerMask.NameToLayer("Creature"))
                    {
                        //�ִϸ��̼� ����
                        creature.curCreatureSpinEnum = CreatureSpinEnum.None;
                        creature.curCreatureMoveEnum = CreatureMoveEnum.Idle;
                        creature.isAttack = true;//���� �Է� ����

                        int r = UnityEngine.Random.Range(0, 2);
                        if (r == 0) creature.anim.SetTrigger("isAttackLeft");
                        else if (r == 1) creature.anim.SetTrigger("isAttackRight");
                    }
                    break;
            }
        }

        //Debug.Log("spin: " + actions.DiscreteActions[0] + "action: " + actions.DiscreteActions[1]);
    }



    #region �޸���ƽ: Ű���带 ���� ������Ʈ�� ����
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var disCreteActionOut = actionsOut.DiscreteActions;

        if (creature.behaviorParameters.BehaviorType == Unity.MLAgents.Policies.BehaviorType.HeuristicOnly)
        {
            int spin = 1;//ȸ�� ����
            if (Input.GetKey(KeyCode.LeftArrow))//��ȸ��
                spin = 0;
            else if (Input.GetKey(KeyCode.RightArrow))//��ȸ��
                spin = 2;

            int action = 0;//�׼� ����
            if (Input.GetKey(KeyCode.UpArrow))//�ȱ�
                action = 1;
            else if (Input.GetKey(KeyCode.Z))//����
                action = 2;

            disCreteActionOut[0] = spin;
            disCreteActionOut[1] = action;
        }
        else
        {
            disCreteActionOut[0] = 1;
            disCreteActionOut[1] = 0;
        }

    }
    #endregion

    #region ������ ����, 5�� �� �ѹ� ȣ��
    public override void CollectObservations(VectorSensor sensor)
    {
        //1. ��ġ��, �޾ƿ��� �����Ͱ� ���� ���� ����
        if (gameObject.layer == LayerMask.NameToLayer("Creature")) //������ �ʿ� ���ڳ�
        {
            //���� �ڽ��� ��ġ
            sensor.AddObservation(transform.position.x);//state size = 1     x,y,z�� ��� �޾ƿ��� size�� 3�� ��
            sensor.AddObservation(transform.position.z);
            //���� �ڽ��� ȸ��
            sensor.AddObservation(transform.rotation.y);
            //���� �ڽ��� ����
            //sensor.AddObservation(rigid.velocity.x);
            //sensor.AddObservation(rigid.velocity.z);

            //�ڱ� Ÿ���� ����
            //sensor.AddObservation(creature.ourTower.position.x);
            //sensor.AddObservation(creature.ourTower.position.z);
            sensor.AddObservation(creature.ourTowerManager.curHealth / creature.ourTowerManager.maxHealth);

            //��� Ÿ���� ����
            //sensor.AddObservation(creature.enemyTower.position.x);
            //sensor.AddObservation(creature.enemyTower.position.z);
            sensor.AddObservation(creature.enemyTowerManager.curHealth / creature.enemyTowerManager.maxHealth);

            //�������� �Ÿ�
            sensor.AddObservation(creature.curRange / creature.maxRange);

            //����� ���� ��ġ
            sensor.AddObservation(creature.curTarget.position.x);
            sensor.AddObservation(creature.curTarget.position.z);

            //�츮 Ÿ������ ���� ����� ���� ��ġ
            sensor.AddObservation(creature.ourTowerManager.curTarget.position.x);
            sensor.AddObservation(creature.ourTowerManager.curTarget.position.z);
        }
    }
    #endregion
    [Header("ũ����")]
    public Creature creature;

    [Header("����ϴ� �Ѿ�")]
    public Transform useBullet;


    #region �ʷ� ����ü ����
    public void AgentAttack()
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
        tracer_rigid.velocity = tracer_bullet.bulletSpeed * transform.forward;
        //Ȱ��ȭ
        tracer_bullet.BulletOn(creature);
    }
    #endregion
}
