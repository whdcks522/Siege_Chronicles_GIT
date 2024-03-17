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


    [Header("전투 UI")]
    public Slider PlayerResourceSlider;//자원 슬라이더
    public Text PlayerResourceText;//자원 텍스트

    //맵을 클릭해서 무기를 사용할 준비 완료 여부
    bool isFocus;

    //최근에 사용한 스펠의 데이터
    SpellData recentSpellData;

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

        //자원 게이지 관리
        PlayerResourceSlider.value = blueTowerManager.curTowerResource / blueTowerManager.maxTowerResource;
        PlayerResourceText.text = blueTowerManager.curTowerResource.ToString("F1") + "/" + blueTowerManager.maxTowerResource.ToString("F0");

        //스펠 비율 보여주기
        for (int i = 0; i < spellBtnArr.Length; i++)
        {
            if (spellBtnArr[i].spellData != null)
            {
                spellBtnArr[i].spellBtnIcon.fillAmount = blueTowerManager.curTowerResource / spellBtnArr[i].spellData.spellValue;
            }
        }

        //스킬 범위 이동
        if(isFocus)
            ShowWeaponArea();
    }

    #region 전투 씬에서 스펠 버튼 클릭

    [Header("전투 스펠버튼 배열")]
    public SpellButton[] spellBtnArr = new SpellButton[4];
    public void OnClick(int _index) 
    {
        SpellData spellData = spellBtnArr[_index].GetComponent<SpellButton>().spellData;

        //비용
        int value = spellData.spellValue;

        if (blueTowerManager.curTowerResource >= value)//비용이 충분한 경우
        {
            blueTowerManager.curTowerResource -= value;

            if (spellData.spellType == SpellData.SpellType.Creature)//크리쳐를 누른 경우
            {
                blueTowerManager.SpawnCreature(spellData.spellPrefab.name);
            }
            else if(spellData.spellType == SpellData.SpellType.Weapon)//무기를 누른 경우
            {
                if (spellData.isFocus) 
                {
                    recentSpellData = spellData;
                    FocusControl(true, true);
                }
                else if(!spellData.isFocus)
                {
                    blueTowerManager.WeaponSort(spellData.spellPrefab.name);
                } 
            }
        }
        else if (blueTowerManager.curTowerResource < value) //비용이 모자른 경우
        {
            //비용 부족 효과음
        }
    }
    #endregion

    #region 포커스 관리

    //포커스 상태 전환
    public void FocusControl(bool _isFocus, bool _isUse) //포커스 했는지 여부, 사용한건지 취소한건지 여부
    {
        if (!_isFocus && !_isUse) //포커스를 취소한 경우
        {
            //자원 반환
            blueTowerManager.curTowerResource += recentSpellData.spellValue;
        }

        //ui 비활성화
        isFocus = _isFocus;
        //영역 관리
        clickPoint.gameObject.SetActive(_isFocus);

        //버튼 비활성화
        for (int i = 0; i < spellBtnArr.Length; i++)
        {
            spellBtnArr[i].gameObject.SetActive(!_isFocus);
        }
    }
    #endregion

    #region 무기 영역 표시;

    public Transform clickPoint;//클릭한 곳

    void ShowWeaponArea()
    {
        int layerMask = LayerMask.GetMask("MainMap"); // "Default" 레이어와 충돌하도록 설정

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask, QueryTriggerInteraction.Ignore))
            // 트리거는 무시한다
            clickPoint.position = hit.point;

        //타워 레이더 조작
        blueTowerManager.RadarControl(clickPoint.position);

        if (Input.GetMouseButton(0))//좌클릭 spellData.spellType == SpellData.SpellType.Weapon
        {
            blueTowerManager.WeaponSort(recentSpellData.spellPrefab.name);
            FocusControl(false, true);
        }
        else if (Input.GetMouseButton(1))//좌클릭 spellData.spellType == SpellData.SpellType.Weapon
        {
            FocusControl(false, false);
        }
    }
    
    #endregion

}
