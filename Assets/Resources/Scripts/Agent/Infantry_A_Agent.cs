using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.VisualScripting;
using UnityEngine;
using static Creature;

public class Infantry_A_Agent : ParentAgent
{


    //���� ������ ���� ����, �ȸ����� ����
    //���� ������ ���� ����

    //  mlagents-learn --force
    //mlagents-learn "D:\gitHubDeskTop\ML_EX_GIT\config\ppo\Enemy_Orc.yaml" --run-id=Enemy_Orc_K --resum(2�ð��������� ���� ��� ���۵�)
    //mlagents-learn "D:\Unities\Github_DeskTop\ML_EX_GIT\config\ppo\Custom_shooter_A.yaml" --run-id=ustom_shooter_B --resum

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        creature = GetComponent<Creature>();
        anim = GetComponentInChildren<Animator>();

        gameManager = creature.gameManager;
        objectManager = gameManager.objectManager;

        if(creature.curTeamEnum == TeamEnum.Blue)
            enemyCreatureFolder = objectManager.redCreatureFolder;
        else if (creature.curTeamEnum == TeamEnum.Red)
            enemyCreatureFolder = objectManager.blueCreatureFolder;
    }

    
    public override void OnActionReceived(ActionBuffers actions)//�׼� ����(������ ����), �� �� ȣ�� 
    {
        if (!creature.isAttack && gameObject.layer == LayerMask.NameToLayer("Creature")) 
        {
            rewardValue =  GetCumulativeReward();


            //������� ���� ����
            GetMatchingVelocityReward();
            //������ �Ÿ� ���
            RangeCalculate();

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
                    if (gameObject.activeSelf)
                    {
                        if (curRange <= maxRange)//��Ÿ���� �������鼭, �Ÿ� �̳����� ��
                        {
                            //�ִϸ��̼� ����
                            creature.curCreatureSpinEnum = CreatureSpinEnum.None;
                            creature.curCreatureMoveEnum = CreatureMoveEnum.Idle;
                            creature.isAttack = true;//���� �Է� ����

                            int r = UnityEngine.Random.Range(0, 2);
                            if (r == 0) anim.SetTrigger("isAttackLeft");
                            else if (r == 1) anim.SetTrigger("isAttackRight");
                        }
                        else AddReward(-0.5f);
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

        if (behaviorParameters.BehaviorType == Unity.MLAgents.Policies.BehaviorType.HeuristicOnly)
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
        if (gameObject.layer == LayerMask.NameToLayer("Creature") && gameObject.activeSelf) //������ �ʿ� ���ڳ�
        {
            //���� �ڽ��� ��ġ
            sensor.AddObservation(transform.position.x);//state size = 1     x,y,z�� ��� �޾ƿ��� size�� 3�� ��
            sensor.AddObservation(transform.position.z);
            //���� �ڽ��� ����
            sensor.AddObservation(rigid.velocity.x);
            sensor.AddObservation(rigid.velocity.z);

            //�ڱ� Ÿ���� ����
            sensor.AddObservation(creature.enemyTower.position.x);
            sensor.AddObservation(creature.enemyTower.position.z);
            //��� Ÿ���� ����
            sensor.AddObservation(creature.enemyTower.position.x);
            sensor.AddObservation(creature.enemyTower.position.z);

            //������ �Ÿ�
            sensor.AddObservation(curRange / maxRange);

            for (int i = 0; i < enemyCreatureFolder.childCount; i++)
            {
                if (enemyCreatureFolder.GetChild(i).gameObject.activeSelf)//Ȱ��ȭ���ִٸ�
                {
                    sensor.AddObservation(enemyCreatureFolder.GetChild(i).position.x);
                    sensor.AddObservation(enemyCreatureFolder.GetChild(i).position.z);
                }
            }
        }
    }
    #endregion


    #region ��Ȳ�� ���� ����
    override public void AgentAttack()
    {
        GameObject slash = objectManager.CreateObj("Infantry_A_Slash", ObjectManager.PoolTypes.BulletPool);
        Bullet slash_bullet = slash.GetComponent<Bullet>();
        
        //�̵�
        slash.transform.position = transform.position + transform.forward + Vector3.up * 3;

        //ȸ��
        slash.transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x + 90,
            transform.rotation.eulerAngles.y - 180, transform.rotation.eulerAngles.z - 90);
        //Ȱ��ȭ
        slash_bullet.BulletOn(this);
    }
    #endregion
}
