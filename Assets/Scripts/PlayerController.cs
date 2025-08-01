using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }

    [Header("�̵� ����")]
    public float moveSpeed = 5f;          // �÷��̾� �̵� �ӵ�

    [Header("���� ����")]
    public float JumpForce = 5f;          // ���� ��
    private bool IsTouchingBlock = false; // �ٴ�(���)�� ����ִ��� ����
    private int JumpCount = 0;            // ���� ���� Ƚ��
    public int MaxJumpCount = 2;          // �ִ� ���� ���� Ƚ��(�������� ��)

    [Header("�����̵� ����")]
    private bool IsSliding = false;        // �����̵� ������ ����
    public float SlideOffsetDelta = 0.57f; // �����̵� �� �ݶ��̴� y-offset ���淮
    public float SlideSizeDelta = 1.7f;    // �����̵� �� �ݶ��̴� height ���淮
    public float SlideModelYOffset = 0.3f; // �����̵� �� �� y-offset ���淮

    [Header("��/�ݶ��̴� ����")]
    public Transform PlayerModel;          // �ִϸ��̼�/��������Ʈ�� �ִ� �ڽ� ������Ʈ
    private Vector3 originalModelLocalPos; // ���� ���� ��ġ

    private BoxCollider2D BoxCollider;      // �÷��̾��� BoxCollider2D
    private Vector2 OriginalBoxOffset;      // �ݶ��̴��� ���� offset
    private Vector2 OriginalBoxSize;        // �ݶ��̴��� ���� size
    private Vector2 OriginalColliderOffset; // ���� �� y-offset ������

    private Rigidbody2D Rb;               // �÷��̾��� Rigidbody2D
    private Animator Animator;            // �÷��̾� �ִϸ�����

    private float HitTime = -999f;           // ������ �ǰ� �ð�(���� �ð� üũ��)
    public float InvincibleDuration = 1.0f;  // �ǰ� �� ���� ���� �ð�(��)
    public float JumpColliderYOffset = 0.5f; // ���� �� �ݶ��̴� y-offset �ӽ� ���氪

    private void Awake()
    {
        // �̱��� ����: Instance�� �̹� ������ �ڱ� �ڽ� �ı�
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        Rb = GetComponent<Rigidbody2D>();
        BoxCollider = GetComponent<BoxCollider2D>();

        // �ݶ��̴��� ���� �� ����(�����̵�/���� �� ������)
        if (BoxCollider != null)
        {
            OriginalBoxOffset = BoxCollider.offset;
            OriginalBoxSize = BoxCollider.size;
            OriginalColliderOffset = BoxCollider.offset;
        }

        // �ִϸ����� ���� �� �� ��ġ ����
        if (PlayerModel != null)
        {
            Animator = PlayerModel.GetComponent<Animator>();
            originalModelLocalPos = PlayerModel.localPosition;
        }
    }

    private void Update()
    {
        MoveForward();

        // �����̵� �߿��� ���� �Ұ�
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

    // �׻� ���������� �̵�(���� ���� ��Ÿ��)
    private void MoveForward()
    {
        Rb.velocity = new Vector2(moveSpeed, Rb.velocity.y);
    }

    // ���� ó��(�������� ����)
    private void Jump()
    {
        if (JumpCount < MaxJumpCount)
        {
            Rb.velocity = new Vector2(Rb.velocity.x, 0f); // ���� �� y�ӵ� �ʱ�ȭ
            Rb.AddForce(Vector2.up * JumpForce, ForceMode2D.Impulse);
            Animator?.SetTrigger("Jump");
            JumpCount++;

            SoundManager.Instance.SFXPlay(SFXType.Jump);

            // ���� �� �ݶ��̴� y-offset �ӽ� ����(���� �� ����)
            if (BoxCollider != null)
            {
                var offset = BoxCollider.offset;
                offset.y = OriginalColliderOffset.y + JumpColliderYOffset;
                BoxCollider.offset = offset;
            }
        }
    }

    // �����̵� �Է� ó��
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

    // �����̵� ����: �ݶ��̴�/�� ��ġ/�ִϸ��̼� ����
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

    // �����̵� ����: �ݶ��̴�/�� ��ġ/�ִϸ��̼� ����
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

    // �ٴ�(���) �浹 ó�� �� ��ֹ� �浹 �� ������
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Block"))
        {
            IsTouchingBlock = true;
            JumpCount = 0;

            // ���� �� ����� �ݶ��̴� offset ����
            if (BoxCollider != null)
                BoxCollider.offset = OriginalColliderOffset;
        }

        // ��ֹ��� �浹 �� ������ �� ī�޶� ��鸲
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            TakeDamage();
            CameraController.Instance.ShakeCamera(0.2f, 0.3f);
        }
    }

    // �ٴڿ��� �������� �� ó��
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Block"))
        {
            IsTouchingBlock = false;
        }
    }

    // �ݶ��̴� offset�� �ӽ÷� ����� ��� ����
    private void RestoreColliderOffsetIfNeeded()
    {
        if (IsTouchingBlock && BoxCollider != null && BoxCollider.offset != OriginalColliderOffset)
        {
            BoxCollider.offset = OriginalColliderOffset;
        }
    }

    // �ǰ� ó��(���� �ð� ����)
    public void TakeDamage()
    {
        // ���� �ð� ������ ������ ����
        if (Time.time < HitTime + InvincibleDuration)
            return;

        GameManager.Instance.DecreaseHP(); // ����� ó���� GameManager���� ����
        HitTime = Time.time;

        Debug.Log("TakeDamage called!");
        Animator?.SetTrigger("HitTime");
    }

    // �������� ������ ��� ���ӿ���
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("DeathZone"))
        {
            GameManager.Instance.GameOver();
        }
    }
}
