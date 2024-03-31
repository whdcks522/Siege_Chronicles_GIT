using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    #region 초기화
    private void Awake()
    {
        if (!gameManager.isML)
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
    }
    #endregion

    //Bgm 플레이어
    AudioSource bgmPlayer;
    public enum Bgm { Start, Select, Battle }
    [Header("멀티 Bgm")]
    public AudioClip[] BgmClips;

    #region BGM 재생
    public void PlayBgm(Bgm _bgm)//스태틱 BGM 재생
    {
        if (!gameManager.isML) 
        {
            bgmPlayer.Stop();
            switch (_bgm)
            {
                case Bgm.Start:
                    bgmPlayer.clip = BgmClips[0];
                    //bgmPlayer.volume = 0.5f;
                    break;
                case Bgm.Select:
                    bgmPlayer.clip = BgmClips[1];
                    break;
                case Bgm.Battle:
                    bgmPlayer.clip = BgmClips[2];
                    break;
            }
            bgmPlayer.Play();
        }    
    }
    #endregion

    #region SFX 재생

    AudioSource[] sfxPlayers;//Sfx 플레이어 배열
    [Header("만들 Sfx 플레이어의 개수")]
    public int channels;
    int curIndex;//현재 실행 중 인 플레이어 번호

    public enum Sfx//효과음의 종류
    {
        //타워나 생명체 피격
        TowerCrashSfx,
        //총알
        GunSfx, FlameSfx, GrandCureSfx,
        //UI
        PaperSfx, LevelControlSfx, spellSuccessSfx, spellFailSfx, BankSfx, SpeedSfx,
        //승리와 패배
        WinSfx, LoseSfx
    }

    [Header("타워 파괴 Sfx")]
    public AudioClip[] towerCrashSfxClips;
    [Header("사격 Sfx")]
    public AudioClip[] gunSfxClips;
    [Header("화염구 Sfx")]
    public AudioClip[] flameSfxClips;
    [Header("대회복 Sfx")]
    public AudioClip[] grandCureSfxClips;

    [Header("종이 Sfx")]
    public AudioClip[] paperSfxClips;
    [Header("레벨 조절 Sfx")]
    public AudioClip[] levelControlSfxClips;
    [Header("스펠 성공 Sfx")]
    public AudioClip[] spellSuccessSfxClips;
    [Header("스펠 실패 Sfx")]
    public AudioClip[] spellFailSfxClips;
    [Header("은행 Sfx")]
    public AudioClip[] bankSfxClips;
    [Header("가속 Sfx")]
    public AudioClip[] speedSfxClips;

    [Header("승리 Sfx")]
    public AudioClip[] winSfxClips;
    [Header("패배 Sfx")]
    public AudioClip[] loseSfxClips;

    AudioClip[] tmpSfxClips;
    public void PlaySfx(Sfx sfx)
    {
        if (!gameManager.isML) 
        {
            for (int index = 0; index < sfxPlayers.Length; index++)
            {
                int loopIndex = (index + curIndex) % sfxPlayers.Length;//최근에 사용한 인덱스에서 0부터 증가해가며 가능한 것 탐색
                if (sfxPlayers[loopIndex].isPlaying)//실행중이라면 continue
                    continue;

                tmpSfxClips = null;

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
                    case Sfx.LevelControlSfx:
                        tmpSfxClips = levelControlSfxClips;
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

                int sfxIndex = tmpSfxClips.Length == 1 ? 0 : Random.Range(0, tmpSfxClips.Length);
                //int sfxIndex = Random.Range(0, tmpSfxClips.Length);
                Debug.Log(sfxIndex);

                curIndex = loopIndex;
                sfxPlayers[loopIndex].clip = tmpSfxClips[sfxIndex];//(int)sfx
                sfxPlayers[loopIndex].Play();
                break;
            }
        }
    }
    #endregion

    [Header("매니저")]
    public GameManager gameManager;
}
