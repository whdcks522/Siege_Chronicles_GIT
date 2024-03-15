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

   
    public Slider PlayerResourceSlider;



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

        //��ų ���� �̵�
        //MapClick();

        //�ڿ� ������ ����
        PlayerResourceSlider.value = blueTowerManager.curTowerResource / blueTowerManager.maxTowerResource;
        //���� ���� �����ֱ�
        for (int i = 0; i < spellBtnArr.Length; i++)
        {
            if (spellBtnArr[i].spellData != null)
            {
                spellBtnArr[i].spellBtnIcon.fillAmount = blueTowerManager.curTowerResource / spellBtnArr[i].spellData.spellValue;
            }
        }

    }

    public void OnClick(int _index) 
    {
        SpellData spellData = spellBtnArr[_index].GetComponent<SpellButton>().spellData;

        //���
        int value = spellData.spellValue;

        if (blueTowerManager.curTowerResource >= value)
        {
            blueTowerManager.curTowerResource -= value;

            if (spellData.spellType == SpellData.SpellType.Creature)
            {
                blueTowerManager.SpawnCreature(spellData.spellPrefab.name);
            }
        }
        else if (blueTowerManager.curTowerResource < value) //���� ���ڶ�
        {
            //���� ȿ����
        }

    }

    #region �� Ŭ���ϱ�
    [Header("Ŭ���� ��")]
    public Transform clickPoint;

    [Header("���� �����ư �迭")]
    public SpellButton[] spellBtnArr = new SpellButton[4];
    /*
    void MapClick()
    {
        int layerMask = LayerMask.GetMask("MainMap"); // "Default" ���̾�� �浹�ϵ��� ����

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask, QueryTriggerInteraction.Ignore))
            // Ʈ���Ŵ� �����Ѵ�
            clickPoint.position = hit.point;

        //Ÿ�� ���̴� ����
        blueTowerManager.RadarControl(clickPoint);

        if (Input.GetMouseButton(0) && curPlayerResource >= spellData.spellValue)//��Ŭ�� spellData.spellType == SpellData.SpellType.Weapon
        {
            //���� ��� ó��
            curPlayerResource -= spellData.spellValue;

            //���⺰ �Լ� ����
            switch (spellData.spellPrefab.name) 
            {
                case "Tower_Gun":
                    MapClick_Gun();
                    break;
                case "Tower_Flame":
                    MapClick_Flame();
                    break;
                default:
                    Debug.LogError("�ƹ��͵� ����");
                    break;
            }
        }
    }
    */
    #endregion

    #region �̴ϰ�
    void MapClick_Gun()
    {
        Debug.Log("Gun");

        return;

        GameObject bullet = objectManager.CreateObj("Tower_Gun", ObjectManager.PoolTypes.BulletPool);
        Bullet bullet_bullet = bullet.GetComponent<Bullet>();
        Rigidbody bullet_rigid = bullet.GetComponent<Rigidbody>();

        bullet_bullet.gameManager = gameManager;
        bullet_bullet.Init();


        //�̵�
        bullet.transform.position = cameraCloud.position;
        //����
        bullet_rigid.velocity = (clickPoint.position - bullet.transform.position).normalized * bullet_bullet.bulletSpeed;

        //Ȱ��ȭ
        bullet_bullet.BulletOnByTower(Creature.TeamEnum.Blue);
    }
    #endregion

    #region ���̾
    void MapClick_Flame() 
    {
        GameObject bullet = objectManager.CreateObj("Tower_Flame", ObjectManager.PoolTypes.BulletPool);
        Bullet bullet_bullet = bullet.GetComponent<Bullet>();
        Rigidbody bullet_rigid = bullet.GetComponent<Rigidbody>();

        bullet_bullet.gameManager = gameManager;
        bullet_bullet.Init();


        //�̵�
        bullet.transform.position = cameraCloud.position;
        //����
        bullet_rigid.velocity = (clickPoint.position - bullet.transform.position).normalized * bullet_bullet.bulletSpeed;

        //Ȱ��ȭ
        bullet_bullet.BulletOnByTower(Creature.TeamEnum.Blue);
    }
    #endregion

}
