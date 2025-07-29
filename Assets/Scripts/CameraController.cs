using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    public float FixedY = 0f;              // Y값 (고정)
    public float ZOffset = -10f;           // 카메라 Z축 거리
    public float FollowSpeed = 5f;         // 따라가기 속도

    private Transform target;              // 따라갈 대상

    private void Start()
    {
        // 태그가 "Player"인 오브젝트 자동 추적
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            target = player.transform;
        }
        else
        {
            Debug.LogWarning("Player 태그가 지정된 오브젝트를 찾을 수 없습니다.");
        }
    }

    // 카메라가 모든 오브젝트의 업데이트 후에 위치를 조정
    private void LateUpdate()
    {
        if (target != null)
        {
            Vector3 targetPos = new Vector3(target.position.x, FixedY, ZOffset);
            transform.position = Vector3.Lerp(transform.position, targetPos, FollowSpeed * Time.deltaTime);
        }
    }

}
