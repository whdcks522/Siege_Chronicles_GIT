using Google.Protobuf.WellKnownTypes;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Unity.MLAgents;
using Unity.MLAgents.Policies;
using Unity.VisualScripting;
using UnityEditor.Presets;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class Creature : MonoBehaviour
{
    [Header("���̴��� ���� �ؽ���")]
    public Texture baseTexture;
    public SkinnedMeshRenderer skinnedMeshRenderer;//���̴��� ���� ��Ų ������
    public MeshRenderer meshRenderer;//���̿� ���� ��Ų ������

    [Header("����ü�� �ִ� ü��")]
    public float maxHealth;
    [Header("����ü�� ���� ü��")]
    public float curHealth;

    [Header("���� ���� ������")]
    public bool isAttack = false;

    [Header("�츮 Ÿ��")]
    public Transform ourTower;
    public TowerManager ourTowerManager;
    [Header("��� Ÿ��")]
    public Transform enemyTower;
    public TowerManager enemyTowerManager;
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

    public enum CreatureTypeEnum { Dummy, Infantry_A, Shooter_A }//�ӽŷ������� ���Ҽ� �ִ� �ൿ
    public CreatureTypeEnum curCreatureTypeEnum;

    

    public Rigidbody rigid;
    public Animator anim;
    public Agent agent;

    public Transform enemyCreatureFolder;//������� ����ִ� ����
    public BehaviorParameters behaviorParameters;//����Ʈ������ ������ �ǹǷ� �����ϱ� ����

    [Header("�ӽŷ��� ������(AI �Ŵ������� ����)")]
    public bool isML;

    [Header("�Ŵ���")]
    public GameManager gameManager;
    public ObjectManager objectManager;

    UIManager UIManager;
    Transform cameraGround;
    Transform mainCamera;
    //--------




    //public enum CreatureTypeEnum { Melee, Range }//���ϴ� ��
    //public TeamEnum curTeamEnum;
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


        //�ؽ��� ���͸��� ����
        if (curCreatureTypeEnum != CreatureTypeEnum.Dummy)
        {
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
                //�� ���� ����
                enemyCreatureFolder = objectManager.redCreatureFolder;
            }
            else if (curTeamEnum == TeamEnum.Red)//���� ��
            {
                //�� ���� ����
                enemyCreatureFolder = objectManager.blueCreatureFolder;
            }
        }
        else if (curCreatureTypeEnum == CreatureTypeEnum.Dummy)
        {
            Revive();
        }
    }


    #region ����ü Ȱ��ȭ
    public void Revive()
    {

        if (curCreatureTypeEnum != CreatureTypeEnum.Dummy)
        {
            //��ġ �ʱ�ȭ
            transform.position = startPoint.position;
            //ȸ�� �ʱ�ȭ
            transform.LookAt(enemyTower.position);
        }
        else if (curCreatureTypeEnum == CreatureTypeEnum.Dummy) 
        {
            //���� ��ġ �ʱ�ȭ
            int r = Random.Range(0, DummyPoints.Length);

            transform.position = DummyPoints[r].position;
        }


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

        if (curCreatureTypeEnum != CreatureTypeEnum.Dummy)
            anim.SetTrigger("isRage");

        VisibleWarp();
    }
    #endregion


    #region ���� ����
    private void FixedUpdate()
    {
        if (gameObject.layer == LayerMask.NameToLayer("Creature") && curCreatureTypeEnum != CreatureTypeEnum.Dummy)
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
            if (bullet.curTeamEnum != curTeamEnum)//���� �ٸ� ���
            {
                //���ط� Ȯ��
                Agent bulletAgent = bullet.bulletHost.agent;
                float damage = bullet.bulletDamage;

                //������ ���� ����
                bulletAgent.AddReward(damage / 10f);
                //�ǰ��� ���� ����
                if(curCreatureTypeEnum != CreatureTypeEnum.Dummy)
                    agent.AddReward(-damage / 100f);

                //���� ����
                damageControl(damage);

                //�ǰ��� �Ѿ� ��ó��
                if (bullet.curBulletMoveEnum != Bullet.BulletMoveEnum.Slash)
                    bullet.BulletOff();
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

        //��� �ʱ�ȭ
        if (curHealth > 0 && !isAttack && curCreatureTypeEnum != CreatureTypeEnum.Dummy)//�ǰݴ��ϰ� ��� �����鼭, �������� �ƴ϶��
        {
            anim.SetTrigger("isHit");
        }
        else if (curHealth <= 0) AlmostDead();

    }
    #endregion

    #region ���ó��
    void AlmostDead()
    {
        if (isML)
        {
            //Debug.Log("���");
            agent.EndEpisode();
        }
        else if (!isML)
        {
            //�ǰݴ����� �ʵ���, ���̾� ����
            gameObject.layer = LayerMask.NameToLayer("WarpCreature");

            //�ִϸ��̼� ����
            if (curCreatureTypeEnum != CreatureTypeEnum.Dummy)
                anim.SetTrigger("isDeath");

            //�̴� UI �ݱ�
            miniCanvas.SetActive(false);

            //���� ����
            //�ְ���
            InvisibleWarp();
        }
    }

    //������ ����
    public void CompletelyDead() => gameObject.SetActive(false);


    #endregion

    #region �´� �������� ���� �ִ���

    //��ǥ ���� ����
    public Vector3 goalVec;
    //�̵��ϴ� ����
    Vector3 curVec;

    public void GetMatchingVelocityReward()
    {
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
            float tmpReward = (cosineSimilarity + 1f) / 2f;  //0f ~ 1f
            tmpReward -= 0.5f;                         //-0.5f ~ 0.5f

            //Debug.Log(tmpReward);

            agent.AddReward(tmpReward / 1000f);
        }
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

    #region ȯ�� �ʱ�ȭ
    public Transform[] DummyPoints;
    public void resetEnv() 
    {
        //���� ��Ȱ
        Revive();

        //��ž ü�� ����
        ourTowerManager.TowerOn();
        enemyTowerManager.TowerOn();

        //�� ��Ȱ
        foreach (Transform obj in enemyCreatureFolder)
        {
            Creature crt = obj.GetComponent<Creature>();
            crt.Revive();
        }

        //DummyClear();

        
    }

    void DummyClear(int count) 
    {
        if (count == 1)
        {
            //���� 2�� ��ġ ����
            int r = Random.Range(0, DummyPoints.Length);

            //���� ��Ȱ
            enemyCreatureFolder.GetChild(0).GetComponent<Creature>().Revive();

            //���� ��ġ ����
            enemyCreatureFolder.GetChild(0).transform.position = DummyPoints[r].position;
        }
        else if (count == 2) 
        {
            //���� 2�� ��ġ ����
            int saveR = Random.Range(0, DummyPoints.Length);

            int newR = Random.Range(0, DummyPoints.Length);
            while (saveR == newR)
            {
                newR = Random.Range(0, DummyPoints.Length);
            }

            for (int i = 0; i < enemyCreatureFolder.childCount; i++)
            {
                //���� ��Ȱ
                enemyCreatureFolder.GetChild(i).GetComponent<Creature>().Revive();

                //���� ��ġ ����
                if (i == 0)
                    enemyCreatureFolder.GetChild(i).transform.position = DummyPoints[saveR].position;
                else if (i == 1)
                    enemyCreatureFolder.GetChild(i).transform.position = DummyPoints[newR].position;
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
        

        float duration = 1.05f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            if (curHealth <= 0 && !InVisible) break;


            float progress = elapsedTime / duration;
            float value = Mathf.Lerp(firstValue, targetValue, progress);
            elapsedTime += Time.deltaTime;

            if (curCreatureTypeEnum != CreatureTypeEnum.Dummy)
                skinnedMeshRenderer.material.SetFloat("_AlphaFloat", value);
            else if (curCreatureTypeEnum == CreatureTypeEnum.Dummy)
                meshRenderer.material.SetFloat("_AlphaFloat", value);

            yield return null;
        }
        if (curCreatureTypeEnum != CreatureTypeEnum.Dummy)
            skinnedMeshRenderer.material.SetFloat("_AlphaFloat", targetValue);
        else if (curCreatureTypeEnum == CreatureTypeEnum.Dummy)
            meshRenderer.material.SetFloat("_AlphaFloat", targetValue);

        if (InVisible)//�Ⱥ��̵���
        {
            //�ǰݴ����� �ʵ���, ���̾� ����
            gameObject.layer = LayerMask.NameToLayer("WarpCreature");

            if (curCreatureTypeEnum == CreatureTypeEnum.Dummy)
                gameObject.SetActive(false);
        }
        else if(!InVisible)//���̵��� 
        {
            //�ǰݴ��ϵ���, ���̾� ����
            gameObject.layer = LayerMask.NameToLayer("Creature");
        }
         
    }
    #endregion
}
