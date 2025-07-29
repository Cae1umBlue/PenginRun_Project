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
    public Transform GroundCheck; // ���� ���� �������� �� �ٴ� üũ�� ��������
    public float GroundCheckRadius = 0.2f; // �ش� ������ �׶��� ���̾ ������츸 ���� ����
    public LayerMask GroundLayer; // �ٴ� ���̾� 

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
        Rb.velocity = new Vector2(Rb.velocity.x, JumpForce);
        Animator.SetTrigger("Jump");
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
}
