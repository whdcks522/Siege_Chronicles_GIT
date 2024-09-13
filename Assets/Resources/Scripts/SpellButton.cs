using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellButton : MonoBehaviour
{
    public enum SpellBtnEnum {LeftBtn, RightBtn, SelectBtn, BattleBtn }//어떤 종류의 스펠 버튼인지
    [Header("스펠 버튼의 종류")]
    public SpellBtnEnum curSpellBtnEnum;

    [Header("스펠 버튼의 비용 텍스트")]
    public Text spellBtnValue;

    [Header("스펠 버튼의 포커스 여부 체크")]
    public Text spellBtnFocus;



    [Header("스펠 버튼의 쉐이더")]
    public Image spellBtnShader;

    [Header("스펠 버튼의 아이콘 이미지")]
    public Image spellBtnIcon;

    [Header("스펠 버튼의 초록색 체크")]
    public GameObject spellBtnCheck;

    [Header("스펠 스크립타블 객체")]
    public SpellData spellData;

    [Header("버튼의 부모")]
    public SpellButton originSpellBtn;



    [Header("매니저")]
    public GameManager gameManager;
    UIManager UiManager;
    SelectManager selectManager;
    AudioManager audioManager;

    void Awake()
    {
        UiManager = gameManager.uiManager;
        selectManager = gameManager.uiManager.selectManager;
        audioManager = gameManager.audioManager;

        if (spellData != null)//스펠 데이터가 있으면
        {
            IconChange(this);
        }

        //if (spellData == null && curSpellBtnEnum == SpellBtnEnum.LeftBtn)//스펠 데이터가 없으면서 왼쪽 버튼인 경우인 경우 버튼 비활성화 
            //ButtonOff();
        
        if (curSpellBtnEnum == SpellBtnEnum.BattleBtn) 
        {
            //전투 버튼은 체크 마크 안씀
            Destroy(spellBtnCheck);
        }
    }

    public void ButtonOff(bool isDestroy) //버튼 비활성화
    {
        if (isDestroy)//파괴하는 경우
        {
            //쉐이더 삭제
            if (spellBtnShader != null)
                Destroy(spellBtnShader.gameObject);

            //비용 텍스트 삭제
            Destroy(spellBtnValue.gameObject);

            //아이콘 이미지 삭제
            Destroy(spellBtnIcon.gameObject);

            //포커스 이미지 삭제
            Destroy(spellBtnFocus);
        }
        else //비활성화 하는 경우
        {
            //비용 텍스트 비활성화
            spellBtnValue.gameObject.SetActive(false);

            //아이콘 이미지 비활성화
            spellBtnIcon.gameObject.SetActive(false);
        }

        //버튼 이미지 비활성화
        GetComponent<Button>().interactable = false;
        GetComponent<Outline>().enabled = false;
    }

    #region 아이콘 이미지 갱신
    public void IconChange(SpellButton tmpSpellBtn)//전투에서는 안씀
    {
        //포커스 이미지 활성화
        tmpSpellBtn.spellBtnFocus.gameObject.SetActive(false);

        if (tmpSpellBtn.spellData != null) //스펠 데이터가 있다면
        {
            //비용 텍스트 관리
            tmpSpellBtn.spellBtnValue.gameObject.SetActive(true);
            tmpSpellBtn.spellBtnValue.text = tmpSpellBtn.spellData.spellValue.ToString();

            //아이콘 이미지 관리
            tmpSpellBtn.spellBtnIcon.sprite = spellData.spellIcon;


            if (spellData.spellType == SpellData.SpellType.Creature)//크리쳐인 경우
            {
                tmpSpellBtn.spellBtnIcon.color = Color.white;
            }
            else//주술인 경우
            {
                tmpSpellBtn.spellBtnIcon.color = Color.black;

                if (tmpSpellBtn.spellData.isFocus) 
                {
                    //포커스 텍스트 활성화
                    tmpSpellBtn.spellBtnFocus.gameObject.SetActive(true);

                    //포커스 텍스트 색 조정
                    Color focusTextColor = spellData.focusColor;
                    focusTextColor.a = 1;

                    tmpSpellBtn.spellBtnFocus.color = focusTextColor;
                }
            }
        }
        else if (tmpSpellBtn.spellData == null)//스펠 데이터가 없다면
        {
            //비용 텍스트 관리
            tmpSpellBtn.spellBtnValue.gameObject.SetActive(false);
            //아이콘 이미지 관리
            tmpSpellBtn.spellBtnIcon.sprite = null;
            //색 초기화
            tmpSpellBtn.spellBtnIcon.color = Color.white;
        }
    }
    #endregion

    #region 우측에서 선택한 스펠에 대한 정보 보여줌
    public void OnClick()
    {
        if (curSpellBtnEnum != SpellBtnEnum.BattleBtn)
        {
            spellBtnValue.GetComponent<Animator>().SetBool("isFlash", true);
        }

        if ((curSpellBtnEnum == SpellBtnEnum.LeftBtn || curSpellBtnEnum == SpellBtnEnum.SelectBtn) && spellData != null) 
        {
            selectManager.selectedSpellBtn.spellData = spellData;

            //스펠 버튼 아이콘 바꾸기
            IconChange(selectManager.selectedSpellBtn);

            //스펠 이름 바꾸기
            selectManager.selectedSpellName.text = spellData.spellName;

            //스펠 타입 바꾸기
            selectManager.selectedSpellTypeImage.gameObject.SetActive(true);

            if (spellData.spellType == SpellData.SpellType.Creature)//크리쳐를 대상으로 하는 경우
            {
                //스펠 타입 이미지 변경
                selectManager.selectedSpellTypeImage.sprite = selectManager.spellTypeImageArr[0].sprite;
                selectManager.selectedSpellTypeImage.color = selectManager.spellTypeImageArr[0].color;

                //스펠 타입 텍스트 변경
                selectManager.selectedSpellTypeText.text = "캐릭터";

                //스펠 정보 바꾸기(체력, 강도:피해량: 이동속도, 포커스 여부, 사거리)
                Creature spellCreature = spellData.spellPrefab.GetComponent<Creature>();
                Bullet spellCreatureBullet = spellCreature.useBullet.GetComponent<Bullet>();
                int spellCreatureBulletDamage = spellCreatureBullet.bulletDamage;
                string rangeType = "<color=#227744>원거리</color>";
                if (spellCreature.bulletStartPoint == null) 
                {
                    rangeType = "<color=#CC5555>근접</color>";
                }
                //체력, 공격력, 사거리, 이동속도
                selectManager.selectedSpellInfo.text =
                "<color=#FF7070>체력</color>: " + spellCreature.maxHealth + "\n" +
                "<color=#4E9D61>공격력</color>: " + spellCreatureBulletDamage / 3 + '/' + spellCreatureBulletDamage / 2 + '/' + spellCreatureBulletDamage + "(<color=#55CC55>난이도별</color>)" + "\n" +
                "<color=#B684FF>공격 사거리</color>: " + spellCreature.maxRange + "(" + rangeType + ")\n" +
                "<color=#406BFF>기동력</color>: " + spellCreature.nav.speed;
            }
            else//스펠을 대상으로 하는 경우
            {
                //스펠 타입 이미지 변경
                selectManager.selectedSpellTypeImage.sprite = selectManager.spellTypeImageArr[1].sprite;
                selectManager.selectedSpellTypeImage.color = selectManager.spellTypeImageArr[1].color;

                //스펠 타입 텍스트 변경
                selectManager.selectedSpellTypeText.text = "대포";

                //스펠 정보 바꾸기(체력, 강도:피해량: 이동속도, 포커스 여부, 사거리)
                Bullet spellWeaponBullet = spellData.spellPrefab.GetComponent<Bullet>();
                int spellWeaponBulletDamage = spellWeaponBullet.bulletDamage;
                int spellWeaponBulletRad = (int)spellWeaponBullet.transform.localScale.x;
                if (spellWeaponBulletDamage == 0 && spellWeaponBullet.endBullet != null) //피해가 0이면서 자식 총알이 있는 경우(화염구)
                {
                    //자식 총알로 대체
                    spellWeaponBulletDamage = spellWeaponBullet.endBullet.GetComponent<Bullet>().bulletDamage;

                    spellWeaponBulletRad = (int)spellWeaponBullet.endBullet.GetComponent<Bullet>().transform.localScale.x;
                }

                //공격력, 피격 대상, 피격 범위, 포커스 여부
                if (spellWeaponBullet.curBulletEffectEnum == Bullet.BulleEffectEnum.Damage)
                {
                    selectManager.selectedSpellInfo.text = "<color=#4E9D61>공격력</color>: " +
                            spellWeaponBulletDamage / 3 + "/" + spellWeaponBulletDamage / 2 + "/" + spellWeaponBulletDamage + "(<color=#55CC55>난이도별</color>)" + "\n";
                }
                else
                {
                    selectManager.selectedSpellInfo.text = "<color=#FB6EFF>치유력</color>: " + spellWeaponBulletDamage + "\n";
                }

                selectManager.selectedSpellInfo.text += "<color=#B684FF>피격 범위</color>: ";
                if (spellWeaponBullet.bulletTarget != null)
                    selectManager.selectedSpellInfo.text += "단일 " + "\n";
                else
                    selectManager.selectedSpellInfo.text += "다중(<color=#770077>" + spellWeaponBulletRad + "</color>)\n";

                selectManager.selectedSpellInfo.text += "<color=#406BFF>드래그 여부</color>: ";
                if (spellData.isFocus)
                {
                    selectManager.selectedSpellInfo.text += "<color=#AA0000>O</color>";
                }
                else 
                {
                    selectManager.selectedSpellInfo.text += "X";
                }
            }

            //스펠 비용 바꾸기
            selectManager.selectedSpellValue.text = spellData.spellValue.ToString();

            //스펠 설명 바꾸기
            selectManager.selectedSpellDesc.text = spellData.spellDesc;

            if (curSpellBtnEnum == SpellBtnEnum.LeftBtn)
            {
                bool isWork = true;

                foreach (SpellButton spellBtn in selectManager.spellBtnArr)
                {
                    if (spellBtn.originSpellBtn == GetComponent<SpellButton>())//이미 눌렀던 버튼이였다면
                    {
                        //스펠 버튼 클릭 실패 효과음 출력
                        audioManager.PlaySfx(AudioManager.Sfx.SpellFailSfx);

                        //체크 아이콘 조작
                        spellBtnCheck.SetActive(false);

                        //부모 설정
                        spellBtn.originSpellBtn = null;

                        //스펠 데이터 갱신
                        spellBtn.spellData = null;

                        //이미지 변경
                        spellBtn.spellData = null;
                        IconChange(spellBtn);

                        isWork = false;
                        break;
                    }
                }

                if (isWork)
                {
                    foreach (SpellButton spellBtn in selectManager.spellBtnArr)
                    {
                        if (spellBtn.originSpellBtn == null)//선점자가 없다면
                        {
                            //스펠 버튼 클릭 성공 효과음 출력
                            audioManager.PlaySfx(AudioManager.Sfx.SpellSuccessSfx);

                            //체크 아이콘 조작
                            spellBtnCheck.SetActive(true);

                            //부모 설정
                            spellBtn.originSpellBtn = GetComponent<SpellButton>();

                            //스펠 데이터 갱신
                            spellBtn.spellData = spellData;

                            //이미지 변경
                            spellBtn.spellData = spellData;
                            IconChange(spellBtn);

                            break;
                        }
                    }
                }
            }
            else if (curSpellBtnEnum == SpellBtnEnum.SelectBtn) 
            {
                //스펠 버튼 클릭 성공 효과음 출력
                audioManager.PlaySfx(AudioManager.Sfx.PaperSfx);
            }
        }
    }
    #endregion
}
