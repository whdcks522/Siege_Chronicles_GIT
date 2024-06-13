using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class Creature : MonoBehaviour
{
    [Header("ũ���� �� �⺻ �ɷ�")]
    public float maxHealth;//����ü�� �ִ� ü��
    public float curHealth;//����ü�� ���� ü��

    [Header("ũ���� �� Ư�� �ɷ� ����")]
    //��ȣ���� ���� �ִ���(���ظ� ���� ����, �ð��� �帧�� ���� ü�� ����, ���к��� ����)
    public int isShield;


    [Header("�츮 Ÿ��")]
    public Transform ourTower;
    public TowerManager ourTowerManager;
    public Transform ourCreatureFolder;//������� ����ִ� ����
    public Transform creatureStartPoint;
    [Header("�߸� ũ���� ����")]
    public Transform grayCreatureFolder;//�߸� ���� ����ִ� ����
    [Header("��� Ÿ��")]
    public Transform enemyTower;
    public TowerManager enemyTowerManager;
    public Transform enemyCreatureFolder;//������� ����ִ� ����


    [Header("UI ����")]
    public GameObject miniCanvas;//ĳ���� ���� �̴� UI
    public Image miniHealth;//ũ������ ü�� ������
    public GameObject CorpseExplosionObj;//��ü������ Ȱ��ȭ�� �ִ��� ��Ÿ���� ������

    [Header("�Ŵ���")]
    public GameManager gameManager;
    public ObjectManager objectManager;
    AudioManager audioManager;
    UIManager UIManager;
    public enum TeamEnum { Blue, Red, Gray }//���ϴ� ��
    [Header("�� ��")]
    public TeamEnum curTeamEnum;
    public Rigidbody rigid;//������Ģ
    public Animator anim;//�ִϸ��̼�
    public NavMeshAgent nav;//�� �̵��ϴ� �׺���̼�

    Transform cameraGround;//ī�޶� �����ϴ� ���� ����
    Transform mainCamera;//���� ī�޶� ��ü(ü�¹ٰ� �� ���� �ٶ� ������)

    private void Awake()
    {
        if (gameManager == null) 
            Debug.LogError("���ӸŴ��� ����");
        UIManager = gameManager.uiManager;

        objectManager = gameManager.objectManager;
        audioManager = gameManager.audioManager;
        mainCamera = UIManager.cameraObj;
        cameraGround = UIManager.cameraGround;
        rigid = GetComponent<Rigidbody>();

        nav = GetComponent<NavMeshAgent>();

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

        ourTowerManager = ourTower.GetComponent<TowerManager>();
        enemyTowerManager = enemyTower.GetComponent<TowerManager>();
        //�������� ����
        creatureStartPoint = ourTowerManager.creatureStartPoint;

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
    [Header("���� ���̵�")]//1, 2 ,3(�⺻: 2)
    public int gameLevel = 2;
    public void BeforeRevive(TeamEnum tmpTeamEnum, GameManager tmpGameManager) 
    {
        //�� ����
        curTeamEnum = tmpTeamEnum;
        //�Ŵ��� ����
        gameManager = tmpGameManager;
        //���̵� ����
        if (curTeamEnum == TeamEnum.Blue) 
        {
            gameLevel = 2;
        }
        else if (curTeamEnum == TeamEnum.Red)
        {
            gameLevel = gameManager.gameLevel;
        }

        Awake();
        Revive();
    }

    public void Revive()//ũ���� ��Ȱ�� ���� �ʱ�ȭ
    {
        //��ġ �ʱ�ȭ
        transform.position = creatureStartPoint.position;
        //ȸ�� �ʱ�ȭ
        transform.LookAt(enemyTower.position);
        //��� �ʱ�ȭ
        curTarget = null;

        //ü�� ȸ��
        curHealth = maxHealth;

        //ü�� UI ����
        miniCanvas.SetActive(true);
        miniHealth.fillAmount = 1;
        CanvasSpin();

        //��ü ���� ��Ȱ��ȭ
        CorpseExplosionObj.SetActive(false);

        if (curTeamEnum == TeamEnum.Blue)
            miniHealth.color = Color.blue;
        else if (curTeamEnum == TeamEnum.Red)
            miniHealth.color = Color.red;

        //������Ʈ Ȱ��ȭ
        gameObject.SetActive(true);

        nav.isStopped = true;

        //��� �ִϸ��̼�
        anim.SetTrigger("isRage");

        //���� ���̵���
        VisibleWarp();
    }
    #endregion

    #region ũ���ĺ� �׼�
    [Header("ũ���� �� ����ü ����")]
    public Transform bulletStartPoint;//�Ѿ��� ���۵Ǵ� ��
    public int yUp;//����ü y�� ��ȯ ��ġ
    public int zUp;//����ü z�� ��ȯ ��ġ, ����ü�� �ɰ����� �����ε� ����
    float split = 11.25f;
    public GameObject useBullet;//����ϴ� ����ü
    public Vector3 targetVec;//��ǥ ���� ����(���Ÿ� ���ݿ����ε� ���)

    public void AgentAction_1()
    {
        if (bulletStartPoint == null)//�������� ���
        {
            GameObject slash = objectManager.CreateObj(useBullet.name, ObjectManager.PoolTypes.BulletPool);
            Bullet slash_bullet = slash.GetComponent<Bullet>();

            //�̵�
            slash.transform.position = transform.position + transform.forward * zUp + Vector3.up * yUp;//���� 3, ������ 1

            //ȸ��
            slash.transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x + 90,
                transform.rotation.eulerAngles.y - 180, transform.rotation.eulerAngles.z - 90);

            //Ȱ��ȭ
            slash_bullet.BulletOn(curTeamEnum);
        }
        else //���Ÿ� ���� ���
        {
            for (int x = yUp; x <= zUp; x++)//3�߾� �߻��ϴ� ũ���ĸ� ����
            {
                // ����ü ����
                GameObject tracer = objectManager.CreateObj(useBullet.name, ObjectManager.PoolTypes.BulletPool);
                Bullet tracer_bullet = tracer.GetComponent<Bullet>();
                Rigidbody tracer_rigid = tracer.GetComponent<Rigidbody>();

                // �ʱ�ȭ
                tracer_bullet.gameManager = gameManager;
                tracer_bullet.Init();

                // ����ü ���� ��ġ ����
                tracer.transform.position = bulletStartPoint.position;

                // ����ü ���� ����
                targetVec = (curTarget.transform.position - transform.position).normalized;

                // ������ ��ȯ�Ͽ� ���ο� ���� ���� ���
                cameraRotation = Quaternion.Euler(0, x * split, 0);
                targetVec = cameraRotation * targetVec;

                // ����ü �ӵ� ����
                tracer_rigid.velocity = targetVec * tracer_bullet.bulletSpeed;

                // ����ü Ȱ��ȭ
                tracer_bullet.BulletOn(curTeamEnum);
            }
        }
    }
    #endregion

    #region ���� ����
    int slashCount = 0;

    //���� ��Ÿ� Ȯ��
    private void Update()//Update: �� ������
    {
        //���� �ʱ�ȭ
        rigid.velocity = Vector3.zero;
        rigid.angularVelocity = Vector3.zero;

        if (gameObject.layer == LayerMask.NameToLayer("Creature") && !nav.isStopped)//ũ���� ���̾�鼭 �޸��� �ִ� ���
        {
            if (!curTarget.gameObject.activeSelf)//����� ��Ȱ��ȭ�� ���¶��
            {
                //��� Ž��
                RangeFirstRangeCalc();

                //��ǥ���� ����
                nav.SetDestination(curTarget.transform.position);

                anim.SetBool("isRun", true);
            }

            //��� Ž��
            RangeFirstRangeCalc();

            if (curRange < maxRange)
            {
                nav.isStopped = true;
                nav.velocity = Vector3.zero;

                //�ִϸ��̼� ����
                anim.SetBool("isRun", false);

                //����� �ٶ� ������
                transform.LookAt(curTarget);

                if (bulletStartPoint == null)//�������� ���
                {
                    slashCount = (slashCount + 1) % 2;
                    if (slashCount == 0) anim.SetTrigger("isAttackLeft");
                    else if (slashCount == 1) anim.SetTrigger("isAttackRight");
                }
                else //���Ÿ� ���� ���
                {
                    anim.SetTrigger("isGun");
                }
            }
        }

        if (isShield != 0 && curHealth > 0)//��ȣ���� ������ ü���� ���� ����
        {
            //ü�� ����
            curHealth -= maxHealth * 1f / isShield * Time.deltaTime;
            //�ּ� ü�º��� ���� �ʵ���
            if (curHealth <= 0)
            {
                curHealth = 0;
                AlmostDead();
            }
            //UI����
            miniHealth.fillAmount = curHealth / maxHealth;
        }

        //ĵ���� ȸ��
        CanvasSpin();
    }
    //ī�޶� ȸ����
    Vector3 cameraVec;
    Quaternion cameraRotation;

    void CanvasSpin()//���� ü�� ĵ���� ȸ��
    {
        // ��ü A���� B�� �ٶ󺸴� ȸ�� ���ϱ�
        cameraVec = mainCamera.transform.position - cameraGround.transform.position;
        cameraRotation = Quaternion.LookRotation(cameraVec);
        // ��ü C�� ȸ�� ����
        miniCanvas.transform.rotation = cameraRotation;
    }
    #endregion

    GameObject damageFont = null;//������ ��Ʈ�� ���� ������Ʈ
    private void OnTriggerEnter(Collider other)//���𰡿� �浹���� ��
    {
        if (other.gameObject.CompareTag("Bullet"))//��ź�� �浹���� ��
        {
            Bullet bullet = other.GetComponent<Bullet>();
            if (bullet.curBulletEffectEnum == Bullet.BulleEffectEnum.Damage && bullet.curTeamEnum != curTeamEnum &&//�ٸ� ���� ���ظ� �ִ� �Ͱ� �浹,
                bullet.bulletDamage != 0 && (bullet.bulletTarget == null || bullet.bulletTarget == gameObject)) //����� ���ų� �ڽ��� ��
            {
                //���ط� Ȯ��(���� ������ ���� �Ⱦ����� ����, ������ �Ⱦ���)
                float damage = bullet.bulletDamage / gameLevel;
                if (isShield != 0)//��ȣ���� ������, ���� ��ȿȭ
                    damage = 0;

                //ũ���� �ǰ� ȿ����
                audioManager.PlaySfx(AudioManager.Sfx.CreatureHitSfx);

                //������ ��Ʈ ���
                if (curTeamEnum == TeamEnum.Blue)//�Ķ� Ÿ���� ������ �Ķ���
                {
                    damageFont = objectManager.CreateObj("BlueDamageFont", ObjectManager.PoolTypes.DamageFontPool);
                }
                else //���� Ÿ���� ������ ������
                {
                    damageFont = objectManager.CreateObj("RedDamageFont", ObjectManager.PoolTypes.DamageFontPool);
                }
                //��Ʈ ��ġ�� ���� ����
                damageFont.transform.position = transform.position;
                damageFont.GetComponent<DamageFont>().ReName(damage.ToString());

                //ü�� ����
                curHealth -= damage;
                //�ּ� ü�º��� ���� �ʵ���
                if (curHealth <= 0)
                {
                    curHealth = 0;
                    AlmostDead();
                }
                //UI����
                miniHealth.fillAmount = curHealth / maxHealth;

                //�ǰ��� �Ѿ� ��ó��
                if (bullet.curBulletMoveEnum != Bullet.BulletMoveEnum.Slash)
                {
                    //�Ѿ� ��Ȱ��ȭ
                    bullet.BulletOff();
                }
            }
            else if (bullet.curBulletEffectEnum == Bullet.BulleEffectEnum.Cure && bullet.curTeamEnum == curTeamEnum) //ȸ���ϴ� �Ͱ� ���� ���� �浹
            {
                //ü�� ȸ��(ȸ��ġ�� ���̵��� ���� ���� ����)
                curHealth += bullet.bulletDamage;

                damageFont = objectManager.CreateObj("PinkDamageFont", ObjectManager.PoolTypes.DamageFontPool);
            
                //��Ʈ ��ġ�� ���� ����
                damageFont.transform.position = transform.position;
                damageFont.GetComponent<DamageFont>().ReName(bullet.bulletDamage.ToString());

                //�ִ� ü���� ���� �ʵ���
                if (curHealth > maxHealth) curHealth = maxHealth;

                //UI����
                miniHealth.fillAmount = curHealth / maxHealth;

            }
        }
    }

    #region ���ó��
    void AlmostDead()
    {
        //�ǰݴ����� �ʵ���, ���̾� ����
        gameObject.layer = LayerMask.NameToLayer("WarpCreature");

        //�ִϸ��̼� ����
        anim.SetTrigger("isDeath");

        nav.isStopped = true;

        //�̴� UI �ݱ�
        miniCanvas.SetActive(false);

        if (!CorpseExplosionObj.activeSelf)//��ü ������ �ƴ� ���
        {
            //ũ���� �ǰ� ȿ����
            audioManager.PlaySfx(AudioManager.Sfx.CreatureHitSfx);
        }
        else if (CorpseExplosionObj.activeSelf)//��ü������ ���
        {
            //��ü���� ���� ȿ����
            audioManager.PlaySfx(AudioManager.Sfx.CorpseExplosionAdaptSfx);

            GameObject bomb = objectManager.CreateObj("Tower_CorpseExplosion", ObjectManager.PoolTypes.BulletPool);
            Bullet bomb_bullet = bomb.GetComponent<Bullet>();
            //��ü ������ �̵�
            bomb.transform.position = transform.position;
            //��ü ������ �� ����
            bomb_bullet.BulletOn(curTeamEnum);
        }

        //�ְ���
        InvisibleWarp();
    }

    //�������� �Ŀ�, ������ ����
    public void CompletelyDead() 
    {
        //�ڱ� Ÿ���� ��ϵ� ũ���� �� ����
        if (gameObject.activeSelf && curHealth <= 0)
        {
            ourTowerManager.curCreatureCount--;

            if (curTeamEnum == TeamEnum.Blue)//����� �ؽ�Ʈ ����
                ourTowerManager.CreatureCountText();

            //����ü ��Ȱ��ȭ
            gameObject.SetActive(false);

            //�߸� ������ �ű��
            transform.parent = objectManager.grayCreatureFolder;
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
    public void RangeFirstRangeCalc()//���� ����� ���� ������
    {
        curRange = (enemyTower.position - transform.position).magnitude - 2;//�ϴ� ���� Ÿ���� ����
        curTarget = enemyTower;

        foreach (Transform obj in enemyCreatureFolder)
        {
            if (obj.gameObject.activeSelf)//obj.gameObject.layer == LayerMask.NameToLayer("Creature")
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

    [Header("�ؽ���")]//õõ�� ���̴� ��
    public Texture baseTexture;
    public SkinnedMeshRenderer skinnedMeshRenderer;//���̴��� ���� ��Ų ������
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

            CompletelyDead();
        }
        else if(!InVisible)//���̵��� 
        {
            //�ǰݴ��ϵ���, ���̾� ����
            gameObject.layer = LayerMask.NameToLayer("Creature");

            nav.isStopped = false;
            //��� Ž��
            RangeFirstRangeCalc();
            //��ǥ���� ����
            nav.SetDestination(curTarget.transform.position);           

            //�޸��� �ִϸ��̼�
            anim.SetBool("isRun", true);
        }
    }
    #endregion
}
