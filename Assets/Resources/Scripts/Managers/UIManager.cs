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

   

    //�÷��̾��� ���� �ڿ�
    float curPlayerResource = 0f;
    //�÷��̾��� �ִ� �ڿ�
    float maxPlayerResource = 10f;

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

    private void Start()
    {
        //����ü ����
        //GameObject a = objectManager.CreateObj("Infantry_A", PoolTypes.CreaturePool);
        //a.transform.position = redCameraTarget.position;
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
        if (maxPlayerResource > curPlayerResource ) 
        {
            //���� ����� ���� �ڿ� ����
            curPlayerResource += Time.deltaTime;
        }
        else if (maxPlayerResource <= curPlayerResource)
        {
            //���� �ڿ����� �ִ�ġ�� ���� �ʵ���
            maxPlayerResource = curPlayerResource;
        }
        PlayerResourceSlider.value = curPlayerResource / maxPlayerResource;



        //���̵� ȭ��ǥ ��ư ������ ī�޶� ȸ��
        curRot += addRot * 2;

        //ī�޶� ��ġ ����(������ ���� ����)
        cameraVec = mul * new Vector3(Mathf.Sin(Mathf.PI * curRot / 360), 0, Mathf.Cos(Mathf.PI * curRot / 360));
        cameraObj.position = cameraCloud.position + cameraVec;

        //ī�޶� ���ϵ��� ����
        cameraObj.LookAt((blueTower.position + redTower.position) / 2f);

        //��ų ���� �̵�
        MapClick();
    }

    #region �� Ŭ���ϱ�
    [Header("Ŭ���� ��")]
    public Transform clickPoint;

    public SpellData spellData;
    
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
                case "Tower_Flame":
                    MapClick_Flame();
                    break;
                default:
                    Debug.LogError("�ƹ��͵� ����");
                    break;
            }
        }
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
