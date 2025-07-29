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

    [Header("�ٴ� üũ ����")] // groundLayer������ ���� ����
    public Transform GroundCheck;
    public float GroundCheckRadius = 0.2f;
    public LayerMask GroundLayer;

    private Rigidbody2D Rb;
    private Animator Animator;

    // ������Ʈ(Rigidbody2D, Animator)�� �ʱ�ȭ
    private void Start()
    {
        Rb = GetComponent<Rigidbody2D>();
        Animator = GetComponent<Animator>();
    }

    // �� �����Ӹ��� �̵�, �ٴ� üũ, �Է�(����/�����̵�)�� ó��
    // �׶��� ���̾ �ִ� ���Ǹ� ���� ����
    private void Update()
    {
        MoveForward();
        UpdateGroundStatus();

        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded)
        {
            Jump();
        }

        HandleSlideInput();
    }

    // �÷��̾ ���������� �̵�
    private void MoveForward()
    {
        Rb.velocity = new Vector2(MoveSpeed, Rb.velocity.y);
    }

    // �÷��̾ �����ϵ��� ó��
    private void Jump()
    {
        Rb.velocity = new Vector2(Rb.velocity.x, JumpForce);
        Animator.SetTrigger("Jump");
    }

    // ����Ʈ�� ������ �ִ� ��츸 �����̵� �Է��� ó��
    private void HandleSlideInput()
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

    // �÷��̾ �ٴڿ� ����ִ��� Ȯ��
    private void UpdateGroundStatus()
    {
        IsGrounded = Physics2D.OverlapCircle(GroundCheck.position, GroundCheckRadius, GroundLayer);
    }
}
