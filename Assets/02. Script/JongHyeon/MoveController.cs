using UnityEngine;
using System.Collections;

public class MoveController : MonoBehaviour
{
    public float moveSpeed = 5f;               // 기본 이동 속도
    public float jumpForce = 10f;              // 점프 힘
    public float groundCheckDistance = 0.1f;   // 지면 체크를 위한 Ray 길이
    public float acceleration = 20f;           // 가속도
    public float airControlFactor = 0.5f;      // 공중에서 이동 속도 감소 비율
    public float rotationSpeed = 10f;          // 회전 속도
    public float groundedLockTime = 1f;        // 착지 후 이동 잠김 시간

    private Rigidbody rb;
    private bool isGrounded;
    private bool canMove = true;
    private Vector3 currentMoveDirection;      // 현재 이동 방향
    private Animator animator;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        bool wasGrounded = isGrounded;
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance);

        if (isGrounded && !wasGrounded)
        {
            StartCoroutine(LockMovementAfterLanding());
        }

        if (canMove)
        {
            Move();
        }
        Jump();

        if (animator != null)
        {
            animator.SetBool("isGround", isGrounded);
        }
    }

    void Move()
    {
        bool isMoving = false;

        // WASD 입력으로 이동 방향 설정
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 targetDirection = new Vector3(horizontal, 0, vertical).normalized;

        // 카메라 방향을 따라 이동
        if (Camera.main != null && targetDirection != Vector3.zero)
        {
            Vector3 cameraForward = Camera.main.transform.forward;
            cameraForward.y = 0;
            Quaternion rotation = Quaternion.LookRotation(cameraForward);
            targetDirection = rotation * targetDirection;

            isMoving = true;
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // 공중에서는 `airControlFactor`를 적용하여 이동 속도를 조절
        float effectiveMoveSpeed = isGrounded ? moveSpeed : moveSpeed * airControlFactor;
        float effectiveAcceleration = isGrounded ? acceleration : acceleration * airControlFactor;

        currentMoveDirection = Vector3.MoveTowards(currentMoveDirection, targetDirection, effectiveAcceleration * Time.deltaTime);
        Vector3 movement = new Vector3(currentMoveDirection.x * effectiveMoveSpeed * Time.deltaTime, 0, currentMoveDirection.z * effectiveMoveSpeed * Time.deltaTime);

        transform.Translate(movement, Space.World);

        // Animator 설정
        if (animator != null)
        {
            animator.SetBool("isMove", isMoving);
            float currentSpeed = movement.magnitude / (moveSpeed * Time.deltaTime);
            animator.SetFloat("speed", Mathf.Clamp01(currentSpeed));
        }
    }

    void Jump()
    {
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            animator.SetTrigger("jump");
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    IEnumerator LockMovementAfterLanding()
    {
        canMove = false;
        yield return new WaitForSeconds(groundedLockTime);
        canMove = true;
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundCheckDistance);
    }
}
