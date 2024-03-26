using Google.Protobuf.WellKnownTypes;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Unity.MLAgents;
using Unity.MLAgents.Policies;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class Creature : MonoBehaviour
{
    [Header("���̴��� ���� �ؽ���")]
    public Texture baseTexture;
    public SkinnedMeshRenderer skinnedMeshRenderer;//���̴��� ���� ��Ų ������

    [Header("����ü�� �ִ� ü��")]
    public float maxHealth;
    [Header("����ü�� ���� ü��")]
    public float curHealth;

    [Header("���� ���� ������")]
    public bool isAttack = false;


    [Header("�츮 Ÿ��")]
    public Transform ourTower;
    public TowerManager ourTowerManager;
    public Transform ourCreatureFolder;//������� ����ִ� ����
    [Header("�߸� ũ���� ����")]
    public Transform grayCreatureFolder;//�߸� ���� ����ִ� ����
    [Header("��� Ÿ��")]
    public Transform enemyTower;
    public TowerManager enemyTowerManager;
    public Transform enemyCreatureFolder;//������� ����ִ� ����
    [Header("�츮 Ÿ������ ������ ���")]
    public Transform startPoint;


    [Header("�Ѿ��� ���۵Ǵ� ��")]
    public Transform bulletStartPoint;

    public GameObject miniCanvas;//ĳ���� ���� �̴� UI
    public Image miniHealth;
    public Text curReward;

    [Header("�޸��� �ӵ�")]
    public int runSpd;
    int rotSpd = 120;//ȸ�� �ӵ�

    Vector3 moveVec;//�̵��� ����
    public enum TeamEnum {Blue, Red, Gray}//���ϴ� ��
    [Header("���ϴ� ��")]
    public TeamEnum curTeamEnum;
    public int teamIndex;

    public enum CreatureMoveEnum { Idle, Run }//�ӽŷ������� ���Ҽ� �ִ� �ൿ
    public CreatureMoveEnum curCreatureMoveEnum;

    public enum CreatureSpinEnum { LeftSpin, None, RightSpin }//�ӽŷ������� ���Ҽ� �ִ� ȸ��
    public CreatureSpinEnum curCreatureSpinEnum;

    public Rigidbody rigid;
    public Animator anim;
    public Agent agent;

    public BehaviorParameters behaviorParameters;//����Ʈ������ ������ �ǹǷ� �����ϱ� ����

    [Header("�Ŵ���")]
    public GameManager gameManager;
    public ObjectManager objectManager;

    UIManager UIManager;//
    Transform cameraGround;//ī�޶� �����ϴ� ��
    
    Transform mainCamera;//���� ī�޶� ��ü
    //--------


    /*
    �̵��ϸ� ����

    ���� ���߸� ����    
    ���� �����߸� ����

    Ÿ�� ���߸� ����
    Ÿ�� �̱������ ����, �������� ����
    */

    private void Awake()
    {
        if (gameManager == null) 
            Debug.LogError("���ӸŴ��� ����");
        UIManager = gameManager.uiManager;

        objectManager = gameManager.objectManager;
        mainCamera = UIManager.cameraObj;
        cameraGround = UIManager.cameraGround;

        //���͸��� ����
        skinnedMeshRenderer.material.SetTexture("_BaseTexture", baseTexture);

        if (curTeamEnum == TeamEnum.Blue)//�Ķ� ��
        {
            //�Ʊ� Ÿ�� ����
            ourTower = gameManager.blueTower;
            //�� Ÿ�� ����
            enemyTower = gameManager.redTower;
            //�±� ����
            gameObject.tag = "BlueCreature";

        }
        else if (curTeamEnum == TeamEnum.Red)//���� ��
        {
            //�Ʊ� Ÿ�� ����
            ourTower = gameManager.redTower;
            //�� Ÿ�� ����
            enemyTower = gameManager.blueTower;
            //�±� ����
            gameObject.tag = "RedCreature";
        }
        teamIndex = (int)(curTeamEnum);

        ourTowerManager = ourTower.GetComponent<TowerManager>();
        enemyTowerManager = enemyTower.GetComponent<TowerManager>();
        //�������� ����
        startPoint = ourTowerManager.creatureStartPoint;

        if (curTeamEnum == TeamEnum.Blue)//�Ķ� ��
        {
            //�Ʊ� ���� ����
            ourCreatureFolder = objectManager.blueCreatureFolder;
            //�� ���� ����
            enemyCreatureFolder = objectManager.redCreatureFolder;
        }
        else if (curTeamEnum == TeamEnum.Red)//���� ��
        {
            //�Ʊ� ���� ����
            ourCreatureFolder = objectManager.redCreatureFolder;
            //�� ���� ����
            enemyCreatureFolder = objectManager.blueCreatureFolder;
        }
        //�θ� ���� ����
        transform.parent = ourCreatureFolder;
        //��� �� �߸� ������ ����
        grayCreatureFolder = gameManager.objectManager.grayCreatureFolder;
    }

    #region ����ü Ȱ��ȭ

    public void BeforeRevive(TeamEnum tmpTeamEnum, GameManager tmpGameManager) 
    {
        //�� ����
        curTeamEnum = tmpTeamEnum;
        //�Ŵ��� ����
        gameManager = tmpGameManager;

        Awake();
        Revive();
    }

    public void Revive()
    {
        //��ġ �ʱ�ȭ
        transform.position = startPoint.position;
        //ȸ�� �ʱ�ȭ
        transform.LookAt(enemyTower.position);

        //���� ��� �ð� �ʱ�ȭ
        isAttack = false;
        //���� �ʱ�ȭ
        rigid.velocity = Vector3.zero;
        //ü�� ȸ��
        curHealth = maxHealth;


        //ü�� UI ����
        miniCanvas.SetActive(true);
        miniHealth.fillAmount = 1;

        if (curTeamEnum == TeamEnum.Blue)
            miniHealth.color = Color.blue;
        else if (curTeamEnum == TeamEnum.Red)
            miniHealth.color = Color.red;

        //������Ʈ Ȱ��ȭ
        gameObject.SetActive(true);
        //��� �ִϸ��̼�
        curCreatureSpinEnum = CreatureSpinEnum.None;
        curCreatureMoveEnum = CreatureMoveEnum.Idle;

        anim.SetTrigger("isRage");

        VisibleWarp();
    }
    #endregion


    #region ���� ����
    private void FixedUpdate()
    {
        if (gameObject.layer == LayerMask.NameToLayer("Creature"))
        {

            //�ൿ ����
            switch (curCreatureMoveEnum)
            {
                case CreatureMoveEnum.Idle://���߱�
                    moveVec = new Vector3(0, rigid.velocity.y, 0);
                    if (moveVec.y >= 0)
                        moveVec.y = 0;
                    rigid.velocity = moveVec;


                    anim.SetBool("isRun", false);
                    break;
                case CreatureMoveEnum.Run://�޸���
                    moveVec = new Vector3(0, rigid.velocity.y, 0) + transform.forward * runSpd;
                    if (moveVec.y >= 0)
                        moveVec.y = 0;
                    rigid.velocity = moveVec.normalized * runSpd;
                    rigid.angularVelocity = Vector3.zero;

                    anim.SetBool("isRun", true);
                    break;
            }
            switch (curCreatureSpinEnum)
            {
                case CreatureSpinEnum.LeftSpin:
                    moveVec = transform.rotation.eulerAngles;
                    // �������� ���� ȸ���մϴ� (���⼭�� y�� ���� �����մϴ�)
                    moveVec.y -= rotSpd * Time.deltaTime;
                    // ���ο� ȸ������ �����մϴ�
                    transform.rotation = Quaternion.Euler(moveVec);

                    break;
                case CreatureSpinEnum.None:
                    //ȸ�� ���ӵ� �ʱ�ȭ
                    moveVec = transform.rotation.eulerAngles;
                    moveVec.x = 0;
                    moveVec.z = 0;
                    transform.localEulerAngles = moveVec;

                    rigid.angularVelocity = Vector3.zero;
                    break;

                case CreatureSpinEnum.RightSpin:
                    moveVec = transform.rotation.eulerAngles;
                    // �������� ���� ȸ���մϴ� (���⼭�� y�� ���� �����մϴ�)
                    moveVec.y += rotSpd * Time.deltaTime;
                    // ���ο� ȸ������ �����մϴ�
                    transform.rotation = Quaternion.Euler(moveVec);
                    break;
            }
            
        }
    }
    #endregion

    //ī�޶� ȸ����
    Vector3 cameraVec;
    Quaternion lookRotation;
    private void LateUpdate()
    {
        // ��ü A���� B�� �ٶ󺸴� ȸ�� ���ϱ�
        cameraVec = mainCamera.transform.position - cameraGround.transform.position;
        lookRotation = Quaternion.LookRotation(cameraVec);

        // ��ü C�� ȸ�� ����
        miniCanvas.transform.rotation = lookRotation;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Bullet"))//��ź�� �浹���� ��
        {
            Bullet bullet = other.GetComponent<Bullet>();
            if (bullet.curBulletEffectEnum == Bullet.BulleEffectEnum.Damage)
            {
                if (bullet.curTeamEnum != curTeamEnum)//���� �ٸ� ���
                {
                    //���ط� Ȯ��
                    float damage = bullet.bulletDamage;

                    if (bullet.isCreature)
                    {
                        Agent bulletAgent = bullet.bulletHost.agent;
                        //������ ���� ����
                        bulletAgent.AddReward(damage / 10f);
                    }

                    //���� ����
                    damageControl(damage);

                    //�ǰ��� �Ѿ� ��ó��
                    if (bullet.curBulletMoveEnum != Bullet.BulletMoveEnum.Slash)
                    {
                        bullet.BulletOff();
                    }
                }
            }
            else if (bullet.curBulletEffectEnum == Bullet.BulleEffectEnum.Cure) 
            {
                if (bullet.curTeamEnum == curTeamEnum)//���� �ٸ� ���
                {
                    //���� ����
                    damageControl(bullet.bulletDamage);
                }
            }
        }
    }

    #region �ǰ� ó��
    public void damageControl(float _dmg)
    {
        //���ط� ���
        curHealth -= _dmg;

        if (curHealth < 0) curHealth = 0;
        else if (curHealth > maxHealth) curHealth = maxHealth;

        //UI����
        miniHealth.fillAmount = curHealth / maxHealth;

        //ȿ���� ����
        /*
        if (_dmg >= 0) 
        {
        
        }
        else if (_dmg < 0)
        {

        }
        */

        //��� �ʱ�ȭ
        if (curHealth <= 0)
            AlmostDead();

    }
    #endregion

    #region ���ó��
    void AlmostDead()
    {
        if (gameManager.isML)
        {
            agent.EndEpisode();
        }
        else if (!gameManager.isML)
        {
            //�ǰݴ����� �ʵ���, ���̾� ����
            gameObject.layer = LayerMask.NameToLayer("WarpCreature");

            //�ִϸ��̼� ����
            anim.SetTrigger("isDeath");

            //�̴� UI �ݱ�
            miniCanvas.SetActive(false);

            //�ְ���
            InvisibleWarp();
        }
    }

    //������ ����
    public void CompletelyDead() 
    {
        //����ü ��Ȱ��ȭ
        gameObject.SetActive(false);
        //�߸� ������ �ű��
        transform.parent = objectManager.grayCreatureFolder;

    }
    #endregion

    #region �´� �������� ���� �ִ���

    //��ǥ ���� ����
    public Vector3 goalVec;
    //�̵��ϴ� ����
    Vector3 curVec;

    public float GetMatchingVelocityReward()
    {
        float tmpReward = 0;
        //��ǥ ���� ����
        goalVec = (curTarget.transform.position - transform.position).normalized;
        //���簪 ���ִ� ����
        curVec = rigid.velocity.normalized;

        // �� ���� ������ ���� ��� (���� ����)
        float angle = Vector3.Angle(goalVec, curVec);
        // �ڻ��� ���絵 ��� (-1���� 1������ ��)
        float cosineSimilarity = Mathf.Cos(angle * Mathf.Deg2Rad);

        if (curCreatureMoveEnum != CreatureMoveEnum.Idle)//���ִٸ� 0�� ��ȯ
        {
            tmpReward = (cosineSimilarity + 1f) / 2f;  //0f ~ 1f
            tmpReward -= 0.5f;                         //-0.5f ~ 0.5f

            //Debug.Log(tmpReward);
        }
        return tmpReward;
    }
    #endregion

    #region ������� �Ÿ� ���
    [Header("���� ������ �ִ� �Ÿ�")]
    public float maxRange;
    [Header("���� ������ �Ÿ�")]
    public float curRange;
    [Header("���� ����� ���")]
    public Transform curTarget;

    public void EnemyFirstRangeCalc()//��� ���� ��� ���� ����
    {
        bool isLive = false;
        curRange = 9999;

        foreach (Transform obj in enemyCreatureFolder) 
        {

            if (obj.gameObject.layer == LayerMask.NameToLayer("Creature")) 
            {
                isLive = true;

                //������ �Ÿ�
                float tmpRange = (obj.position - transform.position).magnitude;
                if (tmpRange < curRange)
                {
                    curRange = tmpRange;
                    curTarget = obj;
                }
            } 
        }

        if (!isLive)//���� ���� ���ٸ�
        {
            curTarget = enemyTower;
            curRange = (curTarget.position - transform.position).magnitude - 2;//Ÿ���� �β� ���
        }
    }

    public void RangeFirstRangeCalc()//����� ������ ���
    {

        curRange = (enemyTower.position - transform.position).magnitude - 2;//�ϴ� ���� Ÿ���� ����
        curTarget = enemyTower;

        foreach (Transform obj in enemyCreatureFolder)
        {
            if (obj.gameObject.layer == LayerMask.NameToLayer("Creature"))
            {
                //������ �Ÿ�
                float tmpRange = (obj.position - transform.position).magnitude;
                if (tmpRange < curRange)
                {
                    curRange = tmpRange;
                    curTarget = obj;
                }
            }
        }
    }
    #endregion



    #region �ְ��� 
    public void InvisibleWarp() // ���� �Ⱥ��̰� �Ǵ� ��
    {
        StopCoroutine(Dissolve(false));
        StartCoroutine(Dissolve(true));
    }
    public void VisibleWarp() //���� ���̰� �Ǵ� �� 
    {
        if (curHealth > 0)
        {
            StopCoroutine(Dissolve(true));
            StartCoroutine(Dissolve(false));
        }
    }
    IEnumerator Dissolve(bool InVisible)//�ְ��� 1.5�ʰ�
    {
        //�ǰݴ����� �ʵ���, ���̾� ����
        gameObject.layer = LayerMask.NameToLayer("WarpCreature");

        float firstValue = InVisible ? 0f : 1f;      //true�� ���� �Ⱥ��̴� ��
        float targetValue = InVisible ? 1f : 0f;     //false�� ���� ���̴� ��
        

        float duration = 1.5f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            if (curHealth <= 0 && !InVisible) break;


            float progress = elapsedTime / duration;
            float value = Mathf.Lerp(firstValue, targetValue, progress);
            elapsedTime += Time.deltaTime;

            skinnedMeshRenderer.material.SetFloat("_AlphaFloat", value);

            yield return null;
        }
        skinnedMeshRenderer.material.SetFloat("_AlphaFloat", targetValue);

        if (InVisible)//�Ⱥ��̵���
        {
            //�ǰݴ����� �ʵ���, ���̾� ����
            gameObject.layer = LayerMask.NameToLayer("WarpCreature");
        }
        else if(!InVisible)//���̵��� 
        {
            //�ǰݴ��ϵ���, ���̾� ����
            gameObject.layer = LayerMask.NameToLayer("Creature");
        }
         
    }
    #endregion
}
