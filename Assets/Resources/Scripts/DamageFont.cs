using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageFont : MonoBehaviour
{
    [Header("������ ��Ʈ�� �ִ� ����")]
    public float maxTime;
    [Header("������ ��Ʈ�� ���� ����")]
    float curTime;

    public ParticleSystem[] particleSystems = new ParticleSystem[3];


    public void ReName(string _name) 
    {
        name = _name;
        gameObject.SetActive(true);

        //Debug.Log("�̸� ����"+transform.childCount);

        //particleSystems[0] = transform.GetChild(1).transform.GetComponent<ParticleSystem>();
        //particleSystems[1] = transform.GetChild(2).transform.GetComponent<ParticleSystem>();
        //particleSystems[2] = transform.GetChild(3).transform.GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        curTime += Time.deltaTime;
        //Debug.Log("���" + transform.childCount + '/' + curTime);
    }



}
