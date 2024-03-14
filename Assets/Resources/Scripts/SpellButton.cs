using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellButton : MonoBehaviour
{
    public enum SpellBtnEnum {LeftBtn, RightBtn, SelectBtn, BattleBtn }//���ϴ� ��
    [Header("���� ��ư�� ����")]
    public SpellBtnEnum curSpellBtnEnum;

    [Header("���� ��ư�� ������")]
    public Image spellBtnIcon;

    [Header("���� ��ư�� üũ")]
    public GameObject spellBtnCheck;

    [Header("���� ��ũ��Ÿ�� ��ü")]
    public SpellData spellData;

    [Header("��ư�� �θ�")]
    public SpellButton originSpellBtn;

    [Header("�Ŵ���")]
    public GameManager gameManager;
    UIManager UiManager;
    SelectManager selectManager;
    AudioManager audioManager;

    void Awake()
    {
        UiManager = gameManager.uiManager;
        selectManager = gameManager.uiManager.selectManager;
        audioManager = gameManager.audioManager;


        if(spellData != null)//���� �����Ͱ� ������
            spellBtnIcon.sprite = spellData.spellIcon;

        if (spellData == null && curSpellBtnEnum == SpellBtnEnum.LeftBtn)//���� �����Ͱ� �����鼭 ����Ʈ�� ��� ��ư ��Ȱ��ȭ 
        {
            spellBtnIcon.gameObject.SetActive(false);
            GetComponent<Button>().interactable = false;
            GetComponent<Outline>().enabled = false;
        }

    }

    #region ���� ��ư Ŭ��
    public void OnClick()
    {
        if (curSpellBtnEnum == SpellBtnEnum.LeftBtn || curSpellBtnEnum == SpellBtnEnum.SelectBtn) 
        {
            //���� ��ư ������ �ٲٱ�
            selectManager.selectedSpellIcon.sprite = spellData.spellIcon;
            //���� �̸� �ٲٱ�
            selectManager.selectedSpellName.text = spellData.spellName;
            //���� Ÿ�� �ٲٱ�
            selectManager.selectedSpellType.text = "Ÿ��: " + 1;//SpellData.spellType
            //���� ��� �ٲٱ�
            selectManager.selectedSpellValue.text = "���: " + spellData.spellValue;
            //���� ���� �ٲٱ�
            selectManager.selectedSpellDesc.text = "����: " + spellData.spellDesc;

            if (curSpellBtnEnum == SpellBtnEnum.LeftBtn) 
            {
                bool isWork = true;

                foreach (SpellButton spellBtn in selectManager.spellBtnArr)
                {
                    if (spellBtn.originSpellBtn == GetComponent<SpellButton>())//�̹� ������ ��ư�̿��ٸ�
                    {
                        //üũ ������ ����
                        spellBtnCheck.SetActive(false);

                        //�θ� ����
                        spellBtn.originSpellBtn = null;

                        //���� ������ ����
                        spellBtn.spellData = null;

                        //�̹��� ����
                        spellBtn.spellBtnIcon.sprite = null;

                        isWork = false;
                        break;
                    }
                }

                if (isWork)
                {
                    foreach (SpellButton spellBtn in selectManager.spellBtnArr)
                    {
                        if (spellBtn.originSpellBtn == null)//�����ڰ� ���ٸ�
                        {
                            //üũ ������ ����
                            spellBtnCheck.SetActive(true);

                            //�θ� ����
                            spellBtn.originSpellBtn = GetComponent<SpellButton>();

                            //���� ������ ����
                            spellBtn.spellData = spellData;

                            //�̹��� ����
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
