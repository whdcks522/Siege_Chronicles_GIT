using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using static SpellData;

public class SelectManager : MonoBehaviour
{
    public Animator anim;

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
        anim.SetBool("isSelect", true);
    }

    //레벨 슬라이더 조정
    public void LevelControl() 
    {
        if(audioManager != null)
            audioManager.PlaySfx(AudioManager.Sfx.LevelControlSfx);
    }

    #region 게임 시작, 진행 퍼센트 보여주기 힘듬
    [Header("로딩 패널")]
    public int index_Battle;//선택 창 올리는 애니메이션 후, 전투 씬으로 이동할 것인지, 아니면 종료할 것인지
    //0: 기본, 1: 게임 시작, 2: 게임 초기화,

    public void StartGame(int i)//선택창에서 시작 버튼 또는, 초기화 버튼 클릭
    {
        //애니메이션 후 시작할 지, 초기화 할지 설정
        index_Battle = i;

        //종이 넘기는 효과음
        audioManager.PlaySfx(AudioManager.Sfx.PaperSfx);

        // 게임 시작 함수 호출
        anim.SetBool("isSelect", false);
    }
    #endregion

    public void StartActualGame()// 실제 게임 시작 함수
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
                    if(!gameManager.isEnemySpawn)
                    SpawnCreature(i);
                }
                else if (spellBtnArr[i].spellData.spellType == SpellType.Weapon)//무기의 경우
                {
                    SpawnWeapon(spellBtnArr[i].spellData.spellPrefab.name);
                }
            }
            else if (spellBtnArr[i].spellData == null)//없는 경우 버튼 비활성화
            {
                uiManager.spellBtnArr[i].ButtonOff();
            }
        }
        if (!gameManager.isEnemySpawn) 
        {
            for (int i = 0; i < gameManager.creatureSpellDataArr.Length; i++)
            {
                SpawnCreature(i);
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
    int spawnCreatureCount = 2;//소환하는 크리쳐의 수
    void SpawnCreature(int _index) //_index: 전투씬에서 아래에 있는 버튼 중 몇 번째 버튼인지
    {
        for (int j = 0; j < spawnCreatureCount; j++)//크리쳐 별로 spawnCreatureCount개씩 소환
        {
            GameObject obj = objectManager.CreateObj(spellBtnArr[_index].spellData.spellPrefab.name, ObjectManager.PoolTypes.CreaturePool);
            Creature creature = obj.GetComponent<Creature>();

            //활동 전에 설정
            creature.BeforeRevive(Creature.TeamEnum.Blue, gameManager);//블루로 안하면 갈 곳 없다고 오류남

            SuperAgent superAgent = obj.GetComponent<SuperAgent>();
            if (superAgent.useBullet != null)
            {
                SpawnWeapon(superAgent.useBullet.name);
            }
        }
    }
    #endregion

    #region 주술 소환
    void SpawnWeapon(string _str)//_index: 몇 번째 버튼인지
    {
        int mul = 2;

        if (_str == gameManager.Gun.name)//사격은 총알 게임 오브젝트를 추가로 생성
            mul *= spawnCreatureCount;

        for (int j = 0; j < mul; j++)
        {
            GameObject obj = objectManager.CreateObj(_str, ObjectManager.PoolTypes.BulletPool);
            Bullet bullet = obj.GetComponent<Bullet>();

            if (bullet.endBullet != null)//자식 총알도 생성
                objectManager.CreateObj(bullet.endBullet.name, ObjectManager.PoolTypes.BulletPool);
        }
    }
    #endregion
}
