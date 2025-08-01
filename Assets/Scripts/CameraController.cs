using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    public float FixedY = 0f;              // Y�� (����)
    public float ZOffset = -10f;           // ī�޶� Z�� �Ÿ�
    public float FollowSpeed = 5f;         // ���󰡱� �ӵ�
    public float XOffset = 4.5f;           // Ÿ�� �߽ɰ� + X�� ������

    private Transform Target;              // ���� ���

    public static CameraController Instance { get; private set; } // �̱��� �ν��Ͻ�

    // ȭ�� ���� ���� ����
    private float shakeTimer = 0f;
    private float shakeMagnitude = 0f;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        // �±װ� "Player"�� ������Ʈ �ڵ� ����
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Target = player.transform;
        }
    }

    // ī�޶� ��� ������Ʈ�� ������Ʈ �Ŀ� ��ġ�� ����
    private void LateUpdate()
    {
        if (Target != null)
        {
            Vector3 targetPos = new Vector3(Target.position.x + XOffset, FixedY, ZOffset);
            Vector3 finalPos = Vector3.Lerp(transform.position, targetPos, FollowSpeed * Time.deltaTime);

            if (shakeTimer > 0f)
            {
                Vector3 shakeOffset = Random.insideUnitSphere * shakeMagnitude;
                shakeOffset.z = 0;
                transform.position = finalPos + shakeOffset;
                shakeTimer -= Time.deltaTime;
            }
            else
            {
                transform.position = finalPos;
            }
        }
    }

    // �ܺο��� XOffset ���� �������� ������ �� �ֵ��� �޼���
    public void SetXOffset(float offset)
    {
        XOffset = offset;
    }

    // �ܺο��� ī�޶� ������ ȣ���� �� �ִ� �޼���
    public void ShakeCamera(float duration, float magnitude)
    {
        shakeTimer = duration;
        shakeMagnitude = magnitude;
    }
}