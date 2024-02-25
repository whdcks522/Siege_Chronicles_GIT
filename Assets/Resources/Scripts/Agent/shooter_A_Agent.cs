using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using static Creature;

public class shooter_A_Agent : ParentAgent
{
    GameManager gameManager;
    ObjectManager objectManager;
    AudioManager audioManager;
    //AiManager aiManager;
    Animator anim;

    //  mlagents-learn --force
    //mlagents-learn "D:\gitHubDeskTop\ML_EX_GIT\config\ppo\Enemy_Orc.yaml" --run-id=Enemy_Orc_K --resum(2�ð��������� ���� ��� ���۵�)

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        creature = GetComponent<Creature>();
        anim = GetComponentInChildren<Animator>();

        gameManager = creature.gameManager;
        objectManager = gameManager.objectManager;
    }

    
    public override void OnActionReceived(ActionBuffers actions)//�׼� ����(������ ����), �� �� ȣ�� 
    {
        GetMatchingVelocityReward();


        if (!creature.isAttack && gameObject.layer == LayerMask.NameToLayer("Creature"))
        {
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
                        if (creature.curRange <= creature.maxRange)//��Ÿ���� �������鼭, �Ÿ� �̳����� ��
                        {
                            //�ִϸ��̼� ����
                            creature.curCreatureSpinEnum = CreatureSpinEnum.None;
                            creature.curCreatureMoveEnum = CreatureMoveEnum.Idle;
                            creature.isAttack = true;//���� �Է� ����

                            anim.SetTrigger("isGun");
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
            //Debug.Log("�޸���ƽ ���Ÿ�");
            

            int spin = 1;//ȸ�� ����
            if (Input.GetKey(KeyCode.LeftArrow))//��ȸ��
                spin = 0;
            else if (Input.GetKey(KeyCode.RightArrow))//��ȸ��
                spin = 2;

            int action = 0;//�׼� ����
            if (Input.GetKey(KeyCode.UpArrow))//�ȱ�
                action = 1;
            else if (Input.GetKey(KeyCode.Z))//����ü ����
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
        //�ڽ��� ����
        if (gameObject.layer == LayerMask.NameToLayer("Creature")) //������ �ʿ� ���ڳ�
        {
            sensor.AddObservation(transform.position.x);//state size = 1     x,y,z�� ��� �޾ƿ��� size�� 3�� ��
            sensor.AddObservation(transform.position.z);

            //������ ���ϱ⵵ ��
            sensor.AddObservation(rigid.velocity.x);
            sensor.AddObservation(rigid.velocity.z);

            if (creature.target != null) //���� �� ����, �� ��޵�
            {
                //�÷��̾��� ����
                sensor.AddObservation(creature.target.transform.position.x);
                sensor.AddObservation(creature.target.transform.position.z);
                //������ �Ÿ�
                sensor.AddObservation(creature.curRange);
            }
        }
    }
    #endregion

    [Header("�������")]
    public Transform point;

    public override void OnEpisodeBegin()//EndEpisode�� ȣ����� �� ����(���� ȣ���� ���� ��°�� ����)
    {
        creature.Revive();
        transform.position = point.position;
    }

    #region ����ü ����
    override public void AgentAttack()
    {
        GameObject tracer = objectManager.CreateObj("shooter_A_Tracer", ObjectManager.PoolTypes.BulletPool);
        Bullet tracer_bullet = tracer.GetComponent<Bullet>();
        Rigidbody tracer_rigid = tracer.GetComponent<Rigidbody>();

        tracer_bullet.gameManager = gameManager;
        tracer_bullet.Init();


        //�̵�
        tracer.transform.position = creature.bulletStartPoint.position;
        //ȸ��
        tracer_rigid.velocity = tracer_bullet.bulletSpeed * transform.forward;
        //Ȱ��ȭ
        tracer_bullet.BulletOn(this);
    }
    #endregion
}
