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
    public Creature creature;
    

    GameManager gameManager;
    ObjectManager objectManager;
    AudioManager audioManager;
    AiManager aiManager;
    Animator anim;

    public float maxRange;

    //맥스 스텝은 늘린채로 놔도 이상 없음
    //평면에서 학습하다가 환경 2로 옮김
    //<Enemy_Orc>
    //mlagents-learn "D:\gitHubDeskTop\ML_EX_GIT\config\ppo\Enemy_Orc.yaml" --run-id=Enemy_Orc_K --resum(2시간즈음부터 성능 향상 시작됨)

    Coroutine bigCor;
    WaitForSeconds wait = new WaitForSeconds(0.12f);

    //대상과의 거리
    float curRange;

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
        if(creature.target != null)
        curRange = (creature.target.transform.position - transform.position).magnitude;
        //AddReward(-0.0005f);

        //  Discrete Action(정수를 반환함, 특정 행동에 사용하기 좋음(AllBuffered와 같은 느낌?))
        //  mlagents-learn --force

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
                        if (!creature.isAttack && curRange <= maxRange)//쿨타임이 돌았으면서, 거리 이내여야 함
                        {
                            //애니메이션 관리
                            creature.curCreatureMoveEnum = CreatureMoveEnum.Idle;
                            creature.isAttack = true;//동시 입력 방지

                            int r = Random.Range(0, 2);
                            if (r == 0) anim.SetTrigger("isAttackLeft");
                            else if (r == 1) anim.SetTrigger("isAttackRight");

                            //주황색 참격 생성
                            GameObject slash = objectManager.CreateObj("Infantry_A_Slash", ObjectManager.PoolTypes.BulletPool);
                            Bullet slash_bullet = slash.GetComponent<Bullet>();
                            slash_bullet.BulletOn(this);
                            //이동
                            slash.transform.position = transform.position + transform.forward + Vector3.up * 3;
                            //회전
 
                            slash.transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x + 90,
                                transform.rotation.eulerAngles.y - 180, transform.rotation.eulerAngles.z - 90);

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

        int x = 0;

        if (Input.GetKey(KeyCode.LeftArrow))
            x = 2;
        else if (Input.GetKey(KeyCode.UpArrow))
            x = 1;
        else if (Input.GetKey(KeyCode.RightArrow))
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
        if (gameObject.activeSelf) //죽으면 필요 없자너
        {
            sensor.AddObservation(transform.position.x);//state size = 1     x,y,z를 모두 받아오면 size가 3이 됨
            sensor.AddObservation(transform.position.y);

            //가속을 더하기도 함
            sensor.AddObservation(rigid.velocity.x);//state size = 1
            sensor.AddObservation(rigid.velocity.y);

            if (creature.target != null) //시작 한 순간, 빈 취급됨
            {
                //플레이어의 정보
                sensor.AddObservation(creature.target.transform.position.x);
                sensor.AddObservation(creature.target.transform.position.y);
                //각각의 거리
                sensor.AddObservation(curRange);
            }

            sensor.AddObservation(StepCount / (float)MaxStep);//진행한 스텝 비율    //state size = 1
        }
    }
    #endregion

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.CompareTag("Outline")) //맵 밖으로 나가지면 사망
        {
            AddReward(-1f);

        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //if (collision.gameObject == player)
        {
            //AddReward(10f);//점프였나 거기
            //EndEpisode();//이것만으로 초기화가 되진 않음
        }
    }

    [Header("재시작점")]
    public Transform point;

    public override void OnEpisodeBegin()//EndEpisode가 호출됐을 때 사용됨(씬을 호출할 때는 통째로 삭제)
    {
        transform.position = point.position;
        creature.Revive();
    }
}
