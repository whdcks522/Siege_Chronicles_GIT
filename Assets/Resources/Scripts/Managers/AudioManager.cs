using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    #region 초기화
    private void Awake()
    {
        uiManager = gameManager.uiManager;

        //배경음 플레이어 초기화
        GameObject bgmObject = new GameObject("BgmPlayer");
        bgmObject.transform.parent = transform;
        bgmPlayer = bgmObject.AddComponent<AudioSource>();//bgmPlayer 변수로 컴포넌트 생성하면서 동시에 저장
        bgmPlayer.playOnAwake = false;

        //효과음 플레이어 초기화
        GameObject sfxObject = new GameObject("SfxPlayers");
        sfxObject.transform.parent = transform;
        sfxPlayers = new AudioSource[channels];
        for (int index = 0; index < channels; index++)
        {
            sfxPlayers[index] = sfxObject.AddComponent<AudioSource>();
            sfxPlayers[index].playOnAwake = false;
        }
    }
    #endregion



    #region BGM 재생

    //Bgm 플레이어
    AudioSource bgmPlayer;
    public enum Bgm { StartBgm, BattleBgm, WinBgm, LoseBgm}
    [Header("Bgm 배열")]
    public AudioClip[] BgmClips;

    public void PlayBgm(Bgm _bgm)//BGM 재생
    {
        bgmPlayer.Stop();

        if (gameManager.isBgm) 
        {
            switch (_bgm)
            {
                case Bgm.StartBgm:
                    bgmPlayer.clip = BgmClips[0];
                    bgmPlayer.loop = true;
                    break;
                case Bgm.BattleBgm:
                    bgmPlayer.clip = BgmClips[1];
                    bgmPlayer.loop = true;
                    break;
                case Bgm.WinBgm:
                    bgmPlayer.clip = BgmClips[2];
                    bgmPlayer.loop = false;
                    break;
                case Bgm.LoseBgm:
                    bgmPlayer.clip = BgmClips[3];
                    bgmPlayer.loop = false;
                    break;
            }
            bgmPlayer.Play();
        } 
    }
    #endregion

    #region SFX 재생

    AudioSource[] sfxPlayers;//Sfx 플레이어 배열
    [Header("만들 Sfx 플레이어의 개수")]
    public int channels;//한 번에 몇개의 효과음을 출력 가능한지
    int curIndex;//현재 실행 중 인 플레이어 번호

    public enum Sfx//효과음의 종류
    {
        //타워 피격, 타워 파괴, 크리쳐 피격, 크리쳐 사망
        TowerCrashSfx, CreatureHitSfx, CreatureDeadSfx, 
        //주술(스킬)
        GunSfx, FlameSfx, GrandCureSfx, CorpseExplosionAdaptSfx, CorpseExplosionBombSfx,
        //UI
        PaperSfx, LevelControlSfx, SpellSuccessSfx, SpellFailSfx, BankSfx, SpeedSfx,
    }

    [Header("타워 파괴 Sfx")]
    public AudioClip[] towerCrashSfxClips;
    [Header("크리쳐 피격 Sfx")]
    public AudioClip[] creatureHitSfxClips;
    [Header("크리쳐 사망 Sfx")]
    public AudioClip[] creatureDeadSfxClips;

    [Header("사격 Sfx")]
    public AudioClip[] gunSfxClips;
    [Header("화염구 Sfx")]
    public AudioClip[] flameSfxClips;
    [Header("대회복 Sfx")]
    public AudioClip[] grandCureSfxClips;
    [Header("시체폭발 적용 Sfx")]
    public AudioClip[] corpseExplosionAdaptSfxClips;
    [Header("시체폭발 폭발 Sfx")]
    public AudioClip[] corpseExplosionBombSfxClips;

    [Header("종이(버튼) Sfx")]
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

    

    AudioClip[] tmpSfxClips;
    public void PlaySfx(Sfx sfx)
    {
        if (uiManager.settingBackground.activeSelf && sfx != Sfx.PaperSfx)//설정 화면에서는 종이 넘기는 소리만 가능함
            return;

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
                case Sfx.CreatureHitSfx:
                    tmpSfxClips = creatureHitSfxClips;
                    break;
                case Sfx.CreatureDeadSfx:
                    tmpSfxClips = creatureDeadSfxClips;
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
                case Sfx.CorpseExplosionAdaptSfx:
                    tmpSfxClips = corpseExplosionAdaptSfxClips;
                    break;
                case Sfx.CorpseExplosionBombSfx:
                    tmpSfxClips = corpseExplosionBombSfxClips;
                    break;

                case Sfx.PaperSfx:
                    tmpSfxClips = paperSfxClips;
                    break;
                case Sfx.LevelControlSfx:
                    tmpSfxClips = levelControlSfxClips;
                    break;
                case Sfx.SpellSuccessSfx:
                    tmpSfxClips = spellSuccessSfxClips;
                    break;
                case Sfx.SpellFailSfx:
                    tmpSfxClips = spellFailSfxClips;
                    break;
                case Sfx.BankSfx:
                    tmpSfxClips = bankSfxClips;
                    break;
                case Sfx.SpeedSfx:
                    tmpSfxClips = speedSfxClips;
                    break;

                default:
                    Debug.Log("Sfx 없음");
                    break;
            }

            //효과음의 배열의 크기가 1이 아닌 경우만 랜덤으로 구함
            int sfxIndex = tmpSfxClips.Length == 1 ? 0 : Random.Range(0, tmpSfxClips.Length);

            //현재 쉬고 있는 플레이어에서 효과음 재생
            curIndex = loopIndex;
            sfxPlayers[loopIndex].clip = tmpSfxClips[sfxIndex];//(int)sfx
            sfxPlayers[loopIndex].Play();
            break;
        }
    }
    #endregion

    [Header("매니저")]
    public GameManager gameManager;
    UIManager uiManager;
}
