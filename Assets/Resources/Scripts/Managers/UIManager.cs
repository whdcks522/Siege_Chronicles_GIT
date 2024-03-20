using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static ObjectManager;

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
    int curRot = -160;//���� ȸ����
    int addRot = 0;//��ư���� ȸ���� �� ����ϴ� ����
    Vector3 cameraVec;//ī�޶� ȸ���� ����

    int fly = 50;//ī�޶� �ϴÿ��� ��� ����


    [Header("���� UI")]
    public Slider PlayerResourceSlider;//�ڿ� �����̴�
    public Text PlayerResourceText;//�ڿ� �ؽ�Ʈ

    //���� Ŭ���ؼ� ���⸦ ����� �غ� �Ϸ� ����
    bool isFocus;

    //�ֱٿ� ����� ������ ������
    SpellData recentSpellData;

    [Header("�Ŵ���")]
    public SelectManager selectManager;
    public GameManager gameManager;
    ObjectManager objectManager;

    private void Awake()
    {
        objectManager = gameManager.objectManager;
        blueTower = gameManager.blueTower;
        blueTowerManager = blueTower.GetComponent<TowerManager>();  
        redTower = gameManager.redTower;

        //���� �� ��, ī�޶� ��ġ ����
        cameraGround.transform.position = (blueTower.position + redTower.transform.position) / 2f;
        cameraCloud.transform.position = Vector3.up * fly + cameraGround.position;

        //���� �ؽ�Ʈ ����
        bankText.text = "Lv." + (blueTowerManager.curBankIndex + 1) + "(" + blueTowerManager.BankValueArr[blueTowerManager.curBankIndex] + ")";
    }

    //��ư���� ī�޶� ����
    public void CameraSpin(int _spin) => addRot = _spin;

    #region ��� ����

    int speed = 0;
    public Text SpeedControlText;
    public void SpeedControl()
    {
        speed++;
        speed = (speed % 3);

        Time.timeScale = (speed + 1);

        SpeedControlText.text = "x" + (speed + 1);
    }
    #endregion

    #region ���� ����
    [Header("���� ���� UI")]
    public Image bankBtn;
    public Text bankText;
    public void BankControl() 
    {
        if (blueTowerManager.curTowerResource >= blueTowerManager.BankValueArr[blueTowerManager.curBankIndex])//����� ����� ���
        {
            //ó�� ���� ȿ����


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

            //���� ���� �����ֱ�
            for (int i = 0; i < spellBtnArr.Length; i++)
            {
                if (spellBtnArr[i].spellData != null)
                {
                    spellBtnArr[i].spellBtnIcon.fillAmount = blueTowerManager.curTowerResource / spellBtnArr[i].spellData.spellValue;
                }
            }

            //��ų ���� �̵�
            if (isFocus)
                ShowWeaponArea();

            //���� ��ư Ȱ��ȭ ����
            if(blueTowerManager.curBankIndex < blueTowerManager.BankValueArr.Length && bankBtn.GetComponent<Button>().interactable)
                bankBtn.fillAmount = blueTowerManager.curTowerResource / blueTowerManager.BankValueArr[blueTowerManager.curBankIndex];

                //ũ���� �� ���� ǥ��
                creatureCountText.text = blueTowerManager.curCreatureCount.ToString() + "/" + blueTowerManager.maxCreatureCount.ToString();
        }
    }
    [Header("ũ���� ���� �ؽ�Ʈ")]
    public Text creatureCountText;

    #region ���� ������ ���� ��ư Ŭ��

    [Header("���� �����ư �迭")]
    public SpellButton[] spellBtnArr = new SpellButton[4];
    public void OnClick(int _index) 
    {
        SpellData spellData = spellBtnArr[_index].GetComponent<SpellButton>().spellData;

        //���
        int value = spellData.spellValue;

        if (blueTowerManager.curTowerResource >= value && !isFocus)//����� ����� ���
        {
            blueTowerManager.curTowerResource -= value;

            if (spellData.spellType == SpellData.SpellType.Creature)//ũ���ĸ� ���� ���
            {
                blueTowerManager.SpawnCreature(spellData.spellPrefab.name);
            }
            else if(spellData.spellType == SpellData.SpellType.Weapon)//���⸦ ���� ���
            {
                if (spellData.isFocus) 
                {
                    recentSpellData = spellData;
                    FocusControl(true, true);
                }
                else if(!spellData.isFocus)
                {
                    blueTowerManager.WeaponSort(spellData.spellPrefab.name);
                } 
            }
        }
        else if (blueTowerManager.curTowerResource < value) //����� ���ڸ� ���
        {
            //��� ���� ȿ����
        }
    }
    #endregion

    #region ��Ŀ�� ����

    //��Ŀ�� ���� ��ȯ
    public void FocusControl(bool _isFocus, bool _isUse) //��Ŀ�� �ߴ��� ����, ����Ѱ��� ����Ѱ��� ����
    {
        if (!_isFocus && !_isUse) //��Ŀ���� ����� ���
        {
            //�ڿ� ��ȯ
            blueTowerManager.curTowerResource += recentSpellData.spellValue;
        }

        //ui ��Ȱ��ȭ
        isFocus = _isFocus;
        //���� ����
        clickPoint.gameObject.SetActive(_isFocus);
        FocusObj.gameObject.SetActive(_isFocus);

        if (_isFocus)
        {
            FocusLeftText.text = "���� '" + recentSpellData.spellName + "' ���";
            FocusRightText.text = "�ڿ� '" + recentSpellData.spellValue + "' ��ȯ";

            //��ư Ŭ�� ��Ȱ��ȭ
            for (int i = 0; i < spellBtnArr.Length; i++)
            {
                spellBtnArr[i].GetComponent<Button>().interactable = false;
            }
        }
        else if (!_isFocus)
        {
            //��ư Ŭ�� Ȱ��ȭ
            for (int i = 0; i < spellBtnArr.Length; i++)
            {
                if (spellBtnArr[i].spellData != null)
                    spellBtnArr[i].GetComponent<Button>().interactable = true;
            }
        }
    }
    #endregion

    #region ���� ���� ǥ��;

    public Transform clickPoint;//Ŭ���� ��
    public GameObject FocusObj;//��Ŀ�� ���� UI
    public Text FocusLeftText;//��Ŀ������ ��, ���� ���� �ؽ�Ʈ
    public Text FocusRightText;//��Ŀ������ ��, ���� ������ �ؽ�Ʈ
    void ShowWeaponArea()
    {
        int layerMask = LayerMask.GetMask("MainMap"); // "Default" ���̾�� �浹�ϵ��� ����

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask, QueryTriggerInteraction.Ignore))
            // Ʈ���Ŵ� �����Ѵ�
            clickPoint.position = hit.point;

        //Ÿ�� ���̴� ����
        blueTowerManager.RadarControl(clickPoint.position);

        if (Input.GetMouseButton(0))        //��Ŭ��
        {
            blueTowerManager.WeaponSort(recentSpellData.spellPrefab.name);
            FocusControl(false, true);
        }
        else if (Input.GetMouseButton(1))   //��Ŭ��
        {
            FocusControl(false, false);
        }
    }
    
    #endregion

}
