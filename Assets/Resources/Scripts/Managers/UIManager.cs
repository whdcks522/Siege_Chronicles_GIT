using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    //bool isFocus;

    //�ֱٿ� ����� ������ ������
    public SpellData curSpellData;

    [Header("�Ŵ���")]
    public SelectManager selectManager;
    public GameManager gameManager;
    ObjectManager objectManager;
    AudioManager audioManager;

    private void Awake()
    {
        objectManager = gameManager.objectManager;
        audioManager = gameManager.audioManager;
        blueTower = gameManager.blueTower;
        blueTowerManager = blueTower.GetComponent<TowerManager>();  
        redTower = gameManager.redTower;

        //���� �� ��, ī�޶� ��ġ ����
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

        //���� �ʱ�ȭ
        blueTowerManager.curBankIndex = 0;
        bankText.text = "Lv." + (blueTowerManager.curBankIndex + 1) + "(" + blueTowerManager.BankValueArr[blueTowerManager.curBankIndex] + ")";

        //��� �ʱ�ȭ
        if (speed == 1) 
        {
            SpeedControl();
            SpeedControl();
        }
        else if (speed == 2)
        {
            SpeedControl();
        }
    }
    #endregion

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
            audioManager.PlaySfx(AudioManager.Sfx.spellFailSfx);
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
            if (clickPoint.gameObject.activeSelf)
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
    Vector3 clickScaleVec;
    public void OnClick(int _index) 
    {
        //Ŭ���� ��ư�� ���
        SpellData spellData = spellBtnArr[_index].GetComponent<SpellButton>().spellData;

        //���
        int value = spellData.spellValue;

        if (blueTowerManager.curTowerResource >= value && curSpellData == null)//����� ����� ���
        {
            //���� ���� ȿ����
            audioManager.PlaySfx(AudioManager.Sfx.spellSuccessSfx);

            //��� ����
            blueTowerManager.curTowerResource -= value;

            if (spellData.spellType == SpellData.SpellType.Creature)//ũ���ĸ� ���� ���
            {
                blueTowerManager.SpawnCreature(spellData.spellPrefab.name);
            }
            else if(spellData.spellType == SpellData.SpellType.Weapon)//���⸦ ���� ���
            {
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
            audioManager.PlaySfx(AudioManager.Sfx.spellFailSfx);
        }
    }
    #endregion

    #region ���� ��ư ���
    [Header("���� ���")]
    public GameObject settingBackground;
    public void SettingControl(bool isOpen) 
    {
        //��� ����
        settingBackground.SetActive(isOpen);

        //�ð� ����
        if (isOpen)
            Time.timeScale = 0.001f;
        else if (!isOpen)
        {
            SpeedControl();
            SpeedControl();
            SpeedControl();
        }
    }
    #endregion



    #region ��Ŀ�� ���� ��ȯ
    [Header("������ ��")]
    public GameObject worldLight;

    public void FocusOn()
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
    public void FocusOff(bool isEffect) //�ڿ� ��ȯ ����
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
        SpeedControl();
        SpeedControl();
        SpeedControl();
    }
    #endregion


    #region ���� ���� ǥ��;

    [Header("Ŭ�� ��Ŀ�� ���� ��ҵ�")]
    public Transform clickPoint;//Ŭ���� ��
    public Transform clickSphere;//Ŭ���� ���� ���� ����
    public Material clickMat;//Ŭ���� ���� ���� ������ ���͸��� 

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

        //��Ŀ�� UI ���� ����
        //focusVec = cameraObj.transform.position - cameraGround.transform.position;
        //lookRotation = Quaternion.LookRotation(focusVec);
        //focusCanvas.transform.rotation = lookRotation;
    }
    //��Ŀ���� ĵ���� ȸ����
    //Vector3 focusVec;
    //Quaternion lookRotation;

    #endregion
}
