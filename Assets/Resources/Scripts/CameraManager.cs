using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public Transform cameraObj;
    public Transform cameraParent;
    public Transform cameraTarget1;
    public Transform cameraTarget2;

    public int rot;
    int mul = 25;//ī�޶� ȸ�� ����
    Vector3 cameraVec;

    private void Start()
    {
        
    }

    public int fly = 40;

    private void Update()
    {
        cameraParent.transform.position = Vector3.up * fly + (cameraTarget1.transform.position + cameraTarget2.transform.position) / 2f;

        //ī�޶� ��ġ ����
        cameraVec = mul * new Vector3(Mathf.Sin(Mathf.PI * rot / 360), 0, Mathf.Cos(Mathf.PI * rot / 360));
        cameraObj.position = cameraParent.position + cameraVec;

        //�þ� ����
        cameraObj.LookAt((cameraTarget1.transform.position + cameraTarget2.transform.position) / 2f);
    }


    #region ���� �߻�

    #endregion
}
