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


    AudioSource[] sfxPlayers;//Sfx �÷��̾� �迭
    [Header("���� Sfx �÷��̾��� ����")]
    public int channels;
    int curIndex;//���� ���� �� �� �÷��̾� ��ȣ
    
    public enum Sfx
    {

        TowerCrash, Gun, Flame, GrandCure,
        Paper, Spawn, Upgrade, Warning, Win, Lose
    }

    [Header("Ÿ�� �ı� Sfx")]
    public AudioClip[] towerCrashSfxClips;
    [Header("��� Sfx")]
    public AudioClip[] gunSfxClips;
    [Header("ȭ���� Sfx")]
    public AudioClip[] flameSfxClips;
    [Header("��ȸ�� Sfx")]
    public AudioClip[] grandCureSfxClips;

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
    public void PlaySfx(Sfx sfx)
    {
        if (!gameManager.isSound) 
        {
            for (int index = 0; index < sfxPlayers.Length; index++)
            {
                int loopIndex = (index + curIndex) % sfxPlayers.Length;//�ֱٿ� ����� �ε������� 0���� �����ذ��� ������ �� Ž��
                if (sfxPlayers[loopIndex].isPlaying) continue;//�������̶�� continue

                AudioClip[] tmpSfxClips = null;

                switch (sfx)
                {
                    case Sfx.TowerCrash:
                        tmpSfxClips = towerCrashSfxClips;
                        break;
                    case Sfx.Gun:
                        tmpSfxClips = gunSfxClips;
                        break;
                    case Sfx.Flame:
                        tmpSfxClips = flameSfxClips;
                        break;
                    case Sfx.GrandCure:
                        tmpSfxClips = grandCureSfxClips;
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
