using UnityEngine;
using System.Collections;

public class MoveController : MonoBehaviour
{
    public float moveSpeed = 5f;             // �⺻ �̵� �ӵ�
    public float airMoveSpeedFactor = 0.0f;  // ���� �� �̵� �ӵ� ���� ����
    public float jumpForce = 10f;            // ���� ��
    public float groundCheckDistance = 0.1f; // ���� üũ�� ���� Ray ����
    public float acceleration = 20f;         // ���ӵ�
    public float rotationSpeed = 10f;        // ȸ�� �ӵ�
    public float groundedLockTime = 1f;      // ���� �� �������� ���� �ð�

    private Rigidbody rb;
    private bool isGrounded;
    private bool canMove = true;             // �̵� ���� ����
    private Vector3 lastMoveDirection;       // ������ �̵� ����
    private Vector3 currentMoveDirection;    // ���� �̵� ����

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
        // ���� ���¿����� ���� ����
        bool wasGrounded = isGrounded;

        // ���� ���� ���� ����
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance);

        // ���� �� �̵� ��� �ڷ�ƾ ����
        if (isGrounded && !wasGrounded)
        {
            StartCoroutine(LockMovementAfterLanding());
        }

        // �̵� �� ���� ó�� (�̵� ������ ���� Move ȣ��)
        if (canMove)
        {
            Move();
        }
        Jump();

        // �ִϸ������� isGround �Ķ���� ����
        if (animator != null)
        {
            animator.SetBool("isGround", isGrounded);
        }
    }

    void Move()
    {
        // �̵� ���θ� �ʱ�ȭ
        bool isMoving = false;

        if (isGrounded)
        {
            // WASD �Է��� ���� �̵� ���� ����
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            // �̵� ���� ��� �� ����ȭ
            Vector3 targetDirection = new Vector3(horizontal, 0, vertical).normalized;

            // ī�޶��� y�� ȸ���� �����Ͽ� �̵� ������ ī�޶� ���� �������� ��ȯ
            if (Camera.main != null)
            {
                Vector3 cameraForward = Camera.main.transform.forward;
                cameraForward.y = 0; // y�� ���� �����Ͽ� ��� �̵��� ����
                Quaternion rotation = Quaternion.LookRotation(cameraForward);
                targetDirection = rotation * targetDirection;
            }

            // �̵� ������ ���� ���� ���� ���� �� ȸ�� ����
            if (targetDirection != Vector3.zero)
            {
                lastMoveDirection = targetDirection;
                isMoving = true; // �̵� ������ ����

                // ��ǥ �������� �ε巴�� ȸ��
                Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }

            // MoveTowards�� ����Ͽ� ������ ������ ����
            currentMoveDirection = Vector3.MoveTowards(currentMoveDirection, targetDirection, acceleration * Time.deltaTime);
        }

        // �̵� �ӵ� ����
        float effectiveMoveSpeed = isGrounded ? moveSpeed : moveSpeed * airMoveSpeedFactor;

        // x, z �������θ� Translate ����
        Vector3 movement = new Vector3(currentMoveDirection.x * effectiveMoveSpeed * Time.deltaTime, 0, currentMoveDirection.z * effectiveMoveSpeed * Time.deltaTime);
        transform.Translate(movement, Space.World);

        // Animator�� isMove �Ķ���� ����
        if (animator != null)
        {
            animator.SetBool("isMove", isMoving);

            // ���� �ӵ��� ���� �̵� �ӵ��� ����Ͽ� 0 ~ 1 ������ ����ȭ
            float currentSpeed = movement.magnitude / (moveSpeed * Time.deltaTime);
            animator.SetFloat("speed", Mathf.Clamp01(currentSpeed));
        }
    }

    void Jump()
    {
        // ���� �Է� ó��
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            animator.SetTrigger("jump");
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    IEnumerator LockMovementAfterLanding()
    {
        // ���� �� ���� �ð� ���� �̵� �Ұ��� ����
        canMove = false;
        yield return new WaitForSeconds(groundedLockTime);
        canMove = true;
    }

    void OnDrawGizmos()
    {
        // Gizmos�� ���� üũ Ray�� �ð�ȭ�մϴ�.
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundCheckDistance);
    }
}
