using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using static Creature;

public class Infantry_A_Agent : ParentAgent
{
    Rigidbody rigid;

    GameManager gameManager;
    ObjectManager objectManager;
    AudioManager audioManager;
    //AiManager aiManager;
    Animator anim;

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
        //AddReward(-0.0005f);

        int index = actions.DiscreteActions[0];

        if (!creature.isAttack) 
        {
            switch (index)
            {
                case 0://서있기
                    creature.curCreatureMoveEnum = CreatureMoveEnum.Idle;
                    break;
                case 1://달리기
                    creature.curCreatureMoveEnum = CreatureMoveEnum.Run;
                    break;
                case 2://왼쪽으로 회전
                    creature.curCreatureMoveEnum = CreatureMoveEnum.LeftSpin;
                    break;
                case 3://오른쪽으로 회전
                    creature.curCreatureMoveEnum = CreatureMoveEnum.RightSpin;
                    break;
                case 4://공격
                    if (gameObject.activeSelf)
                    {
                        if (!creature.isAttack && creature.curRange <= creature.maxRange)//쿨타임이 돌았으면서, 거리 이내여야 함
                        {
                            //애니메이션 관리
                            creature.curCreatureMoveEnum = CreatureMoveEnum.Idle;
                            creature.isAttack = true;//동시 입력 방지

                            int r = Random.Range(0, 2);
                            if (r == 0) anim.SetTrigger("isAttackLeft");
                            else if (r == 1) anim.SetTrigger("isAttackRight");
                        }
                        else AddReward(-0.5f);
                    }
                    break;
            }
        } 
    }



    #region 휴리스틱: 키보드를 통해 에이전트를 조정
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var disCreteActionOut = actionsOut.DiscreteActions;

        int spin = 0;

        if (Input.GetKey(KeyCode.LeftArrow))
            spin = 2;
        else if (Input.GetKey(KeyCode.UpArrow))
            spin = 1;
        if (Input.GetKey(KeyCode.RightArrow))
            x = 3;
        else if (Input.GetKey(KeyCode.Z))
            x = 4;

        disCreteActionOut[0] = x;
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

            sensor.AddObservation(StepCount / (float)MaxStep);//진행한 스텝 비율    //state size = 1
        }
    }
    #endregion

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.CompareTag("Outline")) //맵 밖으로 나가지면 사망
        {
            //AddReward(-1f);
            //EndEpisode();
        }
    }

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
