using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellButton : MonoBehaviour
{
    public enum SpellBtnEnum {LeftBtn, RightBtn, SelectBtn, BattleBtn }//� ������ ���� ��ư����
    [Header("���� ��ư�� ����")]
    public SpellBtnEnum curSpellBtnEnum;

    [Header("���� ��ư�� ��� �ؽ�Ʈ")]
    public Text spellBtnValue;

    [Header("���� ��ư�� ���̴�")]
    public Image spellBtnShader;

    [Header("���� ��ư�� ������ �̹���")]
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


        if (spellData != null)//���� �����Ͱ� ������
        {
            IconChange(this);
        }

        if (spellData == null && curSpellBtnEnum == SpellBtnEnum.LeftBtn)//���� �����Ͱ� �����鼭 ���� ��ư�� ����� ��� ��ư ��Ȱ��ȭ 
        {
            ButtonOff();
        }
    }

    public void ButtonOff() //��ư ��Ȱ��ȭ
    {
        //��� �ؽ�Ʈ ����
        spellBtnValue.gameObject.SetActive(false);
        //������ �̹��� ����
        spellBtnIcon.gameObject.SetActive(false);

        //��ư �̹��� ��Ȱ��ȭ
        GetComponent<Button>().interactable = false;
        GetComponent<Outline>().enabled = false;
    }

    //public Renderer spellRenderer;

    #region ������ �̹��� ����
    public void IconChange(SpellButton tmpSpellBtn)//���������� �Ⱦ�
    {
        if (tmpSpellBtn.spellData != null) //���� �����Ͱ� �ִٸ�
        {
            //��� �ؽ�Ʈ ����
            tmpSpellBtn.spellBtnValue.gameObject.SetActive(true);
            tmpSpellBtn.spellBtnValue.text = tmpSpellBtn.spellData.spellValue.ToString();

            //������ �̹��� ����
            tmpSpellBtn.spellBtnIcon.sprite = spellData.spellIcon;

            if (spellData.spellType == SpellData.SpellType.Creature)//ũ������ ���
            {
                tmpSpellBtn.spellBtnIcon.color = Color.white;

                //�����ư ���͸��� ��ȭ
                //tmpSpellBtn.spellBtnShader.material = gameManager.SpellCreatureMat;
            }
            else //if (spellData.spellType == SpellData.SpellType.Weapon) //�ּ��� ���
            {
                tmpSpellBtn.spellBtnIcon.color = Color.black;

                //���� ��ư ���͸��� ��ȭ
                //spellBtnShader.material = gameManager.
                //tmpSpellBtn.spellBtnShader.material = gameManager.SpellWeaponMat;
            }
        }
        else if (tmpSpellBtn.spellData == null)//���� �����Ͱ� ���ٸ�
        {
            //��� �ؽ�Ʈ ����
            tmpSpellBtn.spellBtnValue.gameObject.SetActive(false);
            //������ �̹��� ����
            tmpSpellBtn.spellBtnIcon.sprite = null;
            //�� �ʱ�ȭ
            tmpSpellBtn.spellBtnIcon.color = Color.white;
        }
    }
    #endregion

    #region ���� ��ư Ŭ��
    public void OnClick()
    {
        if (curSpellBtnEnum != SpellBtnEnum.BattleBtn)
        {
            spellBtnValue.GetComponent<Animator>().SetBool("isFlash", true);
        }

        if ((curSpellBtnEnum == SpellBtnEnum.LeftBtn || curSpellBtnEnum == SpellBtnEnum.SelectBtn) && spellData != null) 
        {
            selectManager.selectedSpellBtn.spellData = spellData;

            //���� ��ư ������ �ٲٱ�
            IconChange(selectManager.selectedSpellBtn);

            //���� �̸� �ٲٱ�
            selectManager.selectedSpellName.text = spellData.spellName;

            //���� Ÿ�� �ٲٱ�
            selectManager.selectedSpellTypeImage.gameObject.SetActive(true);
            if (spellData.spellType == SpellData.SpellType.Creature) 
            {
                //���� Ÿ�� �ؽ�Ʈ ����
                selectManager.selectedSpellTypeText.text = "ũ����";
                //���� Ÿ�� �̹��� ����
                selectManager.selectedSpellTypeImage.sprite = selectManager.spellTypeImageArr[0];
            }
            else// if (spellData.spellType == SpellData.SpellType.Weapon)
            {
                //���� Ÿ�� �ؽ�Ʈ ����
                selectManager.selectedSpellTypeText.text = "�ּ�";
                //���� Ÿ�� �̹��� ����
                selectManager.selectedSpellTypeImage.sprite = selectManager.spellTypeImageArr[1];
            }

            //���� ��� �ٲٱ�
            selectManager.selectedSpellValue.text = spellData.spellValue.ToString();
            //���� ���� �ٲٱ�
            selectManager.selectedSpellDesc.text = spellData.spellDesc;

            if (curSpellBtnEnum == SpellBtnEnum.LeftBtn)
            {
                bool isWork = true;

                foreach (SpellButton spellBtn in selectManager.spellBtnArr)
                {
                    if (spellBtn.originSpellBtn == GetComponent<SpellButton>())//�̹� ������ ��ư�̿��ٸ�
                    {
                        //���� ��ư Ŭ�� ���� ȿ���� ���
                        audioManager.PlaySfx(AudioManager.Sfx.SpellFailSfx);

                        //üũ ������ ����
                        spellBtnCheck.SetActive(false);

                        //�θ� ����
                        spellBtn.originSpellBtn = null;

                        //���� ������ ����
                        spellBtn.spellData = null;

                        //�̹��� ����
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
                        if (spellBtn.originSpellBtn == null)//�����ڰ� ���ٸ�
                        {
                            //���� ��ư Ŭ�� ���� ȿ���� ���
                            audioManager.PlaySfx(AudioManager.Sfx.SpellSuccessSfx);

                            //üũ ������ ����
                            spellBtnCheck.SetActive(true);

                            //�θ� ����
                            spellBtn.originSpellBtn = GetComponent<SpellButton>();

                            //���� ������ ����
                            spellBtn.spellData = spellData;

                            //�̹��� ����
                            spellBtn.spellData = spellData;
                            IconChange(spellBtn);

                            break;
                        }
                    }
                }
            }
            else if (curSpellBtnEnum == SpellBtnEnum.SelectBtn) 
            {
                //���� ��ư Ŭ�� ���� ȿ���� ���
                audioManager.PlaySfx(AudioManager.Sfx.PaperSfx);
            }
        }
    }
    #endregion
}
