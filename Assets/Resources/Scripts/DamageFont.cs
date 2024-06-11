using CartoonFX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageFont : MonoBehaviour
{
    [Header("데미지 폰트의 최대 수명")]
    public float maxTime;
    float curTime;//데미지 폰트의 현재 수명

    public void ReName(string _name) //이름 변경하며, 활성화
    {
        // 자식 오브젝트를 부모 오브젝트의 자식 목록의 맨 아래로 이동시킵니다.
        transform.SetAsLastSibling();

        //이름 변경
        name = _name;
        curTime = 0;
        gameObject.SetActive(true);
    }

    private void FixedUpdate()
    {
        curTime += Time.deltaTime;

        if(curTime > maxTime) //시간 지나면 자동적으로 비활성화
        {
            gameObject.SetActive(false);
        }
    }
}
