using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    #region �ʱ�ȭ
    private void Awake()
    {
        uiManager = gameManager.uiManager;

        if (!gameManager.isML)
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
    }
    #endregion

    //Bgm �÷��̾�
    AudioSource bgmPlayer;
    public enum Bgm { Start, Select, Battle }
    [Header("��Ƽ Bgm")]
    public AudioClip[] BgmClips;

    #region BGM ���
    public void PlayBgm(Bgm _bgm)//����ƽ BGM ���
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

    #region SFX ���

    AudioSource[] sfxPlayers;//Sfx �÷��̾� �迭
    [Header("���� Sfx �÷��̾��� ����")]
    public int channels;//�� ���� ��� ȿ������ ��� ��������
    int curIndex;//���� ���� �� �� �÷��̾� ��ȣ

    public enum Sfx//ȿ������ ����
    {
        //Ÿ���� ����ü �ǰ�
        TowerCrashSfx,
        //�Ѿ�
        GunSfx, FlameSfx, GrandCureSfx,
        //UI
        PaperSfx, LevelControlSfx, SpellSuccessSfx, SpellFailSfx, BankSfx, SpeedSfx,
        //�¸��� �й�
        WinSfx, LoseSfx
    }

    [Header("Ÿ�� �ı� Sfx")]
    public AudioClip[] towerCrashSfxClips;
    [Header("��� Sfx")]
    public AudioClip[] gunSfxClips;
    [Header("ȭ���� Sfx")]
    public AudioClip[] flameSfxClips;
    [Header("��ȸ�� Sfx")]
    public AudioClip[] grandCureSfxClips;

    [Header("���� Sfx")]
    public AudioClip[] paperSfxClips;
    [Header("���� ���� Sfx")]
    public AudioClip[] levelControlSfxClips;
    [Header("���� ���� Sfx")]
    public AudioClip[] spellSuccessSfxClips;
    [Header("���� ���� Sfx")]
    public AudioClip[] spellFailSfxClips;
    [Header("���� Sfx")]
    public AudioClip[] bankSfxClips;
    [Header("���� Sfx")]
    public AudioClip[] speedSfxClips;

    [Header("�¸� Sfx")]
    public AudioClip[] winSfxClips;
    [Header("�й� Sfx")]
    public AudioClip[] loseSfxClips;

    AudioClip[] tmpSfxClips;
    public void PlaySfx(Sfx sfx)
    {
        if (!gameManager.isML)
        {
            if (uiManager.settingBackground.activeSelf && sfx != Sfx.PaperSfx)
                return;

            for (int index = 0; index < sfxPlayers.Length; index++)
            {
                int loopIndex = (index + curIndex) % sfxPlayers.Length;//�ֱٿ� ����� �ε������� 0���� �����ذ��� ������ �� Ž��
                if (sfxPlayers[loopIndex].isPlaying)//�������̶�� continue
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

                //ȿ������ �迭�� ũ�Ⱑ 1�� �ƴ� ��츸 �������� ����
                int sfxIndex = tmpSfxClips.Length == 1 ? 0 : Random.Range(0, tmpSfxClips.Length);

                //���� ���� �ִ� �÷��̾�� ȿ���� ���
                curIndex = loopIndex;
                sfxPlayers[loopIndex].clip = tmpSfxClips[sfxIndex];//(int)sfx
                sfxPlayers[loopIndex].Play();
                break;
            }
        }
    }
    #endregion

    [Header("�Ŵ���")]
    public GameManager gameManager;
    UIManager uiManager;
}
