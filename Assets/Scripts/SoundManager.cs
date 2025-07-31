using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

/* 제작 해야할 목록
BGM
배경음악 재생 : BGM 오브젝트
SFX
캐릭터
    1. 점프
    2. 슬라이딩
    3. 장애물 피격
    4. 추락(?)
UISound
클릭(선택)
점수창
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
        // 배경음 플레이어 초기화
        GameObject bgmObject = new GameObject("BgmPlayer"); // bgm을 재생하는 오브젝트 생성
        bgmObject.transform.parent = transform;
        bgmPlayer = bgmObject.AddComponent<AudioSource>();
        bgmPlayer.playOnAwake = false;
        bgmPlayer.loop = true;
        bgmPlayer.volume = bgmVolume;
        bgmPlayer.clip = bgmClip;

        // 효과음 플레이어 초기화
        GameObject sfxObject = new GameObject("SfxPlayer"); // bgm을 재생하는 오브젝트 생성
        sfxObject.transform.parent = transform;
        sfxPlayers = new AudioSource[channels];

        for(int i = 0; i < sfxList.Length; i++)
        {
            sfxPlayers[i] = sfxObject.AddComponent<AudioSource>();
            sfxPlayers[i].playOnAwake = false;
            sfxPlayers[i].volume = sfxVolume;
        }


    }

    public void SFXPlay(SFXType type) // 효과음 재생 ex) SFXPlay("Jump", clip) clip은 인스펙터 창에서 직접 넣기 
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

    public void BgSoundPlay(AudioClip clip) // 배경음 재생(재생할 배경음 삽입)
    {
        bgmPlayer.outputAudioMixerGroup = mixer.FindMatchingGroups("BGM")[0];
        bgmPlayer.clip = clip; 
        bgmPlayer.loop = true; // 반복
        bgmPlayer.volume = 0.1f; // 배경음 볼륨
        bgmPlayer.Play();
    }

}

