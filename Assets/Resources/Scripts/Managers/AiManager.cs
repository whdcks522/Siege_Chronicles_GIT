using System.Collections;
using System.Collections.Generic;
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

    private void Awake()
    {
        objectManager = gameManager.objectManager;
    }


    private void Update()
    {
        curTime += Time.deltaTime;
        if (curTime >= maxTime) 
        {
            AiClear(0);
        }
    }

    #region 시간이 다되거나, 성이 파괴되면 초기화
    public void AiClear(int warIndex)
    {
        StartCoroutine(AiClearCor(warIndex));

    }

    new WaitForSeconds wait01 = new WaitForSeconds(0.5f);
    
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

                if (warIndex == 1)
                    agent.AddReward(10f);
                else if (warIndex == -1)
                    agent.AddReward(-5f);

                agent.AgentOn();

                yield return wait01;
            }
        }

        

        creatureSize = objectManager.redCreatureFolder.childCount;//빨간 팀
        if (creatureSize > 0)
        {
            for (int i = 0; i < creatureSize; i++)
            {
                ParentAgent agent = objectManager.redCreatureFolder.GetChild(i).GetComponent<ParentAgent>();

                if (warIndex == -1)
                    agent.AddReward(10f);
                else if (warIndex == 1)
                    agent.AddReward(-5f);

                agent.AgentOn();
                yield return wait01;
            }
        }
    }
    #endregion
}
