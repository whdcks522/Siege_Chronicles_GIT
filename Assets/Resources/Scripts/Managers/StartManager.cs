using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartManager : MonoBehaviour
{
    [Header("����ȭ�� �ִϸ��̼�")]
    public Animator startAnim;

    [Header("�Ŵ���")]
    public GameManager gameManager;
    AudioManager audioManager;

    private void Start()
    {
        audioManager = gameManager.audioManager;

        //���� ȭ�� ��� ���� ���
        audioManager.PlayBgm(AudioManager.Bgm.StartBgm);
    }

    public void StartGame()//���� ��ư ����
    {
        startAnim.SetBool("isStart", false);

        //���� �ѱ�� ȿ����
        audioManager.PlaySfx(AudioManager.Sfx.PaperSfx);
    }
}
