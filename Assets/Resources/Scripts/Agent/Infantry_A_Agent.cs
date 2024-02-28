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
    /*
     D:\Unities\Github_DeskTop\ML_EX_GIT\config\ppo
     */


    //���� ������ ���� ����, �ȸ����� ����
    //���� ������ ���� ����

    /*
     ��ũ��Ʈ������ ������ �ڵ�����

- Initialize : ȯ���� ����� �� ȣ��Ǵ� �ʱ�ȭ �Լ�(����� ��)
- CollectObservations : Agent���� Vector Observation ������ ������ �ִ� �Լ� 
- OnActionReceived : Agent�� ������ �ൿ�� ����, ���� ������Ʈ, ���Ǽҵ� ���� 
- OnEpisodeBegin : �� ���Ǽҵ尡 ���۵� �� ȣ��Ǵ� �Լ�
- Heuristic : �����ڰ� ���� ����� ������ �޸���ƽ ��忡�� ��� 
     */

    //mlagents-learn --force
    //mlagents-learn "D:\Unities\Github_DeskTop\ML_EX_GIT\config\poca\Custom_Infantry_A.yaml" --run-id=Custom_Infantry_G --resum

    //mlagents-learn "D:\Unities\Github_DeskTop\ML_EX_GIT\config\poca\StrikersVsGoalie.yaml" --run-id=Custom_Soccer --resum

    //mlagents-learn "D:\Unities\Github_DeskTop\ML_EX_GIT\config\ppo\Basic.yaml" --run-id=Custom_Basic --resum
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


    ���� �߻�: RuntimeError: Could not allocate bytes object!
    https://mingchin.tistory.com/419
     */
    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        creature = GetComponent<Creature>();
        anim = GetComponentInChildren<Animator>();

        gameManager = creature.gameManager;
        objectManager = gameManager.objectManager;

        StateReturn();
    }

    
    public override void OnActionReceived(ActionBuffers actions)//�׼� ����(������ ����), �� �� ȣ�� 
    {
        if (!creature.isAttack && gameObject.layer == LayerMask.NameToLayer("Creature")) 
        {
            rewardValue =  GetCumulativeReward();
            creature.curReward.text = rewardValue.ToString("F1");

            //������� ���� ����
            GetMatchingVelocityReward();
            //������ �Ÿ� ���
            RangeCalculate();

            

            switch (actions.DiscreteActions[0])
            {
                case 0://�������� ȸ��
                    creature.curCreatureSpinEnum = CreatureSpinEnum.LeftSpin;
                    break;
                case 1://���ֱ�
                    creature.curCreatureSpinEnum = CreatureSpinEnum.None;
                    break;
                case 2://���������� ȸ��
                    creature.curCreatureSpinEnum = CreatureSpinEnum.RightSpin;
                    break;
            }

            switch (actions.DiscreteActions[1]) 
            {
                case 0://���ֱ�
                    creature.curCreatureMoveEnum = CreatureMoveEnum.Idle;
                    break;
                case 1://�޸���
                    creature.curCreatureMoveEnum = CreatureMoveEnum.Run;
                    break;
                case 2://����
                    if (curRange <= maxRange && gameObject.layer == LayerMask.NameToLayer("Creature"))
                    {
                        //�ִϸ��̼� ����
                        creature.curCreatureSpinEnum = CreatureSpinEnum.None;
                        creature.curCreatureMoveEnum = CreatureMoveEnum.Idle;
                        creature.isAttack = true;//���� �Է� ����

                        int r = UnityEngine.Random.Range(0, 2);
                        if (r == 0) anim.SetTrigger("isAttackLeft");
                        else if (r == 1) anim.SetTrigger("isAttackRight");
                    }
                    break;
            }
        }

        //Debug.Log("spin: " + actions.DiscreteActions[0] + "action: " + actions.DiscreteActions[1]);
    }

    

    #region �޸���ƽ: Ű���带 ���� ������Ʈ�� ����
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var disCreteActionOut = actionsOut.DiscreteActions;

        if (behaviorParameters.BehaviorType == Unity.MLAgents.Policies.BehaviorType.HeuristicOnly)
        {
            int spin = 1;//ȸ�� ����
            if (Input.GetKey(KeyCode.LeftArrow))//��ȸ��
                spin = 0;
            else if (Input.GetKey(KeyCode.RightArrow))//��ȸ��
                spin = 2;

            int action = 0;//�׼� ����
            if (Input.GetKey(KeyCode.UpArrow))//�ȱ�
                action = 1;
            else if (Input.GetKey(KeyCode.Z))//����
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

    #region ������ ����, 5�� �� �ѹ� ȣ��
    public override void CollectObservations(VectorSensor sensor)
    {
        //1. ��ġ��, �޾ƿ��� �����Ͱ� ���� ���� ����
        if (gameObject.layer == LayerMask.NameToLayer("Creature")) //������ �ʿ� ���ڳ�
        {
            //���� �ڽ��� ��ġ
            sensor.AddObservation(transform.position.x);//state size = 1     x,y,z�� ��� �޾ƿ��� size�� 3�� ��
            sensor.AddObservation(transform.position.z);
            //���� �ڽ��� ȸ��
            sensor.AddObservation(transform.rotation.y);
            //���� �ڽ��� ����
            //sensor.AddObservation(rigid.velocity.x);
            //sensor.AddObservation(rigid.velocity.z);

            //�ڱ� Ÿ���� ����
            //sensor.AddObservation(creature.ourTower.position.x);
            //sensor.AddObservation(creature.ourTower.position.z);
            sensor.AddObservation(creature.ourTowerManager.curHealth / creature.ourTowerManager.maxHealth);

            //��� Ÿ���� ����
            //sensor.AddObservation(creature.enemyTower.position.x);
            //sensor.AddObservation(creature.enemyTower.position.z);
            sensor.AddObservation(creature.enemyTowerManager.curHealth / creature.enemyTowerManager.maxHealth);

            //�������� �Ÿ�
            sensor.AddObservation(curRange / maxRange);

            //����� ���� ��ġ
            sensor.AddObservation(curTarget.position.x);
            sensor.AddObservation(curTarget.position.z);

            //�츮 Ÿ������ ���� ����� ���� ��ġ
            sensor.AddObservation(creature.ourTowerManager.curTarget.position.x);
            sensor.AddObservation(creature.ourTowerManager.curTarget.position.z);
        }
    }
    #endregion


    #region ��Ȳ�� ���� ����
    override public void AgentAttack()
    {
        string bulletName = useBullet.name;

        GameObject slash = objectManager.CreateObj(bulletName, ObjectManager.PoolTypes.BulletPool);
        Bullet slash_bullet = slash.GetComponent<Bullet>();
        
        //�̵�
        slash.transform.position = transform.position + transform.forward + Vector3.up * 3;

        //ȸ��
        slash.transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x + 90,
            transform.rotation.eulerAngles.y - 180, transform.rotation.eulerAngles.z - 90);
        //Ȱ��ȭ
        slash_bullet.BulletOn(this);
    }
    #endregion
}
