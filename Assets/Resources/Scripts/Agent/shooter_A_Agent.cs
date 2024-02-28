using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using static Creature;

public class shooter_A_Agent : ParentAgent
{
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

        if (!creature.isAttack && gameObject.layer == LayerMask.NameToLayer("Creature"))
        {
            rewardValue = GetCumulativeReward();
            creature.curReward.text = rewardValue.ToString("F2");

            //곧바로 이동하면 점수
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
                    if (curRange <= maxRange && gameObject.layer == LayerMask.NameToLayer("Creature"))//살아있으면서, 거리 이내여야 함
                    {
                        //애니메이션 관리
                        creature.curCreatureSpinEnum = CreatureSpinEnum.None;
                        creature.curCreatureMoveEnum = CreatureMoveEnum.Idle;
                        creature.isAttack = true;//동시 입력 방지

                        anim.SetTrigger("isGun");
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
            else if (Input.GetKey(KeyCode.Z))//투사체 공격
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
        //자신의 정보
        if (gameObject.layer == LayerMask.NameToLayer("Creature") && gameObject.activeSelf) //죽으면 필요 없자너
        {
            //현재 자신의 위치
            sensor.AddObservation(transform.position.x);//state size = 1     x,y,z를 모두 받아오면 size가 3이 됨
            sensor.AddObservation(transform.position.z);
            //현재 자신의 회전
            sensor.AddObservation(transform.rotation.y);
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


    #region 투사체 생성
    override public void AgentAttack()
    {
        string bulletName = useBullet.name;

        GameObject tracer = objectManager.CreateObj(bulletName, ObjectManager.PoolTypes.BulletPool);
        Bullet tracer_bullet = tracer.GetComponent<Bullet>();
        Rigidbody tracer_rigid = tracer.GetComponent<Rigidbody>();

        tracer_bullet.gameManager = gameManager;
        tracer_bullet.Init();


        //이동
        tracer.transform.position = creature.bulletStartPoint.position;
        //회전
        tracer_rigid.velocity = tracer_bullet.bulletSpeed * transform.forward;
        //활성화
        tracer_bullet.BulletOn(this);
    }
    #endregion
}
