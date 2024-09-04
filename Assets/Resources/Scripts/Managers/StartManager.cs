using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartManager : MonoBehaviour
{
    [Header("시작화면 애니메이션")]
    public Animator startAnim;

    [Header("매니저")]
    public GameManager gameManager;
    AudioManager audioManager;

    private void Start()
    {
        audioManager = gameManager.audioManager;

        //시작 화면 배경 음악 재생
        audioManager.PlayBgm(AudioManager.Bgm.StartBgm);
    }

    public void StartGame()//시작 버튼 누름
    {
        startAnim.SetBool("isStart", false);

        //종이 넘기는 효과음
        audioManager.PlaySfx(AudioManager.Sfx.PaperSfx);
    }
}
