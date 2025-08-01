using UnityEngine;

public class Obstacle : MonoBehaviour
{
    // 트리거 충돌 시 플레이어에게 데미지 처리
    // 충돌체에 IsTrigger 설정 (충돌하여도 반응하지 않음)
    // HP 감소 확인
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            var player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage();
                // 카메라 떨림 효과 호출 (예시: 0.2초, 0.3 강도)
                CameraController.Instance.ShakeCamera(0.2f, 0.3f);
            }
        }
    }
}
