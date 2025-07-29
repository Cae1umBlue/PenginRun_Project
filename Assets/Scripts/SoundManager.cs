using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 제작 목록
BGM
배경음악 재생 : BGM 오브젝트
SFX
캐릭터
    1. 점프
    2. 슬라이딩
    3. 장애물 피격
    4. 추락
UISound
클릭(선택)
점수창
*/


public class SoundManager : MonoBehaviour
{
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

    public void SFXPlay(string sfxName, AudioClip clip) // 효과음 재생 ("Jump", clip)
    {
        GameObject go = new GameObject(sfxName + "Sound"); // 소리를 재생하는 오브젝트 생성
        AudioSource audioSource = go.AddComponent<AudioSource>(); // 오브젝트에 AudioSource 컴포넌트 추가
        audioSource.clip = clip;
        audioSource.Play();

        Destroy(go, clip.length); // 효과음이 끝나면 소리 오브젝트 삭제
    }
}

