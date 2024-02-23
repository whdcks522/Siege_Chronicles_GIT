using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ObjectManager;

public class UIManager : MonoBehaviour
{
    [Header("ī�޶� ��ü")]
    public Transform cameraObj;
    [Header("ī�޶� ��ġ")]
    public Transform cameraParent;
    [Header("�Ķ� ���� ���� ����")]
    public Transform blueCameraTarget;
    [Header("���� ���� ���� ����")]
    public Transform redCameraTarget;

    int mul = 45;//ī�޶� ȸ�� �ӵ�
    int curRot = -160;//���� ȸ����
    int addRot = 0;//��ư���� ȸ���� �� ����ϴ� ����
    Vector3 cameraVec;//ī�޶� ȸ���� ����

    int fly = 50;//ī�޶� �ϴÿ��� ��� ����

    public GameManager gameManager;
    ObjectManager objectManager;

    private void Awake()
    {
        objectManager = gameManager.objectManager;
    }

    private void Start()
    {
        //���� �� ��, ī�޶� ��ġ ����
        cameraParent.transform.position = Vector3.up * fly + (blueCameraTarget.position + redCameraTarget.transform.position) / 2f;

        //����ü ����
        GameObject a = objectManager.CreateObj("Infantry_A", PoolTypes.CreaturePool);
        a.transform.position = redCameraTarget.position;
    }

    

    private void Update()
    {
        curRot += addRot;

        //ī�޶� ��ġ ����
        cameraVec = mul * new Vector3(Mathf.Sin(Mathf.PI * curRot / 360), 0, Mathf.Cos(Mathf.PI * curRot / 360));
        cameraObj.position = cameraParent.position + cameraVec;

        //ī�޶� ���ϵ��� ����
        cameraObj.LookAt((blueCameraTarget.position + redCameraTarget.position) / 2f);
    }

    //��ư���� ī�޶� ����
    public void CameraSpin(int _spin) => addRot = _spin;

    //[CreateAssetMenu(fileName = "SingleInfoData", menuName = "Scriptable Ojbect/SingleInfo")]
    //ScriptableObject//��ũ��Ÿ�� ������Ʈ ���

    #region ���� ��ư Ŭ��
    public void spellBtn()
    {

    }
    #endregion
}
