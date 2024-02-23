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

    [Header("ĳ���� ���� �̴� UI")]
    public GameObject miniUI;
    public Image miniHealth;

    public int spin, _inputY;

    Rigidbody rigid;

    //����ü�� ���� ���
    bool isDead = false;
    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }

    #region ����ü Ȱ��ȭ
    public void activateRPC()
    {
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

    #region �̵�
    public void xyRPC(int x, int y)
    {
        //_inputX = x;
        _inputY = y;
    }
    #endregion


}
