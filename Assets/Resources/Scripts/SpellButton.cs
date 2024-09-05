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

    [Header("���� ��ư�� ��Ŀ�� ���� üũ")]
    public Image spellBtnFocus;



    [Header("���� ��ư�� ���̴�")]
    public Image spellBtnShader;

    [Header("���� ��ư�� ������ �̹���")]
    public Image spellBtnIcon;

    [Header("���� ��ư�� �ʷϻ� üũ")]
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

        //if (spellData == null && curSpellBtnEnum == SpellBtnEnum.LeftBtn)//���� �����Ͱ� �����鼭 ���� ��ư�� ����� ��� ��ư ��Ȱ��ȭ 
            //ButtonOff();
        
        if (curSpellBtnEnum == SpellBtnEnum.BattleBtn) 
        {
            //���� ��ư�� üũ ��ũ �Ⱦ�
            Destroy(spellBtnCheck);
        }
    }

    public void ButtonOff(bool isDestroy) //��ư ��Ȱ��ȭ
    {
        if (isDestroy)//�ı��ϴ� ���
        {
            //���̴� ����
            if (spellBtnShader != null)
                Destroy(spellBtnShader.gameObject);

            //��� �ؽ�Ʈ ����
            Destroy(spellBtnValue.gameObject);

            //������ �̹��� ����
            Destroy(spellBtnIcon.gameObject);

            //��Ŀ�� �̹��� ����
            Destroy(spellBtnFocus.gameObject);
        }
        else //��Ȱ��ȭ �ϴ� ���
        {
            //��� �ؽ�Ʈ ��Ȱ��ȭ
            spellBtnValue.gameObject.SetActive(false);

            //������ �̹��� ��Ȱ��ȭ
            spellBtnIcon.gameObject.SetActive(false);
        }

        //��ư �̹��� ��Ȱ��ȭ
        GetComponent<Button>().interactable = false;
        GetComponent<Outline>().enabled = false;
    }

    #region ������ �̹��� ����
    public void IconChange(SpellButton tmpSpellBtn)//���������� �Ⱦ�
    {
        //��Ŀ�� �̹��� Ȱ��ȭ
        tmpSpellBtn.spellBtnFocus.gameObject.SetActive(false);

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
            }
            else//�ּ��� ���
            {
                tmpSpellBtn.spellBtnIcon.color = Color.black;

                if (tmpSpellBtn.spellData.isFocus) 
                {
                    //��Ŀ�� �̹��� Ȱ��ȭ
                    tmpSpellBtn.spellBtnFocus.gameObject.SetActive(true);
                }
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

    #region �������� ������ ���翡 ���� ���� ������
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

            if (spellData.spellType == SpellData.SpellType.Creature)//ũ���ĸ� ������� �ϴ� ���
            {
                //���� Ÿ�� �̹��� ����
                selectManager.selectedSpellTypeImage.sprite = selectManager.spellTypeImageArr[0].sprite;
                selectManager.selectedSpellTypeImage.color = selectManager.spellTypeImageArr[0].color;

                //���� Ÿ�� �ؽ�Ʈ ����
                selectManager.selectedSpellTypeText.text = "ĳ����";

                //���� ���� �ٲٱ�(ü��, ����:���ط�: �̵��ӵ�, ��Ŀ�� ����, ��Ÿ�)
                Creature spellCreature = spellData.spellPrefab.GetComponent<Creature>();
                Bullet spellCreatureBullet = spellCreature.useBullet.GetComponent<Bullet>();
                int spellCreatureBulletDamage = spellCreatureBullet.bulletDamage;
                string rangeType = "<color=#227744>���Ÿ�</color>";
                if (spellCreature.bulletStartPoint == null) 
                {
                    rangeType = "<color=#CC5555>����</color>";
                }
                //ü��, ���ݷ�, ��Ÿ�, �̵��ӵ�
                selectManager.selectedSpellInfo.text =
                "<color=#FF7070>ü��</color>: " + spellCreature.maxHealth + "\n" +
                "<color=#4E9D61>���ݷ�</color>: " + spellCreatureBulletDamage / 3 + '/' + spellCreatureBulletDamage / 2 + '/' + spellCreatureBulletDamage + "(<color=#55CC55>���̵���</color>)" + "\n" +
                "<color=#B684FF>���� ��Ÿ�</color>: " + spellCreature.maxRange + "(" + rangeType + ")\n" +
                "<color=#406BFF>�⵿��</color>: " + spellCreature.nav.speed;
            }
            else//������ ������� �ϴ� ���
            {
                //���� Ÿ�� �̹��� ����
                selectManager.selectedSpellTypeImage.sprite = selectManager.spellTypeImageArr[1].sprite;
                selectManager.selectedSpellTypeImage.color = selectManager.spellTypeImageArr[1].color;

                //���� Ÿ�� �ؽ�Ʈ ����
                selectManager.selectedSpellTypeText.text = "����";

                //���� ���� �ٲٱ�(ü��, ����:���ط�: �̵��ӵ�, ��Ŀ�� ����, ��Ÿ�)
                Bullet spellWeaponBullet = spellData.spellPrefab.GetComponent<Bullet>();
                int spellWeaponBulletDamage = spellWeaponBullet.bulletDamage;
                int spellWeaponBulletRad = (int)spellWeaponBullet.transform.localScale.x;
                if (spellWeaponBulletDamage == 0 && spellWeaponBullet.endBullet != null) //���ذ� 0�̸鼭 �ڽ� �Ѿ��� �ִ� ���(ȭ����)
                {
                    //�ڽ� �Ѿ˷� ��ü
                    spellWeaponBulletDamage = spellWeaponBullet.endBullet.GetComponent<Bullet>().bulletDamage;

                    spellWeaponBulletRad = (int)spellWeaponBullet.endBullet.GetComponent<Bullet>().transform.localScale.x;
                }

                //���ݷ�, �ǰ� ���, �ǰ� ����, ��Ŀ�� ����
                if (spellWeaponBullet.curBulletEffectEnum == Bullet.BulleEffectEnum.Damage)
                {
                    selectManager.selectedSpellInfo.text = "<color=#4E9D61>���ݷ�</color>: " +
                            spellWeaponBulletDamage / 3 + "/" + spellWeaponBulletDamage / 2 + "/" + spellWeaponBulletDamage + "(<color=#55CC55>���̵���</color>)" + "\n";
                }
                else
                {
                    selectManager.selectedSpellInfo.text = "<color=#FB6EFF>ġ����</color>: " + spellWeaponBulletDamage + "\n";
                }

                selectManager.selectedSpellInfo.text += "<color=#B684FF>�ǰ� ����</color>: ";
                if (spellWeaponBullet.bulletTarget != null)
                    selectManager.selectedSpellInfo.text += "���� " + "\n";
                else
                    selectManager.selectedSpellInfo.text += "����(<color=#770077>" + spellWeaponBulletRad + "</color>)\n";

                selectManager.selectedSpellInfo.text += "<color=#406BFF>�巡�� ����</color>: ";
                if (spellData.isFocus)
                {
                    selectManager.selectedSpellInfo.text += "<color=#AA0000>O</color>";
                }
                else 
                {
                    selectManager.selectedSpellInfo.text += "X";
                }
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
