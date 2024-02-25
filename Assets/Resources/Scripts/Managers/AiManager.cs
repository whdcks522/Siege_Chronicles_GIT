using System.Collections;
using System.Collections.Generic;
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

    #region �ð��� �ٵǰų�, ���� �ı��Ǹ� �ʱ�ȭ
    public void AiClear(int warIndex)
    {
        //warIndex
        // 1�̸� �Ķ� �� �¸�
        // 0�̸� ���º�
        // -1�̸� ���� �� �¸�

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

                if (warIndex == 1)
                    agent.AddReward(20f);
                else if (warIndex == -1)
                    agent.AddReward(-5f);

                agent.EndEpisode();
            }
        }

        creatureSize = objectManager.redCreatureFolder.childCount;//���� ��
        if (creatureSize > 0)
        {
            for (int i = 0; i < creatureSize; i++)
            {
                ParentAgent agent = objectManager.redCreatureFolder.GetChild(i).GetComponent<ParentAgent>();

                if (warIndex == -1)
                    agent.AddReward(20f);
                else if (warIndex == 1)
                    agent.AddReward(-5f);

                agent.EndEpisode();
            }
        }

    }
    #endregion

}
