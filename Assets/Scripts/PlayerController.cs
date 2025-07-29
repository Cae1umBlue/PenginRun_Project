using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("�̵� ����")]
    public float MoveSpeed = 5f;

    [Header("���� ����")]
    public float JumpForce = 10f;
    private bool IsGrounded = false;

    [Header("�����̵� ����")]
    private bool IsSliding = false;

    [Header("�ٴ� üũ ����")] // GroundLayer������ ���� ����
    public Transform GroundCheck; // ���� ���� �������� �� �ٴ� üũ�� ���� ����
    public float GroundCheckRadius = 0.2f; // �ش� ������ GroundLayer�� ���� ��츸 ����, �����̵� ����
    public LayerMask GroundLayer; // �ٴ� ���̾� 

    private float hitTime = -999f;              // ���������� �ǰݴ��� �ð�
    public float invincibleDuration = 1.0f;     // ���� ���� �ð�

    private Rigidbody2D Rb;
    private Animator Animator;

    // �÷��̾� �ӽ� ü�� (�浹 �׽�Ʈ)
    public int Health = 3;

    // ������Ʈ(Rigidbody2D, Animator)�� �ʱ�ȭ
    private void Start()
    {
        Rb = GetComponent<Rigidbody2D>();
        Animator = GetComponent<Animator>();
    }

    // �� �����Ӹ��� �̵�, �ٴ� üũ, �Է�(����/�����̵�)�� ó��
    // GroundLayer�� �ִ� ���Ǹ� ���� ����
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

    // �÷��̾ ���������� �̵�
    private void MoveForward()
    {
        Rb.velocity = new Vector2(MoveSpeed, Rb.velocity.y);
    }

    // �÷��̾ �����ϵ��� ó��
    private void Jump()
    {
        if (Rb.velocity.y <= 0.1f) // ���� �� ��� �߿��� ����
        {
            Rb.velocity = new Vector2(Rb.velocity.x, JumpForce);
            Animator.SetTrigger("Jump");
        }
    }

    // ����Ʈ�� ������ �ִ� ��츸 �����̵� �Է��� ó��
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

    // �߰� �޼ҵ�
    // �÷��̾ �ٴڿ� ����ִ��� Ȯ��
    private void UpdateGroundStatus()
    {
        IsGrounded = Physics2D.OverlapCircle(GroundCheck.position, GroundCheckRadius, GroundLayer);
    }

    /// Obstacle ��ֹ��� �÷��̾ �΋Hĥ ��� 1�� ������� ����
    public void TakeDamage()
    {
        // ���� ���¶�� ����� ����
        if (Time.time < hitTime + invincibleDuration)
        {
            return;
        }

        // �ǰ� ó��
        Health -= 1;
        hitTime = Time.time; // �ǰ� �ð� ���

        // ü���� -1 ���Ϸ� �������� ��� ����
        if (Health < -1)
        {
            Health = -1;
        }

        // �ǰ� �ִϸ��̼� Ʈ����
        if (Animator != null)
        {
            Animator.SetTrigger("HitTime");
        }

        // ü���� 0 ���϶�� ���ӿ��� ó��
        if (Health <= 0)
        {
            // ���ӿ��� �ִϸ��̼� �� ���ӿ��� UI Ȱ��ȭ?
        }
    }
}
