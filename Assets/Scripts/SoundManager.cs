using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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


public class SoundManager : MonoBehaviour
{
    public AudioSource bgSound;

    public static SoundManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
            //SceneManager.sceneLoaded += OnSceneLoaded; // �� �̵��� ����� �޼��� �߰�
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SFXPlay(string sfxName, AudioClip clip) // ȿ���� ��� ex) SFXPlay("Jump", clip) clip�� �ν����� â���� ���� �ֱ� 
    {
        GameObject go = new GameObject(sfxName + "Sound"); // �Ҹ��� ����ϴ� ������Ʈ ����
        AudioSource audioSource = go.AddComponent<AudioSource>(); // ������Ʈ�� AudioSource ������Ʈ �߰�
        audioSource.clip = clip;
        audioSource.Play();

        Destroy(go, clip.length); // ȿ������ ������ �Ҹ� ������Ʈ ����
    }

    public void BgSoundPlay(AudioClip clip) // ����� ���(����� ����� ����)
    {
        bgSound.clip = clip; 
        bgSound.loop = true; // �ݺ�
        bgSound.volume = 0.1f; // ����� ����
        bgSound.Play();
    }

}

