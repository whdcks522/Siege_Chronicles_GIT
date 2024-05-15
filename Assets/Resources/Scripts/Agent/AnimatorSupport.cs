using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;

public class AnimatorSupport : MonoBehaviour
{
    Creature creature;

    private void Awake()
    {
        creature = transform.parent.GetComponent<Creature>();

        anim = GetComponent<Animator>();
    }

    //���� ��� �ʱ�ȭ
    public void AttackClear()
    {
        //creature.isAttack = false;
        creature.nav.isStopped = false;
    }

    //����ü ������ ��� ó��
    public void CompletelyDeadAnimation()
    {
        creature.CompletelyDead();
    }

    //������Ʈ�� �׼� 1(���� ������ �����ؼ� Ŀ����), 
    public void AgentAction_1() => creature.AgentAction_1();


    public GameManager gameManager;
    public void MovetoSelect()//SelectManager Ȱ��ȭ �ִϸ��̼� ��(StartGame�� ���� ȣ��)
    {
        transform.parent.gameObject.SetActive(false);

        //���� â ����
        gameManager.uiManager.selectManager.gameObject.SetActive(true);
    }
    public void MovetoBattle()//BattleManager Ȱ��ȭ �ִϸ��̼� ��
    {
        if (gameManager.uiManager.selectManager.index_Battle == 1)//���� ȭ������ ��ȯ
        {
            gameManager.uiManager.selectManager.StartActualGame();
        }
        else if (gameManager.uiManager.selectManager.index_Battle == 2) //���� �ʱ�ȭ
        {
            gameManager.ResetGame();
        }
    }

    Animator anim;
    public void FlashSupport()//���� ȭ�鿡�� �ؽ�Ʈ ��鸮�� ��
    {
        anim.SetBool("isFlash", false);
    }
}
