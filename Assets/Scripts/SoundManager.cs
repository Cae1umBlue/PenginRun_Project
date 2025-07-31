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

    public void SFXPlay(SFXType type, AudioClip clip) // ȿ���� ��� ex) SFXPlay("Jump", clip) clip�� �ν����� â���� ���� �ֱ� 
    {
        AudioSource audioSource = go.AddComponent<AudioSource>(); // ������Ʈ�� AudioSource ������Ʈ �߰�
        audioSource.outputAudioMixerGroup = mixer.FindMatchingGroups("SFX")[0];
        audioSource.clip = clip;
        audioSource.Play();

        Destroy(go, clip.length); // ȿ������ ������ �Ҹ� ������Ʈ ����
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

