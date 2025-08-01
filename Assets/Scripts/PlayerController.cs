using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }

    [Header("이동 관련")]
    public float moveSpeed = 5f;

    [Header("점프 관련")]
    public float JumpForce = 5f;
    private bool IsTouchingBlock = false;
    private int JumpCount = 0;
    public int MaxJumpCount = 2;

    [Header("슬라이드 관련")]
    private bool IsSliding = false;
    public float SlideOffsetDelta = 0.57f;
    public float SlideSizeDelta = 1.65f;
    public float SlideModelYOffset = 0.3f;

    [Header("모델/콜라이더 연결")]
    public Transform PlayerModel;
    private Vector3 originalModelLocalPos;

    private BoxCollider2D BoxCollider;
    private Vector2 OriginalBoxOffset;
    private Vector2 OriginalBoxSize;
    private Vector2 OriginalColliderOffset;

    private Rigidbody2D Rb;
    private Animator Animator;

    private float HitTime = -999f;
    public float InvincibleDuration = 1.0f;
    public float JumpColliderYOffset = 0.5f;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        Rb = GetComponent<Rigidbody2D>();
        BoxCollider = GetComponent<BoxCollider2D>();

        if (BoxCollider != null)
        {
            OriginalBoxOffset = BoxCollider.offset;
            OriginalBoxSize = BoxCollider.size;
            OriginalColliderOffset = BoxCollider.offset;
        }

        if (PlayerModel != null)
        {
            Animator = PlayerModel.GetComponent<Animator>();
            originalModelLocalPos = PlayerModel.localPosition;
        }
    }

    private void Update()
    {
        MoveForward();

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

    private void MoveForward()
    {
        Rb.velocity = new Vector2(moveSpeed, Rb.velocity.y);
    }

    private void Jump()
    {
        if (JumpCount < MaxJumpCount)
        {
            Rb.velocity = new Vector2(Rb.velocity.x, 0f);
            Rb.AddForce(Vector2.up * JumpForce, ForceMode2D.Impulse);
            Animator?.SetTrigger("Jump");
            JumpCount++;

            SoundManager.Instance.SFXPlay(SFXType.Jump);

            if (BoxCollider != null)
            {
                var offset = BoxCollider.offset;
                offset.y = OriginalColliderOffset.y + JumpColliderYOffset;
                BoxCollider.offset = offset;
            }
        }
    }

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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Block"))
        {
            IsTouchingBlock = true;
            JumpCount = 0;

            if (BoxCollider != null)
                BoxCollider.offset = OriginalColliderOffset;
        }

        if (collision.gameObject.CompareTag("Obstacle"))
        {
            TakeDamage();
            CameraController.Instance.ShakeCamera(0.2f, 0.3f);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Block"))
        {
            IsTouchingBlock = false;
        }
    }

    private void RestoreColliderOffsetIfNeeded()
    {
        if (IsTouchingBlock && BoxCollider != null && BoxCollider.offset != OriginalColliderOffset)
        {
            BoxCollider.offset = OriginalColliderOffset;
        }
    }

    public void TakeDamage()
    {
        if (Time.time < HitTime + InvincibleDuration)
            return;

        GameManager.Instance.DecreaseHP();
        HitTime = Time.time;

        Debug.Log("TakeDamage called!");
        Animator?.SetTrigger("HitTime");
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("DeathZone"))
        {
            GameManager.Instance.GameOver();
        }
    }
}
