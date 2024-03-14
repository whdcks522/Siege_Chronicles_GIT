using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellButton : MonoBehaviour
{
    public enum SpellBtnEnum {LeftBtn, RightBtn, SelectBtn, BattleBtn }//속하는 팀
    [Header("스펠 버튼의 종류")]
    public SpellBtnEnum curSpellBtnEnum;

    [Header("스펠 버튼의 아이콘")]
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


        if(spellData != null)//스펠 데이터가 없으면
            spellBtnIcon.sprite = spellData.spellIcon;

        if (spellData == null && curSpellBtnEnum == SpellBtnEnum.LeftBtn)//스펠 데이터가 없으면서 리스트인 경우 버튼 비활성화 
        {
            spellBtnIcon.gameObject.SetActive(false);
            GetComponent<Button>().interactable = false;
            GetComponent<Outline>().enabled = false;
        }

    }

    #region 스펠 버튼 클릭
    public void OnClick()
    {
        if (curSpellBtnEnum == SpellBtnEnum.LeftBtn || curSpellBtnEnum == SpellBtnEnum.SelectBtn) 
        {
            //스펠 버튼 아이콘 바꾸기
            selectManager.selectedSpellIcon.sprite = spellData.spellIcon;
            //스펠 이름 바꾸기
            selectManager.selectedSpellName.text = spellData.spellName;
            //스펠 타입 바꾸기
            selectManager.selectedSpellType.text = "타입: " + 1;//SpellData.spellType
            //스펠 비용 바꾸기
            selectManager.selectedSpellValue.text = "비용: " + spellData.spellValue;
            //스펠 설명 바꾸기
            selectManager.selectedSpellDesc.text = "설명: " + spellData.spellDesc;

            if (curSpellBtnEnum == SpellBtnEnum.LeftBtn) 
            {
                bool isWork = true;

                foreach (SpellButton spellBtn in selectManager.spellBtnArr)
                {
                    if (spellBtn.originSpellBtn == GetComponent<SpellButton>())//이미 눌렀던 버튼이였다면
                    {
                        //체크 아이콘 조작
                        spellBtnCheck.SetActive(false);

                        //부모 설정
                        spellBtn.originSpellBtn = null;

                        //스펠 데이터 갱신
                        spellBtn.spellData = null;

                        //이미지 변경
                        spellBtn.spellBtnIcon.sprite = null;

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
                            //체크 아이콘 조작
                            spellBtnCheck.SetActive(true);

                            //부모 설정
                            spellBtn.originSpellBtn = GetComponent<SpellButton>();

                            //스펠 데이터 갱신
                            spellBtn.spellData = spellData;

                            //이미지 변경
                            spellBtn.spellBtnIcon.sprite = spellData.spellIcon;

                            break;
                        }
                    }
                }
            }
        }
    }
    #endregion
}
