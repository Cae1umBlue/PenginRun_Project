using UnityEngine;

public class Obstacle : MonoBehaviour
{
    // Ʈ���� �浹 �� �÷��̾�� ������ ó��
    // �浹ü�� IsTrigger ���� (�浹�Ͽ��� �������� ����)
    // HP ���� Ȯ��
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            var player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage();
                // ī�޶� ���� ȿ�� ȣ�� (����: 0.2��, 0.3 ����)
                CameraController.Instance.ShakeCamera(0.2f, 0.3f);
            }
        }
    }
}
