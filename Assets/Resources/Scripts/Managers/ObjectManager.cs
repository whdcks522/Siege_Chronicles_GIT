using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{

    [Header("�Ѿ��� ������ ����")]
    public Transform bulletFolder;
    [Header("����Ʈ�� ������ ����")]
    public Transform effectFolder;

    [Header("����� ���� ������ ����")]
    public Transform blueCreatureFolder;
    [Header("������ ���� ������ ����")]
    public Transform redCreatureFolder;


    //�Ѿ� ����Ʈ
    string[] bulletNames = { "Infantry_A_Slash" };
    //�Ѿ� �ּҰ� ����� ��
    List<GameObject>[] bulletPools;
    

    //����Ʈ ����Ʈ
    string[] effectNames = { "Explosion 2", "Explosion 3", "Explosion 6", "Explosion 2_Cure", "Explosion 2_PowerUp",
                                "Text 52_Creature", "Text 52_Player", "congratulation 9"};
    //����Ʈ �ּҰ� ����� ��
    List<GameObject>[] effectPools;
    

    //�� ����Ʈ
    string[] creatureNames = { "Infantry_A", "shooter_A"};
    //�� �ּҰ� ����� ��
    List<GameObject>[] creaturePools;

    public enum PoolTypes
    {
        BulletPool, EffectPool, CreaturePool
    }

    private void Awake()
    {
        //�Ѿ� Ǯ �ʱ�ȭ
        bulletPools = new List<GameObject>[bulletNames.Length];
        for (int index = 0; index < bulletNames.Length; index++)//Ǯ �ϳ��ϳ� �ʱ�ȭ
            bulletPools[index] = new List<GameObject>();

        //����Ʈ Ǯ �ʱ�ȭ(4���� ����)
        effectPools = new List<GameObject>[effectNames.Length];
        for (int index = 0; index < effectNames.Length; index++)//Ǯ �ϳ��ϳ� �ʱ�ȭ
            effectPools[index] = new List<GameObject>();

        //�� Ǯ �ʱ�ȭ(4���� ����)
        creaturePools = new List<GameObject>[creatureNames.Length];
        for (int index = 0; index < creatureNames.Length; index++)//Ǯ �ϳ��ϳ� �ʱ�ȭ
            creaturePools[index] = new List<GameObject>();
    }

    public GameObject CreateObj(string _name, PoolTypes poolTypes) //������ �� �θ���, ������ ����
    {
        //�ݵ�� �Ź� �ʱ�ȭ
        GameObject tmpGameObject = null;

        List<GameObject>[] tmpPools = null;
        string[] tmpNames = null;

        switch (poolTypes)
        {
            case PoolTypes.BulletPool:
                tmpPools = bulletPools;
                tmpNames = bulletNames;
                break;
            case PoolTypes.EffectPool:
                tmpPools = effectPools;
                tmpNames = effectNames;//awake���� �����ߴ�
                break;
            case PoolTypes.CreaturePool:
                tmpPools = creaturePools;
                tmpNames = creatureNames;//awake���� �����ߴ�
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

        //������ �����ϰ� select�� �Ҵ�
        if (!tmpGameObject)
        {
            //tmpGameObject = Instantiate(Resources.Load<GameObject>(tmpNames[index]), Vector3.zero, Quaternion.identity);

            string path = "";

            switch (poolTypes)
            {
                case PoolTypes.BulletPool:
                    path = "Bullet/" + tmpNames[index]; // ���� �������� �����Ͽ� ��� ����
                    tmpGameObject = Instantiate(Resources.Load<GameObject>(path), Vector3.zero, Quaternion.identity);
                    tmpGameObject.transform.parent = bulletFolder;
                    break;
                case PoolTypes.EffectPool:
                    
                    break;
                case PoolTypes.CreaturePool:
                    path = "Creature/" + tmpNames[index]; // ���� �������� �����Ͽ� ��� ����
                    tmpGameObject = Instantiate(Resources.Load<GameObject>(path), Vector3.zero, Quaternion.identity);
                    break;
            }

            //�ӽ� ����Ʈ�� ���ϱ�
            tmpPools[index].Add(tmpGameObject);

            //����ȭ
            switch (poolTypes)
            {
                case PoolTypes.BulletPool:
                    bulletPools = tmpPools;
                    break;
                case PoolTypes.EffectPool:
                    effectPools = tmpPools;
                    break;
                case PoolTypes.CreaturePool:
                    creaturePools = tmpPools;
                    break;
            }
        }
        return tmpGameObject;
    }

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
