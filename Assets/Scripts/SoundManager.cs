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
    [SerializeField] private AudioMixer mixer;

    [Header("#BGM")]
    [SerializeField] private AudioSource bgmPlayer;
    [SerializeField] private AudioClip bgmClip;
    public float bgmVolume;

    [Header("#SFX")]
    [SerializeField] private AudioSource sfxPlayer;
    [SerializeField] private AudioClip[] sfxList;
    public float sfxVolume;

    public static SoundManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SFXPlay(SFXType type, AudioClip clip) // 효과음 재생 ex) SFXPlay("Jump", clip) clip은 인스펙터 창에서 직접 넣기 
    {
        AudioSource audioSource = go.AddComponent<AudioSource>(); // 오브젝트에 AudioSource 컴포넌트 추가
        audioSource.outputAudioMixerGroup = mixer.FindMatchingGroups("SFX")[0];
        audioSource.clip = clip;
        audioSource.Play();

        Destroy(go, clip.length); // 효과음이 끝나면 소리 오브젝트 삭제
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

