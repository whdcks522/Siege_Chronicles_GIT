using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

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
    AudioManager audioManager;

    private void Awake()
    {
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

        //크리쳐 수 제한 초기화
        blueTowerManager.CreatureCountText();

        //플레이어 은행 초기화
        bankText.text = "Lv." + (blueTowerManager.curBankIndex + 1) + "(" + blueTowerManager.BankValueArr[blueTowerManager.curBankIndex] + ")";
        bankAnim.SetTrigger("isFlash");

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
    public Animator SpeedAnim;
    public void SpeedControl(bool isSfx)
    {
        if (isSfx) 
        {
            //속도 조절 효과음 출력
            audioManager.PlaySfx(AudioManager.Sfx.SpeedSfx);
        }
        SpeedAnim.SetBool("isFlash", true);

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
    public Text bankText;//은행 텍스트
    public Animator bankAnim;//은행 애니메이션
    public void BankControl()//은행 버튼 클릭
    {
        bankAnim.SetBool("isFlash", true);

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
        }
    }
    [Header("크리쳐 제한 텍스트")]
    public Text creatureCountText;
    public Animator creatureCountAnim;

    #region 전투 씬에서 스펠 버튼 클릭

    [Header("전투 스펠버튼 배열")]
    public SpellButton[] spellBtnArr = new SpellButton[4];
    Vector3 clickScaleVec;//주술의 스킬 영역을 보여주는데 사용되는 벡터

    public void OnClick(int _index) //전투 화면에서 밑의 4개의 버튼 중 1개를 클릭함
    {
        //클릭한 버튼의 스펠 정보
        SpellData spellData = spellBtnArr[_index].GetComponent<SpellButton>().spellData;
        //해당 스펠의 사용 비용
        int value = spellData.spellValue;

        if (blueTowerManager.curTowerResource >= value && curSpellData == null)//비용이 충분하면서 포커스 중이 아니라면
        {
            //스펠 성공 효과음
            audioManager.PlaySfx(AudioManager.Sfx.SpellSuccessSfx);

            if (spellData.spellType == SpellData.SpellType.Creature)//크리쳐를 누른 경우
            {
                if (blueTowerManager.CreatureCountCheck()) 
                {
                    //비용 감소
                    blueTowerManager.curTowerResource -= value;
                    //해당 크리쳐 소환
                    blueTowerManager.SpawnCreature(spellData.spellPrefab.name);
                }    
            }
            else if(spellData.spellType == SpellData.SpellType.Weapon)//주술을 누른 경우
            {
                //비용 감소
                blueTowerManager.curTowerResource -= value;

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
    public GameObject settingBackground;//설정 창 배경
    public GameObject victoryTitle;//승리 시 나올 텍스트
    public GameObject defeatTitle;//패배 시 나올 텍스트
    public GameObject startBtn;//이어하기 버튼(게임 종료 시, 비활성화)
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
