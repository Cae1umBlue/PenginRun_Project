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
    public AudioMixer mixer;
    public AudioSource bgSound;
    public AudioClip[] bgList;
    public static SoundManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
            SceneManager.sceneLoaded += OnSceneLoaded; // 씬 이동시 배경음 메서드 추가
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1) // 씬에 따라 배경음악 변경
    {
        for(int i =0; i < bgList.Length; i++)
        {
            if(arg0.name == bgList[i].name)
                BgSoundPlay(bgList[i]);
        }
    }

    public void SFXPlay(SFXType type, AudioClip clip) // 효과음 재생 ex) SFXPlay("Jump", clip) clip은 인스펙터 창에서 직접 넣기 
    {
        GameObject go = new GameObject(type + "Sound"); // 소리를 재생하는 오브젝트 생성
        AudioSource audioSource = go.AddComponent<AudioSource>(); // 오브젝트에 AudioSource 컴포넌트 추가
        audioSource.outputAudioMixerGroup = mixer.FindMatchingGroups("SFX")[0];
        audioSource.clip = clip;
        audioSource.Play();

        Destroy(go, clip.length); // 효과음이 끝나면 소리 오브젝트 삭제
    }

    public void BgSoundPlay(AudioClip clip) // 배경음 재생(재생할 배경음 삽입)
    {
        bgSound.outputAudioMixerGroup = mixer.FindMatchingGroups("BGM")[0];
        bgSound.clip = clip; 
        bgSound.loop = true; // 반복
        bgSound.volume = 0.1f; // 배경음 볼륨
        bgSound.Play();
    }

}

