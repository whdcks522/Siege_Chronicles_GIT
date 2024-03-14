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

   

    //플레이어의 현재 자원
    float curPlayerResource = 0f;
    //플레이어의 최대 자원
    float maxPlayerResource = 10f;

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

    private void Start()
    {
        //생명체 생성
        //GameObject a = objectManager.CreateObj("Infantry_A", PoolTypes.CreaturePool);
        //a.transform.position = redCameraTarget.position;
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
        if (maxPlayerResource > curPlayerResource ) 
        {
            //스펠 사용을 위한 자원 증가
            curPlayerResource += Time.deltaTime;
        }
        else if (maxPlayerResource <= curPlayerResource)
        {
            //현재 자원량이 최대치를 넘지 않도록
            maxPlayerResource = curPlayerResource;
        }
        PlayerResourceSlider.value = curPlayerResource / maxPlayerResource;



        //사이드 화살표 버튼 누르면 카메라 회전
        curRot += addRot * 2;

        //카메라 위치 관리(위에서 퍼진 정도)
        cameraVec = mul * new Vector3(Mathf.Sin(Mathf.PI * curRot / 360), 0, Mathf.Cos(Mathf.PI * curRot / 360));
        cameraObj.position = cameraCloud.position + cameraVec;

        //카메라가 향하도록 관리
        cameraObj.LookAt((blueTower.position + redTower.position) / 2f);

        //스킬 범위 이동
        MapClick();
    }

    #region 맵 클릭하기
    [Header("클릭한 곳")]
    public Transform clickPoint;

    public SpellData spellData;
    
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
                case "Tower_Flame":
                    MapClick_Flame();
                    break;
                default:
                    Debug.LogError("아무것도 없음");
                    break;
            }
        }
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
