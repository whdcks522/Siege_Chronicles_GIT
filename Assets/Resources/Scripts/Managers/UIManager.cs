using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("ī�޶� ��ü")]
    public Transform cameraObj;
    [Header("ī�޶� ��ġ")]
    public Transform cameraCloud;
    [Header("ī�޶� �ٶ󺸴� ����")]
    public Transform cameraGround;

    Transform blueTower;//�Ķ� ���� ���� ����
    TowerManager blueTowerManager;//�Ķ� ���� ��ũ��Ʈ

    Transform redTower;//���� ���� ���� ����

    int mul = 45;//ī�޶� ȸ�� �ӵ�
    int curRot = -160;//���� ī�޶� ȸ����
    int addRot = 0;//��ư���� ȸ���� �� ����ϴ� ����
    Vector3 cameraVec;//ī�޶� ȸ���� ����

    int fly = 50;//ī�޶� �ϴÿ��� ��� ����


    [Header("���� UI")]
    public Slider PlayerResourceSlider;//�÷��̾��� �ڿ� �����̴�
    public Text PlayerResourceText;//�÷��̾��� �ڿ� �ؽ�Ʈ

    //�ֱٿ� ����� ������ ������
    public SpellData curSpellData;

    [Header("�Ŵ���")]
    public SelectManager selectManager;
    public GameManager gameManager;
    AudioManager audioManager;

    private void Awake()
    {
        audioManager = gameManager.audioManager;
        blueTower = gameManager.blueTower;
        blueTowerManager = blueTower.GetComponent<TowerManager>();  
        redTower = gameManager.redTower;

        //���� �� ��, ī�޶� �� ��ġ Ȯ��
        cameraGround.transform.position = (blueTower.position + redTower.transform.position) / 2f;
        cameraCloud.transform.position = Vector3.up * fly + cameraGround.position;
    }

    //��ư���� ī�޶� ����
    public void CameraSpin(int _spin) => addRot = _spin;

    #region UI ���� �ʱ�ȭ
    public void resetUI() 
    {
        //��Ŀ�� �ʱ�ȭ
        FocusOff(false);

        //ũ���� �� ���� �ʱ�ȭ
        blueTowerManager.CreatureCountText();

        //�÷��̾� ���� �ʱ�ȭ
        bankText.text = "Lv." + (blueTowerManager.curBankIndex + 1) + "(" + blueTowerManager.BankValueArr[blueTowerManager.curBankIndex] + ")";
        bankAnim.SetTrigger("isFlash");

        //��� �ʱ�ȭ
        if (speed == 1) 
        {
            SpeedControl(false);
        }

        //ī�޶� ȸ�� �ʱ�ȭ
        curRot = -160;
    }
    #endregion

    #region ��� ����
    int speed = 0;
    public Text SpeedControlText;
    public Animator SpeedAnim;
    public void SpeedControl(bool isSfx)
    {
        if (isSfx) 
        {
            //�ӵ� ���� ȿ���� ���
            audioManager.PlaySfx(AudioManager.Sfx.SpeedSfx);
        }
        SpeedAnim.SetBool("isFlash", true);

        //�ӵ� ����
        speed++;
        speed = (speed % 2);

        //�ð� ����
        Time.timeScale = (speed + 1);

        //���� ��ȯ
        SpeedControlText.text = "x" + (speed + 1);
    }
    #endregion

    #region ���� ����
    [Header("���� ���� UI")]
    public Image bankBtn;//���� �̹���
    public Text bankText;//���� �ؽ�Ʈ
    public Animator bankAnim;//���� �ִϸ��̼�
    public void BankControl()//���� ��ư Ŭ��
    {
        bankAnim.SetBool("isFlash", true);

        if (blueTowerManager.curTowerResource >= blueTowerManager.BankValueArr[blueTowerManager.curBankIndex])//����� ����� ���
        {
            //���� ȿ����
            audioManager.PlaySfx(AudioManager.Sfx.BankSfx);

            //��� ó��
            blueTowerManager.curTowerResource -= blueTowerManager.BankValueArr[blueTowerManager.curBankIndex];

            //���� ��� ����
            blueTowerManager.curBankIndex++;

            if (blueTowerManager.curBankIndex != blueTowerManager.BankValueArr.Length)//0, 1, 2, 3�� �� Ŭ���� ���
            {
                //���� �ؽ�Ʈ ����
                bankText.text = "Lv." + (blueTowerManager.curBankIndex + 1) + "(" + blueTowerManager.BankValueArr[blueTowerManager.curBankIndex] + ")";
            }
            else if (blueTowerManager.curBankIndex == blueTowerManager.BankValueArr.Length)//4�� �� Ŭ���� ���
            {
                //���� �ؽ�Ʈ ����
                bankText.text = "Lv.5(-)";
                //�̹��� ä���
                bankBtn.fillAmount = 1;
                //��ư Ŭ�� ��Ȱ��ȭ
                bankBtn.GetComponent<Button>().interactable = false;
            }   
        }
        else if (blueTowerManager.curTowerResource < blueTowerManager.BankValueArr[blueTowerManager.curBankIndex]) //����� ���ڸ� ���
        {
            //��� ���� ȿ����
            audioManager.PlaySfx(AudioManager.Sfx.SpellFailSfx);
        }
    }
    #endregion

    private void LateUpdate()
    {
        //���̵� ȭ��ǥ ��ư ������ ī�޶� ȸ��
        curRot += addRot * 2;

        //ī�޶� ��ġ ����(������ ���� ����)
        cameraVec = mul * new Vector3(Mathf.Sin(Mathf.PI * curRot / 360), 0, Mathf.Cos(Mathf.PI * curRot / 360));
        cameraObj.position = cameraCloud.position + cameraVec;

        //ī�޶� ���ϵ��� ����
        cameraObj.LookAt((blueTower.position + redTower.position) / 2f);

        if (!gameManager.isML) 
        {
            //�ڿ� ������ ����
            PlayerResourceSlider.value = blueTowerManager.curTowerResource / blueTowerManager.maxTowerResource;
            PlayerResourceText.text = blueTowerManager.curTowerResource.ToString("F1") + "/" + blueTowerManager.maxTowerResource.ToString("F0");

            //���� �ڿ� ���� �����ֱ�
            for (int i = 0; i < spellBtnArr.Length; i++)
            {
                if (spellBtnArr[i].spellData != null)
                {
                    spellBtnArr[i].spellBtnIcon.fillAmount = blueTowerManager.curTowerResource / spellBtnArr[i].spellData.spellValue;
                }
            }

            //��Ŀ�������� ��ų ���� �̵�
            if (clickPoint.gameObject.activeSelf)
                ShowWeaponArea();

            //���� ��ư Ȱ��ȭ ����
            if(blueTowerManager.curBankIndex < blueTowerManager.BankValueArr.Length && bankBtn.GetComponent<Button>().interactable)
                bankBtn.fillAmount = blueTowerManager.curTowerResource / blueTowerManager.BankValueArr[blueTowerManager.curBankIndex];
        }
    }
    [Header("ũ���� ���� �ؽ�Ʈ")]
    public Text creatureCountText;
    public Animator creatureCountAnim;

    #region ���� ������ ���� ��ư Ŭ��

    [Header("���� �����ư �迭")]
    public SpellButton[] spellBtnArr = new SpellButton[4];
    Vector3 clickScaleVec;//�ּ��� ��ų ������ �����ִµ� ���Ǵ� ����

    public void OnClick(int _index) //���� ȭ�鿡�� ���� 4���� ��ư �� 1���� Ŭ����
    {
        //Ŭ���� ��ư�� ���� ����
        SpellData spellData = spellBtnArr[_index].GetComponent<SpellButton>().spellData;
        //�ش� ������ ��� ���
        int value = spellData.spellValue;

        if (blueTowerManager.curTowerResource >= value && curSpellData == null)//����� ����ϸ鼭 ��Ŀ�� ���� �ƴ϶��
        {
            //���� ���� ȿ����
            audioManager.PlaySfx(AudioManager.Sfx.SpellSuccessSfx);

            if (spellData.spellType == SpellData.SpellType.Creature)//ũ���ĸ� ���� ���
            {
                if (blueTowerManager.CreatureCountCheck()) 
                {
                    //��� ����
                    blueTowerManager.curTowerResource -= value;
                    //�ش� ũ���� ��ȯ
                    blueTowerManager.SpawnCreature(spellData.spellPrefab.name);
                }    
            }
            else if(spellData.spellType == SpellData.SpellType.Weapon)//�ּ��� ���� ���
            {
                //��� ����
                blueTowerManager.curTowerResource -= value;

                if (spellData.isFocus) 
                {
                    //���� ������ �ӽ� ����
                    curSpellData = spellData;

                    //Ŭ�� ����Ʈ�� ���͸��� ��ȭ
                    clickMat.SetColor("_AlphaColor", spellData.focusColor);

                    //Ŭ�� ����Ʈ�� ũ�� ��ȭ
                    float size = spellData.spellPrefab.transform.localScale.x;
                    Bullet bullet = spellData.spellPrefab.GetComponent<Bullet>();

                    if (bullet.endBullet != null)//�ڽ��� ������ �ڽ��� ũ��� ����
                        size = bullet.endBullet.transform.localScale.x;

                    clickScaleVec = new Vector3(size, size, size);
                    clickSphere.localScale = clickScaleVec;
                }
                else if(!spellData.isFocus)
                {
                    blueTowerManager.WeaponSort(spellData.spellPrefab.name);
                } 
            }
        }
        else if (blueTowerManager.curTowerResource < value) //����� ���ڸ� ���
        {
            //���� ���� ȿ����
            audioManager.PlaySfx(AudioManager.Sfx.SpellFailSfx);
        }
    }
    #endregion

    #region ���� ��ư ���
    [Header("���� ���")]
    public GameObject settingBackground;//���� â ���
    public GameObject victoryTitle;//�¸� �� ���� �ؽ�Ʈ
    public GameObject defeatTitle;//�й� �� ���� �ؽ�Ʈ
    public GameObject startBtn;//�̾��ϱ� ��ư(���� ���� ��, ��Ȱ��ȭ)
    public void SettingControl(bool isOpen)//���� Ȱ��ȭ ����
    {
        //������ ȿ����
        audioManager.PlaySfx(AudioManager.Sfx.PaperSfx);

        //�̹��� ����
        settingBackground.SetActive(isOpen);

        //�ð� ����
        if (isOpen)
            Time.timeScale = 0.001f;
        else if (!isOpen)
        {
            SpeedControl(false);
            SpeedControl(false);
        }
    }
    #endregion

    #region ��Ŀ�� ���� ��ȯ
    [Header("������ ��")]
    public GameObject worldLight;

    public void FocusOn()//���� ������� �ּ� ������ ����(��Ŀ�� ����)
    {
        if (curSpellData != null)
        {
            //���� ���� Ȱ��ȭ
            clickPoint.gameObject.SetActive(true);
            //�� ��Ȱ��ȭ
            worldLight.SetActive(false);
            //�ð� ����
            Time.timeScale = 0.2f;
        }
    }
    public void FocusOff(bool isEffect) //�ڿ� ��ȯ ����(��Ŀ�� ����)
    {
        //-1: �ڿ� ��ȯ, 0: ������ ��Ȱ��ȭ, 1: ���� ��� 
        if (curSpellData != null && isEffect) 
        {
            if (!clickPoint.gameObject.activeSelf) //�ڿ� ��ȯ
            {
                blueTowerManager.curTowerResource += curSpellData.spellValue;
                curSpellData = null;
            }
            else if (clickPoint.gameObject.activeSelf) //���� ���
            {
                blueTowerManager.WeaponSort(curSpellData.spellPrefab.name);
                curSpellData = null;
            }
        }
        //���� ���� ��Ȱ��ȭ
        clickPoint.gameObject.SetActive(false);
        //�� Ȱ��ȭ
        worldLight.SetActive(true);
        //�ð� ����ȭ
        SpeedControl(false);
        SpeedControl(false);
    }
    #endregion


    #region ���� ���� ǥ��;

    [Header("Ŭ�� ��Ŀ�� ���� ��ҵ�")]
    public Transform clickPoint;//Ŭ���� ��
    public Transform clickSphere;//Ŭ���� ���� ���� ����
    public Material clickMat;//Ŭ���� ���� ���� ������ ���͸��� 

    void ShowWeaponArea()//ȭ�� ������ ��ġ�� ������
    {
        int layerMask = LayerMask.GetMask("MainMap"); // "Default" ���̾�� �浹�ϵ��� ����

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask, QueryTriggerInteraction.Ignore))
            // Ʈ���Ŵ� �����Ѵ�
            clickPoint.position = hit.point;

        //Ÿ�� ���̴� ����
        blueTowerManager.RadarControl(clickPoint.position);
    }
    #endregion
}
