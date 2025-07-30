// �±׸� �� ����Ͽ� �浹�ǵ��� �ۼ��� �ص׽��ϴ�.
// �⺻������ Player ĳ���ʹ� Player�±׸� ������ �ϸ�˴ϴ�.
// �ٴ��� Block �±׸� ������ �Ͽ����ϴ�.
// ��ֹ��� Obstacle ��ũ��Ʈ�� ���� ������Ʈ�� �����Ͽ����ϴ�.

using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; } // �̱��� �ν��Ͻ�

    [Header("�̵� ����")]
    public float moveSpeed = 5f;

    [Header("���� ����")]
    public float JumpForce = 5f;
    private bool IsTouchingBlock = false; // Block �±� ������Ʈ�� ���� ������
    private int JumpCount = 0; // ���� ���� Ƚ��
    public int MaxJumpCount = 2; // �ִ� ���� Ƚ��

    [Header("�����̵� ����")]
    private bool IsSliding = false;

    private float HitTime = -999f;              // ���������� �ǰݴ��� �ð�
    public float InvincibleDuration = 1.0f;     // ���� ���� �ð�

    private Rigidbody2D Rb;
    private Animator Animator;

    // �÷��̾� �ӽ� ü�� (�浹 �׽�Ʈ)
    public int currentHP = 100;

    // Collider ����
    private Collider2D PlayerCollider;
    private Vector2 OriginalColliderOffset;
    public float JumpColliderYOffset = 0.5f; // ���� �� �ø� y��

    // �̱��� �ν��Ͻ� ����
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

    // ������Ʈ(Rigidbody2D, Animator)�� �ʱ�ȭ
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

    // �� �����Ӹ��� �̵�, �Է�(����/�����̵�)�� ó��
    private void Update()
    {
        MoveForward();

        // Block �±� ������Ʈ�� ���� ���� ���� ���� ����
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (IsTouchingBlock)
            {
                JumpCount = 0; // �ٴڿ� ������� ���� ī��Ʈ ����
                Jump();
            }
            else if (JumpCount > 0 && JumpCount < MaxJumpCount)
            {
                Jump(); // ���߿����� ������ ���� ����
            }
        }

        Slide();
        RestoreColliderOffsetIfNeeded();
    }

    // �÷��̾ ���������� �̵�
    private void MoveForward()
    {
        Rb.velocity = new Vector2(moveSpeed, Rb.velocity.y);
    }

    // �÷��̾ �����ϵ��� ó��
    private void Jump()
    {
        if (JumpCount < MaxJumpCount)
        {
            Rb.velocity = new Vector2(Rb.velocity.x, 0f); // ���� Y�ӵ� �ʱ�ȭ
            Rb.AddForce(Vector2.up * JumpForce, ForceMode2D.Impulse);
            Animator.SetTrigger("Jump");
            JumpCount++;

            // ���� �� Collider�� y �������� �ø�
            if (PlayerCollider != null)
            {
                var offset = PlayerCollider.offset;
                offset.y = OriginalColliderOffset.y + JumpColliderYOffset;
                PlayerCollider.offset = offset;
            }
        }
    }

    // ����Ʈ�� ������ �ִ� ��츸 �����̵� �Է��� ó��
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

    // Block �±� ������Ʈ�� �浹 �� �ٴڿ� ���� ������ ó��
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Block"))
        {
            IsTouchingBlock = true;
            JumpCount = 0; // ���� Ƚ�� �ʱ�ȭ

            // ������ �������Ƿ� Collider ������ ����
            if (PlayerCollider != null)
            {
                PlayerCollider.offset = OriginalColliderOffset;
            }
        }
    }

    // Block �±� ������Ʈ���� ������ ��
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Block"))
        {
            IsTouchingBlock = false;
        }
    }

    // ������ ������ �� Collider �������� ����
    private void RestoreColliderOffsetIfNeeded()
    {
        // �ٴڿ� ����ְ�, Collider �������� ���� ���� �ƴϸ� ����
        if (IsTouchingBlock && PlayerCollider != null && PlayerCollider.offset != OriginalColliderOffset)
        {
            PlayerCollider.offset = OriginalColliderOffset;
        }
    }

    // Obstacle ��ֹ��� �÷��̾ �ε�ĥ ��� 1�� ������� �ް�, HitTime �ִϸ��̼��� �����
    public void TakeDamage()
    {
        // ���� ���¶�� ����� ����
        if (Time.time < HitTime + InvincibleDuration)
        {
            return;
        }

        // �ǰ� ó��
        currentHP -= 20;
        HitTime = Time.time; // �ǰ� �ð� ���

        // ü���� -1 ���Ϸ� �������� ��� ����
        if (currentHP < -1)
        {
            currentHP = -1;
        }

        // �ǰ� �ִϸ��̼� Ʈ����
        if (Animator != null)
        {
            Animator.SetTrigger("HitTime");
        }

        // ü���� 0 ���϶�� ���ӿ��� ó��
        if (currentHP <= 0)
        {
            // ���ӿ��� �ִϸ��̼� �� ���ӿ��� UI Ȱ��ȭ?
            if (GameManager.Instance != null)
            {
                GameManager.Instance.GameOver();
            }
        }
    }

    // ü�� ȸ���� �޼���. Heal ������ ȹ�� �� ȣ���.
    public void Heal(int amount)
    {
        currentHP += amount;
    }
}
