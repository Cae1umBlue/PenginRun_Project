using UnityEngine;

public class Obstacle : MonoBehaviour
{
    // 트리거 충돌 시 플레이어에게 데미지 처리
    // 충돌체에 is trigger 설정 (충돌하여도 반응하지 않음)
    // HP감소 확인
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
