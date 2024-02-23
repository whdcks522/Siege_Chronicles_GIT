using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class Creature : MonoBehaviour
{
    [Header("생명체의 최대 체력")]
    public float maxHealth;
    [Header("생명체의 현재 체력")]
    public float curHealth;

    [Header("생명체의 최대 공격 대기 시간")]
    public float maxTime;
    [Header("생명체의 현재 공격 대기 시간")]
    public float curTime;

    [Header("캐릭터 위의 미니 UI")]
    public GameObject miniUI;
    public Image miniHealth;

    public int spin, _inputY;

    Rigidbody rigid;

    //생명체의 상태 목록
    bool isDead = false;
    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }

    #region 생명체 활성화
    public void activateRPC()
    {
        curTime = 0;
        //가속 초기화
        rigid.velocity = Vector3.zero;
        //체력 회복
        curHealth = maxHealth;
        miniHealth.fillAmount = 1;

        isDead = false;

        //오브젝트 활성화
        gameObject.SetActive(true);
    }
    #endregion

    #region 이동
    public void xyRPC(int x, int y)
    {
        //_inputX = x;
        _inputY = y;
    }
    #endregion


}
