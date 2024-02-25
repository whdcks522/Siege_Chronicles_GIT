using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Creature;

public class TowerManager : MonoBehaviour
{
    [Header("Ÿ���� �ִ� ü��")]
    public float maxHealth;
    [Header("Ÿ��ü�� ���� ü��")]
    public float curHealth;

    public TeamEnum curTeamEnum;

    [Header("Ÿ���� �Ʊ� ���� ��ġ")]
    public Transform creatureStartPoint;
    [Header("Ÿ���� ĳ�� ��ġ")]
    public Transform bulletStartPoint;

    [Header("ĳ���� ���� �̴� UI")]
    public GameObject miniCanvas;
    public Image miniHealth;

    [Header("�Ŵ���")]
    public GameManager gameManager;
    UIManager UIManager;
    Transform cameraGround;
    Transform mainCamera;
    private void Awake()
    {
        UIManager = gameManager.uiManager;
        mainCamera = UIManager.cameraObj;
        cameraGround = UIManager.cameraGround;


        if (curTeamEnum == TeamEnum.Blue)
            miniHealth.color = Color.blue;
        else if (curTeamEnum == TeamEnum.Red)
            miniHealth.color = Color.red;
    }



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

            Bullet bullet = other.gameObject.GetComponent<Bullet>();
            if (bullet.curTeamEnum != curTeamEnum)//���� �ٸ� ���
            {
                //���ط� Ȯ��
                ParentAgent bulletParentAgent = bullet.bulletHost;
                float damage = bullet.bulletDamage;

                //���� ����
                bulletParentAgent.AddReward(damage / 10f);
                //���� ����
                damageControl(damage);

                //�ǰ��� �Ѿ� ��ó��
                if (bullet.curBulletMoveEnum != Bullet.BulletMoveEnum.Slash)
                    bullet.BulletOff();
            }
        }
    }


    void damageControl(float _dmg)
    {
        //���ط� ���
        Debug.Log(_dmg);
        curHealth -= _dmg;
        if (curHealth < 0) curHealth = 0;
        else if (curHealth > maxHealth) curHealth = maxHealth;

        //UI����
        miniHealth.fillAmount = curHealth / maxHealth;

        //��� �ʱ�ȭ
        if (curHealth > 0)//�ǰ��ϰ� ��� ����
        {
            //anim.SetTrigger("isHit");
        }
        else if (curHealth <= 0) Dead();
    }

    void Dead() 
    {
        if(gameManager.isML)
            Revive();
    }

    void Revive() 
    {
        curHealth = maxHealth;
        miniHealth.fillAmount = 1;
    }
}
