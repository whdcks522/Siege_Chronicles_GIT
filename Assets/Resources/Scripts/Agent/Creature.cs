using Google.Protobuf.WellKnownTypes;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Presets;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class Creature : MonoBehaviour
{
    [Header("���̴��� ���� �ؽ���")]
    public Texture baseTexture;
    [Header("���̴��� ���� ��Ų ������")]
    public SkinnedMeshRenderer skinnedMeshRenderer;

    [Header("����ü�� �ִ� ü��")]
    public float maxHealth;
    [Header("����ü�� ���� ü��")]
    float curHealth;

    [Header("����� ��")]
    public Transform target;
    [Header("���� ������ �ִ� �Ÿ�")]
    public float maxRange;
    [Header("���� ������ �Ÿ�")]
    public float curRange;

    [Header("�츮 ��")]
    public Transform ourTower;
    [Header("��� ��")]
    public Transform enemyTower;

    [Header("�Ѿ��� ���۵Ǵ� ��")]
    public Transform bulletStart;

    [Header("ĳ���� ���� �̴� UI")]
    public GameObject miniUI;
    public Image miniHealth;

    [Header("�޸��� �ӵ�")]
    public int runSpd;
    int rotSpd = 120;//ȸ�� �ӵ�

    Vector3 moveVec;//�̵��� ����

    

    public GameManager gameManager;
    Rigidbody rigid;
    Animator anim;
    ParentAgent parentAgent;

    public enum CreatureMoveEnum {Idle, Run}//�ӽŷ������� ���Ҽ� �ִ� �ൿ
    public CreatureMoveEnum curCreatureMoveEnum;
    
    public enum CreatureSpinEnum { LeftSpin, None,  RightSpin }//�ӽŷ������� ���Ҽ� �ִ� ȸ��
    public CreatureSpinEnum curCreatureSpinEnum;

    public enum TeamEnum { None, Blue, Red }//���ϴ� ��
    public TeamEnum curTeamEnum;

    //public enum CreatureTypeEnum { Melee, Range }//���ϴ� ��
    //public TeamEnum curTeamEnum;

    //����ü�� ���� ���
    bool isDead = false;
    [Header("���� ���� ������")]
    public bool isAttack = false;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        parentAgent = GetComponent<ParentAgent>();

        //�ؽ��� ���͸��� ����
        skinnedMeshRenderer.material.SetTexture("_BaseTexture", baseTexture);

        //int index = System.Array.IndexOf(System.Enum.GetValues(typeof(CreatureMove)), curCreatureMove);
        //Debug.Log(index);
    }

    #region ����ü Ȱ��ȭ
    public void Revive()
    {
        //���� ��� �ð� �ʱ�ȭ
        isAttack = false;
        //������ �Ÿ� �ʱ�ȭ
        curRange = 0;
        //���� �ʱ�ȭ
        rigid.velocity = Vector3.zero;
        //ü�� ȸ��
        curHealth = maxHealth;
        isDead = false;

        //miniHealth.fillAmount = 1;

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
            //if (target != null)
               // creature.curRange = (creature.target.transform.position - transform.position).magnitude;

            switch (curCreatureMoveEnum)
            {
                case CreatureMoveEnum.Idle://���߱�
                    moveVec = new Vector3(0, rigid.velocity.y, 0);
                    rigid.velocity = moveVec;

                    anim.SetBool("isRun", false);
                    break;
                case CreatureMoveEnum.Run://�޸���
                    moveVec = new Vector3(0, rigid.velocity.y, 0) + transform.forward * runSpd;
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

                    //anim.SetBool("isRun", false);
                    break;
                case CreatureSpinEnum.None:
                    break;
                case CreatureSpinEnum.RightSpin:
                    moveVec = transform.rotation.eulerAngles;
                    // �������� ���� ȸ���մϴ� (���⼭�� y�� ���� �����մϴ�)
                    moveVec.y += rotSpd * Time.deltaTime;
                    // ���ο� ȸ������ �����մϴ�
                    transform.rotation = Quaternion.Euler(moveVec);

                    //anim.SetBool("isRun", false);
                    break;
            }
        }
    }
    #endregion

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Bullet"))//��ź�� �浹���� ��
        {
            Bullet bullet = other.GetComponent<Bullet>();
            if (bullet.curTeamEnum != curTeamEnum)//���� �ٸ� ���
            {
                ParentAgent bulletHost = bullet.GetComponent<ParentAgent>();
                float damage = bullet.bulletDamage;

                bulletHost.AddReward(damage / 100f);//20�̸� 0.2f
            }
        }
    }

    #region �ǰ� ó��
    public void damageControlRPC(float _dmg)
    {
        if (isDead)
            return;

        //���ط� ���
        curHealth -= _dmg;
        if (curHealth < 0) curHealth = 0;
        else if (curHealth > maxHealth) curHealth = maxHealth;
        //UI����
        //miniHealth.fillAmount = curHealth / maxHealth;

        //��� �ʱ�ȭ
        if (curHealth > 0)//�ǰ��ϰ� ��� ����
        {
            anim.SetTrigger("isHit");
        }
        else if (curHealth <= 0) Dead();
    }
    #endregion

    #region ���ó��
    void Dead()
    {
        //��� ó��
        isDead = true;
        //�ִϸ��̼� ����
        anim.SetTrigger("isDeath");
        //�̴� UI �ݱ�
        //miniHealth.fillAmount = 0;
        //���� ����

        //�� ����
        Invoke("SoonDie", 1.5f);
    }
    #endregion

    #region ��� ��, �Ҹ�
    void SoonDie()//�׾��� ���� ��, ������ ���� ó��
    {
        //���ӿ�����Ʈ Ȱ��ȭ
        gameObject.SetActive(false);
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
    IEnumerator Dissolve(bool  InVisible)//�ְ��� 1.5�ʰ�
    {
        

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
