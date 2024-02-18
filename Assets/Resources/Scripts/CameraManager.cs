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
    int mul = 25;//카메라 회전 배율
    Vector3 cameraVec;

    private void Update()
    {
        //카메라 위치 관리
        cameraVec = mul * new Vector3(Mathf.Sin(Mathf.PI * rot / 360), 0, Mathf.Cos(Mathf.PI * rot / 360));
        cameraObj.position = cameraParent.position + cameraVec;

        //시야 관리
        cameraObj.LookAt((cameraTarget1.transform.position + cameraTarget2.transform.position) / 2f);
    }


    #region 난사 발사

    #endregion
}
