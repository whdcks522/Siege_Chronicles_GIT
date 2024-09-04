using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("적이 크리쳐를 소환할 것인지")]
    public bool isEnemySpawn;

    [Header("Bmg 여부")]
    public bool isBgm;

    [Header("블루팀 성")]
    public Transform blueTower;
    public TowerManager blueTowerManager;
    [Header("레드팀 성")]
    public Transform redTower;
    public TowerManager redTowerManager;

    [Header("게임 난이도")]//1, 2 ,3(기본: 2)
    public int gameLevel;

    [Header("각 팀별로 소환가능한 크리쳐의 최대 수")]
    public int maxCreatureCount;

    //적 타워가 소환하는 용도
    [Header("크리쳐 스펠 데이터 프리펩 배열")]
    public SpellData[] creatureSpellDataArr;//Infantry, Shooter, Shielder, Accountant 프리펩

    //타워 매너저에서 무기를 분류하는 용도
    [Header("무기 프리펩")]
    public GameObject Gun;//Gun 프리펩
    public GameObject Flame;//Flame 프리펩
    public GameObject GrandCure;//GrandCure 프리펩
    public GameObject CorpseExplosion;//CorpseExplosion 프리펩

    [Header("뱅크 버튼을 누르기 위해 필요한 비용 배열")]
    public int[] BankValueArr;
    [Header("뱅크 버튼을 눌러서 자원이 증가하게 되는 속도 배열")]
    public float[] BankSpeedArr;

    [Header("매니저 목록")]
    public ObjectManager objectManager;
    public UIManager uiManager;
    public AudioManager audioManager;
    public FireManager fireManager;

    #region 현재 스펠 그대로 게임을 '재시도'
    public void RetryGame()
    {
        //시작 화면 배경 음악 재생
        audioManager.PlayBgm(AudioManager.Bgm.BattleBgm);

        //종이 넘기는 효과음
        audioManager.PlaySfx(AudioManager.Sfx.PaperSfx);

        //총알 초기화
        for (int i = 0; i < objectManager.bulletFolder.childCount; i++)
        {
            objectManager.bulletFolder.GetChild(i).gameObject.SetActive(false);
        }
        //데미지 폰트 초기화
        for (int i = 0; i < objectManager.damageFontFolder.childCount; i++)
        {
            objectManager.damageFontFolder.GetChild(i).gameObject.SetActive(false);
        }
        //파랑 크리쳐 초기화
        for (int i = 0; i < objectManager.blueCreatureFolder.childCount; i++)
        {
            objectManager.blueCreatureFolder.GetChild(i).gameObject.SetActive(false);
        }
        //빨강 크리쳐 초기화
        for (int i = 0; i < objectManager.redCreatureFolder.childCount; i++)
        {
            objectManager.redCreatureFolder.GetChild(i).gameObject.SetActive(false);
        }

        //타워 초기화
        blueTowerManager.ResetTower();
        redTowerManager.ResetTower();

        //UI 초기화
        uiManager.resetUI();

        //설정 화면의 글자 초기화
        uiManager.startBtn.interactable = true;
        uiManager.victoryTitle.SetActive(false);
        uiManager.defeatTitle.SetActive(false);

        //순위 불러오기
        fireManager.StopCor();
    }
    #endregion

    //기본 시간 배율
    public int defaultTimeScale = 1;
    public void ResetGame()//게임을 '처음으로'
    {
        //시간 배속을 1로
        Time.timeScale = defaultTimeScale;

        //화면을 처음 화면으로
        SceneManager.LoadScene(0);
    }

    public void QuitGame()//게임을 '종료하기'
    {
        Application.Quit();
    }

    private void OnApplicationPause(bool pause)//게임하다가 잠시 앱을 비활성화 했을 때
    {
        if ((pause == true) && uiManager.selectManager == null && !uiManager.settingBackground.activeSelf)//선택이 지났으면서, 설정 화면이 꺼져있다면
        {
            //일시정지
            uiManager.SettingControl(true);
        }
    }

    readonly string totalStoreUrl = "https://play.google.com/store/apps/developer?id=%EC%9D%B4%EC%82%AD";
    public void OpenWebSite() //웹 사이트 열기
    {
        //개별 사이트: 
        //Application.OpenURL("https://play.google.com/store/apps/details?id=com.IssacCompany.Siege_Chronicle");

        //전체 사이트
        Application.OpenURL(totalStoreUrl);
    }

    //선언 자체 만으로 멈춰버림
    //[Header("네트워크 접속했는지 확인")]
    public bool OnlineCheck() 
    {
        bool isOnline;
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            // 인터넷 연결이 안되었을때
            isOnline = false;
        }
        else if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
        {
            // 데이터로 인터넷 연결이 되었을때
            isOnline = true;
        }
        else
        {
            // 와이파이로 연결이 되었을때
            isOnline = true;
        }
        return isOnline;
    }
    
    
}
