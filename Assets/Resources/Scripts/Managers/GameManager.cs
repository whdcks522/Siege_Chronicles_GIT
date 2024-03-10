using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("����� ��")]
    public Transform blueTower;
    [Header("������ ��")]
    public Transform redTower;


    

    [Header("�Ŵ��� ���")]
    public ObjectManager objectManager;
    public UIManager uiManager;
    public AudioManager audioManager;

    [Header("�ӽŷ���������")]
    public bool isML;
}
