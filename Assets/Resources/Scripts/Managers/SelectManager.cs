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

    //레벨 슬라이더 조정
    public void LevelControl()=> audioManager.PlaySfx(AudioManager.Sfx.LevelControlSfx);

    #region 게임 시작, 진행 퍼센트 보여주기 힘듬
    [Header("로딩 패널")]
    public Image loadPanel;
    public int maxCreatureCount; //타워 매니저에 ui용 겹쳐서 존재
    public void StartGame()
    {
        //종이 넘기는 효과음
        audioManager.PlaySfx(AudioManager.Sfx.PaperSfx);

        // 게임 시작 함수 호출
        StartCoroutine(StartFadeOut());
    }
    #endregion


    Color fadeColor;
    IEnumerator StartFadeOut()//페이드 아웃
    {
        // 로딩 아이콘 활성화
        loadPanel.gameObject.SetActive(true);

        fadeColor = loadPanel.color;
        float time = 1, minTime = 0;

        while (time > minTime)
        {
            time -= Time.deltaTime;
            float t = time / 1;//대기 시간

            fadeColor.a = Mathf.Lerp(1, 0, t);
            loadPanel.color = fadeColor;

            yield return null;
        }
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
                //버튼에 스펠 데이터 전달
                uiManager.spellBtnArr[i].spellData = spellBtnArr[i].spellData;
                //해당 버튼의 이미지 변경
                spellBtnArr[i].IconChange(uiManager.spellBtnArr[i]);


                //오브젝트 풀링을 위해 미리 생성
                if (spellBtnArr[i].spellData.spellType == SpellType.Creature)//생명체의 경우
                {
                    SpawnCreature(i);
                }
                else if (spellBtnArr[i].spellData.spellType == SpellType.Weapon)//무기의 경우
                {
                    SpawnWeapon(i);
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

        // 게임 레벨 설정
        gameManager.gameLevel = (int)levelSlider.value;
    }

    #region 크리쳐 소환
    void SpawnCreature(int _index) //_index: 몇 번째 버튼인지
    {
        for (int j = 0; j < 4; j++)
        {
            GameObject obj = objectManager.CreateObj(spellBtnArr[_index].spellData.spellPrefab.name, ObjectManager.PoolTypes.CreaturePool);
            Creature creature = obj.GetComponent<Creature>();
            //활동 전에 설정
            creature.BeforeRevive(Creature.TeamEnum.Blue, gameManager);//블루로 안하면 갈 곳 없다고 오류남
            SuperAgent superAgent = obj.GetComponent<SuperAgent>();
            if (superAgent.useBullet != null)
            {

            }
        }
    }
    #endregion

    #region 무기 소환
    void SpawnWeapon(int _index)//_index: 몇 번째 버튼인지
    {
        int mul = 1;
        if (spellBtnArr[_index].spellData.spellPrefab.name == gameManager.Gun.name)
            mul = 3;

        for (int j = 0; j < 4 * mul; j++)
        {
            GameObject obj = objectManager.CreateObj(spellBtnArr[_index].spellData.spellPrefab.name, ObjectManager.PoolTypes.BulletPool);
            Bullet bullet = obj.GetComponent<Bullet>();
            if (bullet.endBullet != null)//자식 총알도 생성
                objectManager.CreateObj(bullet.endBullet.name, ObjectManager.PoolTypes.BulletPool);
        }
    }
    #endregion
}
