using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("�Ѿ��� �ִ� ����")]
    public float maxTime;
    float curTime = 0f;

    [Header("�Ѿ��� ���ط�")]
    public int bulletDamage;

    [Header("�Ѿ��� �ӵ�")]
    public float bulletSpeed;

    [Header("�Ѿ��� ����")]
    public ParentAgent bulletHost;

    public enum BulletMoveEnum
    {
       Tracer, Canon
    }
    [Header("�Ѿ��� �̵�")]
    public BulletMoveEnum curBulletMoveEnum;

    [Header("��Ʈ ����Ʈ")]
    public string hitStr;

    public GameManager gameManager;
    ObjectManager objectManager;
    Rigidbody rigid;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        if (curBulletMoveEnum == BulletMoveEnum.Canon) 
            rigid.useGravity = true;
    }

    private void Update()
    {
        curTime += Time.deltaTime;
        if (curTime > maxTime)
        {
            bulletOff();
        }
    }

    #region �Ѿ� Ȱ�� ����ȭ
    public void bulletOnRPC()
    {
        //���ӿ�����Ʈ Ȱ��ȭ
        gameObject.SetActive(true);
        //ȸ�� �ʱ�ȭ
        //transform.rotation = Quaternion.identity;
        //�ð� ����ȭ
        curTime = 0f;
    }
    #endregion

    #region �Ѿ� ��Ȱ�� ����ȭ
    public void bulletOff()//Į�� �¾Ұų�, �ڿ��Ҹ��߰ų�
    {

        //���ӿ�����Ʈ ��Ȱ��ȭ
        gameObject.SetActive(false);
        //-1: �÷��̾ �Ѿ˿� ���� ���
        // 0: �ڿ� �Ҹ��� ���
        //+1: �÷��̾ Į�� �Ѿ��� �ı��� ���
        /*
        if (isHit)//���� ����Ʈ ���
        {
            GameObject hit = gameManager.CreateObj(hitStr, GameManager.PoolTypes.BulletType);
            Effect hitEffect = hit.GetComponent<Effect>();

            if (PhotonNetwork.InRoom)
            {
                if (PhotonNetwork.IsMasterClient)
                    hitEffect.photonView.RPC("effectOnRPC", RpcTarget.AllBuffered, transform.position);
            }
            else if (!PhotonNetwork.InRoom)
            {
                hitEffect.effectOnRPC(transform.position);
            }
        }
        */

    }
    #endregion

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.CompareTag("Outline")) //�� ������ �������� ����
        {
            bulletOff();
        }
    }
}