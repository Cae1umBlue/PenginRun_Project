using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    public float FixedY = 0f;              // Y값 (고정)
    public float ZOffset = -10f;           // 카메라 Z축 거리
    public float FollowSpeed = 5f;         // 따라가기 속도
    public float XOffset = 4.5f;           // 타겟 중심값 + X축 오프셋

    private Transform Target;              // 따라갈 대상

    public static CameraController Instance { get; private set; } // 싱글톤 인스턴스

    // 화면 떨림 관련 변수
    private float shakeTimer = 0f;
    private float shakeMagnitude = 0f;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        // 태그가 "Player"인 오브젝트 자동 추적
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Target = player.transform;
        }
    }

    // 카메라가 모든 오브젝트의 업데이트 후에 위치를 조정
    private void LateUpdate()
    {
        if (Target != null)
        {
            Vector3 targetPos = new Vector3(Target.position.x + XOffset, FixedY, ZOffset);
            Vector3 finalPos = Vector3.Lerp(transform.position, targetPos, FollowSpeed * Time.deltaTime);

            if (shakeTimer > 0f)
            {
                Vector3 shakeOffset = Random.insideUnitSphere * shakeMagnitude;
                shakeOffset.z = 0;
                transform.position = finalPos + shakeOffset;
                shakeTimer -= Time.deltaTime;
            }
            else
            {
                transform.position = finalPos;
            }
        }
    }

    // 외부에서 XOffset 값을 동적으로 변경할 수 있도록 메서드
    public void SetXOffset(float offset)
    {
        XOffset = offset;
    }

    // 외부에서 카메라 떨림을 호출할 수 있는 메서드
    public void ShakeCamera(float duration, float magnitude)
    {
        shakeTimer = duration;
        shakeMagnitude = magnitude;
    }
}