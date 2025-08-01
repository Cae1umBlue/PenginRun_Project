using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public enum SFXType
{
    Jump, 
    Slide,
    Hit,
    Fall, // 맵 밖으로 추락시 사운드
    Coin,
    item,
    Heal,
    UIClick,
    ScoreUI
}

public class SoundManager : MonoBehaviour
{
    [Header("Mixer")]
    [SerializeField] private AudioMixer mixer;

    [Header("BGM")]
    [SerializeField] private AudioClip bgmClip;
    private AudioSource bgmPlayer;
    public float bgmVolume;

    [Header("SFX")]
    [SerializeField] private AudioClip[] sfxList;
    private AudioSource[] sfxPlayers;
    public int channels;
    public float sfxVolume;
    private int channelIndex;

    public static SoundManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Init();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Init()
    {
        // 배경음 플레이어 초기화
        GameObject bgmObject = new GameObject("BgmPlayer"); // bgm을 재생하는 오브젝트 생성
        bgmObject.transform.parent = transform;
        bgmPlayer = bgmObject.AddComponent<AudioSource>();
        bgmPlayer.playOnAwake = false;
        bgmPlayer.loop = true;
        bgmPlayer.volume = bgmVolume;
        bgmPlayer.clip = bgmClip;
        bgmPlayer.outputAudioMixerGroup = mixer.FindMatchingGroups("BGM")[0];


        // 효과음 플레이어 초기화
        GameObject sfxObject = new GameObject("SfxPlayer"); // SFX를 재생하는 오브젝트 생성
        sfxObject.transform.parent = transform;
        sfxPlayers = new AudioSource[channels];

        for(int i = 0; i < channels; i++) // 채널 수 만큼 반복
        {
            sfxPlayers[i] = sfxObject.AddComponent<AudioSource>();
            sfxPlayers[i].playOnAwake = false;
            sfxPlayers[i].volume = sfxVolume;
            sfxPlayers[i].outputAudioMixerGroup = mixer.FindMatchingGroups("SFX")[0];

        }
    }

    public void SFXPlay(SFXType type) // 효과음 재생 
    {
        for (int i = 0; i < sfxList.Length; i++)
        {
            int loopIndex = (i + channelIndex) % sfxPlayers.Length;

            if (sfxPlayers[loopIndex] == null)
                continue;

            if (sfxPlayers[loopIndex].isPlaying)
                continue;

            channelIndex = loopIndex;
            sfxPlayers[loopIndex].clip = sfxList[(int)type];
            sfxPlayers[loopIndex].PlayOneShot(sfxList[(int)type]);
            break;
        }
    }

    public void BGMPlay(bool isPlay) 
    {
        if (isPlay)
        {
            bgmPlayer.Play();
        }
        else
        {
            bgmPlayer.Stop();
        }
    }


    public void BGMVolume(float val)
    {
        mixer.SetFloat("BGMVolume", Mathf.Log10(val) * 20);
    }

    public void SFXVolume(float val)
    {
        mixer.SetFloat("SFXVolume", Mathf.Log10(val) * 20);
    }
}

