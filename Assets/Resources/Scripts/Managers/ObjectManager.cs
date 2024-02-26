using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{

    [Header("총알을 저장할 폴더")]
    public Transform bulletFolder;


    [Header("블루팀 적을 저장할 폴더")]
    public Transform blueCreatureFolder;
    [Header("레드팀 적를 저장할 폴더")]
    public Transform redCreatureFolder;

    //적 리스트
    string[] creatureNames = { "Infantry_A", "Shooter_A" };
    //적 주소가 저장될 곳
    List<GameObject>[] creaturePools;

    //총알 리스트
    string[] bulletNames = { "Infantry_A_Slash", "Shooter_A_Tracer", "Shooter_A_Bomb" };
    //총알 주소가 저장될 곳
    List<GameObject>[] bulletPools;
    
    

    public enum PoolTypes
    {
        BulletPool, EffectPool, CreaturePool
    }

    private void Awake()
    {
        //적 풀 초기화(4개씩 수정)
        creaturePools = new List<GameObject>[creatureNames.Length];
        for (int index = 0; index < creatureNames.Length; index++)//풀 하나하나 초기화
            creaturePools[index] = new List<GameObject>();

        //총알 풀 초기화
        bulletPools = new List<GameObject>[bulletNames.Length];
        for (int index = 0; index < bulletNames.Length; index++)//풀 하나하나 초기화
            bulletPools[index] = new List<GameObject>();

        
    }

    public GameObject CreateObj(string _name, PoolTypes poolTypes) //있으면 적 부르고, 없으면 생성
    {
        //반드시 매번 초기화
        GameObject tmpGameObject = null;

        List<GameObject>[] tmpPools = null;
        string[] tmpNames = null;

        switch (poolTypes)
        {
            case PoolTypes.CreaturePool:
                tmpPools = creaturePools;
                tmpNames = creatureNames;//awake에서 선언했니
                break;
            case PoolTypes.BulletPool:
                tmpPools = bulletPools;
                tmpNames = bulletNames;
                break;
            
        }

        int index = NametoIndex(tmpNames, _name);//이름을 번호로
        foreach (GameObject item in tmpPools[index])//있다면 찾아봄
        {
            if (!item.activeSelf)
            {
                tmpGameObject = item;
                break;
            }
        }

        //없으면 생성하고 select에 할당
        if (!tmpGameObject)
        {
            string path = "";

            switch (poolTypes)
            {
                case PoolTypes.BulletPool:
                    path = "Bullet/" + tmpNames[index]; // 서브 폴더명을 포함하여 경로 설정
                    tmpGameObject = Instantiate(Resources.Load<GameObject>(path), Vector3.zero, Quaternion.identity);
                    tmpGameObject.transform.parent = bulletFolder;
                    break;
                case PoolTypes.CreaturePool:
                    path = "Creature/" + tmpNames[index]; // 서브 폴더명을 포함하여 경로 설정
                    tmpGameObject = Instantiate(Resources.Load<GameObject>(path), Vector3.zero, Quaternion.identity);
                    break;
            }

            //임시 리스트에 더하기
            tmpPools[index].Add(tmpGameObject);

            //동기화
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

    #region 오브젝트 풀링에서 순서 출력
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
