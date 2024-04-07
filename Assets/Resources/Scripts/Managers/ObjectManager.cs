using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{

    [Header("�Ѿ��� ������ ����")]
    public Transform bulletFolder;

    [Header("�߸� ũ���� ������ ����")]
    public Transform grayCreatureFolder;
    [Header("����� ũ���ĸ� ������ ����")]
    public Transform blueCreatureFolder;
    [Header("������ ũ���ĸ� ������ ����")]
    public Transform redCreatureFolder;

    //�� ����Ʈ
    string[] creatureNames = { "Infantry", "Shooter", "Shielder" };
    //�� �ּҰ� ����� ��
    List<GameObject>[] creaturePools;

    //�Ѿ� ����Ʈ
    string[] bulletNames = { "Infantry_Effect", "Shooter_Tracer", "Shooter_Tracer_Effect", "Shielder_Effect",
        "Tower_Gun", "Tower_Gun_Effect", "Tower_Flame", "Tower_Flame_Effect","Tower_GrandCure", "Tower_CorpseExplosion"};
    //�Ѿ� �ּҰ� ����� ��
    List<GameObject>[] bulletPools;
    

    public enum PoolTypes
    {
        BulletPool, CreaturePool
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
    }

    #region ���� ������Ʈ ��ȯ
    //���� ������Ʈ�� ���� �ӽ� ��ü
    GameObject tmpGameObject = null;
    //���ӿ�����Ʈ ���
    string[] tmpNames = null;
    //���ӿ�����Ʈ �� ����Ʈ
    List<GameObject>[] tmpPools;
    
    //������ ���
    string path = "";

    public GameObject CreateObj(string _name, PoolTypes poolTypes) //������ �� �θ���, ������ ����
    {
        //�ݵ�� �Ź� �ʱ�ȭ
        tmpGameObject = null;
        tmpNames = null;
        tmpPools = null;
        
        switch (poolTypes)
        {
            case PoolTypes.CreaturePool:
                tmpPools = creaturePools;
                tmpNames = creatureNames;//awake���� �����ߴϴ�
                break;
            case PoolTypes.BulletPool:
                tmpPools = bulletPools;
                tmpNames = bulletNames;
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
            switch (poolTypes)
            {
                case PoolTypes.BulletPool:
                    path = "Bullet/" + tmpNames[index]; // ���� �������� �����Ͽ� ��� ����
                    tmpGameObject = Instantiate(Resources.Load<GameObject>(path), Vector3.zero, Quaternion.identity);
                    tmpGameObject.transform.parent = bulletFolder;
                    break;
                case PoolTypes.CreaturePool:
                    path = "Creature/" + tmpNames[index]; // ���� �������� �����Ͽ� ��� ����
                    tmpGameObject = Instantiate(Resources.Load<GameObject>(path), Vector3.zero, Quaternion.identity);
                    tmpGameObject.transform.parent = grayCreatureFolder;
                    break;
            }

            //�ӽ� ����Ʈ�� ���ϱ�
            tmpPools[index].Add(tmpGameObject);

            //����ȭ
            switch (poolTypes)
            {
                case PoolTypes.CreaturePool:
                    creaturePools = tmpPools;
                    break;
                case PoolTypes.BulletPool:
                    bulletPools = tmpPools;
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
