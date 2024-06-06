using CartoonFX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageFont : MonoBehaviour
{
    [Header("������ ��Ʈ�� �ִ� ����")]
    public float maxTime;
    float curTime;//������ ��Ʈ�� ���� ����

    public void ReName(string _name) //�̸� �����ϸ�, Ȱ��ȭ
    {
        name = _name;
        curTime = 0;
        gameObject.SetActive(true);
    }

    private void FixedUpdate()
    {
        curTime += Time.deltaTime;

        if(curTime > maxTime) //�ð� ������ �ڵ������� ��Ȱ��ȭ
        {
            gameObject.SetActive(false);
        }
    }
}