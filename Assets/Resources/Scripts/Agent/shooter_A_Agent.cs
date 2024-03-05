using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using static Creature;

public class Shooter_A_Agent : Agent
{
    public override void OnActionReceived(ActionBuffers actions)//�׼� ����(������ ����), �� �� ȣ�� 
    {
        if (!creature.isAttack && gameObject.layer == LayerMask.NameToLayer("Creature"))
        {
            //������ �Ÿ� ���
            creature.EnemyFirstRangeCalc();

            //������� ���� ����
            creature.GetMatchingVelocityReward();

            //�ڵ� ����
            AddReward(-0.001f);

            if (actions.DiscreteActions[0] == 1 && actions.DiscreteActions[1] == 0)
                AddReward(-0.0002f);

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

                        transform.LookAt(creature.curTarget);
                        creature.anim.SetTrigger("isGun");
                    }
                    break;
            }
        }

        //Debug.Log("spin: " + actions.DiscreteActions[0] + "action: " + actions.DiscreteActions[1]);
    }

    //mlagents-learn "D:\Unities\Github_DeskTop\ML_EX_GIT\config\ppo\Siege_Creature.yaml" --run-id=Custom_Shoter_3 --resum

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
            //���� �ڽ��� ����
            sensor.AddObservation(creature.rigid.velocity.x);
            sensor.AddObservation(creature.rigid.velocity.z);
            //���� �ڽ��� ȸ��
            sensor.AddObservation(transform.rotation.y);

            //�������� �Ÿ�
            if (creature.curRange != 0)
                sensor.AddObservation(creature.maxRange / creature.curRange);

            //����� ���� ��ġ
            if (creature.curTarget != null)
            {
                sensor.AddObservation(creature.curTarget.position.x);
                sensor.AddObservation(creature.curTarget.position.z);
            }

            
            sensor.AddObservation(creature.teamIndex);
        }
    }
    #endregion

    [Header("ũ����")]
    public Creature creature;

    [Header("����ϴ� �Ѿ�")]
    public Transform useBullet;

    public override void OnEpisodeBegin()
    {
        //creature.resetEnv();
        creature.Revive();
    }
}
