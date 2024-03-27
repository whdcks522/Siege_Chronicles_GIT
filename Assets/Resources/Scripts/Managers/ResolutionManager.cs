using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResolutionManager : MonoBehaviour
{
    void Awake() {
        var camera = GetComponent<Camera>();
        var r = camera.rect;
        var scaleheight = ((float)Screen.width / Screen.height) / (16f / 9f);
        var scalewidth = 1f / scaleheight;
        if (scaleheight < 1f) {
            r.height = scaleheight; 
            r.y = (1f - scaleheight) / 2f; 
        } 
        else 
        {
            r.width = scalewidth;
            r.x = (1f - scalewidth) / 2f; 
        }
        camera.rect = r;
    }
    ///void OnPreCull() => GL.Clear(true, true, Color.black);

    /*
    private void Start()
    {
        SetResolution(); // 초기에 게임 해상도 고정
    }

     //해상도 설정하는 함수 
    void SetResolution()
    {
        int setWidth = 1920; // 사용자 설정 너비
        int setHeight = 1080; // 사용자 설정 높이

        int deviceWidth = Screen.width; // 기기 너비 저장
        int deviceHeight = Screen.height; // 기기 높이 저장

        Screen.SetResolution(setWidth, (int)(((float)deviceHeight / deviceWidth) * setWidth), true); // SetResolution 함수 제대로 사용하기

        if ((float)setWidth / setHeight < (float)deviceWidth / deviceHeight) // 기기의 해상도 비가 더 큰 경우
        {
            float newWidth = ((float)setWidth / setHeight) / ((float)deviceWidth / deviceHeight); // 새로운 너비
            Camera.main.rect = new Rect((1f - newWidth) / 2f, 0f, newWidth, 1f); // 새로운 Rect 적용
        }
        else // 게임의 해상도 비가 더 큰 경우
        {
            float newHeight = ((float)deviceWidth / deviceHeight) / ((float)setWidth / setHeight); // 새로운 높이
            Camera.main.rect = new Rect(0f, (1f - newHeight) / 2f, 1f, newHeight); // 새로운 Rect 적용
        }
    }
    */
}
