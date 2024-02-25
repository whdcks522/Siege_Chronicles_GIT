using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.VisualScripting;
using UnityEngine;
using static Creature;

public class Infantry_A_Agent : ParentAgent
{


    //공격 맞으면 점수 증가, 안맞으면 감소
    //방향 맞으면 점수 증가

    /*
     스크립트에서의 간단한 코드정리

- Initialize : 환경이 실행될 떄 호출되는 초기화 함수
- CollectObservations : Agent에게 Vector Observation 정보를 전달해 주는 함수 
- OnActionReceived : Agent가 결정한 행동을 전달, 보상 업데이트, 에피소드 종료 
- OnEpisodeBegin : 각 에피소드가 시작될 떄 호출되는 함수
- Heuristic : 개발자가 직접 명령을 내리는 휴리스틱 모드에서 사용 
     */

    //  mlagents-learn --force
    //mlagents-learn "D:\gitHubDeskTop\ML_EX_GIT\config\ppo\Enemy_Orc.yaml" --run-id=Enemy_Orc_K --resum(2시간즈음부터 성능 향상 시작됨)
    //mlagents-learn "D:\Unities\Github_DeskTop\ML_EX_GIT\config\ppo\Custom_shooter_A.yaml" --run-id=ustom_shooter_C --resum

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        creature = GetComponent<Creature>();
        anim = GetComponentInChildren<Animator>();

        gameManager = creature.gameManager;
        objectManager = gameManager.objectManager;

        if(creature.curTeamEnum == TeamEnum.Blue)
            enemyCreatureFolder = objectManager.redCreatureFolder;
        else if (creature.curTeamEnum == TeamEnum.Red)
            enemyCreatureFolder = objectManager.blueCreatureFolder;
    }

    
    public override void OnActionReceived(ActionBuffers actions)//액션 기입(가능한 동작), 매 번 호출 
    {
        if (!creature.isAttack && gameObject.layer == LayerMask.NameToLayer("Creature")) 
        {
            rewardValue =  GetCumulativeReward();


            //방향따라 점수 증가
            GetMatchingVelocityReward();
            //적과의 거리 계산
            RangeCalculate();

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
                        if (curRange <= maxRange)//쿨타임이 돌았으면서, 거리 이내여야 함
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

        if (behaviorParameters.BehaviorType == Unity.MLAgents.Policies.BehaviorType.HeuristicOnly)
        {
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
        else 
        {
            disCreteActionOut[0] = 1;
            disCreteActionOut[1] = 0;
        }
            
    }
    #endregion

    #region 관찰할 정보, 5번 당 한번 호출
    public override void CollectObservations(VectorSensor sensor)
    {
        //1. 수치형, 받아오는 데이터가 적을 수록 좋음
        if (gameObject.layer == LayerMask.NameToLayer("Creature") && gameObject.activeSelf) //죽으면 필요 없자너
        {
            //현재 자신의 위치
            sensor.AddObservation(transform.position.x);//state size = 1     x,y,z를 모두 받아오면 size가 3이 됨
            sensor.AddObservation(transform.position.z);
            //현재 자신의 가속
            sensor.AddObservation(rigid.velocity.x);
            sensor.AddObservation(rigid.velocity.z);

            //자기 타워의 정보
            sensor.AddObservation(creature.enemyTower.position.x);
            sensor.AddObservation(creature.enemyTower.position.z);
            //상대 타워의 정보
            sensor.AddObservation(creature.enemyTower.position.x);
            sensor.AddObservation(creature.enemyTower.position.z);

            //각각의 거리
            sensor.AddObservation(curRange / maxRange);

            for (int i = 0; i < enemyCreatureFolder.childCount; i++)
            {
                if (enemyCreatureFolder.GetChild(i).gameObject.activeSelf)//활성화돼있다면
                {
                    sensor.AddObservation(enemyCreatureFolder.GetChild(i).position.x);
                    sensor.AddObservation(enemyCreatureFolder.GetChild(i).position.z);
                }
            }
        }
    }
    #endregion


    #region 주황색 참격 생성
    override public void AgentAttack()
    {
        GameObject slash = objectManager.CreateObj("Infantry_A_Slash", ObjectManager.PoolTypes.BulletPool);
        Bullet slash_bullet = slash.GetComponent<Bullet>();
        
        //이동
        slash.transform.position = transform.position + transform.forward + Vector3.up * 3;

        //회전
        slash.transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x + 90,
            transform.rotation.eulerAngles.y - 180, transform.rotation.eulerAngles.z - 90);
        //활성화
        slash_bullet.BulletOn(this);
    }
    #endregion
}
