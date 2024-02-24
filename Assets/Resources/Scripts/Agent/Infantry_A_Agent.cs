using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using static Creature;

public class Infantry_A_Agent : ParentAgent
{
    Rigidbody rigid;

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
        //AddReward(-0.0005f);

        int index = actions.DiscreteActions[0];

        if (!creature.isAttack) 
        {
            switch (index)
            {
                case 0://���ֱ�
                    creature.curCreatureMoveEnum = CreatureMoveEnum.Idle;
                    break;
                case 1://�޸���
                    creature.curCreatureMoveEnum = CreatureMoveEnum.Run;
                    break;
                case 2://�������� ȸ��
                    creature.curCreatureMoveEnum = CreatureMoveEnum.LeftSpin;
                    break;
                case 3://���������� ȸ��
                    creature.curCreatureMoveEnum = CreatureMoveEnum.RightSpin;
                    break;
                case 4://����
                    if (gameObject.activeSelf)
                    {
                        if (!creature.isAttack && creature.curRange <= creature.maxRange)//��Ÿ���� �������鼭, �Ÿ� �̳����� ��
                        {
                            //�ִϸ��̼� ����
                            creature.curCreatureMoveEnum = CreatureMoveEnum.Idle;
                            creature.isAttack = true;//���� �Է� ����

                            int r = Random.Range(0, 2);
                            if (r == 0) anim.SetTrigger("isAttackLeft");
                            else if (r == 1) anim.SetTrigger("isAttackRight");
                        }
                        else AddReward(-0.5f);
                    }
                    break;
            }
        } 
    }



    #region �޸���ƽ: Ű���带 ���� ������Ʈ�� ����
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var disCreteActionOut = actionsOut.DiscreteActions;

        int spin = 0;

        if (Input.GetKey(KeyCode.LeftArrow))
            spin = 2;
        else if (Input.GetKey(KeyCode.UpArrow))
            spin = 1;
        if (Input.GetKey(KeyCode.RightArrow))
            x = 3;
        else if (Input.GetKey(KeyCode.Z))
            x = 4;

        disCreteActionOut[0] = x;
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

            sensor.AddObservation(StepCount / (float)MaxStep);//������ ���� ����    //state size = 1
        }
    }
    #endregion

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.CompareTag("Outline")) //�� ������ �������� ���
        {
            //AddReward(-1f);
            //EndEpisode();
        }
    }

    [Header("�������")]
    public Transform point;

    public override void OnEpisodeBegin()//EndEpisode�� ȣ����� �� ����(���� ȣ���� ���� ��°�� ����)
    {
        creature.Revive();
        transform.position = point.position;
    }

    #region ��Ȳ�� ���� ����
    override public void AgentAttack()
    {
        GameObject slash = objectManager.CreateObj("Infantry_A_Slash", ObjectManager.PoolTypes.BulletPool);
        Bullet slash_bullet = slash.GetComponent<Bullet>();
        slash_bullet.BulletOn(this);
        //�̵�
        slash.transform.position = transform.position + transform.forward + Vector3.up * 3;

        //ȸ��
        slash.transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x + 90,
            transform.rotation.eulerAngles.y - 180, transform.rotation.eulerAngles.z - 90);
    }
    #endregion
}
