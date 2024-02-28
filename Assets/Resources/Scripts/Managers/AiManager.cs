using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.VisualScripting;
using UnityEngine;

public class AiManager : MonoBehaviour
{
    [Header("�ӽŷ��������� ���� Ȯ��")]
    public bool isML;

    [Header("�ӽ� ������ �ִ� �ð�")]
    public float maxTime;
    [Header("�ӽ� ������ ���� �ð�")]
    public float curTime = 0f;

    public GameManager gameManager;
    ObjectManager objectManager;

    //�׷� ��ȣ ���� �ʿ�(�׳� �ϸ� unityagentsexception: onepisodebegin called recursively. this might happen if you call environmentstep() or endepisode() from custom code such as collectobservations() or onactionreceived().)
    public SimpleMultiAgentGroup blueAgentGroup;
    public SimpleMultiAgentGroup redAgentGroup;

    private void Awake()
    {
        objectManager = gameManager.objectManager;
    }

    private void Start()
    {
        blueAgentGroup = new SimpleMultiAgentGroup();
        redAgentGroup = new SimpleMultiAgentGroup();

        for (int i = 0; i < objectManager.blueCreatureFolder.childCount; i++)
        {
            ParentAgent agent = objectManager.blueCreatureFolder.GetChild(i).GetComponent<ParentAgent>();
            blueAgentGroup.RegisterAgent(agent);
        }
        for (int i = 0; i < objectManager.redCreatureFolder.childCount; i++)
        {
            ParentAgent agent = objectManager.redCreatureFolder.GetChild(i).GetComponent<ParentAgent>();
            redAgentGroup.RegisterAgent(agent);
        }
    }

    private void FixedUpdate()
    {
        curTime += 1;

        blueAgentGroup.AddGroupReward(0.001f);
        redAgentGroup.AddGroupReward(0.001f);

        if (curTime >= maxTime && maxTime > 0)//Ÿ�� ����
        {

            AiEnd(0);
        }
    }


    #region �ð��� �ٵǰų�, ���� �ı��Ǹ� �ʱ�ȭ
    public void AiEnd(int warIndex)
    {
        //warIndex
        // 1�̸� �Ķ� �� �¸�
        // 0�̸� ���º�
        // -1�̸� ���� �� �¸�

        if (warIndex == 0)
        {
            blueAgentGroup.GroupEpisodeInterrupted();
            redAgentGroup.GroupEpisodeInterrupted(); 
        }
        else if (warIndex != 0) 
        {
            if (warIndex == 1) //�Ķ� ��
            {
                blueAgentGroup.AddGroupReward(20f);
                redAgentGroup.AddGroupReward(-10f);
            }
            else if (warIndex == -1) //���� ��
            {
                blueAgentGroup.AddGroupReward(-10f);
                redAgentGroup.AddGroupReward(20f);
            }

            blueAgentGroup.EndGroupEpisode();
            redAgentGroup.EndGroupEpisode();
        }

        AiClear();
    }

    
    void AiClear() 
    {
        //�ð� �ʱ�ȭ
        curTime = 0f;

        //�Ѿ� ��Ȱ��ȭ
        for (int i = 0; i < objectManager.bulletFolder.childCount; i++)
        {
            objectManager.bulletFolder.GetChild(i).gameObject.SetActive(false);
        }

        //�ǹ� ü�� �ʱ�ȭ
        gameManager.blueTower.GetComponent<TowerManager>().TowerOn();
        gameManager.redTower.GetComponent<TowerManager>().TowerOn();

        //ũ���� �ʱ�ȭ
        int creatureSize = objectManager.blueCreatureFolder.childCount;//�Ķ� ��
        if (creatureSize > 0)
        {
            for (int i = 0; i < creatureSize; i++)
            {
                ParentAgent agent = objectManager.blueCreatureFolder.GetChild(i).GetComponent<ParentAgent>();
                //agent.EndEpisode();
                agent.StateReturn();
            }
        }

        creatureSize = objectManager.redCreatureFolder.childCount;//���� ��
        if (creatureSize > 0)
        {
            for (int i = 0; i < creatureSize; i++)
            {
                ParentAgent agent = objectManager.redCreatureFolder.GetChild(i).GetComponent<ParentAgent>();
                //agent.EndEpisode();
                agent.StateReturn();
            }
        }
    }
    #endregion
}
