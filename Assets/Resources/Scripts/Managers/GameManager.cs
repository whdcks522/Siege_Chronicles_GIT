using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("����� ��")]
    public Transform blueTower;
    [Header("������ ��")]
    public Transform redTower;

    [Header("�ӽŷ��������� ���� Ȯ��")]
    public bool isML;

    [Header("�Ŵ��� ���")]
    public ObjectManager objectManager;
    public UIManager uiManager;
    public AudioManager audioManager;
    public AiManager aiManager;
    

    #region �ӽ� ���� ȯ�� �ʱ�ȭ
    public void ClearEnv()
    {
        
    }
    #endregion



}
