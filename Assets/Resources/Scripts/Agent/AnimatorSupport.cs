using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;

public class AnimatorSupport : MonoBehaviour
{
    Creature creature;
    SuperAgent superAgent;

    private void Awake()
    {
        creature = transform.parent.GetComponent<Creature>();
        superAgent = transform.parent.GetComponent<SuperAgent>();
    }

    //���� ��� �ʱ�ȭ
    public void AttackClear() => creature.isAttack = false;

    //����ü ������ ��� ó��
    public void CompletelyDeadAnimation()
    {
        creature.CompletelyDead();
    }

    //������Ʈ�� �׼� 1(���� ������ �����ؼ� Ŀ����), 
    public void AgentAction_1() => superAgent.AgentAction_1();


    public GameManager gameManager;
    public void MovetoSelect()//SelectManager Ȱ��ȭ �ִϸ��̼� ��(StartGame�� ���� ȣ��)
    {
        transform.parent.gameObject.SetActive(false);

        //���� â ����
        gameManager.uiManager.selectManager.gameObject.SetActive(true);
    }
    public void MovetoBattle()//BattleManager Ȱ��ȭ �ִϸ��̼� ��
    {
        Debug.Log(gameManager.uiManager.selectManager.index_Battle);

        if (gameManager.uiManager.selectManager.index_Battle == 1)//���� ȭ������ ��ȯ
        {
            gameManager.uiManager.selectManager.StartActualGame();
        }
        else if (gameManager.uiManager.selectManager.index_Battle == 2) //���� �ʱ�ȭ
        {
            gameManager.ResetGame();
        }
    }


}
