using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [Header("카메라 객체")]
    public Transform cameraObj;
    [Header("카메라 위치")]
    public Transform cameraParent;
    [Header("성 1")]
    public Transform cameraTarget1;
    [Header("성 2")]
    public Transform cameraTarget2;

    int rot;//현재 회전값
    int addRot = 0;//버튼으로 회전할 때 사용하는 논리값
    int mul = 35;//카메라 회전 배율
    Vector3 cameraVec;

    private void Start()
    {
        cameraParent.transform.position = Vector3.up * fly + (cameraTarget1.transform.position + cameraTarget2.transform.position) / 2f;
    }

    public int fly;

    private void Update()
    {
        rot += addRot;

        //카메라 위치 관리
        cameraVec = mul * new Vector3(Mathf.Sin(Mathf.PI * rot / 360), 0, Mathf.Cos(Mathf.PI * rot / 360));
        cameraObj.position = cameraParent.position + cameraVec;

        //카메라가 향하도록 관리
        cameraObj.LookAt((cameraTarget1.transform.position + cameraTarget2.transform.position) / 2f);
    }

    //버튼으로 카메라 조작
    public void CameraSpin(int _spin) => addRot = _spin;

    //[CreateAssetMenu(fileName = "SingleInfoData", menuName = "Scriptable Ojbect/SingleInfo")]
    //ScriptableObject//스크립타블 오브젝트 상속

    #region 난사 발사

    #endregion
}
