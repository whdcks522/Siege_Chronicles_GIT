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
    }

    #region ��ư���� ī�޶� ����
    public void CameraSpin(int _spin) => addRot = _spin;

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

    private void Update()
    {
        //���̵� ȭ��ǥ ��ư ������ ī�޶� ȸ��
        curRot += addRot * 2;

        //ī�޶� ��ġ ����(������ ���� ����)
        cameraVec = mul * new Vector3(Mathf.Sin(Mathf.PI * curRot / 360), 0, Mathf.Cos(Mathf.PI * curRot / 360));
        cameraObj.position = cameraCloud.position + cameraVec;

        //ī�޶� ���ϵ��� ����
        cameraObj.LookAt((blueTower.position + redTower.position) / 2f);

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
        if(isFocus)
            ShowWeaponArea();
    }

    #region ���� ������ ���� ��ư Ŭ��

    [Header("���� �����ư �迭")]
    public SpellButton[] spellBtnArr = new SpellButton[4];
    public void OnClick(int _index) 
    {
        SpellData spellData = spellBtnArr[_index].GetComponent<SpellButton>().spellData;

        //���
        int value = spellData.spellValue;

        if (blueTowerManager.curTowerResource >= value)//����� ����� ���
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

        //��ư ��Ȱ��ȭ
        for (int i = 0; i < spellBtnArr.Length; i++)
        {
            spellBtnArr[i].gameObject.SetActive(!_isFocus);
        }
    }
    #endregion

    #region ���� ���� ǥ��;

    public Transform clickPoint;//Ŭ���� ��

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

        if (Input.GetMouseButton(0))//��Ŭ�� spellData.spellType == SpellData.SpellType.Weapon
        {
            blueTowerManager.WeaponSort(recentSpellData.spellPrefab.name);
            FocusControl(false, true);
        }
        else if (Input.GetMouseButton(1))//��Ŭ�� spellData.spellType == SpellData.SpellType.Weapon
        {
            FocusControl(false, false);
        }
    }
    
    #endregion

}
