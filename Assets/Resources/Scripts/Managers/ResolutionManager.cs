using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResolutionManager : MonoBehaviour
{
    public Camera mainCamera;

    void Awake() 
    {
        var r = mainCamera.rect;
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
        mainCamera.rect = r;

    }
}
