using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    // public float FixedY = 0f;              // Y값 (고정) - 삭제
    public float ZOffset = -10f;           // 카메라 Z축 거리
    public float FollowSpeed = 5f;         // 따라가기 속도
    public float XOffset = 4.5f;           // 타겟 중심값 + X축 오프셋

    private Transform Target;              // 따라갈 대상

    public static CameraController Instance { get; private set; } // 싱글톤 인스턴스

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
            //Vector3 targetPos = new Vector3(Target.position.x + XOffset, FixedY, ZOffset); // Y값 고정 - 삭제


            // Y값을 고정하지 않고 타겟의 Y값을 따라가도록 수정
            Vector3 targetPos = new Vector3(Target.position.x + XOffset, Target.position.y, ZOffset);
            transform.position = Vector3.Lerp(transform.position, targetPos, FollowSpeed * Time.deltaTime);
        }
    }

    // 외부에서 XOffset 값을 동적으로 변경할 수 있도록 메서드
    public void SetXOffset(float offset)
    {
        XOffset = offset;
    }
}
