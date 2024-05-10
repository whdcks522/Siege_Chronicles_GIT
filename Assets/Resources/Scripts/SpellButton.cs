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

    [Header("스펠 버튼의 아이콘 이미지")]
    public Image spellBtnIcon;

    [Header("스펠 버튼의 체크")]
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

        if (spellData == null && curSpellBtnEnum == SpellBtnEnum.LeftBtn)//스펠 데이터가 없으면서 리스트인 경우 버튼 비활성화 
        {
            ButtonOff();
        }
    }

    public void ButtonOff() //버튼 비활성화
    {
        //비용 텍스트 관리
        spellBtnValue.gameObject.SetActive(false);
        //아이콘 이미지 관리
        spellBtnIcon.gameObject.SetActive(false);

        //버튼 이미지 비활성화
        GetComponent<Button>().interactable = false;
        GetComponent<Outline>().enabled = false;
    }

    #region 아이콘 이미지 갱신
    public void IconChange(SpellButton tmpSpellBtn) 
    {
        if (tmpSpellBtn.spellData != null) //스펠 데이터가 있다면
        {
            //비용 텍스트 관리
            tmpSpellBtn.spellBtnValue.gameObject.SetActive(true);
            tmpSpellBtn.spellBtnValue.text = tmpSpellBtn.spellData.spellValue.ToString();

            //아이콘 이미지 관리
            tmpSpellBtn.spellBtnIcon.sprite = spellData.spellIcon;

            if (spellData.spellType == SpellData.SpellType.Weapon)
            {
                tmpSpellBtn.spellBtnIcon.color = Color.black;
            }
            else if (spellData.spellType != SpellData.SpellType.Weapon)
            {
                tmpSpellBtn.spellBtnIcon.color = Color.white;
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

    #region 스펠 버튼 클릭
    public void OnClick()
    {
        if ((curSpellBtnEnum == SpellBtnEnum.LeftBtn || curSpellBtnEnum == SpellBtnEnum.SelectBtn) && spellData != null) 
        {
            //스펠 버튼 아이콘 바꾸기
            selectManager.selectedSpellBtn.spellData = spellData;
            IconChange(selectManager.selectedSpellBtn);

            //스펠 이름 바꾸기
            selectManager.selectedSpellName.text = spellData.spellName;

            //스펠 타입 바꾸기
            if (spellData.spellType == SpellData.SpellType.Creature) 
            {
                selectManager.selectedSpellType.text = "크리쳐";
            }
            else if (spellData.spellType == SpellData.SpellType.Weapon)
            {
                selectManager.selectedSpellType.text = "주술";
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
