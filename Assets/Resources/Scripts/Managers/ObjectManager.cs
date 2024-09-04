using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    [Header("�Ѿ��� ������ ����")]
    public Transform bulletFolder;

    [Header("������ ��Ʈ ������ ����")]
    public Transform damageFontFolder;

    [Header("�߸� ũ���� ������ ����")]
    public Transform grayCreatureFolder;
    [Header("����� ũ���ĸ� ������ ����")]
    public Transform blueCreatureFolder;
    [Header("������ ũ���ĸ� ������ ����")]
    public Transform redCreatureFolder;

    //�� ����Ʈ
    readonly string[] creatureNames = { "Infantry", "Shooter", "Shielder", "Accountant" };
    //�� �ּҰ� ����� ��
    List<GameObject>[] creaturePools;

    //�Ѿ� ����Ʈ
    readonly string[] bulletNames = { "Infantry_Effect", "Shooter_Tracer", "Shooter_Tracer_Effect", "Shielder_Effect", "Accountant_Tracer", "Accountant_Tracer_Effect",
        "Tower_Gun", "Tower_Gun_Effect", "Tower_Flame", "Tower_Flame_Effect","Tower_GrandCure", "Tower_CorpseExplosion"};
    //�Ѿ� �ּҰ� ����� ��
    List<GameObject>[] bulletPools;

    //������ ��Ʈ ����Ʈ
    readonly string[] damageFontNames = { "BlueDamageFont", "RedDamageFont", "PinkDamageFont" };
    //������ ��Ʈ�� ����� ��
    List<GameObject>[] damageFontPools;


    public enum PoolTypes
    {
        CreaturePool, BulletPool, DamageFontPool
    }

    private void Awake()
    {
        //�� Ǯ �ʱ�ȭ(4���� ����)
        creaturePools = new List<GameObject>[creatureNames.Length];
        for (int index = 0; index < creatureNames.Length; index++)//Ǯ �ϳ��ϳ� �ʱ�ȭ
            creaturePools[index] = new List<GameObject>();

        //�Ѿ� Ǯ �ʱ�ȭ
        bulletPools = new List<GameObject>[bulletNames.Length];
        for (int index = 0; index < bulletNames.Length; index++)//Ǯ �ϳ��ϳ� �ʱ�ȭ
            bulletPools[index] = new List<GameObject>();

        //�Ѿ� Ǯ �ʱ�ȭ
        damageFontPools = new List<GameObject>[damageFontNames.Length];
        for (int index = 0; index < damageFontNames.Length; index++)//Ǯ �ϳ��ϳ� �ʱ�ȭ
            damageFontPools[index] = new List<GameObject>();
    }

    #region ���� ������Ʈ ��ȯ
    
    GameObject tmpGameObject = null;//���� ������Ʈ�� ���� �ӽ� ��ü
    string[] tmpNames = null;//���ӿ�����Ʈ ���
    List<GameObject>[] tmpPools;//���ӿ�����Ʈ �� ����Ʈ
    string path = "";//������ ���
    readonly string bulletPath = "Bullet/";
    readonly string creaturePath = "Creature/";
    readonly string damageFontPath = "DamageFont/";

    public GameObject CreateObj(string _name, PoolTypes poolTypes) //������ �� �θ���, ������ ����
    {
        //�ݵ�� �Ź� �ʱ�ȭ
        tmpGameObject = null;
        tmpNames = null;
        tmpPools = null;
        
        switch (poolTypes)//-------���� �ʿ��� �κ� 1
        {
            case PoolTypes.CreaturePool:
                tmpPools = creaturePools;
                tmpNames = creatureNames;//awake���� �����ߴϴ�
                break;
            case PoolTypes.BulletPool:
                tmpPools = bulletPools;
                tmpNames = bulletNames;
                break;
            case PoolTypes.DamageFontPool:
                tmpPools = damageFontPools;
                tmpNames = damageFontNames;
                break;

        }

        int index = NametoIndex(tmpNames, _name);//�̸��� ��ȣ��
        foreach (GameObject item in tmpPools[index])//�ִٸ� ã�ƺ�
        {
            if (!item.activeSelf)
            {
                tmpGameObject = item;
                break;
            }
        }

        //������ ����
        if (!tmpGameObject)
        {
            switch (poolTypes)//--------���� �ʿ��� �κ� 2
            {
                case PoolTypes.BulletPool:
                    path = bulletPath + tmpNames[index]; // ���� �������� �����Ͽ� ��� ����
                    tmpGameObject = Instantiate(Resources.Load<GameObject>(path), Vector3.zero, Quaternion.identity);
                    tmpGameObject.transform.parent = bulletFolder;
                    break;
                case PoolTypes.CreaturePool:
                    path = creaturePath + tmpNames[index]; // ���� �������� �����Ͽ� ��� ����
                    tmpGameObject = Instantiate(Resources.Load<GameObject>(path), Vector3.zero, Quaternion.identity);
                    tmpGameObject.transform.parent = grayCreatureFolder;
                    break;
                case PoolTypes.DamageFontPool:
                    path = damageFontPath + tmpNames[index]; // ���� �������� �����Ͽ� ��� ����
                    tmpGameObject = Instantiate(Resources.Load<GameObject>(path), Vector3.zero, Quaternion.identity);
                    tmpGameObject.transform.parent = damageFontFolder;
                    break;
            }

            //�ӽ� ����Ʈ�� ���ϱ�
            tmpPools[index].Add(tmpGameObject);

            //����ȭ
            switch (poolTypes)//------------------���� �ʿ��� �κ� 3
            {
                case PoolTypes.CreaturePool:
                    creaturePools = tmpPools;
                    break;
                case PoolTypes.BulletPool:
                    bulletPools = tmpPools;
                    break;
                case PoolTypes.DamageFontPool:
                    damageFontPools = tmpPools;
                    break;
            }
        }
        return tmpGameObject;
    }
    #endregion

    #region ������Ʈ Ǯ������ ���� ���
    int NametoIndex(string[] tmpNames, string _name)
    {
        for (int i = 0; i < tmpNames.Length; i++)
        {
            //Debug.Log(tmpNames[i]);
            if (string.Equals(tmpNames[i], _name))
            {
                return i;
            }
        }
        Debug.Log("Error: -1");
        return -1;
    }
    #endregion
}
