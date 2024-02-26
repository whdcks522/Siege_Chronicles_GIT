using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Policies;
using UnityEngine;
using static Creature;

public class ParentAgent : Agent
{
    [Header("���� ��")]
    public float rewardValue;

    [Header("������� ����ִ� ����")]
    public Transform enemyCreatureFolder;

    [Header("����ϴ� �Ѿ�")]
    public Transform useBullet;

    public BehaviorParameters behaviorParameters;
    public Creature creature;
    public Rigidbody rigid;//��� ����
    public Animator anim;

    public GameManager gameManager;
    public ObjectManager objectManager;
    
    public AudioManager audioManager;
    

    //������Ʈ���� ���� ����(���)
    virtual public void AgentAttack() 
    {

    }


    //��ǥ ���� ����
    Vector3 goalVec;
    //�̵��ϴ� ����
    Vector3 curVec;

    public void GetMatchingVelocityReward() 
    {
        //��ǥ ���� ����
        goalVec = (creature.enemyTower.transform.position - transform.position).normalized;
        //���簪 ���ִ� ����
        curVec = rigid.velocity.normalized;


        // �� ���� ������ ���� ��� (���� ����)
        float angle = Vector3.Angle(goalVec, curVec);
        // �ڻ��� ���絵 ��� (-1���� 1������ ��)
        float cosineSimilarity = Mathf.Cos(angle * Mathf.Deg2Rad);

        float reward = 0f;

        
        if (creature.curCreatureMoveEnum == CreatureMoveEnum.Idle)//���ִٸ� 0�� ��ȯ
            reward = 0f;
        else // 0���� 1������ ���� ��ȯ
            reward = (cosineSimilarity + 1f) / 2f;

        //Debug.Log(reward); //0f ~ 1f
        AddReward(reward / 2000f);
    }

    public void AgentOn() 
    {
        EndEpisode();
    }

    public override void OnEpisodeBegin()//EndEpisode�� ȣ����� �� ����(���� ȣ���� ���� ��°�� ����)
    {
        StateReturn();
    }

    public void StateReturn() 
    {
        creature.Revive();
        transform.position = creature.startPoint.position;
    }

    [Header("���� ������ �ִ� �Ÿ�")]
    public float maxRange;
    [Header("���� ������ �Ÿ�")]
    public float curRange;

    #region ������� �Ÿ� ���
    protected void RangeCalculate() 
    {
        curRange = (creature.enemyTower.position - transform.position).magnitude - 2;//Ÿ���� �β� ���

        for (int i = 0; i < enemyCreatureFolder.childCount; i++)
        {
            if (enemyCreatureFolder.GetChild(i).gameObject.activeSelf)//Ȱ��ȭ���ִٸ�
            {
                //������ �Ÿ�
                float tmpRange = (enemyCreatureFolder.GetChild(i).position - transform.position).magnitude;
                curRange = Mathf.Min(curRange, tmpRange);
            }
        }

    }
    #endregion 

    /*
    �̵��ϸ� ����

    ���� ���߸� ����    
    ���� �����߸� ����

    Ÿ�� ���߸� ����
    Ÿ�� �̱������ ����, �������� ����
    */
}
