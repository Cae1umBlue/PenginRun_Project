using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum SFXType
{
    Jump, 
    Slide,
    Hit,
    Fall, // �� ������ �߶��� ����
    Coin,
    item,
    Heal,
    UIClick,
    ScoreUI
}

public class SoundManager : MonoBehaviour
{
    [Header("VolumeControl")]
    [SerializeField] private AudioMixer mixer;
    private Slider bgmSlider;
    private Slider sfxSlider;

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

    private void Init()
    {
        // ����� �÷��̾� �ʱ�ȭ
        GameObject bgmObject = new GameObject("BgmPlayer"); // bgm�� ����ϴ� ������Ʈ ����
        bgmObject.transform.parent = transform;
        bgmPlayer = bgmObject.AddComponent<AudioSource>();
        bgmPlayer.playOnAwake = false;
        bgmPlayer.loop = true;
        bgmPlayer.volume = bgmVolume;
        bgmPlayer.clip = bgmClip;
        bgmPlayer.outputAudioMixerGroup = mixer.FindMatchingGroups("BGM")[0];


        // ȿ���� �÷��̾� �ʱ�ȭ
        GameObject sfxObject = new GameObject("SfxPlayer"); // SFX�� ����ϴ� ������Ʈ ����
        sfxObject.transform.parent = transform;
        sfxPlayers = new AudioSource[channels];

        for(int i = 0; i < channels; i++) // ä�� �� ��ŭ �ݺ�
        {
            sfxPlayers[i] = sfxObject.AddComponent<AudioSource>();
            sfxPlayers[i].playOnAwake = false;
            sfxPlayers[i].volume = sfxVolume;
            sfxPlayers[i].outputAudioMixerGroup = mixer.FindMatchingGroups("SFX")[0];

        }
    }

    public void SetSliders(Slider bgm, Slider sfx)
    {
        bgmSlider = bgm;
        sfxSlider = sfx;

        if (bgmSlider != null)
        {
            float bgmVal = PlayerPrefs.GetFloat("BGMVolume", 0.75f);
            bgmSlider.value = bgmVal;
            bgmSlider.onValueChanged.AddListener(BGMVolume);
            BGMVolume(bgmVal);
        }

        if (sfxSlider != null)
        {
            float sfxVal = PlayerPrefs.GetFloat("SFXVolume", 0.75f);
            sfxSlider.value = sfxVal;
            sfxSlider.onValueChanged.AddListener(SFXVolume);
            SFXVolume(sfxVal);
        }
    }

    public void SFXPlay(SFXType type) // ȿ���� ��� 
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


    private void BGMVolume(float val)
    {
        mixer.SetFloat("BGMVolume", Mathf.Log10(val) * 20);
        PlayerPrefs.SetFloat("BGMVolume", val);
    }

    private void SFXVolume(float val)
    {
        mixer.SetFloat("SFXVolume", Mathf.Log10(val) * 20);
        PlayerPrefs.SetFloat("SFXVolume", val);
    }
}

