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
    public float curHealth;

    [Header("����ü�� �ִ� ���� ��� �ð�")]
    public float maxTime;
    [Header("����ü�� ���� ���� ��� �ð�")]
    public float curTime;

    [Header("�츮 ��")]
    public Transform ourTower;
    [Header("��� ��")]
    public Transform enemyTower;

    [Header("ĳ���� ���� �̴� UI")]
    public GameObject miniUI;
    public Image miniHealth;

    [Header("�޸��� �ӵ�")]
    public int runSpd;
    int rotSpd = 30;//ȸ�� �ӵ�

    Vector3 moveVec;//�̵��� ����

    Rigidbody rigid;
    Animator anim;

    public enum CreatureMoveEnum {Idle, Run, LeftSpin, RightSpin }//�ӽŷ������� ���Ҽ� �ִ� �ൿ
    public CreatureMoveEnum curCreatureMoveEnum;

    public enum TeamEnum { Blue, Red }//�ӽŷ������� ���Ҽ� �ִ� �ൿ
    public TeamEnum curTeamEnum;

    //����ü�� ���� ���
    bool isDead = false;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();

        //int index = System.Array.IndexOf(System.Enum.GetValues(typeof(CreatureMove)), curCreatureMove);
        //Debug.Log(index);
    }

    private void OnEnable()
    {
        //���� ��� �ð� �ʱ�ȭ
        curTime = 0;
        //���� �ʱ�ȭ
        rigid.velocity = Vector3.zero;
        //ü�� ȸ��
        curHealth = maxHealth;
        miniHealth.fillAmount = 1;

        isDead = false;

        //������Ʈ Ȱ��ȭ
        gameObject.SetActive(true);

        VisibleWarp();
    }

    #region ����ü Ȱ��ȭ
    public void Revive()
    {
        
    }
    #endregion

    #region �ִϸ��̼� ����
    void AnimationControl() 
    {
    
    }
    #endregion


    #region ���� ����
    private void FixedUpdate()
    {
        switch (curCreatureMoveEnum) 
        {
            case CreatureMoveEnum.Idle://���߱�
                moveVec = new Vector3(0, rigid.velocity.y, 0);
                rigid.velocity = moveVec;

                anim.SetBool("isIdle", true);
                break;
            case CreatureMoveEnum.Run://�޸���
                moveVec = new Vector3(0, rigid.velocity.y, 0) + Vector3.forward * runSpd;
                rigid.velocity = moveVec.normalized * runSpd;
                rigid.angularVelocity = Vector3.zero;

                anim.SetBool("isRun", true);
                break;
            case CreatureMoveEnum.LeftSpin:
                moveVec = transform.rotation.eulerAngles;
                // �������� ���� ȸ���մϴ� (���⼭�� y�� ���� �����մϴ�)
                moveVec.y -= rotSpd * Time.deltaTime;
                // ���ο� ȸ������ �����մϴ�
                transform.rotation = Quaternion.Euler(moveVec);

                anim.SetBool("isIdle", true);
                break;
            case CreatureMoveEnum.RightSpin:
                moveVec = transform.rotation.eulerAngles;
                // �������� ���� ȸ���մϴ� (���⼭�� y�� ���� �����մϴ�)
                moveVec.y += rotSpd * Time.deltaTime;
                // ���ο� ȸ������ �����մϴ�
                transform.rotation = Quaternion.Euler(moveVec);

                anim.SetBool("isIdle", true);
                break;
        }
    }
    #endregion

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Bomb"))//��ź�� �浹���� ��
        {
            //int damage = other.gameObject.GetComponent<Bomb>().bombDmg;
            //�ǰ� ó��
            //damageControlRPC(damage * 3);
        }
    }

    #region �ǰ� ó��
    public void damageControlRPC(float _dmg)
    {
        if (isDead)
            return;

        //���ط� ���
        curHealth -= _dmg;
        if (curHealth <= 0) curHealth = 0;
        else if (curHealth > maxHealth) curHealth = maxHealth;
        //UI����
        miniHealth.fillAmount = curHealth / maxHealth;

        //��� �ʱ�ȭ
        if (curHealth > 0)//�ǰ�
        {
               
        }
        else if (curHealth <= 0) Dead();
    }
    #endregion

    #region ���ó��
    void Dead()
    {
        //if (!isML)
        {
            //��� ó��
            isDead = true;
            //curCreatureMove = CreatureMoveEnum.Idle;
            
            //�̴� UI �ݱ�
            miniHealth.fillAmount = 0;
            //���� ����
            
            //�� ����
            Invoke("SoonDie", 1.5f);
        }
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
    IEnumerator Dissolve(bool b)//�ְ��� 1.5�ʰ�
    {
        //���̾� ����

        float firstValue = b ? 0f : 1f;      //true�� ���� �Ⱥ��̴� ��
        float targetValue = b ? 1f : 0f;     //false�� ���� ���̴� ��

        float duration = 1.5f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            if (curHealth <= 0 && !b) break;
            float progress = elapsedTime / duration;
            float value = Mathf.Lerp(firstValue, targetValue, progress);
            elapsedTime += Time.deltaTime;

            skinnedMeshRenderer.material.SetFloat("_AlphaFloat", value);
            yield return null;
        }
        skinnedMeshRenderer.material.SetFloat("_AlphaFloat", targetValue);

        //���̾� ����

    }
    #endregion
}
