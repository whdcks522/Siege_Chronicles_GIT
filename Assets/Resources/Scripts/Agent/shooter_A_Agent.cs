using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using static Creature;

public class shooter_A_Agent : Agent
{
    public override void OnActionReceived(ActionBuffers actions)//액션 기입(가능한 동작), 매 번 호출 
    {
        if (!creature.isAttack && gameObject.layer == LayerMask.NameToLayer("Creature"))
        {
            //creature.rewardValue = GetCumulativeReward();
            //creature.curReward.text = creature.rewardValue.ToString("F1");

            //방향따라 점수 증가
            creature.GetMatchingVelocityReward();

            //적과의 거리 계산
            creature.RangeCalculate();



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
                        //대상을 보도록
                        transform.LookAt(creature.curTarget);

                        //애니메이션 관리
                        creature.curCreatureSpinEnum = CreatureSpinEnum.None;
                        creature.curCreatureMoveEnum = CreatureMoveEnum.Idle;
                        creature.isAttack = true;//동시 입력 방지

                        int r = UnityEngine.Random.Range(0, 2);
                        if (r == 0) creature.anim.SetTrigger("isAttackLeft");
                        else if (r == 1) creature.anim.SetTrigger("isAttackRight");
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
            //현재 자신의 회전
            sensor.AddObservation(transform.rotation.y);
            //현재 자신의 가속
            //sensor.AddObservation(rigid.velocity.x);
            //sensor.AddObservation(rigid.velocity.z);

            //자기 타워의 정보
            //sensor.AddObservation(creature.ourTower.position.x);
            //sensor.AddObservation(creature.ourTower.position.z);
            sensor.AddObservation(creature.ourTowerManager.curHealth / creature.ourTowerManager.maxHealth);

            //상대 타워의 정보
            //sensor.AddObservation(creature.enemyTower.position.x);
            //sensor.AddObservation(creature.enemyTower.position.z);
            sensor.AddObservation(creature.enemyTowerManager.curHealth / creature.enemyTowerManager.maxHealth);

            //적까지의 거리
            sensor.AddObservation(creature.curRange / creature.maxRange);

            //가까운 적의 위치
            sensor.AddObservation(creature.curTarget.position.x);
            sensor.AddObservation(creature.curTarget.position.z);
            sensor.AddObservation(creature.curHealth / creature.maxHealth);

            //우리 타워에서 가장 가까운 적의 위치
            sensor.AddObservation(creature.ourTowerManager.curTarget.position.x);
            sensor.AddObservation(creature.ourTowerManager.curTarget.position.z);
        }
    }
    #endregion
    [Header("크리쳐")]
    public Creature creature;

    [Header("사용하는 총알")]
    public Transform useBullet;


    #region 초록 투사체 생성
    public void AgentAttack()
    {
        string bulletName = useBullet.name;

        GameObject tracer = creature.objectManager.CreateObj(bulletName, ObjectManager.PoolTypes.BulletPool);
        Bullet tracer_bullet = tracer.GetComponent<Bullet>();
        Rigidbody tracer_rigid = tracer.GetComponent<Rigidbody>();

        tracer_bullet.gameManager = creature.gameManager;
        tracer_bullet.Init();


        //이동
        tracer.transform.position = creature.bulletStartPoint.position;
        //회전
        tracer_rigid.velocity = tracer_bullet.bulletSpeed * transform.forward;
        //활성화
        tracer_bullet.BulletOn(creature);
    }
    #endregion
}
