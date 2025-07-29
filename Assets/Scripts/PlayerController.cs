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

    [Header("바닥 체크 관련")] // groundLayer에서만 점프 가능
    public Transform GroundCheck; // 추후 투명도 설정으로 긴 바닥 체크를 구현예정
    public float GroundCheckRadius = 0.2f; // 해당 범위에 그라운드 레이어가 있을경우만 점프 가능
    public LayerMask GroundLayer; // 바닥 레이어 

    private Rigidbody2D Rb;
    private Animator Animator;

    // 컴포넌트(Rigidbody2D, Animator)를 초기화
    private void Start()
    {
        Rb = GetComponent<Rigidbody2D>();
        Animator = GetComponent<Animator>();
    }

    // 매 프레임마다 이동, 바닥 체크, 입력(점프/슬라이드)을 처리
    // 그라운드 레이어에 있는 조건만 점프 가능
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
        Rb.velocity = new Vector2(Rb.velocity.x, JumpForce);
        Animator.SetTrigger("Jump");
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
}
