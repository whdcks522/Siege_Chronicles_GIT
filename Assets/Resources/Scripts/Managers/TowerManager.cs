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

    [Header("����� ��")]
    public Transform target;
    [Header("���� ������ �ִ� �Ÿ�")]
    public float maxRange;
    [Header("���� ������ �Ÿ�")]
    public float curRange;

    public TeamEnum curTeamEnum;
    [Header("�츮 ��")]
    public Transform ourTower;
    [Header("��� ��")]
    public Transform enemyTower;

    [Header("�Ѿ��� ���۵Ǵ� ��")]
    public Transform bulletStartPoint;

    [Header("ĳ���� ���� �̴� UI")]
    public GameObject miniUI;
    public Image miniHealth;

    Transform mainCamera;
    GameManager gameManager;
    private void Awake()
    {
        mainCamera = gameManager.uiManager.cameraObj;
    }

    private void LateUpdate()
    {
        miniUI.transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward,
            mainCamera.transform.rotation * Vector3.up);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Bullet"))//��ź�� �浹���� ��
        {
            Bullet bullet = other.GetComponent<Bullet>();
            if (bullet.curTeamEnum != curTeamEnum)//���� �ٸ� ���
            {
                //���ط� Ȯ��
                ParentAgent bulletParentAgent = bullet.bulletHost;
                float damage = bullet.bulletDamage;

                //���� ����
                bulletParentAgent.AddReward(damage / 10f);
                //���� ����
                damageControl(damage);
            }
        }
    }

    void damageControl(float _dmg)
    {
        //���ط� ���
        curHealth -= _dmg;
        if (curHealth < 0) curHealth = 0;
        else if (curHealth > maxHealth) curHealth = maxHealth;
        //UI����
        //miniHealth.fillAmount = curHealth / maxHealth;

        //��� �ʱ�ȭ
        if (curHealth > 0)//�ǰ��ϰ� ��� ����
        {
            //anim.SetTrigger("isHit");
        }
        else if (curHealth <= 0) Dead();
    }

    void Dead() 
    {
        
    }
}
