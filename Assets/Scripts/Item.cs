using System.Collections;
using System.Collections.Generic;
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
    public ItemType itemType = ItemType.Score;  // 이 아이템의 타입

    public int scoreValue = 10;   // Score 아이템 점수
    public int healAmount = 1;    // Heal 아이템 회복량
    public float speedAmount = 1f;   // SpeedUp/SlowDown 속도 변화량
    public float effectDuration = 5f;   // 속도 효과 지속 시간

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 플레이어 태그가 아닐 경우 무시
        if (!other.CompareTag("Player")) return;

        switch (itemType)
        {
            case ItemType.Score:
                // 점수 아이템 획득
                ScoreManager.Instance.AddScore(scoreValue);
                break;

            case ItemType.Heal:
                // 체력 회복 아이템 획득
                PlayerController.Instance.Heal(healAmount);  // PlayerController에 Heal(int) 메서드 필요
                UIManager.Instance.UpdateHPUI(PlayerController.Instance.currentHP);
                break;

            case ItemType.SpeedUp:
                // 속도 증가 아이템 획득
                StartCoroutine(ApplySpeedEffect(speedAmount));
                break;

            case ItemType.SlowDown:
                // 속도 감소 아이템 획득
                StartCoroutine(ApplySpeedEffect(-speedAmount));
                break;
        }

        // 아이템 획득 효과음 재생 (선택)
        // SoundManager.Instance.PlaySFX("ItemPickup");

        // 아이템 오브젝트 파괴
        Destroy(gameObject);
    }

    // 속도 증감 효과를 일정 시간 적용 후 원상복구하는 코루틴
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