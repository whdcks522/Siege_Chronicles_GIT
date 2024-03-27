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
        SetResolution(); // �ʱ⿡ ���� �ػ� ����
    }

     //�ػ� �����ϴ� �Լ� 
    void SetResolution()
    {
        int setWidth = 1920; // ����� ���� �ʺ�
        int setHeight = 1080; // ����� ���� ����

        int deviceWidth = Screen.width; // ��� �ʺ� ����
        int deviceHeight = Screen.height; // ��� ���� ����

        Screen.SetResolution(setWidth, (int)(((float)deviceHeight / deviceWidth) * setWidth), true); // SetResolution �Լ� ����� ����ϱ�

        if ((float)setWidth / setHeight < (float)deviceWidth / deviceHeight) // ����� �ػ� �� �� ū ���
        {
            float newWidth = ((float)setWidth / setHeight) / ((float)deviceWidth / deviceHeight); // ���ο� �ʺ�
            Camera.main.rect = new Rect((1f - newWidth) / 2f, 0f, newWidth, 1f); // ���ο� Rect ����
        }
        else // ������ �ػ� �� �� ū ���
        {
            float newHeight = ((float)deviceWidth / deviceHeight) / ((float)setWidth / setHeight); // ���ο� ����
            Camera.main.rect = new Rect(0f, (1f - newHeight) / 2f, 1f, newHeight); // ���ο� Rect ����
        }
    }
    */
}
