using UnityEngine;

public class Obstacle : MonoBehaviour
{
    // Ʈ���� �浹 �� �÷��̾�� ������ ó��
    // �浹ü�� is trigger ���� (�浹�Ͽ��� �������� ����)
    // HP���� Ȯ��
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            var player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage();
            }
        }
    }
}
