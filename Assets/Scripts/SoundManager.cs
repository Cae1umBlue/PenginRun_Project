using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* ���� ���
BGM
������� ��� : BGM ������Ʈ
SFX
ĳ����
    1. ����
    2. �����̵�
    3. ��ֹ� �ǰ�
    4. �߶�
UISound
Ŭ��(����)
����â
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

    public void SFXPlay(string sfxName, AudioClip clip) // ȿ���� ��� ("Jump", clip)
    {
        GameObject go = new GameObject(sfxName + "Sound"); // �Ҹ��� ����ϴ� ������Ʈ ����
        AudioSource audioSource = go.AddComponent<AudioSource>(); // ������Ʈ�� AudioSource ������Ʈ �߰�
        audioSource.clip = clip;
        audioSource.Play();

        Destroy(go, clip.length); // ȿ������ ������ �Ҹ� ������Ʈ ����
    }
}

