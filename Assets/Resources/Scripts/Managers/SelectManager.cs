using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using static SpellData;
using Unity.VisualScripting;

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
    public Image selectedSpellTypeImage;
    public Sprite[] spellTypeImageArr;//바꿀 이미지가 어디에 있는지
    public Text selectedSpellTypeText;

    [Header("선택된 스펠 비용")]
    public Text selectedSpellValue;

    [Header("선택된 스펠 설명")]
    public Text selectedSpellDesc;

    [Header("스펠버튼 배열")]
    public SpellButton[] spellBtnArr = new SpellButton[4];

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
    [Header("애니메이션 후 전투씬으로 이동할 것인지, 초기화할 것인지")]
    public int index_Battle;//0: 기본, 1: 게임 시작, 2: 게임 초기화,

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
                //쉐이더 배경 활성화
                uiManager.spellBtnArr[i].spellBtnShader.gameObject.SetActive(true);

                //버튼에 스펠 데이터 전달
                uiManager.spellBtnArr[i].spellData = spellBtnArr[i].spellData;
                //해당 버튼의 이미지 변경
                spellBtnArr[i].IconChange(uiManager.spellBtnArr[i]);

                //오브젝트 풀링을 위해 미리 생성
                if (spellBtnArr[i].spellData.spellType == SpellType.Creature)//크리쳐의 경우
                {
                    //크리쳐 미리 생성
                    SpawnCreature(spellBtnArr[i].spellData.spellPrefab.name);
                }
                else if (spellBtnArr[i].spellData.spellType == SpellType.Weapon)//무기의 경우(안 고른 경우가 있을 수 있어서)
                {
                    //무기 미리 생성
                    SpawnWeapon(spellBtnArr[i].spellData.spellPrefab.name);
                }
            }
            else //if (spellBtnArr[i].spellData == null) //없는 경우 버튼 비활성화
            {
                uiManager.spellBtnArr[i].ButtonOff(true);
            }
        }

        if (gameManager.isEnemySpawn)//적도 소환하는 상태면, 추가로 소환
        {
            for (int i = 0; i < gameManager.creatureSpellDataArr.Length; i++)
            {
                SpawnCreature(gameManager.creatureSpellDataArr[i].spellPrefab.name);
            }
        }
        
        //전투 환경 초기화
        gameManager.RetryGame();

        // 게임 레벨 설정
        gameManager.gameLevel = (int)levelSlider.value;

        // 선택 매니저 삭제
        Destroy(gameObject);
    }

    #region 크리쳐 소환
    void SpawnCreature(string _str) //str 크리쳐를 gameManager.maxCreatureCount개씩 소환
    {
        for (int j = 0; j < gameManager.maxCreatureCount; j++)//크리쳐 별로 gameManager.maxCreatureCount개씩 소환
        {
            GameObject obj = objectManager.CreateObj(_str, ObjectManager.PoolTypes.CreaturePool);
            Creature creature = obj.GetComponent<Creature>();

            //활동 전에 설정
            creature.BeforeRevive(Creature.TeamEnum.Blue, gameManager);//블루로 안하면 갈 곳 없다고 오류남

            SpawnWeapon(creature.useBullet.name);
        }
    }
    #endregion

    #region 주술(스킬) 소환
    void SpawnWeapon(string _str)//str 주술을 spawnCreatureCount개씩 소환
    {
        int mul = 2;
        //사격은 적군 전체
        if (_str == gameManager.Gun.name) mul = gameManager.maxCreatureCount;
        //시체 폭발은 아군 전체에 적용
        else if (_str == gameManager.CorpseExplosion.name) mul = gameManager.maxCreatureCount;
        //개구리는 3발씩 쏨
        else if (_str == gameManager.creatureSpellDataArr[1].spellPrefab.GetComponent<Creature>().useBullet.name) mul = 6;


        for (int i = 0; i < mul; i++)
        {
            GameObject obj = objectManager.CreateObj(_str, ObjectManager.PoolTypes.BulletPool);
            Bullet bullet = obj.GetComponent<Bullet>();

            if (bullet.endBullet != null) //자식 총알도 생성
            {
                for (int j = 0; j < 2; j++)
                    objectManager.CreateObj(bullet.endBullet.name, ObjectManager.PoolTypes.BulletPool);
            }
        }
    }
    #endregion
}
