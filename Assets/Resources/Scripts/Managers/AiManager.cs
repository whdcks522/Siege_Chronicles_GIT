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
    private SimpleMultiAgentGroup blueAgentGroup;
    private SimpleMultiAgentGroup redAgentGroup;

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


    private void Update()
    {
        curTime += Time.deltaTime;
        if (curTime >= maxTime)//Ÿ�� ����
        {

            AiClear(0);
        }
    }

    #region �ð��� �ٵǰų�, ���� �ı��Ǹ� �ʱ�ȭ
    public void AiClear(int warIndex)
    {
        if (warIndex == 0)
        {
            blueAgentGroup.GroupEpisodeInterrupted();
            redAgentGroup.GroupEpisodeInterrupted();
            
        }
        else if (warIndex != 0) 
        {
            if (warIndex == 1) //�Ķ� ��
            {
                blueAgentGroup.AddGroupReward(10f);
                redAgentGroup.AddGroupReward(-5f);
            }
            else if (warIndex == -1) //���� ��
            {
                blueAgentGroup.AddGroupReward(-5f);
                redAgentGroup.AddGroupReward(10f);
            }

            blueAgentGroup.EndGroupEpisode();
            redAgentGroup.EndGroupEpisode();
        }
        
        StartCoroutine(AiClearCor(warIndex));
    }

    WaitForSeconds wait01 = new WaitForSeconds(0.5f);
    
    IEnumerator AiClearCor(int warIndex) 
    {
        //warIndex
        // 1�̸� �Ķ� �� �¸�
        // 0�̸� ���º�
        // -1�̸� ���� �� �¸�

        yield return null;

        //�ð� �ʱ�ȭ
        curTime = 0f;

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

                //if (warIndex == 1)
                //    agent.AddReward(10f);
                //else if (warIndex == -1)
                //    agent.AddReward(-5f);

                agent.StateReturn();

                yield return wait01;
            }
        }

        

        creatureSize = objectManager.redCreatureFolder.childCount;//���� ��
        if (creatureSize > 0)
        {
            for (int i = 0; i < creatureSize; i++)
            {
                ParentAgent agent = objectManager.redCreatureFolder.GetChild(i).GetComponent<ParentAgent>();

                //if (warIndex == -1)
                 //   agent.AddReward(10f);
                //else if (warIndex == 1)
                 //   agent.AddReward(-5f);

                agent.StateReturn();
                yield return wait01;
            }
        }
    }
    #endregion
}
