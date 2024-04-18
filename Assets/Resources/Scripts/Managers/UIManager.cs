using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    int curRot = -160;//현재 카메라 회전값
    int addRot = 0;//버튼으로 회전할 때 사용하는 논리값
    Vector3 cameraVec;//카메라 회전용 벡터

    int fly = 50;//카메라를 하늘에서 띄운 정도


    [Header("전투 UI")]
    public Slider PlayerResourceSlider;//플레이어의 자원 슬라이더
    public Text PlayerResourceText;//플레이어의 자원 텍스트

    //최근에 사용한 스펠의 데이터
    public SpellData curSpellData;

    [Header("매니저")]
    public SelectManager selectManager;
    public GameManager gameManager;
    ObjectManager objectManager;
    AudioManager audioManager;

    private void Awake()
    {
        objectManager = gameManager.objectManager;
        audioManager = gameManager.audioManager;
        blueTower = gameManager.blueTower;
        blueTowerManager = blueTower.GetComponent<TowerManager>();  
        redTower = gameManager.redTower;

        //시작 할 때, 카메라 축 위치 확인
        cameraGround.transform.position = (blueTower.position + redTower.transform.position) / 2f;
        cameraCloud.transform.position = Vector3.up * fly + cameraGround.position;
    }

    //버튼으로 카메라 조작
    public void CameraSpin(int _spin) => addRot = _spin;

    #region UI 정보 초기화
    public void resetUI() 
    {
        //포커스 초기화
        FocusOff(false);

        //플레이어 은행 초기화
        //blueTowerManager.curBankIndex = 0;
        bankText.text = "Lv." + (blueTowerManager.curBankIndex + 1) + "(" + blueTowerManager.BankValueArr[blueTowerManager.curBankIndex] + ")";

        //배속 초기화
        if (speed == 1) 
        {
            SpeedControl(false);
        }

        //카메라 회전 초기화
        curRot = -160;
    }
    #endregion

    #region 배속 조정
    int speed = 0;
    public Text SpeedControlText;
    public void SpeedControl(bool isSfx)
    {
        if (isSfx) 
        {
            //속도 조절 효과음 출력
            audioManager.PlaySfx(AudioManager.Sfx.SpeedSfx);
        }

        //속도 조정
        speed++;
        speed = (speed % 2);

        //시간 조절
        Time.timeScale = (speed + 1);

        //글자 변환
        SpeedControlText.text = "x" + (speed + 1);
    }
    #endregion

    #region 은행 관리
    [Header("은행 관련 UI")]
    public Image bankBtn;//은행 이미지
    public Text bankText;//은행 텍스ㅡㅌ
    public void BankControl() 
    {
        if (blueTowerManager.curTowerResource >= blueTowerManager.BankValueArr[blueTowerManager.curBankIndex])//비용이 충분한 경우
        {
            //은행 효과음
            audioManager.PlaySfx(AudioManager.Sfx.BankSfx);

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
            audioManager.PlaySfx(AudioManager.Sfx.SpellFailSfx);
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

            //스펠 자원 비율 보여주기
            for (int i = 0; i < spellBtnArr.Length; i++)
            {
                if (spellBtnArr[i].spellData != null)
                {
                    spellBtnArr[i].spellBtnIcon.fillAmount = blueTowerManager.curTowerResource / spellBtnArr[i].spellData.spellValue;
                }
            }

            //포커스됐으면 스킬 범위 이동
            if (clickPoint.gameObject.activeSelf)
                ShowWeaponArea();

            //은행 버튼 활성화 관리
            if(blueTowerManager.curBankIndex < blueTowerManager.BankValueArr.Length && bankBtn.GetComponent<Button>().interactable)
                bankBtn.fillAmount = blueTowerManager.curTowerResource / blueTowerManager.BankValueArr[blueTowerManager.curBankIndex];

                //크리쳐 수 제한 표시
                creatureCountText.text = blueTowerManager.curCreatureCount.ToString() + "/" + gameManager.maxCreatureCount.ToString();
        }
    }
    [Header("크리쳐 제한 텍스트")]
    public Text creatureCountText;

    #region 전투 씬에서 스펠 버튼 클릭

    [Header("전투 스펠버튼 배열")]
    public SpellButton[] spellBtnArr = new SpellButton[4];
    Vector3 clickScaleVec;
    public void OnClick(int _index) 
    {
        //클릭한 버튼의 비용
        SpellData spellData = spellBtnArr[_index].GetComponent<SpellButton>().spellData;

        //비용
        int value = spellData.spellValue;

        if (blueTowerManager.curTowerResource >= value && curSpellData == null)//비용이 충분한 경우
        {
            //스펠 성공 효과음
            audioManager.PlaySfx(AudioManager.Sfx.SpellSuccessSfx);

            //비용 감소
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
                    curSpellData = spellData;

                    //클릭 포인트의 매터리얼 변화
                    clickMat.SetColor("_AlphaColor", spellData.focusColor);

                    //클릭 포인트의 크기 변화
                    float size = spellData.spellPrefab.transform.localScale.x;
                    Bullet bullet = spellData.spellPrefab.GetComponent<Bullet>();

                    if (bullet.endBullet != null)//자식이 있으면 자식의 크기로 설정
                        size = bullet.endBullet.transform.localScale.x;

                    clickScaleVec = new Vector3(size, size, size);
                    clickSphere.localScale = clickScaleVec;
                }
                else if(!spellData.isFocus)
                {
                    blueTowerManager.WeaponSort(spellData.spellPrefab.name);
                } 
            }
        }
        else if (blueTowerManager.curTowerResource < value) //비용이 모자른 경우
        {
            //스펠 실패 효과음
            audioManager.PlaySfx(AudioManager.Sfx.SpellFailSfx);
        }
    }
    #endregion

    #region 세팅 버튼 기능
    [Header("세팅 배경")]
    public GameObject settingBackground;
    public void SettingControl(bool isOpen)//세팅 활성화 관리
    {
        //페이지 효과음
        audioManager.PlaySfx(AudioManager.Sfx.PaperSfx);

        //이미지 조절
        settingBackground.SetActive(isOpen);

        //시간 조절
        if (isOpen)
            Time.timeScale = 0.001f;
        else if (!isOpen)
        {
            SpeedControl(false);
            SpeedControl(false);
        }
    }
    #endregion

    #region 포커스 상태 전환
    [Header("월드의 빛")]
    public GameObject worldLight;

    public void FocusOn()//맵이 까매지며 주술 영역이 보임(포커스 실행)
    {
        if (curSpellData != null)
        {
            //무기 영역 활성화
            clickPoint.gameObject.SetActive(true);
            //빛 비활성화
            worldLight.SetActive(false);
            //시간 감속
            Time.timeScale = 0.2f;
        }
    }
    public void FocusOff(bool isEffect) //자원 반환 여부(포커스 해제)
    {
        //-1: 자원 반환, 0: 영역만 비활성화, 1: 무기 사용 
        if (curSpellData != null && isEffect) 
        {
            if (!clickPoint.gameObject.activeSelf) //자원 반환
            {
                blueTowerManager.curTowerResource += curSpellData.spellValue;
                curSpellData = null;
            }
            else if (clickPoint.gameObject.activeSelf) //무기 사용
            {
                blueTowerManager.WeaponSort(curSpellData.spellPrefab.name);
                curSpellData = null;
            }
        }
        //무기 영역 비활성화
        clickPoint.gameObject.SetActive(false);
        //빛 활성화
        worldLight.SetActive(true);
        //시간 정상화
        SpeedControl(false);
        SpeedControl(false);
    }
    #endregion


    #region 무기 영역 표시;

    [Header("클릭 포커스 관련 요소들")]
    public Transform clickPoint;//클릭한 곳
    public Transform clickSphere;//클릭한 곳의 원형 영역
    public Material clickMat;//클릭한 곳의 원형 영역의 매터리얼 

    void ShowWeaponArea()//화면 누르는 위치를 보여줌
    {
        int layerMask = LayerMask.GetMask("MainMap"); // "Default" 레이어와 충돌하도록 설정

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask, QueryTriggerInteraction.Ignore))
            // 트리거는 무시한다
            clickPoint.position = hit.point;

        //타워 레이더 조작
        blueTowerManager.RadarControl(clickPoint.position);
    }
    #endregion
}
