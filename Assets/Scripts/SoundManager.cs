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
    GameOver,
    Item,
    UIClick,
    Score
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

    public static SoundManager Instance;

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

        for(int i = 0; i < sfxList.Length; i++)
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

}

