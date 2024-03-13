using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellButton : MonoBehaviour
{
    
    

    [Header("스펠 버튼의 아이콘")]
    public Image SpellBtnIcon;

    [Header("스펠 스크립타블 객체")]
    public SpellData SpellData;

    [Header("매니저")]

    public GameManager gameManager;
    UIManager UiManager;
    SelectManager selectManager;
    AudioManager audioManager;

    void Awake()
    {
        if (SpellData != null)
        {
            UiManager = gameManager.uiManager;
            selectManager = gameManager.uiManager.selectManager;
            audioManager = gameManager.audioManager;

            SpellBtnIcon.sprite = SpellData.spellIcon;
        }
        else if (SpellData == null) 
        {
            SpellBtnIcon.gameObject.SetActive(false);
            GetComponent<Button>().interactable = false;
        }

    }

    #region 스펠 버튼 클릭
    public void OnClick()
    {
        //스펠 버튼 아이콘 바꾸기
        selectManager.selectedSpellIcon.sprite = SpellData.spellIcon;
        //스펠 이름 바꾸기
        selectManager.selectedSpellName.text = SpellData.spellName;
        //스펠 타입 바꾸기
        selectManager.selectedSpellType.text = "타입: " + 1;//SpellData.spellType
        //스펠 비용 바꾸기
        selectManager.selectedSpellValue.text = "비용: " + SpellData.spellValue;
        //스펠 설명 바꾸기
        selectManager.selectedSpellDesc.text = "설명: " + SpellData.spellDesc;
    }
    #endregion
}
