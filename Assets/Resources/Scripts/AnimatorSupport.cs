using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AnimatorSupport : MonoBehaviour
{
    Creature creature;
    private void Awake()
    {
        creature = transform.parent.GetComponent<Creature>();
        anim = GetComponent<Animator>();
    }

    //공격 대기 초기화
    public void AttackClear()
    {
        creature.nav.isStopped = false;
    }

    //생명체 완전히 사망 처리
    public void CompletelyDeadAnimation()
    {
        creature.CompletelyDead();
    }

    //에이전트별 액션 1
    public void AgentAction_1() => creature.AgentAction_1();


    public GameManager gameManager;
    public void MovetoSelect()//SelectManager 활성화 애니메이션 용(StartGame에 의해 호출)
    {
        //시작 창 삭제
        Destroy(transform.parent.gameObject);

        //선택 창 열기
        gameManager.uiManager.selectManager.gameObject.SetActive(true);
    }
    public void MovetoBattle()//BattleManager 활성화 애니메이션 용
    {
        if (gameManager.uiManager.selectManager.index_Battle == 1)//전투 화면으로 전환
        {
            gameManager.uiManager.selectManager.StartActualGame();
        }
        else if (gameManager.uiManager.selectManager.index_Battle == 2) //게임 초기화
        {
            gameManager.ResetGame();
        }
    }

    Animator anim;
    public void FlashSupport()//전투 화면에서 텍스트 흔들리는 용
    {
        anim.SetBool("isFlash", false);
    }
}
