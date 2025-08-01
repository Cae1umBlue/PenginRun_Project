using System.Collections;
using UnityEngine;

// 아이템 종류 정의
public enum ItemType
{
    Score,      // 점수 아이템
    Heal,       // 체력 회복 아이템
    SpeedUp,    // 속도 증가 아이템
    SlowDown    // 속도 감소 아이템
}

// 플레이어와 충돌 시 효과 적용 후 파괴되는 아이템 스크립트
public class Item : MonoBehaviour
{
    [Header("아이템 타입")]
    public ItemType itemType = ItemType.Score;  // 이 아이템의 타입

    [Header("Score 아이템 설정")]
    public int scoreValue = 10;   // Score 아이템 점수

    [Header("Heal 아이템 설정")]
    public float healAmount = 1f;    // Heal 아이템 회복량

    [Header("SpeedUp/SlowDown 설정")]
    public float speedAmount = 1f;   // 속도 변화량
    public float effectDuration = 5f;   // 속도 효과 지속 시간

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 플레이어 태그가 아닐 경우 무시
        if (!other.CompareTag("Player")) return;

        switch (itemType)
        {
            case ItemType.Score:
                // 점수 아이템 획득
                SoundManager.Instance.SFXPlay(SFXType.Coin);
                ScoreManager.Instance.AddScore(scoreValue);
                break;

            case ItemType.Heal:
                // 체력 회복 아이템 획득
                GameManager.Instance.IncreaseHP(healAmount);
                SoundManager.Instance.SFXPlay(SFXType.Heal);
                break;

            case ItemType.SpeedUp:
                // 속도 증가 아이템 획득
                SoundManager.Instance.SFXPlay(SFXType.item);
                StartCoroutine(ApplySpeedEffect(speedAmount));
                break;

            case ItemType.SlowDown:
                // 속도 감소 아이템 획득
                SoundManager.Instance.SFXPlay(SFXType.item);
                StartCoroutine(ApplySpeedEffect(-speedAmount));
                break;
        }

            // 아이템 파괴
            Destroy(gameObject);
    }

    /// <summary>
    /// amount 만큼 moveSpeed를 조정했다가, effectDuration 후 원상복귀합니다.
    /// (amount > 0: 가속, amount < 0: 감속)
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