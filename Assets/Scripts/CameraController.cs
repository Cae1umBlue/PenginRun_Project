using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    // public float FixedY = 0f;              // Y�� (����) - ����
    public float ZOffset = -10f;           // ī�޶� Z�� �Ÿ�
    public float FollowSpeed = 5f;         // ���󰡱� �ӵ�
    public float XOffset = 4.5f;           // Ÿ�� �߽ɰ� + X�� ������

    private Transform Target;              // ���� ���

    public static CameraController Instance { get; private set; } // �̱��� �ν��Ͻ�

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
            //Vector3 targetPos = new Vector3(Target.position.x + XOffset, FixedY, ZOffset); // Y�� ���� - ����


            // Y���� �������� �ʰ� Ÿ���� Y���� ���󰡵��� ����
            Vector3 targetPos = new Vector3(Target.position.x + XOffset, Target.position.y, ZOffset);
            transform.position = Vector3.Lerp(transform.position, targetPos, FollowSpeed * Time.deltaTime);
        }
    }

    // �ܺο��� XOffset ���� �������� ������ �� �ֵ��� �޼���
    public void SetXOffset(float offset)
    {
        XOffset = offset;
    }
}
