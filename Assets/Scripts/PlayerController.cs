// 태그를 좀 사용하여 충돌되도록 작성을 해뒀습니다.
// 기본적으로 Player 캐릭터는 Player태그를 가지게 하면됩니다.
// 바닥은 Block 태그를 가지게 하였습니다.
// 장애물은 Obstacle 스크립트를 가진 오브젝트로 설정하였습니다.

using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; } // 싱글톤 인스턴스

    [Header("이동 관련")]
    public float moveSpeed = 5f;

    [Header("점프 관련")]
    public float JumpForce = 5f;
    private bool IsTouchingBlock = false; // Block 태그 오브젝트와 접촉 중인지
    private int JumpCount = 0; // 현재 점프 횟수
    public int MaxJumpCount = 2; // 최대 점프 횟수

    [Header("슬라이드 관련")]
    private bool IsSliding = false;

    private float HitTime = -999f;              // 마지막으로 피격당한 시간
    public float InvincibleDuration = 1.0f;     // 무적 지속 시간

    private Rigidbody2D Rb;
    private Animator Animator;

    // 플레이어 임시 체력 (충돌 테스트)
    public int currentHP = 100;

    // Collider 관련
    private Collider2D PlayerCollider;
    private Vector2 OriginalColliderOffset;
    public float JumpColliderYOffset = 0.5f; // 점프 시 올릴 y값

    // 싱글톤 인스턴스 생성
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 컴포넌트(Rigidbody2D, Animator)를 초기화
    private void Start()
    {
        Rb = GetComponent<Rigidbody2D>();
        Animator = GetComponent<Animator>();
        PlayerCollider = GetComponent<Collider2D>();
        if (PlayerCollider != null)
        {
            OriginalColliderOffset = PlayerCollider.offset;
        }
    }

    // 매 프레임마다 이동, 입력(점프/슬라이드)을 처리
    private void Update()
    {
        MoveForward();

        // Block 태그 오브젝트와 접촉 중일 때만 점프 가능
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (IsTouchingBlock)
            {
                JumpCount = 0; // 바닥에 닿았으면 점프 카운트 리셋
                Jump();
            }
            else if (JumpCount > 0 && JumpCount < MaxJumpCount)
            {
                Jump(); // 공중에서만 가능한 더블 점프
            }
        }

        Slide();
        RestoreColliderOffsetIfNeeded();
    }

    // 플레이어를 오른쪽으로 이동
    private void MoveForward()
    {
        Rb.velocity = new Vector2(moveSpeed, Rb.velocity.y);
    }

    // 플레이어가 점프하도록 처리
    private void Jump()
    {
        if (JumpCount < MaxJumpCount)
        {
            Rb.velocity = new Vector2(Rb.velocity.x, 0f); // 기존 Y속도 초기화
            Rb.AddForce(Vector2.up * JumpForce, ForceMode2D.Impulse);
            Animator.SetTrigger("Jump");
            JumpCount++;

            // 점프 시 Collider의 y 오프셋을 올림
            if (PlayerCollider != null)
            {
                var offset = PlayerCollider.offset;
                offset.y = OriginalColliderOffset.y + JumpColliderYOffset;
                PlayerCollider.offset = offset;
            }
        }
    }

    // 쉬프트를 누르고 있는 경우만 슬라이드 입력을 처리
    private void Slide()
    {
        if (Input.GetKey(KeyCode.LeftShift) && IsTouchingBlock)
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

    // Block 태그 오브젝트와 충돌 시 바닥에 닿은 것으로 처리
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Block"))
        {
            IsTouchingBlock = true;
            JumpCount = 0; // 점프 횟수 초기화

            // 점프가 끝났으므로 Collider 오프셋 복구
            if (PlayerCollider != null)
            {
                PlayerCollider.offset = OriginalColliderOffset;
            }
        }
    }

    // Block 태그 오브젝트에서 떨어질 때
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Block"))
        {
            IsTouchingBlock = false;
        }
    }

    // 점프가 끝났을 때 Collider 오프셋을 복구
    private void RestoreColliderOffsetIfNeeded()
    {
        // 바닥에 닿아있고, Collider 오프셋이 원래 값이 아니면 복구
        if (IsTouchingBlock && PlayerCollider != null && PlayerCollider.offset != OriginalColliderOffset)
        {
            PlayerCollider.offset = OriginalColliderOffset;
        }
    }

    // Obstacle 장애물과 플레이어가 부딪칠 경우 1의 대미지를 받고, HitTime 애니메이션이 실행됨
    public void TakeDamage()
    {
        // 무적 상태라면 대미지 무시
        if (Time.time < HitTime + InvincibleDuration)
        {
            return;
        }

        // 피격 처리
        currentHP -= 20;
        HitTime = Time.time; // 피격 시간 기록

        // 체력이 -1 이하로 내려가는 경우 방지
        if (currentHP < -1)
        {
            currentHP = -1;
        }

        // 피격 애니메이션 트리거
        if (Animator != null)
        {
            Animator.SetTrigger("HitTime");
        }

        // 체력이 0 이하라면 게임오버 처리
        if (currentHP <= 0)
        {
            // 게임오버 애니메이션 및 게임오버 UI 활성화?
            if (GameManager.Instance != null)
            {
                GameManager.Instance.GameOver();
            }
        }
    }

    // 체력 회복용 메서드. Heal 아이템 획득 시 호출됨.
    public void Heal(int amount)
    {
        currentHP += amount;
    }
}
