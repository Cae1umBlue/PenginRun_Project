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
        }
    }
}

