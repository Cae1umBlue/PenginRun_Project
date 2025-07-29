using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    public float FixedY = 0f;              // Y�� (����)
    public float ZOffset = -10f;           // ī�޶� Z�� �Ÿ�
    public float FollowSpeed = 5f;         // ���󰡱� �ӵ�

    private Transform target;              // ���� ���

    private void Start()
    {
        // �±װ� "Player"�� ������Ʈ �ڵ� ����
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            target = player.transform;
        }
        else
        {
            Debug.LogWarning("Player �±װ� ������ ������Ʈ�� ã�� �� �����ϴ�.");
        }
    }

    // ī�޶� ��� ������Ʈ�� ������Ʈ �Ŀ� ��ġ�� ����
    private void LateUpdate()
    {
        if (target != null)
        {
            Vector3 targetPos = new Vector3(target.position.x, FixedY, ZOffset);
            transform.position = Vector3.Lerp(transform.position, targetPos, FollowSpeed * Time.deltaTime);
        }
    }

}
