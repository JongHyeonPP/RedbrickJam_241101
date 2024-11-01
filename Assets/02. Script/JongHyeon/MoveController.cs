using UnityEngine;

public class MoveController : MonoBehaviour
{
    public float moveSpeed = 5f;             // �̵� �ӵ�
    public float jumpForce = 5f;             // ���� ��
    public float groundCheckDistance = 0.1f; // ���� üũ�� ���� Ray ����

    private Rigidbody rb;
    private bool isGrounded;
    private Vector3 lastMoveDirection; // ������ �̵� ����

    private Animator animator;

    void Start()
    {
        // Rigidbody ������Ʈ�� �����ɴϴ�.
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody�� �����ϴ�. Rigidbody�� �߰����ּ���.");
        }

        // Animator ������Ʈ�� �����ɴϴ�.
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator�� �����ϴ�. Animator�� �߰����ּ���.");
        }
    }

    void Update()
    {
        // ���� ���� ����
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance);

        // �̵� ���θ� �ʱ�ȭ
        bool isMoving = false;

        // ���� ������ ���� WASD �Է��� �����Ͽ� �̵� ���� ����
        if (isGrounded)
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            // �̵� ���� ��� �� ����ȭ
            Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;

            // ī�޶��� y�� ȸ���� �����Ͽ� �̵� ������ ī�޶� ���� �������� ��ȯ
            if (Camera.main != null)
            {
                Vector3 cameraForward = Camera.main.transform.forward;
                cameraForward.y = 0; // y�� ���� �����Ͽ� ��� �̵��� ����
                Quaternion rotation = Quaternion.LookRotation(cameraForward);
                direction = rotation * direction;
            }

            // �̵� ������ ���� ���� ���� ���� �� ȸ�� ����
            if (direction != Vector3.zero)
            {
                lastMoveDirection = direction;
                transform.rotation = Quaternion.LookRotation(direction);
                isMoving = true; // �̵� ������ ����
            }

            // Rigidbody�� ����Ͽ� �̵� ����
            rb.MovePosition(transform.position + direction * moveSpeed * Time.deltaTime);
        }
        else
        {
            // ���߿� ���� �� ������ �̵� �������� �̵� ����
            rb.MovePosition(transform.position + lastMoveDirection * moveSpeed * Time.deltaTime);
        }

        // ���� �Է� ó��
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        // Animator�� isMove �Ķ���� ����
        if (animator != null)
        {
            animator.SetBool("isMove", isMoving);
        }
    }

    void OnDrawGizmos()
    {
        // Gizmos�� ���� üũ Ray�� �ð�ȭ�մϴ�.
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundCheckDistance);
    }
}
