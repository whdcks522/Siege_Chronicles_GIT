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

        //은행 텍스트 조작
        bankText.text = "Lv." + (blueTowerManager.curBankIndex + 1) + "(" + blueTowerManager.BankValueArr[blueTowerManager.curBankIndex] + ")";
    }

    //버튼으로 카메라 조작
    public void CameraSpin(int _spin) => addRot = _spin;

    #region 배속 조정

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

    #region 은행 관리
    [Header("은행 관련 UI")]
    public Image bankBtn;
    public Text bankText;
    public void BankControl() 
    {
        if (blueTowerManager.curTowerResource >= blueTowerManager.BankValueArr[blueTowerManager.curBankIndex])//비용이 충분한 경우
        {
            //처리 성공 효과음


            //비용 처리
            blueTowerManager.curTowerResource -= blueTowerManager.BankValueArr[blueTowerManager.curBankIndex];

            //은행 계수 증가
            blueTowerManager.curBankIndex++;

            if (blueTowerManager.curBankIndex != blueTowerManager.BankValueArr.Length)//0, 1, 2, 3일 때 클릭한 경우
            {
                //은행 텍스트 관리
                bankText.text = "Lv." + (blueTowerManager.curBankIndex + 1) + "(" + blueTowerManager.BankValueArr[blueTowerManager.curBankIndex] + ")";
            }
            else if (blueTowerManager.curBankIndex == blueTowerManager.BankValueArr.Length)//4일 때 클릭한 경우
            {
                //은행 텍스트 관리
                bankText.text = "Lv.5(-)";
                //이미지 채우기
                bankBtn.fillAmount = 1;
                //버튼 클릭 비활성화
                bankBtn.GetComponent<Button>().interactable = false;
            }   
        }
        else if (blueTowerManager.curTowerResource < blueTowerManager.BankValueArr[blueTowerManager.curBankIndex]) //비용이 모자른 경우
        {
            //비용 부족 효과음
        }
    }
    #endregion

    private void LateUpdate()
    {
        //사이드 화살표 버튼 누르면 카메라 회전
        curRot += addRot * 2;

        //카메라 위치 관리(위에서 퍼진 정도)
        cameraVec = mul * new Vector3(Mathf.Sin(Mathf.PI * curRot / 360), 0, Mathf.Cos(Mathf.PI * curRot / 360));
        cameraObj.position = cameraCloud.position + cameraVec;

        //카메라가 향하도록 관리
        cameraObj.LookAt((blueTower.position + redTower.position) / 2f);

        if (!gameManager.isML) 
        {
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
            if (isFocus)
                ShowWeaponArea();

            //은행 버튼 활성화 관리
            if(blueTowerManager.curBankIndex < blueTowerManager.BankValueArr.Length && bankBtn.GetComponent<Button>().interactable)
                bankBtn.fillAmount = blueTowerManager.curTowerResource / blueTowerManager.BankValueArr[blueTowerManager.curBankIndex];

                //크리쳐 수 제한 표시
                creatureCountText.text = blueTowerManager.curCreatureCount.ToString() + "/" + blueTowerManager.maxCreatureCount.ToString();
        }
    }
    [Header("크리쳐 제한 텍스트")]
    public Text creatureCountText;

    #region 전투 씬에서 스펠 버튼 클릭

    [Header("전투 스펠버튼 배열")]
    public SpellButton[] spellBtnArr = new SpellButton[4];
    public void OnClick(int _index) 
    {
        SpellData spellData = spellBtnArr[_index].GetComponent<SpellButton>().spellData;

        //비용
        int value = spellData.spellValue;

        if (blueTowerManager.curTowerResource >= value && !isFocus)//비용이 충분한 경우
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
                    //무기 데이터 임시 저장
                    recentSpellData = spellData;

                    //클릭 포인트의 매터리얼 변화
                    clickMat.SetColor("_AlphaColor", spellData.focusColor);

                    //포커스 활성화
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

        if (_isFocus)
        {
            focusLeftText.text = "스펠 '" + recentSpellData.spellName + "' 사용";
            focusRightText.text = "자원 '" + recentSpellData.spellValue + "' 반환";

            //버튼 클릭 비활성화
            for (int i = 0; i < spellBtnArr.Length; i++)
            {
                spellBtnArr[i].GetComponent<Button>().interactable = false;
            }
        }
        else if (!_isFocus)
        {
            //버튼 클릭 활성화
            for (int i = 0; i < spellBtnArr.Length; i++)
            {
                if (spellBtnArr[i].spellData != null)
                    spellBtnArr[i].GetComponent<Button>().interactable = true;
            }
        }
    }
    #endregion

    #region 무기 영역 표시;

    [Header("클릭 포커스 관련 요소들")]
    public Transform clickPoint;//클릭한 곳
    public Material clickMat;//클릭한 곳의 매터리얼 
    public GameObject focusCanvas;//포커스 관련 UI
    public Text focusLeftText;//포커스했을 때, 나올 왼쪽 텍스트
    public Text focusRightText;//포커스했을 때, 나올 오른쪽 텍스트
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

        //포커스 UI 방향 조작
        focusVec = cameraObj.transform.position - cameraGround.transform.position;
        lookRotation = Quaternion.LookRotation(focusVec);
        focusCanvas.transform.rotation = lookRotation;

        if (Input.GetMouseButton(0))        //좌클릭
        {
            blueTowerManager.WeaponSort(recentSpellData.spellPrefab.name);
            FocusControl(false, true);
        }
        else if (Input.GetMouseButton(1))   //우클릭
        {
            FocusControl(false, false);
        }
    }
    //카메라 회전값
    Vector3 focusVec;
    Quaternion lookRotation;

    #endregion

}
