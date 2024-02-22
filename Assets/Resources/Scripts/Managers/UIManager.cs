using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [Header("ī�޶� ��ü")]
    public Transform cameraObj;
    [Header("ī�޶� ��ġ")]
    public Transform cameraParent;
    [Header("�� 1")]
    public Transform cameraTarget1;
    [Header("�� 2")]
    public Transform cameraTarget2;

    int rot;//���� ȸ����
    int addRot = 0;//��ư���� ȸ���� �� ����ϴ� ����
    int mul = 35;//ī�޶� ȸ�� ����
    Vector3 cameraVec;

    private void Start()
    {
        cameraParent.transform.position = Vector3.up * fly + (cameraTarget1.transform.position + cameraTarget2.transform.position) / 2f;
    }

    public int fly;

    private void Update()
    {
        rot += addRot;

        //ī�޶� ��ġ ����
        cameraVec = mul * new Vector3(Mathf.Sin(Mathf.PI * rot / 360), 0, Mathf.Cos(Mathf.PI * rot / 360));
        cameraObj.position = cameraParent.position + cameraVec;

        //ī�޶� ���ϵ��� ����
        cameraObj.LookAt((cameraTarget1.transform.position + cameraTarget2.transform.position) / 2f);
    }

    //��ư���� ī�޶� ����
    public void CameraSpin(int _spin) => addRot = _spin;

    //[CreateAssetMenu(fileName = "SingleInfoData", menuName = "Scriptable Ojbect/SingleInfo")]
    //ScriptableObject//��ũ��Ÿ�� ������Ʈ ���

    #region ���� �߻�

    #endregion
}
