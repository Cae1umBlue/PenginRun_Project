using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ������ ���� ����
public enum ItemType
{
    Score,      // ���� ������
    Heal,       // ü�� ȸ�� ������
    SpeedUp,    // �ӵ� ���� ������
    SlowDown    // �ӵ� ���� ������
}

// �÷��̾�� �浹 �� ȿ�� ���� �� �ı��Ǵ� ������ ��ũ��Ʈ
public class Item : MonoBehaviour
{
    public ItemType itemType = ItemType.Score;  // �� �������� Ÿ��

    public int scoreValue = 10;   // Score ������ ����
    public int healAmount = 1;    // Heal ������ ȸ����
    public float speedAmount = 1f;   // SpeedUp/SlowDown �ӵ� ��ȭ��
    public float effectDuration = 5f;   // �ӵ� ȿ�� ���� �ð�

    private void OnTriggerEnter2D(Collider2D other)
    {
        // �÷��̾� �±װ� �ƴ� ��� ����
        if (!other.CompareTag("Player")) return;

        switch (itemType)
        {
            case ItemType.Score:
                // ���� ������ ȹ��
                ScoreManager.Instance.AddScore(scoreValue);
                break;

            case ItemType.Heal:
                // ü�� ȸ�� ������ ȹ��
                PlayerController.Instance.Heal(healAmount);  // PlayerController�� Heal(int) �޼��� �ʿ�
                UIManager.Instance.UpdateHPUI(PlayerController.Instance.currentHP);
                break;

            case ItemType.SpeedUp:
                // �ӵ� ���� ������ ȹ��
                StartCoroutine(ApplySpeedEffect(speedAmount));
                break;

            case ItemType.SlowDown:
                // �ӵ� ���� ������ ȹ��
                StartCoroutine(ApplySpeedEffect(-speedAmount));
                break;
        }

        // ������ ȹ�� ȿ���� ��� (����)
        // SoundManager.Instance.PlaySFX("ItemPickup");

        // ������ ������Ʈ �ı�
        Destroy(gameObject);
    }

    // �ӵ� ���� ȿ���� ���� �ð� ���� �� ���󺹱��ϴ� �ڷ�ƾ
    private IEnumerator ApplySpeedEffect(float amount)
    {
        if (PlayerController.Instance != null)
        {
            PlayerController.Instance.moveSpeed += amount;

            yield return new WaitForSeconds(effectDuration);

            PlayerController.Instance.moveSpeed -= amount;
        }
    }
}