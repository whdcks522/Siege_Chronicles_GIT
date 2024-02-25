using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("블루팀 성")]
    public Transform blueTower;
    [Header("레드팀 성")]
    public Transform redTower;


    

    [Header("매니저 목록")]
    public ObjectManager objectManager;
    public UIManager uiManager;
    public AudioManager audioManager;
    public AiManager aiManager;
    

    #region 머신 러닝 환경 초기화
    public void ClearEnv()
    {
        
    }
    #endregion



}
