using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using static Creature;

public class Infantry_A_Agent : ParentAgent
{
    GameManager gameManager;
    ObjectManager objectManager;
    AudioManager audioManager;
    Animator anim;

    //공격 맞으면 점수 증가, 안맞으면 감소
    //방향 맞으면 점수 증가

    //  mlagents-learn --force
    //mlagents-learn "D:\gitHubDeskTop\ML_EX_GIT\config\ppo\Enemy_Orc.yaml" --run-id=Enemy_Orc_K --resum(2시간즈음부터 성능 향상 시작됨)

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        creature = GetComponent<Creature>();
        anim = GetComponentInChildren<Animator>();

        gameManager = creature.gameManager;
        objectManager = gameManager.objectManager;
    }

    
    public override void OnActionReceived(ActionBuffers actions)//액션 기입(가능한 동작), 매 번 호출 
    {
        //방향따라 점수 증가
        GetMatchingVelocityReward();

        if (!creature.isAttack && gameObject.layer == LayerMask.NameToLayer("Creature")) 
        {
            switch (actions.DiscreteActions[0])
            {
                case 0://왼쪽으로 회전
                    creature.curCreatureSpinEnum = CreatureSpinEnum.LeftSpin;
                    break;
                case 1://서있기
                    creature.curCreatureSpinEnum = CreatureSpinEnum.None;
                    break;
                case 2://오른쪽으로 회전
                    creature.curCreatureSpinEnum = CreatureSpinEnum.RightSpin;
                    break;
            }

            switch (actions.DiscreteActions[1]) 
            {
                case 0://서있기
                    creature.curCreatureMoveEnum = CreatureMoveEnum.Idle;
                    break;
                case 1://달리기
                    creature.curCreatureMoveEnum = CreatureMoveEnum.Run;
                    break;
                case 2://공격
                    if (gameObject.activeSelf)
                    {
                        if (creature.curRange <= creature.maxRange)//쿨타임이 돌았으면서, 거리 이내여야 함
                        {
                            //애니메이션 관리
                            creature.curCreatureSpinEnum = CreatureSpinEnum.None;
                            creature.curCreatureMoveEnum = CreatureMoveEnum.Idle;
                            creature.isAttack = true;//동시 입력 방지

                            int r = UnityEngine.Random.Range(0, 2);
                            if (r == 0) anim.SetTrigger("isAttackLeft");
                            else if (r == 1) anim.SetTrigger("isAttackRight");
                        }
                        else AddReward(-0.5f);
                    }
                    break;
            }
        }

        //Debug.Log("spin: " + actions.DiscreteActions[0] + "action: " + actions.DiscreteActions[1]);
    }

    #region 휴리스틱: 키보드를 통해 에이전트를 조정
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var disCreteActionOut = actionsOut.DiscreteActions;

        int spin = 1;//회전 안함
        if (Input.GetKey(KeyCode.LeftArrow))//좌회전
            spin = 0;
        else if (Input.GetKey(KeyCode.RightArrow))//우회전
            spin = 2;

        int action = 0;//액션 안함
        if (Input.GetKey(KeyCode.UpArrow))//걷기
            action = 1;
        else if (Input.GetKey(KeyCode.Z))//공격
            action = 2;

        disCreteActionOut[0] = spin;
        disCreteActionOut[1] = action;
    }
    #endregion

    #region 관찰할 정보, 5번 당 한번 호출
    public override void CollectObservations(VectorSensor sensor)
    {
        //1. 수치형, 받아오는 데이터가 적을 수록 좋음
        //자신의 정보
        if (gameObject.layer == LayerMask.NameToLayer("Creature")) //죽으면 필요 없자너
        {
            sensor.AddObservation(transform.position.x);//state size = 1     x,y,z를 모두 받아오면 size가 3이 됨
            sensor.AddObservation(transform.position.z);

            //가속을 더하기도 함
            sensor.AddObservation(rigid.velocity.x);
            sensor.AddObservation(rigid.velocity.z);

            if (creature.target != null) //시작 한 순간, 빈 취급됨
            {
                //플레이어의 정보
                sensor.AddObservation(creature.target.transform.position.x);
                sensor.AddObservation(creature.target.transform.position.z);
                //각각의 거리
                sensor.AddObservation(creature.curRange);
            }
        }
    }
    #endregion

    [Header("재시작점")]
    public Transform point;

    public override void OnEpisodeBegin()//EndEpisode가 호출됐을 때 사용됨(씬을 호출할 때는 통째로 삭제)
    {
        creature.Revive();
        transform.position = point.position;
    }

    #region 주황색 참격 생성
    override public void AgentAttack()
    {
        GameObject slash = objectManager.CreateObj("Infantry_A_Slash", ObjectManager.PoolTypes.BulletPool);
        Bullet slash_bullet = slash.GetComponent<Bullet>();
        slash_bullet.BulletOn(this);
        //이동
        slash.transform.position = transform.position + transform.forward + Vector3.up * 3;

        //회전
        slash.transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x + 90,
            transform.rotation.eulerAngles.y - 180, transform.rotation.eulerAngles.z - 90);
    }
    #endregion
}
