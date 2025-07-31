using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

/* ���� �ؾ��� ���
BGM
������� ��� : BGM ������Ʈ
SFX
ĳ����
    1. ����
    2. �����̵�
    3. ��ֹ� �ǰ�
    4. �߶�(?)
UISound
Ŭ��(����)
����â
*/

public enum SFXType
{
    Jump, 
    Slide,
    Hit,
    GameOver,
    Win,
    Item,
    UIClick,
    Score
}

public class SoundManager : MonoBehaviour
{
    [Header("Mixer")]
    [SerializeField] private AudioMixer mixer;

    [Header("#BGM")]
    [SerializeField] private AudioSource bgmPlayer;
    [SerializeField] private AudioClip bgmClip;
    public float bgmVolume;

    [Header("#SFX")]
    [SerializeField] private AudioSource[] sfxPlayers;
    [SerializeField] private AudioClip[] sfxList;
    public int channels;
    public float sfxVolume;
    int channelIndex;

    public static SoundManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
            Init();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Init()
    {
        // ����� �÷��̾� �ʱ�ȭ
        GameObject bgmObject = new GameObject("BgmPlayer"); // bgm�� ����ϴ� ������Ʈ ����
        bgmObject.transform.parent = transform;
        bgmPlayer = bgmObject.AddComponent<AudioSource>();
        bgmPlayer.playOnAwake = false;
        bgmPlayer.loop = true;
        bgmPlayer.volume = bgmVolume;
        bgmPlayer.clip = bgmClip;

        // ȿ���� �÷��̾� �ʱ�ȭ
        GameObject sfxObject = new GameObject("SfxPlayer"); // bgm�� ����ϴ� ������Ʈ ����
        sfxObject.transform.parent = transform;
        sfxPlayers = new AudioSource[channels];

        for(int i = 0; i < sfxList.Length; i++)
        {
            sfxPlayers[i] = sfxObject.AddComponent<AudioSource>();
            sfxPlayers[i].playOnAwake = false;
            sfxPlayers[i].volume = sfxVolume;
        }


    }

    public void SFXPlay(SFXType type) // ȿ���� ��� ex) SFXPlay("Jump", clip) clip�� �ν����� â���� ���� �ֱ� 
    {
        for (int i = 0; i < sfxList.Length; i++)
        {
            int loopIndex = (i + channelIndex) % sfxPlayers.Length;

            if (sfxPlayers[loopIndex].isPlaying)
                continue;

            channelIndex = loopIndex;
            sfxPlayers[loopIndex].clip = sfxList[(int)type];
            sfxPlayers[loopIndex].outputAudioMixerGroup = mixer.FindMatchingGroups("SFX")[0];
            sfxPlayers[loopIndex].Play();
            break;
        }
    }

    public void BgSoundPlay(AudioClip clip) // ����� ���(����� ����� ����)
    {
        bgmPlayer.outputAudioMixerGroup = mixer.FindMatchingGroups("BGM")[0];
        bgmPlayer.clip = clip; 
        bgmPlayer.loop = true; // �ݺ�
        bgmPlayer.volume = 0.1f; // ����� ����
        bgmPlayer.Play();
    }

}

