using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using static Creature;

public class Shooter_A_Agent : Agent
{
    public override void OnActionReceived(ActionBuffers actions)//액션 기입(가능한 동작), 매 번 호출 
    {
        if (!creature.isAttack && gameObject.layer == LayerMask.NameToLayer("Creature"))
        {
            //적과의 거리 계산
            creature.EnemyFirstRangeCalc();

            //방향따라 점수 증가
            creature.GetMatchingVelocityReward();

            //자동 실점
            AddReward(-0.001f);

            if (actions.DiscreteActions[0] == 1 && actions.DiscreteActions[1] == 0)
                AddReward(-0.0002f);

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
                    if (creature.curRange <= creature.maxRange && gameObject.layer == LayerMask.NameToLayer("Creature"))
                    {
                        //애니메이션 관리
                        creature.curCreatureSpinEnum = CreatureSpinEnum.None;
                        creature.curCreatureMoveEnum = CreatureMoveEnum.Idle;
                        creature.isAttack = true;//동시 입력 방지

                        transform.LookAt(creature.curTarget);
                        creature.anim.SetTrigger("isGun");
                    }
                    break;
            }
        }

        //Debug.Log("spin: " + actions.DiscreteActions[0] + "action: " + actions.DiscreteActions[1]);
    }

    //mlagents-learn "D:\Unities\Github_DeskTop\ML_EX_GIT\config\ppo\Infantry_A.yaml" --run-id=Custom_Shoter_1 --resum

    #region 휴리스틱: 키보드를 통해 에이전트를 조정
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var disCreteActionOut = actionsOut.DiscreteActions;

        if (creature.behaviorParameters.BehaviorType == Unity.MLAgents.Policies.BehaviorType.HeuristicOnly)
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
        if (gameObject.layer == LayerMask.NameToLayer("Creature")) //죽으면 필요 없자너
        {
            //현재 자신의 위치
            sensor.AddObservation(transform.position.x);//state size = 1     x,y,z를 모두 받아오면 size가 3이 됨
            sensor.AddObservation(transform.position.z);
            //현재 자신의 가속
            sensor.AddObservation(creature.rigid.velocity.x);
            sensor.AddObservation(creature.rigid.velocity.z);
            //현재 자신의 회전
            sensor.AddObservation(transform.rotation.y);

            //적까지의 거리
            if (creature.curRange != 0)
                sensor.AddObservation(creature.maxRange / creature.curRange);

            //가까운 적의 위치
            if (creature.curTarget != null)
            {
                sensor.AddObservation(creature.curTarget.position.x);
                sensor.AddObservation(creature.curTarget.position.z);
            }
        }
    }
    #endregion
    [Header("크리쳐")]
    public Creature creature;

    [Header("사용하는 총알")]
    public Transform useBullet;

    public override void OnEpisodeBegin()
    {
        creature.resetEnv();
    }
}
