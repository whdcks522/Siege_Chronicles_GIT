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

    //mlagents-learn "D:\Unities\Github_DeskTop\ML_EX_GIT\config\poca\Custom_Infantry_A.yaml" --run-id=Custom_Infantry_CC --resum

    //mlagents-learn "D:\Unities\Github_DeskTop\ML_EX_GIT\config\poca\StrikersVsGoalie.yaml" --run-id=SoccerML --resum

    /*
 Version information:
  ml-agents: 0.28.0,
  ml-agents-envs: 0.28.0,
  Communicator API: 1.5.0,
  PyTorch: 1.13.1+cpu
[INFO] Listening on port 5004. Start training by pressing the Play button in the Unity Editor.
[INFO] Connected to Unity environment with package version 3.0.0-exp.1 and communication version 1.5.0
[INFO] Connected new brain: Striker?team=0
[INFO] Connected new brain: Goalie?team=1
[INFO] Hyperparameters for behavior name Striker:
        trainer_type:   poca
        hyperparameters:
          batch_size:   2048
          buffer_size:  20480
          learning_rate:        0.0003
          beta: 0.005
          epsilon:      0.2
          lambd:        0.95
          num_epoch:    3
          learning_rate_schedule:       constant
          beta_schedule:        constant
          epsilon_schedule:     constant
        network_settings:
          normalize:    False
          hidden_units: 512
          num_layers:   2
          vis_encode_type:      simple
          memory:       None
          goal_conditioning_type:       hyper
          deterministic:        False
        reward_signals:
          extrinsic:
            gamma:      0.99
            strength:   1.0
            network_settings:
              normalize:        False
              hidden_units:     128
              num_layers:       2
              vis_encode_type:  simple
              memory:   None
              goal_conditioning_type:   hyper
              deterministic:    False
        init_path:      None
        keep_checkpoints:       5
        checkpoint_interval:    500000
        max_steps:      30000000
        time_horizon:   1000
        summary_freq:   10000
        threaded:       False
        self_play:
          save_steps:   50000
          team_change:  200000
          swap_steps:   4000
          window:       10
          play_against_latest_model_ratio:      0.5
          initial_elo:  1200.0
        behavioral_cloning:     None
[INFO] Hyperparameters for behavior name Goalie:
        trainer_type:   poca
        hyperparameters:
          batch_size:   2048
          buffer_size:  20480
          learning_rate:        0.0003
          beta: 0.005
          epsilon:      0.2
          lambd:        0.95
          num_epoch:    3
          learning_rate_schedule:       constant
          beta_schedule:        constant
          epsilon_schedule:     constant
        network_settings:
          normalize:    False
          hidden_units: 512
          num_layers:   2
          vis_encode_type:      simple
          memory:       None
          goal_conditioning_type:       hyper
          deterministic:        False
        reward_signals:
          extrinsic:
            gamma:      0.99
            strength:   1.0
            network_settings:
              normalize:        False
              hidden_units:     128
              num_layers:       2
              vis_encode_type:  simple
              memory:   None
              goal_conditioning_type:   hyper
              deterministic:    False
        init_path:      None
        keep_checkpoints:       5
        checkpoint_interval:    500000
        max_steps:      30000000
        time_horizon:   1000
        summary_freq:   10000
        threaded:       False
        self_play:
          save_steps:   50000
          team_change:  200000
          swap_steps:   1000
          window:       10
          play_against_latest_model_ratio:      0.5
          initial_elo:  1200.0
        behavioral_cloning:     None
c:\users\happy\appdata\local\programs\python\python37\lib\site-packages\mlagents\trainers\torch\networks.py:91: UserWarning: Creating a tensor from a list of numpy.ndarrays is extremely slow. Please consider converting the list to a single numpy.ndarray with numpy.array() before converting to a tensor. (Triggered internally at C:\actions-runner\_work\pytorch\pytorch\builder\windows\pytorch\torch\csrc\utils\tensor_new.cpp:233.)
  enc.update_normalization(torch.as_tensor(vec_input))
c:\users\happy\appdata\local\programs\python\python37\lib\site-packages\mlagents\trainers\torch\utils.py:287: UserWarning: The use of `x.T` on tensors of dimension other than 2 to reverse their shape is deprecated and it will throw an error in a future release. Consider `x.mT` to transpose batches of matrices or `x.permute(*torch.arange(x.ndim - 1, -1, -1))` to reverse the dimensions of a tensor. (Triggered internally at C:\actions-runner\_work\pytorch\pytorch\builder\windows\pytorch\aten\src\ATen\native\TensorShape.cpp:3281.)
  for i, _act in enumerate(discrete_actions.long().T)
[INFO] Striker. Step: 10000. Time Elapsed: 61.405 s. Mean Reward: -0.733. Mean Group Reward: 0.267. Training. ELO: 1202.239.
[INFO] Striker. Step: 20000. Time Elapsed: 112.071 s. Mean Reward: -0.792. Mean Group Reward: 0.208. Training. ELO: 1205.683.
[INFO] Striker. Step: 30000. Time Elapsed: 191.050 s. Mean Reward: -0.777. Mean Group Reward: 0.222. Training. ELO: 1209.077.
[INFO] Striker. Step: 40000. Time Elapsed: 233.563 s. Mean Reward: -0.925. Mean Group Reward: 0.075. Training. ELO: 1211.944.
[INFO] Striker. Step: 50000. Time Elapsed: 313.442 s. Mean Reward: -0.985. Mean Group Reward: 0.015. Training. ELO: 1213.359.
[INFO] Striker. Step: 60000. Time Elapsed: 359.951 s. Mean Reward: -0.737. Mean Group Reward: 0.263. Training. ELO: 1215.689.
[INFO] Striker. Step: 70000. Time Elapsed: 439.277 s. Mean Reward: -0.700. Mean Group Reward: 0.299. Training. ELO: 1219.404.
[INFO] Striker. Step: 80000. Time Elapsed: 490.371 s. Mean Reward: -0.939. Mean Group Reward: 0.061. Training. ELO: 1221.693.
[INFO] Striker. Step: 90000. Time Elapsed: 569.268 s. Mean Reward: -0.604. Mean Group Reward: 0.396. Training. ELO: 1224.398.
[INFO] Striker. Step: 100000. Time Elapsed: 611.394 s. Mean Reward: -0.740. Mean Group Reward: 0.259. Training. ELO: 1228.386.
[INFO] Striker. Step: 110000. Time Elapsed: 692.599 s. Mean Reward: -0.783. Mean Group Reward: 0.216. Training. ELO: 1230.988.
[INFO] Striker. Step: 120000. Time Elapsed: 737.499 s. Mean Reward: -0.998. Mean Group Reward: 0.002. Training.
[INFO] Striker. Step: 130000. Time Elapsed: 814.761 s. Mean Reward: -0.937. Mean Group Reward: 0.063. Training. ELO: 1232.696.
[INFO] Striker. Step: 140000. Time Elapsed: 863.325 s. Mean Reward: -0.597. Mean Group Reward: 0.403. Training. ELO: 1236.077.
[INFO] Striker. Step: 150000. Time Elapsed: 942.459 s. Mean Reward: -0.834. Mean Group Reward: 0.166. Training. ELO: 1239.467.
[INFO] Striker. Step: 160000. Time Elapsed: 990.887 s. Mean Reward: -0.637. Mean Group Reward: 0.363. Training. ELO: 1242.328.
[INFO] Striker. Step: 170000. Time Elapsed: 1072.811 s. Mean Reward: -0.645. Mean Group Reward: 0.354. Training. ELO: 1245.967.
[INFO] Striker. Step: 180000. Time Elapsed: 1113.077 s. Mean Reward: -0.663. Mean Group Reward: 0.337. Training. ELO: 1248.765.
[INFO] Striker. Step: 190000. Time Elapsed: 1161.907 s. Mean Reward: -0.587. Mean Group Reward: 0.413. Training. ELO: 1252.354.
[INFO] Striker. Step: 200000. Time Elapsed: 1242.502 s. Mean Reward: -0.994. Mean Group Reward: 0.006. Training.
[INFO] Goalie. Step: 10000. Time Elapsed: 1339.150 s. Mean Reward: 0.713. Mean Group Reward: -0.571. Training. ELO: 1173.706.
[INFO] Goalie. Step: 20000. Time Elapsed: 1420.502 s. Mean Reward: 0.588. Mean Group Reward: -0.471. Training. ELO: 1170.161.
[INFO] Goalie. Step: 30000. Time Elapsed: 1537.590 s. Mean Reward: 0.765. Mean Group Reward: -0.385. Training. ELO: 1167.561.
[INFO] Goalie. Step: 40000. Time Elapsed: 1622.565 s. Mean Reward: 0.649. Mean Group Reward: -0.533. Training. ELO: 1165.162.
[INFO] Goalie. Step: 50000. Time Elapsed: 1733.144 s. Mean Reward: 0.669. Mean Group Reward: -0.467. Training. ELO: 1162.091.
[INFO] Goalie. Step: 60000. Time Elapsed: 1817.837 s. Mean Reward: 0.674. Mean Group Reward: -0.533. Training. ELO: 1158.873.
[INFO] Goalie. Step: 70000. Time Elapsed: 1928.073 s. Mean Reward: 0.657. Mean Group Reward: -0.359. Training. ELO: 1156.684.
[INFO] Goalie. Step: 80000. Time Elapsed: 2020.535 s. Mean Reward: 0.589. Mean Group Reward: -0.556. Training. ELO: 1154.279.
[INFO] Goalie. Step: 90000. Time Elapsed: 2133.026 s. Mean Reward: 0.718. Mean Group Reward: -0.500. Training. ELO: 1151.278.
[INFO] Goalie. Step: 100000. Time Elapsed: 2216.519 s. Mean Reward: 0.529. Mean Group Reward: -0.556. Training. ELO: 1148.386.
[INFO] Goalie. Step: 110000. Time Elapsed: 2331.578 s. Mean Reward: 0.574. Mean Group Reward: -0.611. Training. ELO: 1144.821.
[INFO] Goalie. Step: 120000. Time Elapsed: 2419.819 s. Mean Reward: 0.629. Mean Group Reward: -0.500. Training. ELO: 1141.573.
[INFO] Goalie. Step: 130000. Time Elapsed: 2527.425 s. Mean Reward: 0.526. Mean Group Reward: -0.667. Training. ELO: 1138.199.
[INFO] Goalie. Step: 140000. Time Elapsed: 2618.839 s. Mean Reward: 0.744. Mean Group Reward: -0.429. Training. ELO: 1135.246.
[INFO] Goalie. Step: 150000. Time Elapsed: 2727.678 s. Mean Reward: 0.641. Mean Group Reward: -0.467. Training. ELO: 1133.139.
[INFO] Goalie. Step: 160000. Time Elapsed: 2820.443 s. Mean Reward: 0.812. Mean Group Reward: -0.308. Training. ELO: 1131.266.
[INFO] Goalie. Step: 170000. Time Elapsed: 2926.998 s. Mean Reward: 0.779. Mean Group Reward: -0.333. Training. ELO: 1129.927.
[INFO] Goalie. Step: 180000. Time Elapsed: 3016.948 s. Mean Reward: 0.734. Mean Group Reward: -0.429. Training. ELO: 1128.191.
[INFO] Goalie. Step: 190000. Time Elapsed: 3128.066 s. Mean Reward: 0.668. Mean Group Reward: -0.351. Training. ELO: 1126.864.
[INFO] Goalie. Step: 200000. Time Elapsed: 3218.584 s. Mean Reward: 0.638. Mean Group Reward: -0.562. Training. ELO: 1124.368.
[INFO] Striker. Step: 210000. Time Elapsed: 3261.481 s. Mean Reward: -0.632. Mean Group Reward: 0.368. Training. ELO: 1280.940.
[INFO] Striker. Step: 220000. Time Elapsed: 3345.018 s. Mean Reward: -0.308. Mean Group Reward: 0.692. Training. ELO: 1286.333.
[WARNING] Restarting worker[0] after 'Communicator has exited.'
[INFO] Listening on port 5004. Start training by pressing the Play button in the Unity Editor.


    
[INFO] Infantry_A. Step: 730000. Time Elapsed: 11269.088 s. No episode was completed since last summary. Training.
[INFO] Shooter_A. Step: 910000. Time Elapsed: 11392.454 s. No episode was completed since last summary. Training.
[INFO] Infantry_A. Step: 740000. Time Elapsed: 11502.568 s. Mean Reward: 588.185. Mean Group Reward: 0.000. Training. ELO: 1201.937.
[INFO] Shooter_A. Step: 920000. Time Elapsed: 11614.980 s. Mean Reward: 231.904. Mean Group Reward: 0.000. Training. ELO: 1200.050.

    오류 발생: RuntimeError: Could not allocate bytes object!
    https://mingchin.tistory.com/419
     */
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
            sensor.AddObservation(creature.ourTower.position.x);
            sensor.AddObservation(creature.ourTower.position.z);
            sensor.AddObservation(creature.ourTowerManager.curHealth / creature.ourTowerManager.maxHealth);
            //상대 타워의 정보
            sensor.AddObservation(creature.enemyTower.position.x);
            sensor.AddObservation(creature.enemyTower.position.z);
            sensor.AddObservation(creature.enemyTowerManager.curHealth / creature.enemyTowerManager.maxHealth);

            //각각의 거리
            sensor.AddObservation(curRange / maxRange);

            for (int i = 0; i < enemyCreatureFolder.childCount; i++)
            {
                //크리쳐 상태라면
                if (enemyCreatureFolder.GetChild(i).gameObject.layer == LayerMask.NameToLayer("Creature"))
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
        string bulletName = useBullet.name;

        GameObject slash = objectManager.CreateObj(bulletName, ObjectManager.PoolTypes.BulletPool);
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
