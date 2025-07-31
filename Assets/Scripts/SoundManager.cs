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
            SceneManager.sceneLoaded += OnSceneLoaded; // �� �̵��� ����� �޼��� �߰�
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1) // ���� ���� ������� ����
    {
        for(int i =0; i < bgList.Length; i++)
        {
            if(arg0.name == bgList[i].name)
                BgSoundPlay(bgList[i]);
        }
    }

    public void SFXPlay(SFXType type, AudioClip clip) // ȿ���� ��� ex) SFXPlay("Jump", clip) clip�� �ν����� â���� ���� �ֱ� 
    {
        GameObject go = new GameObject(type + "Sound"); // �Ҹ��� ����ϴ� ������Ʈ ����
        AudioSource audioSource = go.AddComponent<AudioSource>(); // ������Ʈ�� AudioSource ������Ʈ �߰�
        audioSource.outputAudioMixerGroup = mixer.FindMatchingGroups("SFX")[0];
        audioSource.clip = clip;
        audioSource.Play();

        Destroy(go, clip.length); // ȿ������ ������ �Ҹ� ������Ʈ ����
    }

    public void BgSoundPlay(AudioClip clip) // ����� ���(����� ����� ����)
    {
        bgSound.outputAudioMixerGroup = mixer.FindMatchingGroups("BGM")[0];
        bgSound.clip = clip; 
        bgSound.loop = true; // �ݺ�
        bgSound.volume = 0.1f; // ����� ����
        bgSound.Play();
    }

}

