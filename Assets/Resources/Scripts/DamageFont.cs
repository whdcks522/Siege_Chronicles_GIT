using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageFont : MonoBehaviour
{
    [Header("데미지 폰트의 최대 수명")]
    public float maxTime;
    [Header("데미지 폰트의 현재 수명")]
    float curTime;

    public ParticleSystem[] particleSystems = new ParticleSystem[3];


    public void ReName(string _name) 
    {
        name = _name;
        gameObject.SetActive(true);

        //Debug.Log("이름 변경"+transform.childCount);

        //particleSystems[0] = transform.GetChild(1).transform.GetComponent<ParticleSystem>();
        //particleSystems[1] = transform.GetChild(2).transform.GetComponent<ParticleSystem>();
        //particleSystems[2] = transform.GetChild(3).transform.GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        curTime += Time.deltaTime;
        //Debug.Log("상시" + transform.childCount + '/' + curTime);
    }



}
