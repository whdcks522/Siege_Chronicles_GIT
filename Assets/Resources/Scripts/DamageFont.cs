using CartoonFX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageFont : MonoBehaviour
{
    [Header("데미지 폰트의 최대 수명")]
    public float maxTime;
    [Header("데미지 폰트의 현재 수명")]
    float curTime;



    public void ReName(string _name) //이름 변경하며, 활성화
    {
        name = _name;
        curTime = 0;
        gameObject.SetActive(true);
    }

    private void FixedUpdate()
    {
        //curTime += Time.deltaTime;

        //if(curTime  maxTime) 
        {
            //gameObject.SetActive(false);
        }
    }
}
