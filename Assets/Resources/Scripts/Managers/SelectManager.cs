using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using static SpellData;

public class SelectManager : MonoBehaviour
{
    [Header("���õ� ���� ������")]
    public Image selectedSpellIcon;

    [Header("���õ� ���� �̸�")]
    public Text selectedSpellName;

    [Header("���õ� ���� Ÿ��")]
    public Text selectedSpellType;

    [Header("���õ� ���� ���")]
    public Text selectedSpellValue;

    [Header("���õ� ���� ����")]
    public Text selectedSpellDesc;

    [Header("���õ� �����ư �迭")]
    public SpellButton[] spellBtnArr = new SpellButton[4];

    [Header("�Ŵ���")]
    public GameManager gameManager;
    AudioManager audioManager;

    

    void Awake()
    {
        audioManager = gameManager.audioManager;
    }

    public void StartGame()//���� ����
    {
        gameObject.SetActive(false);
        Time.timeScale = 1;
    }

    public void RestartGame()//���� �ʱ�ȭ
    {
        SceneManager.LoadScene(0);
    }


}
