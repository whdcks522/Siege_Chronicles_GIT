using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static ObjectManager;

public class UIManager : MonoBehaviour
{
    [Header("카메라 객체")]
    public Transform cameraObj;
    [Header("카메라 위치")]
    public Transform cameraCloud;
    [Header("카메라가 바라보는 지점")]
    public Transform cameraGround;

    Transform blueTower;//파란 성의 생성 지점
    TowerManager blueTowerManager;//파란 성의 스크립트

    Transform redTower;//빨간 성의 생성 지점

    int mul = 45;//카메라 회전 속도
    int curRot = -160;//현재 회전값
    int addRot = 0;//버튼으로 회전할 때 사용하는 논리값
    Vector3 cameraVec;//카메라 회전용 벡터

    int fly = 50;//카메라를 하늘에서 띄운 정도

   
    public Slider PlayerResourceSlider;



    [Header("매니저")]
    public SelectManager selectManager;
    public GameManager gameManager;
    ObjectManager objectManager;


    private void Awake()
    {
        objectManager = gameManager.objectManager;
        blueTower = gameManager.blueTower;
        blueTowerManager = blueTower.GetComponent<TowerManager>();  
        redTower = gameManager.redTower;

        //시작 할 때, 카메라 위치 고정
        cameraGround.transform.position = (blueTower.position + redTower.transform.position) / 2f;
        cameraCloud.transform.position = Vector3.up * fly + cameraGround.position;
    }

    #region 버튼으로 카메라 조작
    public void CameraSpin(int _spin) => addRot = _spin;

    int speed = 0;
    public Text SpeedControlText;
    public void SpeedControl()
    {
        speed++;
        speed = (speed % 3);

        Time.timeScale = (speed + 1);

        SpeedControlText.text = "x" + (speed + 1);
    }
    #endregion

    private void Update()
    {
        //사이드 화살표 버튼 누르면 카메라 회전
        curRot += addRot * 2;

        //카메라 위치 관리(위에서 퍼진 정도)
        cameraVec = mul * new Vector3(Mathf.Sin(Mathf.PI * curRot / 360), 0, Mathf.Cos(Mathf.PI * curRot / 360));
        cameraObj.position = cameraCloud.position + cameraVec;

        //카메라가 향하도록 관리
        cameraObj.LookAt((blueTower.position + redTower.position) / 2f);

        //스킬 범위 이동
        //MapClick();

        //자원 게이지 관리
        PlayerResourceSlider.value = blueTowerManager.curTowerResource / blueTowerManager.maxTowerResource;
        //스펠 비율 보여주기
        for (int i = 0; i < spellBtnArr.Length; i++)
        {
            if (spellBtnArr[i].spellData != null)
            {
                spellBtnArr[i].spellBtnIcon.fillAmount = blueTowerManager.curTowerResource / spellBtnArr[i].spellData.spellValue;
            }
        }

    }

    public void OnClick(int _index) 
    {
        SpellData spellData = spellBtnArr[_index].GetComponent<SpellButton>().spellData;

        //비용
        int value = spellData.spellValue;

        if (blueTowerManager.curTowerResource >= value)
        {
            blueTowerManager.curTowerResource -= value;

            if (spellData.spellType == SpellData.SpellType.Creature)
            {
                blueTowerManager.SpawnCreature(spellData.spellPrefab.name);
            }
        }
        else if (blueTowerManager.curTowerResource < value) //값이 모자람
        {
            //실패 효과음
        }

    }

    #region 맵 클릭하기
    [Header("클릭한 곳")]
    public Transform clickPoint;

    [Header("전투 스펠버튼 배열")]
    public SpellButton[] spellBtnArr = new SpellButton[4];
    /*
    void MapClick()
    {
        int layerMask = LayerMask.GetMask("MainMap"); // "Default" 레이어와 충돌하도록 설정

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask, QueryTriggerInteraction.Ignore))
            // 트리거는 무시한다
            clickPoint.position = hit.point;

        //타워 레이더 조작
        blueTowerManager.RadarControl(clickPoint);

        if (Input.GetMouseButton(0) && curPlayerResource >= spellData.spellValue)//좌클릭 spellData.spellType == SpellData.SpellType.Weapon
        {
            //무기 비용 처리
            curPlayerResource -= spellData.spellValue;

            //무기별 함수 실행
            switch (spellData.spellPrefab.name) 
            {
                case "Tower_Gun":
                    MapClick_Gun();
                    break;
                case "Tower_Flame":
                    MapClick_Flame();
                    break;
                default:
                    Debug.LogError("아무것도 없음");
                    break;
            }
        }
    }
    */
    #endregion

    #region 미니건
    void MapClick_Gun()
    {
        Debug.Log("Gun");

        return;

        GameObject bullet = objectManager.CreateObj("Tower_Gun", ObjectManager.PoolTypes.BulletPool);
        Bullet bullet_bullet = bullet.GetComponent<Bullet>();
        Rigidbody bullet_rigid = bullet.GetComponent<Rigidbody>();

        bullet_bullet.gameManager = gameManager;
        bullet_bullet.Init();


        //이동
        bullet.transform.position = cameraCloud.position;
        //가속
        bullet_rigid.velocity = (clickPoint.position - bullet.transform.position).normalized * bullet_bullet.bulletSpeed;

        //활성화
        bullet_bullet.BulletOnByTower(Creature.TeamEnum.Blue);
    }
    #endregion

    #region 파이어볼
    void MapClick_Flame() 
    {
        GameObject bullet = objectManager.CreateObj("Tower_Flame", ObjectManager.PoolTypes.BulletPool);
        Bullet bullet_bullet = bullet.GetComponent<Bullet>();
        Rigidbody bullet_rigid = bullet.GetComponent<Rigidbody>();

        bullet_bullet.gameManager = gameManager;
        bullet_bullet.Init();


        //이동
        bullet.transform.position = cameraCloud.position;
        //가속
        bullet_rigid.velocity = (clickPoint.position - bullet.transform.position).normalized * bullet_bullet.bulletSpeed;

        //활성화
        bullet_bullet.BulletOnByTower(Creature.TeamEnum.Blue);
    }
    #endregion

}
