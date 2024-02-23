using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ObjectManager;

public class UIManager : MonoBehaviour
{
    [Header("카메라 객체")]
    public Transform cameraObj;
    [Header("카메라 위치")]
    public Transform cameraParent;
    [Header("파란 성의 생성 지점")]
    public Transform blueCameraTarget;
    [Header("빨간 성의 생성 지점")]
    public Transform redCameraTarget;

    int mul = 45;//카메라 회전 속도
    int curRot = -160;//현재 회전값
    int addRot = 0;//버튼으로 회전할 때 사용하는 논리값
    Vector3 cameraVec;//카메라 회전용 벡터

    int fly = 50;//카메라를 하늘에서 띄운 정도

    public GameManager gameManager;
    ObjectManager objectManager;

    private void Awake()
    {
        objectManager = gameManager.objectManager;
    }

    private void Start()
    {
        //시작 할 때, 카메라 위치 고정
        cameraParent.transform.position = Vector3.up * fly + (blueCameraTarget.position + redCameraTarget.transform.position) / 2f;

        //생명체 생성
        GameObject a = objectManager.CreateObj("Infantry_A", PoolTypes.CreaturePool);
        a.transform.position = blueCameraTarget.position;
    }

    

    private void Update()
    {
        curRot += addRot;

        //카메라 위치 관리
        cameraVec = mul * new Vector3(Mathf.Sin(Mathf.PI * curRot / 360), 0, Mathf.Cos(Mathf.PI * curRot / 360));
        cameraObj.position = cameraParent.position + cameraVec;

        //카메라가 향하도록 관리
        cameraObj.LookAt((blueCameraTarget.position + redCameraTarget.position) / 2f);
    }

    //버튼으로 카메라 조작
    public void CameraSpin(int _spin) => addRot = _spin;

    //[CreateAssetMenu(fileName = "SingleInfoData", menuName = "Scriptable Ojbect/SingleInfo")]
    //ScriptableObject//스크립타블 오브젝트 상속

    #region 스펠 버튼 클릭
    public void spellBtn()
    {

    }
    #endregion
}
