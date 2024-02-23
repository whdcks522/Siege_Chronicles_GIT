using Google.Protobuf.WellKnownTypes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class Creature : MonoBehaviour
{
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
    Vector3 moveVec;//�̵��� ����

    Rigidbody rigid;
    Animator anim;

    public enum CreatureMove {Idle, Run, LeftSpin, RightSpin }
    public CreatureMove curCreatureMove;

    //����ü�� ���� ���
    bool isDead = false;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();

        //int index = System.Array.IndexOf(System.Enum.GetValues(typeof(CreatureMove)), curCreatureMove);
        //Debug.Log(index);
    }

    #region ����ü Ȱ��ȭ
    public void activateRPC()
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
    }
    #endregion

    public float ff;

    #region ���� ����
    private void FixedUpdate()
    {
        switch (curCreatureMove) 
        {
            case CreatureMove.Idle://���߱�
                moveVec = new Vector3(0, rigid.velocity.y, 0);
                rigid.velocity = moveVec;

                anim.SetBool("isIdle", true);
                break;
            case CreatureMove.Run://�޸���
                moveVec = new Vector3(0, rigid.velocity.y, 0) + Vector3.forward * runSpd;
                rigid.velocity = moveVec.normalized * runSpd;
                rigid.angularVelocity = Vector3.zero;

                anim.SetBool("isRun", true);
                break;
            case CreatureMove.LeftSpin:
                moveVec = transform.rotation.eulerAngles;
                // �������� ���� ȸ���մϴ� (���⼭�� y�� ���� �����մϴ�)
                moveVec.y -= ff * Time.deltaTime;
                // ���ο� ȸ������ �����մϴ�
                transform.rotation = Quaternion.Euler(moveVec);

                anim.SetBool("isIdle", true);
                break;
            case CreatureMove.RightSpin:
                moveVec = transform.rotation.eulerAngles;
                // �������� ���� ȸ���մϴ� (���⼭�� y�� ���� �����մϴ�)
                moveVec.y += ff * Time.deltaTime;
                // ���ο� ȸ������ �����մϴ�
                transform.rotation = Quaternion.Euler(moveVec);

                anim.SetBool("isIdle", true);
                break;
        }
    }
    #endregion


}
