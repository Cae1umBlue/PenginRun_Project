using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("이동 관련")]
    public float MoveSpeed = 5f;

    [Header("점프 관련")]
    public float JumpForce = 10f;
    private bool IsGrounded = false;

    [Header("슬라이드 관련")]
    private bool IsSliding = false;

    [Header("바닥 체크 관련")] // GroundLayer에서만 점프 가능
    public Transform GroundCheck; // 추후 투명도 설정으로 긴 바닥 체크를 구현 예정
    public float GroundCheckRadius = 0.2f; // 해당 범위에 GroundLayer가 있을 경우만 점프, 슬라이드 가능
    public LayerMask GroundLayer; // 바닥 레이어 

    private float hitTime = -999f;              // 마지막으로 피격당한 시간
    public float invincibleDuration = 1.0f;     // 무적 지속 시간

    private Rigidbody2D Rb;
    private Animator Animator;

    // 플레이어 임시 체력 (충돌 테스트)
    public int Health = 3;

    // 컴포넌트(Rigidbody2D, Animator)를 초기화
    private void Start()
    {
        Rb = GetComponent<Rigidbody2D>();
        Animator = GetComponent<Animator>();
    }

    // 매 프레임마다 이동, 바닥 체크, 입력(점프/슬라이드)을 처리
    // GroundLayer에 있는 조건만 점프 가능
    private void Update()
    {
        MoveForward();
        UpdateGroundStatus();

        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded)
        {
            Jump();
        }

        Slide();
    }

    // 플레이어를 오른쪽으로 이동
    private void MoveForward()
    {
        Rb.velocity = new Vector2(MoveSpeed, Rb.velocity.y);
    }

    // 플레이어가 점프하도록 처리
    private void Jump()
    {
        if (Rb.velocity.y <= 0.1f) // 점프 중 상승 중에는 막기
        {
            Rb.velocity = new Vector2(Rb.velocity.x, JumpForce);
            Animator.SetTrigger("Jump");
        }
    }

    // 쉬프트를 누르고 있는 경우만 슬라이드 입력을 처리
    private void Slide()
    {
        if (Input.GetKey(KeyCode.LeftShift) && IsGrounded)
        {
            if (!IsSliding)
            {
                IsSliding = true;
                Animator.SetBool("Slide", true);
            }
        }
        else
        {
            if (IsSliding)
            {
                IsSliding = false;
                Animator.SetBool("Slide", false);
            }
        }
    }

    // 추가 메소드
    // 플레이어가 바닥에 닿아있는지 확인
    private void UpdateGroundStatus()
    {
        IsGrounded = Physics2D.OverlapCircle(GroundCheck.position, GroundCheckRadius, GroundLayer);
    }

    /// Obstacle 장애물과 플레이어가 부딫칠 경우 1의 대미지를 받음
    public void TakeDamage()
    {
        // 무적 상태라면 대미지 무시
        if (Time.time < hitTime + invincibleDuration)
        {
            return;
        }

        // 피격 처리
        Health -= 1;
        hitTime = Time.time; // 피격 시간 기록

        // 체력이 -1 이하로 내려가는 경우 방지
        if (Health < -1)
        {
            Health = -1;
        }

        // 피격 애니메이션 트리거
        if (Animator != null)
        {
            Animator.SetTrigger("HitTime");
        }

        // 체력이 0 이하라면 게임오버 처리
        if (Health <= 0)
        {
            // 게임오버 애니메이션 및 게임오버 UI 활성화?
        }
    }
}
