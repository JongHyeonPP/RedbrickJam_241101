using UnityEngine;
using System.Collections;

public class MoveController : MonoBehaviour
{
    public float moveSpeed = 5f;             // 기본 이동 속도
    public float airMoveSpeedFactor = 0.0f;  // 점프 시 이동 속도 감소 비율
    public float jumpForce = 10f;            // 점프 힘
    public float groundCheckDistance = 0.1f; // 지면 체크를 위한 Ray 길이
    public float acceleration = 20f;         // 가속도
    public float rotationSpeed = 10f;        // 회전 속도
    public float groundedLockTime = 1f;      // 착지 후 움직임이 잠기는 시간

    private Rigidbody rb;
    private bool isGrounded;
    private bool canMove = true;             // 이동 가능 여부
    private Vector3 lastMoveDirection;       // 마지막 이동 방향
    private Vector3 currentMoveDirection;    // 현재 이동 방향

    private Animator animator;

    void Start()
    {
        // Rigidbody 컴포넌트를 가져옵니다.
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody가 없습니다. Rigidbody를 추가해주세요.");
        }

        // Animator 컴포넌트를 가져옵니다.
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator가 없습니다. Animator를 추가해주세요.");
        }
    }

    void Update()
    {
        // 이전 상태에서의 착지 여부
        bool wasGrounded = isGrounded;

        // 현재 착지 여부 판정
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance);

        // 착지 시 이동 잠금 코루틴 시작
        if (isGrounded && !wasGrounded)
        {
            StartCoroutine(LockMovementAfterLanding());
        }

        // 이동 및 점프 처리 (이동 가능할 때만 Move 호출)
        if (canMove)
        {
            Move();
        }
        Jump();

        // 애니메이터의 isGround 파라미터 설정
        if (animator != null)
        {
            animator.SetBool("isGround", isGrounded);
        }
    }

    void Move()
    {
        // 이동 여부를 초기화
        bool isMoving = false;

        if (isGrounded)
        {
            // WASD 입력을 통해 이동 방향 설정
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            // 이동 벡터 계산 및 정규화
            Vector3 targetDirection = new Vector3(horizontal, 0, vertical).normalized;

            // 카메라의 y축 회전을 적용하여 이동 방향을 카메라가 보는 방향으로 변환
            if (Camera.main != null)
            {
                Vector3 cameraForward = Camera.main.transform.forward;
                cameraForward.y = 0; // y축 값은 제거하여 평면 이동을 유지
                Quaternion rotation = Quaternion.LookRotation(cameraForward);
                targetDirection = rotation * targetDirection;
            }

            // 이동 방향이 있을 때만 방향 저장 및 회전 적용
            if (targetDirection != Vector3.zero)
            {
                lastMoveDirection = targetDirection;
                isMoving = true; // 이동 중으로 설정

                // 목표 방향으로 부드럽게 회전
                Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }

            // MoveTowards를 사용하여 가속을 빠르게 적용
            currentMoveDirection = Vector3.MoveTowards(currentMoveDirection, targetDirection, acceleration * Time.deltaTime);
        }

        // 이동 속도 결정
        float effectiveMoveSpeed = isGrounded ? moveSpeed : moveSpeed * airMoveSpeedFactor;

        // x, z 방향으로만 Translate 적용
        Vector3 movement = new Vector3(currentMoveDirection.x * effectiveMoveSpeed * Time.deltaTime, 0, currentMoveDirection.z * effectiveMoveSpeed * Time.deltaTime);
        transform.Translate(movement, Space.World);

        // Animator의 isMove 파라미터 설정
        if (animator != null)
        {
            animator.SetBool("isMove", isMoving);

            // 현재 속도를 실제 이동 속도로 계산하여 0 ~ 1 범위로 정규화
            float currentSpeed = movement.magnitude / (moveSpeed * Time.deltaTime);
            animator.SetFloat("speed", Mathf.Clamp01(currentSpeed));
        }
    }

    void Jump()
    {
        // 점프 입력 처리
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            animator.SetTrigger("jump");
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    IEnumerator LockMovementAfterLanding()
    {
        // 착지 후 일정 시간 동안 이동 불가능 상태
        canMove = false;
        yield return new WaitForSeconds(groundedLockTime);
        canMove = true;
    }

    void OnDrawGizmos()
    {
        // Gizmos로 지면 체크 Ray를 시각화합니다.
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundCheckDistance);
    }
}
