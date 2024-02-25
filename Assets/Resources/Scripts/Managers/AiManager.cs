using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.VisualScripting;
using UnityEngine;

public class AiManager : MonoBehaviour
{
    [Header("머신러닝중인지 여부 확인")]
    public bool isML;

    [Header("머신 러닝의 최대 시간")]
    public float maxTime;
    [Header("머신 러닝의 현재 시간")]
    public float curTime = 0f;

    public GameManager gameManager;
    ObjectManager objectManager;

    //그룹 번호 설정 필요(그냥 하면 unityagentsexception: onepisodebegin called recursively. this might happen if you call environmentstep() or endepisode() from custom code such as collectobservations() or onactionreceived().)
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
        if (curTime >= maxTime)//타임 오버
        {

            AiClear(0);
        }
    }

    #region 시간이 다되거나, 성이 파괴되면 초기화
    public void AiClear(int warIndex)
    {
        if (warIndex == 0)
        {
            blueAgentGroup.GroupEpisodeInterrupted();
            redAgentGroup.GroupEpisodeInterrupted();
            
        }
        else if (warIndex != 0) 
        {
            if (warIndex == 1) //파랑 승
            {
                blueAgentGroup.AddGroupReward(10f);
                redAgentGroup.AddGroupReward(-5f);
            }
            else if (warIndex == -1) //빨강 승
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
        // 1이면 파란 팀 승리
        // 0이면 무승부
        // -1이면 빨간 팀 승리

        yield return null;

        //시간 초기화
        curTime = 0f;

        //건물 체력 초기화
        gameManager.blueTower.GetComponent<TowerManager>().TowerOn();
        gameManager.redTower.GetComponent<TowerManager>().TowerOn();

        //크리쳐 초기화
        int creatureSize = objectManager.blueCreatureFolder.childCount;//파란 팀
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

        

        creatureSize = objectManager.redCreatureFolder.childCount;//빨간 팀
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
