using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using static SpellData;

public class SelectManager : MonoBehaviour
{
    [Header("선택된 스펠 버튼")]
    public SpellButton selectedSpellBtn;

    [Header("선택된 스펠 아이콘")]
    public Image selectedSpellIcon;

    [Header("선택된 스펠 이름")]
    public Text selectedSpellName;

    [Header("선택된 스펠 타입")]
    public Text selectedSpellType;

    [Header("선택된 스펠 비용")]
    public Text selectedSpellValue;

    [Header("선택된 스펠 설명")]
    public Text selectedSpellDesc;

    [Header("스펠버튼 배열")]
    public SpellButton[] spellBtnArr = new SpellButton[4];

    [Header("오른쪽 패널")]
    public GameObject rightPanel;


    [Header("레벨 슬라이더")]
    public Slider levelSlider;

    [Header("매니저")]
    public GameManager gameManager;
    UIManager uiManager;
    AudioManager audioManager;
    ObjectManager objectManager;
    
    void Awake()
    {
        uiManager = gameManager.uiManager;
        audioManager = gameManager.audioManager;
        objectManager = gameManager.objectManager;

        //오른쪽 패널 비활성화
        rightPanel.SetActive(false);
    }

    #region 게임 시작, 진행 퍼센트 보여주기 힘듬
    [Header("로딩 아이콘")]
    public GameObject loadIcon;
    public int maxCreatureCount; //타워 매니저에 ui용 겹쳐서 존재
    public void StartGame()
    {
        /*
        //로딩 아이콘 활성화
        loadIcon.gameObject.SetActive(true);

        //UI 비활성화
        gameObject.SetActive(false);

        //게임 레벨 설정
        gameManager.gameLevel = (int)levelSlider.value;
        
        //battleUI로 스펠 전달
        for (int i = 0; i < spellBtnArr.Length; i++)
        {
            if (spellBtnArr[i].spellData != null)//스펠이 있는 경우 이미지 갱신
            {
                //버튼에 스펠데이터 전달
                uiManager.spellBtnArr[i].spellData = spellBtnArr[i].spellData;
                spellBtnArr[i].IconChange(uiManager.spellBtnArr[i]);

                
                //오브젝트 풀링을 위해 미리 생성
                if (spellBtnArr[i].spellData.spellType == SpellType.Creature)//생명체의 경우
                {
                    for (int j = 0; j < 4; j++)
                    {
                        GameObject obj = objectManager.CreateObj(spellBtnArr[i].spellData.spellPrefab.name, ObjectManager.PoolTypes.CreaturePool);
                        Creature creature = obj.GetComponent<Creature>();
                        //활동 전에 설정
                        creature.BeforeRevive(Creature.TeamEnum.Blue, gameManager);//블루로 안하면 갈 곳 없다고 오류남
                        SuperAgent superAgent = obj.GetComponent<SuperAgent>();
                        //superAgent.useBullet
                    }
                }
                else if (spellBtnArr[i].spellData.spellType == SpellType.Weapon)//무기의 경우
                {
                    int mul = 1;
                    if (spellBtnArr[i].spellData.spellPrefab.name == gameManager.Gun.name)
                        mul = 3;

                    for (int j = 0; j < 4 * mul; j++)
                    {
                        GameObject obj = objectManager.CreateObj(spellBtnArr[i].spellData.spellPrefab.name, ObjectManager.PoolTypes.BulletPool);
                        Bullet bullet = obj.GetComponent<Bullet>();
                        if (bullet.endBullet != null)//자식 총알도 생성
                            objectManager.CreateObj(bullet.endBullet.name, ObjectManager.PoolTypes.BulletPool);
                    }
                }
                
            }
            else if (spellBtnArr[i].spellData == null)//없는 경우 버튼 비활성화
            {
                uiManager.spellBtnArr[i].ButtonOff();
            }
        }
        //전투 환경 초기화
        gameManager.RetryGame();

        */
        // 로딩 아이콘 활성화
        loadIcon.gameObject.SetActive(true);

        // 게임 레벨 설정
        gameManager.gameLevel = (int)levelSlider.value;

        // 게임 시작 함수 호출
        //StartCoroutine(StartGameCoroutine());
        StartActualGame();
    }
    #endregion


