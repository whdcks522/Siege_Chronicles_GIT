using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellButton : MonoBehaviour
{
    
    

    [Header("���� ��ư�� ������")]
    public Image SpellBtnIcon;

    [Header("���� ��ũ��Ÿ�� ��ü")]
    public SpellData SpellData;

    [Header("�Ŵ���")]

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

    #region ���� ��ư Ŭ��
    public void OnClick()
    {
        //���� ��ư ������ �ٲٱ�
        selectManager.selectedSpellIcon.sprite = SpellData.spellIcon;
        //���� �̸� �ٲٱ�
        selectManager.selectedSpellName.text = SpellData.spellName;
        //���� Ÿ�� �ٲٱ�
        selectManager.selectedSpellType.text = "Ÿ��: " + 1;//SpellData.spellType
        //���� ��� �ٲٱ�
        selectManager.selectedSpellValue.text = "���: " + SpellData.spellValue;
        //���� ���� �ٲٱ�
        selectManager.selectedSpellDesc.text = "����: " + SpellData.spellDesc;
    }
    #endregion
}
