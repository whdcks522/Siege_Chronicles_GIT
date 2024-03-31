using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    //Bgm �÷��̾�
    AudioSource bgmPlayer;
    public enum BgmMulty { Lobby, PvP }
    [Header("��Ƽ Bgm")]
    public AudioClip[] multyBgmClips;

    [Header("�Ŵ���")]
    public GameManager gameManager;

    #region �ʱ�ȭ
    private void Awake()
    {
        //����� �÷��̾� �ʱ�ȭ
        GameObject bgmObject = new GameObject("BgmPlayer");
        bgmObject.transform.parent = transform;
        bgmPlayer = bgmObject.AddComponent<AudioSource>();//bgmPlayer�� �����ϸ鼭 ���ÿ� ������Ʈ ����
        bgmPlayer.playOnAwake = false;
        bgmPlayer.loop = true;

        //ȿ���� �÷��̾� �ʱ�ȭ
        GameObject sfxObject = new GameObject("SfxPlayers");
        sfxObject.transform.parent = transform;
        sfxPlayers = new AudioSource[channels];//Audio Source �迭 �ʱ�ȭ
        for (int index = 0; index < channels; index++)
        {
            sfxPlayers[index] = sfxObject.AddComponent<AudioSource>();
            sfxPlayers[index].playOnAwake = false;
        }
    }
    #endregion

    public void StopBgm() //��� ���� ���߱�
    {
        bgmPlayer.Stop();
    }
    /*
    public void PlayBgm(BgmStatic _bgm)//����ƽ BGM ���
    {

        bgmPlayer.Stop();
        switch (_bgm)
        {
            case BgmStatic.Home:
                bgmPlayer.clip = staticBgmClips[0];
                //bgmPlayer.volume = 0.5f;
                break;
        }
        bgmPlayer.Play();
    }
    */


    #region SFX ���

    AudioSource[] sfxPlayers;//Sfx �÷��̾� �迭
    [Header("���� Sfx �÷��̾��� ����")]
    public int channels;
    int curIndex;//���� ���� �� �� �÷��̾� ��ȣ

    public enum Sfx
    {
        TowerCrashSfx,
        GunSfx, FlameSfx, GrandCureSfx,
        PaperSfx, spellSuccessSfx, spellFailSfx, BankSfx, WinSfx, LoseSfx
    }

    [Header("Ÿ�� �ı� Sfx")]
    public AudioClip[] towerCrashSfxClips;
    [Header("��� Sfx")]
    public AudioClip[] gunSfxClips;
    [Header("ȭ���� Sfx")]
    public AudioClip[] flameSfxClips;
    [Header("��ȸ�� Sfx")]
    public AudioClip[] grandCureSfxClips;
    [Header("���� �ѱ�� Sfx")]
    public AudioClip[] paperSfxClips;
    [Header("���� ���� Sfx")]
    public AudioClip[] spellSuccessSfxClips;
    [Header("���� ���� Sfx")]
    public AudioClip[] spellFailSfxClips;
    [Header("���� Sfx")]
    public AudioClip[] bankSfxClips;
    [Header("�¸� Sfx")]
    public AudioClip[] winSfxClips;
    [Header("�¸� Sfx")]
    public AudioClip[] loseSfxClips;
    public void PlaySfx(Sfx sfx)
    {
        if (!gameManager.isML) 
        {
            for (int index = 0; index < sfxPlayers.Length; index++)
            {
                int loopIndex = (index + curIndex) % sfxPlayers.Length;//�ֱٿ� ����� �ε������� 0���� �����ذ��� ������ �� Ž��
                if (sfxPlayers[loopIndex].isPlaying) continue;//�������̶�� continue

                AudioClip[] tmpSfxClips = null;

                switch (sfx)
                {
                    case Sfx.TowerCrashSfx:
                        tmpSfxClips = towerCrashSfxClips;
                        break;
                    case Sfx.GunSfx:
                        tmpSfxClips = gunSfxClips;
                        break;
                    case Sfx.FlameSfx:
                        tmpSfxClips = flameSfxClips;
                        break;
                    case Sfx.GrandCureSfx:
                        tmpSfxClips = grandCureSfxClips;
                        break;
                    case Sfx.PaperSfx:
                        tmpSfxClips = paperSfxClips;
                        break;
                    case Sfx.spellSuccessSfx:
                        tmpSfxClips = spellSuccessSfxClips;
                        break;
                    case Sfx.spellFailSfx:
                        tmpSfxClips = spellFailSfxClips;
                        break;
                    case Sfx.BankSfx:
                        tmpSfxClips = bankSfxClips;
                        break;
                    case Sfx.WinSfx:
                        tmpSfxClips = winSfxClips;
                        break;
                    case Sfx.LoseSfx:
                        tmpSfxClips = loseSfxClips;
                        break;

                    default:
                        Debug.Log("Sfx ����");
                        break;
                }

                int sfxIndex = Random.Range(0, tmpSfxClips.Length);

                curIndex = loopIndex;
                sfxPlayers[loopIndex].clip = tmpSfxClips[sfxIndex];//(int)sfx
                sfxPlayers[loopIndex].Play();
                break;
            }
        }
    }
    #endregion
}
