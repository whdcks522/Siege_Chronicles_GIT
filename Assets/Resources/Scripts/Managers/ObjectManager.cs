using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    [Header("총알을 저장할 폴더")]
    public Transform bulletFolder;

    [Header("데미지 폰트 저장할 폴더")]
    public Transform damageFontFolder;

    [Header("중립 크리쳐 저장할 폴더")]
    public Transform grayCreatureFolder;
    [Header("블루팀 크리쳐를 저장할 폴더")]
    public Transform blueCreatureFolder;
    [Header("레드팀 크리쳐를 저장할 폴더")]
    public Transform redCreatureFolder;

    //적 리스트
    string[] creatureNames = { "Infantry", "Shooter", "Shielder", "Accountant" };
    //적 주소가 저장될 곳
    List<GameObject>[] creaturePools;

    //총알 리스트
    string[] bulletNames = { "Infantry_Effect", "Shooter_Tracer", "Shooter_Tracer_Effect", "Shielder_Effect", "Accountant_Tracer", "Accountant_Tracer_Effect",
        "Tower_Gun", "Tower_Gun_Effect", "Tower_Flame", "Tower_Flame_Effect","Tower_GrandCure", "Tower_CorpseExplosion"};
    //총알 주소가 저장될 곳
    List<GameObject>[] bulletPools;

    //데미지 폰트 리스트
    string[] damageFontNames = { "BlueDamageFont", "RedDamageFont", "PinkDamageFont" };
    //데미지 폰트가 저장될 곳
    List<GameObject>[] damageFontPools;


    public enum PoolTypes
    {
        CreaturePool, BulletPool, DamageFontPool
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

        //총알 풀 초기화
        damageFontPools = new List<GameObject>[damageFontNames.Length];
        for (int index = 0; index < damageFontNames.Length; index++)//풀 하나하나 초기화
            damageFontPools[index] = new List<GameObject>();
    }

    #region 게임 오브젝트 반환
    
    GameObject tmpGameObject = null;//게임 오브젝트를 담을 임시 객체
    string[] tmpNames = null;//게임오브젝트 목록
    List<GameObject>[] tmpPools;//게임오브젝트 별 리스트
    string path = "";//폴더의 경로

    public GameObject CreateObj(string _name, PoolTypes poolTypes) //있으면 적 부르고, 없으면 생성
    {
        //반드시 매번 초기화
        tmpGameObject = null;
        tmpNames = null;
        tmpPools = null;
        
        switch (poolTypes)//-------수정 필요한 부분 1
        {
            case PoolTypes.CreaturePool:
                tmpPools = creaturePools;
                tmpNames = creatureNames;//awake에서 선언했니다
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

        int index = NametoIndex(tmpNames, _name);//이름을 번호로
        foreach (GameObject item in tmpPools[index])//있다면 찾아봄
        {
            if (!item.activeSelf)
            {
                tmpGameObject = item;
                break;
            }
        }

        //없으면 생성
        if (!tmpGameObject)
        {
            switch (poolTypes)//--------수정 필요한 부분 2
            {
                case PoolTypes.BulletPool:
                    path = "Bullet/" + tmpNames[index]; // 서브 폴더명을 포함하여 경로 설정
                    tmpGameObject = Instantiate(Resources.Load<GameObject>(path), Vector3.zero, Quaternion.identity);
                    tmpGameObject.transform.parent = bulletFolder;
                    break;
                case PoolTypes.CreaturePool:
                    path = "Creature/" + tmpNames[index]; // 서브 폴더명을 포함하여 경로 설정
                    tmpGameObject = Instantiate(Resources.Load<GameObject>(path), Vector3.zero, Quaternion.identity);
                    tmpGameObject.transform.parent = grayCreatureFolder;
                    break;
                case PoolTypes.DamageFontPool:
                    path = "DamageFont/" + tmpNames[index]; // 서브 폴더명을 포함하여 경로 설정
                    tmpGameObject = Instantiate(Resources.Load<GameObject>(path), Vector3.zero, Quaternion.identity);
                    tmpGameObject.transform.parent = damageFontFolder;
                    break;
            }

            //임시 리스트에 더하기
            tmpPools[index].Add(tmpGameObject);

            //동기화
            switch (poolTypes)//------------------수정 필요한 부분 3
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