    // 게임 시작 코루틴
    private IEnumerator StartGameCoroutine()
    {
        // 여기서 게임 시작 전 필요한 작업들을 수행합니다.

        // 예시로 5초간의 가상 로딩 시간을 줍니다.
        float timer = 0f;
        float loadingTime = 5f; // 5초 동안 로딩
        while (timer < loadingTime)
        {
            timer += Time.deltaTime;
            // 진행 바 업데이트
            Debug.Log((int)(timer / loadingTime * 100));
            yield return null;
        }

        // 로딩이 끝나면 실제로 게임을 시작하는 함수 호출
        StartActualGame();
    }


    // 실제 게임 시작 함수
    private void StartActualGame()
    {
        //battleUI로 스펠 전달
        for (int i = 0; i < spellBtnArr.Length; i++)
        {
            if (spellBtnArr[i].spellData != null)//스펠이 있는 경우 이미지 갱신
            {
                //버튼에 스펠데이터 전달
                uiManager.spellBtnArr[i].spellData = spellBtnArr[i].spellData;
                spellBtnArr[i].IconChange(uiManager.spellBtnArr[i]);


                //오브젝트 풀링을 위해 미리 생성
                if (spellBtnArr[i].spellData.spellType == SpellType.Creature)//생명체의 경우
                {
                    for (int j = 0; j < 4; j++)
                    {
                        GameObject obj = objectManager.CreateObj(spellBtnArr[i].spellData.spellPrefab.name, ObjectManager.PoolTypes.CreaturePool);
                        Creature creature = obj.GetComponent<Creature>();
                        //활동 전에 설정
                        creature.BeforeRevive(Creature.TeamEnum.Blue, gameManager);//블루로 안하면 갈 곳 없다고 오류남
                        SuperAgent superAgent = obj.GetComponent<SuperAgent>();
                        //superAgent.useBullet
                    }
                }
                else if (spellBtnArr[i].spellData.spellType == SpellType.Weapon)//무기의 경우
                {
                    int mul = 1;
                    if (spellBtnArr[i].spellData.spellPrefab.name == gameManager.Gun.name)
                        mul = 3;

                    for (int j = 0; j < 4 * mul; j++)
                    {
                        GameObject obj = objectManager.CreateObj(spellBtnArr[i].spellData.spellPrefab.name, ObjectManager.PoolTypes.BulletPool);
                        Bullet bullet = obj.GetComponent<Bullet>();
                        if (bullet.endBullet != null)//자식 총알도 생성
                            objectManager.CreateObj(bullet.endBullet.name, ObjectManager.PoolTypes.BulletPool);
                    }
                }

            }
            else if (spellBtnArr[i].spellData == null)//없는 경우 버튼 비활성화
            {
                uiManager.spellBtnArr[i].ButtonOff();
            }
        }
        //전투 환경 초기화
        gameManager.RetryGame();

        // UI 비활성화
        gameObject.SetActive(false);
    }



    /*
    private IEnumerator LoadSceneAsyncCoroutine(string sceneName)//비동기적으로 scene 로드(렉 걸릴때 사용)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false; // 로딩이 끝나도 바로 활성화하지 않음

        // 로딩이 끝날 때까지 대기
        while (!asyncLoad.isDone)
        {
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f); // 0.9는 로딩이 끝났을 때의 값

            //loadText.text = "로딩 중: " + Mathf.Floor(progress * 100) + "%";
            if (progress >= 1f)
            {
                asyncLoad.allowSceneActivation = true; // 활성화
            }

            yield return null;
        }
    }
    */
}
