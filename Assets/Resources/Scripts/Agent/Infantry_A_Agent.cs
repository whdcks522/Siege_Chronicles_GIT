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
    public Creature creature;
    

    GameManager gameManager;
    ObjectManager objectManager;
    AudioManager audioManager;
    AiManager aiManager;
    Animator anim;

    public float maxRange;

    //�ƽ� ������ �ø�ä�� ���� �̻� ����
    //��鿡�� �н��ϴٰ� ȯ�� 2�� �ű�
    //<Enemy_Orc>
    //mlagents-learn "D:\gitHubDeskTop\ML_EX_GIT\config\ppo\Enemy_Orc.yaml" --run-id=Enemy_Orc_K --resum(2�ð��������� ���� ��� ���۵�)

    Coroutine bigCor;
    WaitForSeconds wait = new WaitForSeconds(0.12f);

    //������ �Ÿ�
    float curRange;

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
        if(creature.target != null)
        curRange = (creature.target.transform.position - transform.position).magnitude;
        //AddReward(-0.0005f);

        //  Discrete Action(������ ��ȯ��, Ư�� �ൿ�� ����ϱ� ����(AllBuffered�� ���� ����?))
        //  mlagents-learn --force

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
                        if (!creature.isAttack && curRange <= maxRange)//��Ÿ���� �������鼭, �Ÿ� �̳����� ��
                        {
                            //�ִϸ��̼� ����
                            creature.curCreatureMoveEnum = CreatureMoveEnum.Idle;
                            creature.isAttack = true;//���� �Է� ����

                            int r = Random.Range(0, 2);
                            if (r == 0) anim.SetTrigger("isAttackLeft");
                            else if (r == 1) anim.SetTrigger("isAttackRight");

                            //���� ����
                            GameObject slash = objectManager.CreateObj("Infantry_A_Slash", ObjectManager.PoolTypes.BulletPool);
                            slash.transform.forward = transform.forward;
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

        int x = 0;

        if (Input.GetKey(KeyCode.LeftArrow))
            x = 2;
        else if (Input.GetKey(KeyCode.UpArrow))
            x = 1;
        else if (Input.GetKey(KeyCode.RightArrow))
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
        if (gameObject.activeSelf) //������ �ʿ� ���ڳ�
        {
            sensor.AddObservation(transform.position.x);//state size = 1     x,y,z�� ��� �޾ƿ��� size�� 3�� ��
            sensor.AddObservation(transform.position.y);

            //������ ���ϱ⵵ ��
            sensor.AddObservation(rigid.velocity.x);//state size = 1
            sensor.AddObservation(rigid.velocity.y);

            if (creature.target != null) //���� �� ����, �� ��޵�
            {
                //�÷��̾��� ����
                sensor.AddObservation(creature.target.transform.position.x);
                sensor.AddObservation(creature.target.transform.position.y);
                //������ �Ÿ�
                sensor.AddObservation(curRange);
            }

            sensor.AddObservation(StepCount / (float)MaxStep);//������ ���� ����    //state size = 1
        }
    }
    #endregion

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.CompareTag("Outline")) //�� ������ �������� ���
        {
            AddReward(-1f);

        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //if (collision.gameObject == player)
        {
            //AddReward(10f);//�������� �ű�
            //EndEpisode();//�̰͸����� �ʱ�ȭ�� ���� ����
        }
    }

    [Header("�������")]
    public Transform point;

    public override void OnEpisodeBegin()//EndEpisode�� ȣ����� �� ����(���� ȣ���� ���� ��°�� ����)
    {
        transform.position = point.position;
        creature.Revive();
    }
}
