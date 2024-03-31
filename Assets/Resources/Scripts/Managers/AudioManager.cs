using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    //Bgm 플레이어
    AudioSource bgmPlayer;
    public enum BgmMulty { Lobby, PvP }
    [Header("멀티 Bgm")]
    public AudioClip[] multyBgmClips;

    [Header("매니저")]
    public GameManager gameManager;

    #region 초기화
    private void Awake()
    {
        //배경음 플레이어 초기화
        GameObject bgmObject = new GameObject("BgmPlayer");
        bgmObject.transform.parent = transform;
        bgmPlayer = bgmObject.AddComponent<AudioSource>();//bgmPlayer에 저장하면서 동시에 컴포넌트 삽입
        bgmPlayer.playOnAwake = false;
        bgmPlayer.loop = true;

        //효과음 플레이어 초기화
        GameObject sfxObject = new GameObject("SfxPlayers");
        sfxObject.transform.parent = transform;
        sfxPlayers = new AudioSource[channels];//Audio Source 배열 초기화
        for (int index = 0; index < channels; index++)
        {
            sfxPlayers[index] = sfxObject.AddComponent<AudioSource>();
            sfxPlayers[index].playOnAwake = false;
        }
    }
    #endregion

    public void StopBgm() //배경 음악 멈추기
    {
        bgmPlayer.Stop();
    }
    /*
    public void PlayBgm(BgmStatic _bgm)//스태틱 BGM 재생
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


    #region SFX 재생

    AudioSource[] sfxPlayers;//Sfx 플레이어 배열
    [Header("만들 Sfx 플레이어의 개수")]
    public int channels;
    int curIndex;//현재 실행 중 인 플레이어 번호

    public enum Sfx
    {
        TowerCrashSfx,
        GunSfx, FlameSfx, GrandCureSfx,
        PaperSfx, spellSuccessSfx, spellFailSfx, BankSfx, WinSfx, LoseSfx
    }

    [Header("타워 파괴 Sfx")]
    public AudioClip[] towerCrashSfxClips;
    [Header("사격 Sfx")]
    public AudioClip[] gunSfxClips;
    [Header("화염구 Sfx")]
    public AudioClip[] flameSfxClips;
    [Header("대회복 Sfx")]
    public AudioClip[] grandCureSfxClips;
    [Header("종이 넘기기 Sfx")]
    public AudioClip[] paperSfxClips;
    [Header("스펠 성공 Sfx")]
    public AudioClip[] spellSuccessSfxClips;
    [Header("스펠 실패 Sfx")]
    public AudioClip[] spellFailSfxClips;
    [Header("은행 Sfx")]
    public AudioClip[] bankSfxClips;
    [Header("승리 Sfx")]
    public AudioClip[] winSfxClips;
    [Header("승리 Sfx")]
    public AudioClip[] loseSfxClips;
    public void PlaySfx(Sfx sfx)
    {
        if (!gameManager.isML) 
        {
            for (int index = 0; index < sfxPlayers.Length; index++)
            {
                int loopIndex = (index + curIndex) % sfxPlayers.Length;//최근에 사용한 인덱스에서 0부터 증가해가며 가능한 것 탐색
                if (sfxPlayers[loopIndex].isPlaying) continue;//실행중이라면 continue

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
                        Debug.Log("Sfx 없음");
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
