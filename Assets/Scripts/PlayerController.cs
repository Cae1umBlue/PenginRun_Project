using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }

    [Header("이동 관련")]
    public float moveSpeed = 5f;          // 플레이어 이동 속도

    [Header("점프 관련")]
    public float JumpForce = 5f;          // 점프 힘
    private bool IsTouchingBlock = false; // 바닥(블록)에 닿아있는지 여부
    private int JumpCount = 0;            // 현재 점프 횟수
    public int MaxJumpCount = 2;          // 최대 점프 가능 횟수(더블점프 등)

    [Header("슬라이드 관련")]
    private bool IsSliding = false;        // 슬라이드 중인지 여부
    public float SlideOffsetDelta = 0.57f; // 슬라이드 시 콜라이더 y-offset 변경량
    public float SlideSizeDelta = 1.7f;    // 슬라이드 시 콜라이더 height 변경량
    public float SlideModelYOffset = 0.3f; // 슬라이드 시 모델 y-offset 변경량

    [Header("모델/콜라이더 연결")]
    public Transform PlayerModel;          // 애니메이션/스프라이트가 있는 자식 오브젝트
    private Vector3 originalModelLocalPos; // 모델의 원래 위치

    private BoxCollider2D BoxCollider;      // 플레이어의 BoxCollider2D
    private Vector2 OriginalBoxOffset;      // 콜라이더의 원래 offset
    private Vector2 OriginalBoxSize;        // 콜라이더의 원래 size
    private Vector2 OriginalColliderOffset; // 점프 시 y-offset 복구용

    private Rigidbody2D Rb;               // 플레이어의 Rigidbody2D
    private Animator Animator;            // 플레이어 애니메이터

    private float HitTime = -999f;           // 마지막 피격 시간(무적 시간 체크용)
    public float InvincibleDuration = 1.0f;  // 피격 후 무적 지속 시간(초)
    public float JumpColliderYOffset = 0.5f; // 점프 시 콜라이더 y-offset 임시 변경값

    private void Awake()
    {
        // 싱글톤 패턴: Instance가 이미 있으면 자기 자신 파괴
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        Rb = GetComponent<Rigidbody2D>();
        BoxCollider = GetComponent<BoxCollider2D>();

        // 콜라이더의 원래 값 저장(슬라이드/점프 후 복구용)
        if (BoxCollider != null)
        {
            OriginalBoxOffset = BoxCollider.offset;
            OriginalBoxSize = BoxCollider.size;
            OriginalColliderOffset = BoxCollider.offset;
        }

        // 애니메이터 연결 및 모델 위치 저장
        if (PlayerModel != null)
        {
            Animator = PlayerModel.GetComponent<Animator>();
            originalModelLocalPos = PlayerModel.localPosition;
        }
    }

    private void Update()
    {
        MoveForward();

        // 슬라이딩 중에는 점프 불가
        if (Input.GetKeyDown(KeyCode.Space) && !IsSliding)
        {
            if (IsTouchingBlock)
            {
                JumpCount = 0;
                Jump();
            }
            else if (JumpCount > 0 && JumpCount < MaxJumpCount)
            {
                Jump();
            }
        }

        Slide();
        RestoreColliderOffsetIfNeeded();
    }

    // 항상 오른쪽으로 이동(무한 러너 스타일)
    private void MoveForward()
    {
        Rb.velocity = new Vector2(moveSpeed, Rb.velocity.y);
    }

    // 점프 처리(더블점프 지원)
    private void Jump()
    {
        if (JumpCount < MaxJumpCount)
        {
            Rb.velocity = new Vector2(Rb.velocity.x, 0f); // 점프 전 y속도 초기화
            Rb.AddForce(Vector2.up * JumpForce, ForceMode2D.Impulse);
            Animator?.SetTrigger("Jump");
            JumpCount++;

            SoundManager.Instance.SFXPlay(SFXType.Jump);

            // 점프 시 콜라이더 y-offset 임시 변경(착지 시 복구)
            if (BoxCollider != null)
            {
                var offset = BoxCollider.offset;
                offset.y = OriginalColliderOffset.y + JumpColliderYOffset;
                BoxCollider.offset = offset;
            }
        }
    }

    // 슬라이드 입력 처리
    private void Slide()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && IsTouchingBlock && !IsSliding)
        {
            StartSlide();
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift) && IsSliding)
        {
            EndSlide();
        }
    }

    // 슬라이드 시작: 콜라이더/모델 위치/애니메이션 변경
    private void StartSlide()
    {
        IsSliding = true;
        Animator?.SetBool("Slide", true);
        SoundManager.Instance.SFXPlay(SFXType.Slide);

        if (BoxCollider != null)
        {
            BoxCollider.offset = new Vector2(
                OriginalBoxOffset.x,
                OriginalBoxOffset.y - SlideOffsetDelta
            );
            BoxCollider.size = new Vector2(
                OriginalBoxSize.x,
                OriginalBoxSize.y - SlideSizeDelta
            );
        }

        if (PlayerModel != null)
        {
            PlayerModel.localPosition = originalModelLocalPos + new Vector3(0, SlideModelYOffset, 0);
        }
    }

    // 슬라이드 종료: 콜라이더/모델 위치/애니메이션 원복
    private void EndSlide()
    {
        IsSliding = false;
        Animator?.SetBool("Slide", false);

        if (BoxCollider != null)
        {
            BoxCollider.offset = OriginalBoxOffset;
            BoxCollider.size = OriginalBoxSize;
        }

        if (PlayerModel != null)
        {
            PlayerModel.localPosition = originalModelLocalPos;
        }
    }

    // 바닥(블록) 충돌 처리 및 장애물 충돌 시 데미지
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Block"))
        {
            IsTouchingBlock = true;
            JumpCount = 0;

            // 점프 시 변경된 콜라이더 offset 복구
            if (BoxCollider != null)
                BoxCollider.offset = OriginalColliderOffset;
        }

        // 장애물과 충돌 시 데미지 및 카메라 흔들림
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            TakeDamage();
            CameraController.Instance.ShakeCamera(0.2f, 0.3f);
        }
    }

    // 바닥에서 떨어졌을 때 처리
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Block"))
        {
            IsTouchingBlock = false;
        }
    }

    // 콜라이더 offset이 임시로 변경된 경우 복구
    private void RestoreColliderOffsetIfNeeded()
    {
        if (IsTouchingBlock && BoxCollider != null && BoxCollider.offset != OriginalColliderOffset)
        {
            BoxCollider.offset = OriginalColliderOffset;
        }
    }

    // 피격 처리(무적 시간 적용)
    public void TakeDamage()
    {
        // 무적 시간 내에는 데미지 무시
        if (Time.time < HitTime + InvincibleDuration)
            return;

        GameManager.Instance.DecreaseHP(); // 대미지 처리는 GameManager에서 관리
        HitTime = Time.time;

        Debug.Log("TakeDamage called!");
        Animator?.SetTrigger("HitTime");
    }

    // 데스존에 닿으면 즉시 게임오버
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("DeathZone"))
        {
            GameManager.Instance.GameOver();
        }
    }
}
