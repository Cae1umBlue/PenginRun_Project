using System.Collections;
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
    [Header("������ Ÿ��")]
    public ItemType itemType = ItemType.Score;  // �� �������� Ÿ��

    [Header("Score ������ ����")]
    public int scoreValue = 10;   // Score ������ ����

    [Header("Heal ������ ����")]
    public float healAmount = 1f;    // Heal ������ ȸ����

    [Header("SpeedUp/SlowDown ����")]
    public float speedAmount = 1f;   // �ӵ� ��ȭ��
    public float effectDuration = 5f;   // �ӵ� ȿ�� ���� �ð�

    private void OnTriggerEnter2D(Collider2D other)
    {
        // �÷��̾� �±װ� �ƴ� ��� ����
        if (!other.CompareTag("Player")) return;

        switch (itemType)
        {
            case ItemType.Score:
                // ���� ������ ȹ��
                SoundManager.Instance.SFXPlay(SFXType.Coin);
                ScoreManager.Instance.AddScore(scoreValue);
                break;

            case ItemType.Heal:
                // ü�� ȸ�� ������ ȹ��
                GameManager.Instance.IncreaseHP(healAmount);
                SoundManager.Instance.SFXPlay(SFXType.Heal);
                break;

            case ItemType.SpeedUp:
                // �ӵ� ���� ������ ȹ��
                SoundManager.Instance.SFXPlay(SFXType.item);
                StartCoroutine(ApplySpeedEffect(speedAmount));
                break;

            case ItemType.SlowDown:
                // �ӵ� ���� ������ ȹ��
                SoundManager.Instance.SFXPlay(SFXType.item);
                StartCoroutine(ApplySpeedEffect(-speedAmount));
                break;
        }

            // ������ �ı�
            Destroy(gameObject);
    }

    /// <summary>
    /// amount ��ŭ moveSpeed�� �����ߴٰ�, effectDuration �� ���󺹱��մϴ�.
    /// (amount > 0: ����, amount < 0: ����)
    /// </summary>
    private IEnumerator ApplySpeedEffect(float amount)
    {
        var pc = PlayerController.Instance;
        if (pc != null)
        {
            pc.moveSpeed += amount;
            yield return new WaitForSeconds(effectDuration);
            pc.moveSpeed -= amount;
        }
    }
}